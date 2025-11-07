import { Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, Subscription, timer, fromEvent, merge, of, throwError } from 'rxjs';
import { throttleTime, debounceTime, takeUntil, tap, catchError, switchMap } from 'rxjs/operators';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { HttpService } from './http.service';
import { AppSettingsService } from './app-settings.service';
import { SessionExpiryDialogComponent } from '../components/session-expiry-dialog/session-expiry-dialog';

interface TokenChannelMessage {
  type: 'tokenRefreshed' | 'loggedOut';
  sourceId?: string; // Added to prevent self-broadcast loops
}

@Injectable({
  providedIn: 'root'
})
export class TokenRefreshService implements OnDestroy {
  private refreshTimerSubscription: Subscription | null = null;
  private inactivityTimerSubscription: Subscription | null = null;
  private inactivityChannel = new BroadcastChannel('inactivity_channel');
  private tokenChannel = new BroadcastChannel('token_channel');
  private destroy$ = new Subject<void>();
  private inactivityTimeLimitMinutes: number;
  private tokenRefreshBufferTimeMinutes: number;
  private warningSeconds = 60;
  private dialogRef: MatDialogRef<SessionExpiryDialogComponent> | null = null;
  private tabId: string;
  private leaderHeartbeatTimer: any;
  private isStopping = false; // NEW: Guard to prevent stopAll() recursion

  constructor(
    private httpService: HttpService,
    private router: Router,
    private readonly appSettingsService: AppSettingsService,
    private dialog: MatDialog
  ) {
    this.inactivityTimeLimitMinutes =
      this.appSettingsService.getInactiveTimeLimit() ?? 30;
    this.tokenRefreshBufferTimeMinutes =
      this.appSettingsService.getRefreshTokenBufferTime() ?? 10;

    this.tabId = crypto.randomUUID();
    sessionStorage.setItem('tab_id', this.tabId);

    window.addEventListener('beforeunload', () => {
      if (this.isLeaderTab()) {
        localStorage.removeItem('token_leader');
        localStorage.removeItem('token_leader_timestamp');
        clearInterval(this.leaderHeartbeatTimer);
      }
    });

    window.addEventListener('storage', (event) => {
      if (event.key === 'token_leader') {
        const newLeaderId = event.newValue;
        if (newLeaderId === this.tabId) {
          return;
        } else if (!newLeaderId) {
          console.log('[TokenRefreshService] Leadership released. Attempting to become the new leader.');
          this.tryBecomeLeader();
        } else if (newLeaderId !== this.tabId) {
          console.log(`[TokenRefreshService] Another tab (${newLeaderId}) became leader. Stopping my timers.`);
          this.stop();
        }
      }
    });

    this.inactivityChannel.onmessage = (event) => {
      if (event.data === 'reset') {
        this.closeDialog();
        this.startInactivityTimer();
        console.log('Resetting detected, starting timer', new Date(), this.inactivityTimerSubscription);
      }
    };

    this.tokenChannel.onmessage = (event) => {
      const data = event.data as TokenChannelMessage;

      if (data.type === 'tokenRefreshed') {
        console.log('[TokenRefreshService] Token refreshed in another tab');
        this.appSettingsService.loadFromStorage();
        this.stop();
        if (this.appSettingsService.isUserLoggedIn()){
          this.scheduleNextRefresh();
        }
      } else if (data.type === 'loggedOut') {
        // 1. Prevent infinite loop from self-broadcast
        if (data.sourceId === this.tabId) {
          return;
        }

        // 2. FIX: When receiving a remote logout, only run cleanup, not stopAll(),
        // to prevent re-broadcasting and race conditions.
        console.log('[TokenRefreshService] Logout detected in another tab. Stopping timers.');
        this.stop();
        this.clearInactivityTimer();
        this.closeDialog();
        this.stopActivityTracking();

        // Navigate to logout page only if not already there (based on AppSettingsService state)
        if (this.appSettingsService.isUserLoggedIn()) {
          this.router.navigateByUrl('/logout');
        }
      }
    };
  }

  register() {
    if (!this.appSettingsService.isUserLoggedIn()) {
      return;
    }
    this.start();
  }

