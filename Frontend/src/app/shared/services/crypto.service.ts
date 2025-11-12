import * as CryptoJS from 'crypto-js';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CryptoService {
  private readonly keyString = '01234567890123456789012345678901'; // 32-char
  private readonly ivString = '0123456789012345'; // 16-char

  private readonly key = CryptoJS.enc.Utf8.parse(this.keyString);
  private readonly iv = CryptoJS.enc.Utf8.parse(this.ivString);

  encrypt(data: any): string {
    const json = typeof data === 'string' ? data : JSON.stringify(data);
    const encrypted = CryptoJS.AES.encrypt(json, this.key, {
      iv: this.iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });
    return encrypted.toString();
  }

  decrypt(encryptedText: string): any {
    const decrypted = CryptoJS.AES.decrypt(encryptedText, this.key, {
      iv: this.iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });

    const decryptedStr = decrypted.toString(CryptoJS.enc.Utf8);
    try {
      return JSON.parse(decryptedStr);
    } catch {
      return decryptedStr;
    }
  }
}
