import { Component } from '@angular/core';
import {ProposalActionPanelComponent} from './../proposal-action-panel/proposal-action-panel'

@Component({
  selector: 'app-dashboard',
  imports: [ProposalActionPanelComponent],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent {

}