  startActivityTracking() {
    this.log('Starting activity tracking...');

    const mouseMove$ = fromEvent(document, 'mousemove').pipe(throttleTime(2000));
    const scroll$ = fromEvent(document, 'scroll').pipe(throttleTime(2000));
    const otherEvents$ = merge(
      fromEvent(document, 'keydown'),
      fromEvent(document, 'click'),
      fromEvent(document, 'touchstart'),
      fromEvent(document, 'input'),
      fromEvent(document, 'focus')
    ).pipe(debounceTime(300));

    merge(mouseMove$, scroll$, otherEvents$)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.appSettingsService.isUserLoggedIn()) {
          this.stopActivityTracking();
          return;
        }

        console.log('User activity detected, resetting inactivity timer', new Date(), this.inactivityTimerSubscription);
        this.closeDialog();
        this.startInactivityTimer();
        this.inactivityChannel.postMessage('reset');
      });
  }

  /** Stops the activity tracking observable chain. */
  private stopActivityTracking() {
    this.destroy$.next();
  }


  start() {
    // Check if the Subject was closed during a previous logout and recreate it
    if (this.destroy$.closed) {
      this.destroy$ = new Subject<void>();
    }

    this.startActivityTracking();
    this.stop();
    this.tryBecomeLeader();

    if (!this.isLeaderTab()) {
      console.log('[TokenRefreshService] Not leader → waiting for broadcasts');
    }
  }

  stop() {
    this.refreshTimerSubscription?.unsubscribe();
    this.refreshTimerSubscription = null;
    if (this.leaderHeartbeatTimer) {
      clearInterval(this.leaderHeartbeatTimer);
      this.leaderHeartbeatTimer = null;
    }
  }

  /** Stops all timers, the dialog, and activity tracking. Call this on logout. */
  public stopAll() {
    // CRITICAL FIX: Add an idempotency guard to prevent cascade failures
    if (this.isStopping) {
      return;
    }
    this.isStopping = true; // Set flag immediately

    // Broadcast the logout event WITH the source tabId
    this.tokenChannel.postMessage({ type: 'loggedOut', sourceId: this.tabId });

    this.stop();
    this.clearInactivityTimer();
    this.closeDialog();
    this.stopActivityTracking();

    // Reset flag after a short delay to allow the router navigation to take over
    setTimeout(() => this.isStopping = false, 1000);
  }

  private scheduleNextRefresh() {
    if (!this.isLeaderTab()) {
      console.log('[TokenRefreshService] Not leader, skipping refresh schedule');
      return;
    }

    const expirationTime: number | null = this.appSettingsService.getExpirationTime();

    if (!expirationTime) {
      console.warn('[TokenRefreshService] No expiration time found → logging out');
      this.router.navigate(['/logout']);
      return;
    }

    const bufferMs = this.tokenRefreshBufferTimeMinutes * 60 * 1000;
    let timeUntilRefresh = expirationTime - Date.now() - bufferMs;

    if (timeUntilRefresh < 0) {
      timeUntilRefresh = 0;
    }

    console.log(
      `[TokenRefreshService] Scheduling refresh in ${Math.round(timeUntilRefresh / 1000)}s`
    );

    this.refreshTimerSubscription = timer(timeUntilRefresh).subscribe(() => {
      if (!this.isLeaderTab()) {
        console.log('[TokenRefreshService] Another tab is the leader, stopping my refresh.');
        this.stop();
        return;
      }

      this.httpService
        .refreshToken()
        .pipe(
          catchError((err) => {
            console.error('[TokenRefreshService] Refresh failed', err);

            if (err.status === 401 || err.status === 403) {
              console.warn('Token refresh unauthorized. Checking for a new token from another tab...');

              return timer(500).pipe(
                tap(() => {
                  const newExpirationTime = this.appSettingsService.getExpirationTime();
                  if (newExpirationTime && newExpirationTime > Date.now()) {
                    console.log('Found a new valid token. Race condition avoided.');
                    this.scheduleNextRefresh();
                  } else {
                    console.warn('No new token found after delay. Logging out.');
                    this.router.navigate(['/logout']);
                    this.stop();
                  }
                }),
                switchMap(() => of(false))
              );
            }

            this.router.navigate(['/logout']);
            this.stop();
            return throwError(() => err);
          })
        )
        .subscribe((success) => {
          if (success) {
            console.log('[TokenRefreshService] Token refreshed successfully');
            this.tokenChannel.postMessage({ type: 'tokenRefreshed' });
            this.scheduleNextRefresh();
          }
        });
    });
  }

  private startInactivityTimer() {
    this.clearInactivityTimer();
    if (!this.appSettingsService.isUserLoggedIn()) {
      return;
    }

    const totalMs = this.inactivityTimeLimitMinutes * 60 * 1000;
    const warningMs = this.warningSeconds * 1000;
    const showDialogAfter = totalMs - warningMs;

    const showDialog$ = timer(showDialogAfter).pipe(
      tap(() => {
        if (this.appSettingsService.isUserLoggedIn()) {
          this.openExpiryDialog(this.warningSeconds);
        }
      })
    );

    const logout$ = timer(totalMs).pipe(
      tap(() => {
        console.warn('[TokenRefreshService] Inactivity timeout → logging out');
        this.clearInactivityTimer();
        this.stopAll();
        this.router.navigateByUrl('/logout');
      })
    );

    this.inactivityTimerSubscription = merge(showDialog$, logout$).subscribe();
  }

  private clearInactivityTimer() {
    this.inactivityTimerSubscription?.unsubscribe();
    this.inactivityTimerSubscription = null;
  }

  ngOnDestroy() {
    // FIX: Only perform cleanup, do not call stopAll() here,
    // as it could trigger an unwanted broadcast on tab close/route change.
    this.stop();
    this.clearInactivityTimer();
    this.closeDialog();
    this.stopActivityTracking();
    this.destroy$.complete();
    this.inactivityChannel.close();
    this.tokenChannel.close();
  }

  private log(message: string, ...data: any[]) {
    console.log(`[${new Date().toLocaleTimeString()}] [TokenRefreshService] ${message}`, ...data);
  }

  private openExpiryDialog(countdown: number) {
    if (!this.appSettingsService.isUserLoggedIn()) {
      return;
    }
    if (this.dialogRef) return;
    this.dialogRef = this.dialog.open(SessionExpiryDialogComponent, {
      width: '400px',
      disableClose: true,
      data: { countdown },
    });

    this.dialogRef.afterClosed().subscribe((result) => {
      this.dialogRef = null;
      if (result === 'continue') {
        console.log('[TokenRefreshService] User chose to continue session');
        this.startInactivityTimer();
        this.inactivityChannel.postMessage('reset');
      } else if (result === 'logout') {
        console.warn('[TokenRefreshService] User chose logout');
        // This path needs to initiate a full logout (clear token/state and navigate)
        this.router.navigateByUrl('/logout');
      }
    });
  }

  private closeDialog() {
    if (this.dialogRef) {
      this.dialogRef.close('continue');
      this.dialogRef = null;
    }
  }

  private isLeaderTab(): boolean {
    return localStorage.getItem('token_leader') === this.tabId;
  }

  private tryBecomeLeader() {
    const leaderTimestamp = localStorage.getItem('token_leader_timestamp');
    const leaderId = localStorage.getItem('token_leader');

    if (!leaderId || (leaderTimestamp && Date.now() - parseInt(leaderTimestamp, 10) > 10000)) {
      localStorage.setItem('token_leader', this.tabId);
      localStorage.setItem('token_leader_timestamp', Date.now().toString());
      console.log(`[TokenRefreshService] Tab ${this.tabId} became leader`);
      this.startLeaderHeartbeat();
      this.scheduleNextRefresh();
    }
  }

  private startLeaderHeartbeat() {
    if (this.leaderHeartbeatTimer) {
      return;
    }
    this.leaderHeartbeatTimer = setInterval(() => {
      if (this.isLeaderTab()) {
        localStorage.setItem('token_leader_timestamp', Date.now().toString());
      } else {
        clearInterval(this.leaderHeartbeatTimer);
        this.leaderHeartbeatTimer = null;
      }
    }, 5000);
  }
}
