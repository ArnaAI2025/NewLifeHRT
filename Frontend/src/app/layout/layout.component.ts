import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MainLayoutComponent } from './main-layout/main-layout';



@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterModule, MainLayoutComponent],
  template: `
    <app-main-layout></app-main-layout>    
  `
})
export class LayoutComponent {}
