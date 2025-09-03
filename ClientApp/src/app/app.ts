import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule], // Only CommonModule and RouterModule
  template: `<router-outlet></router-outlet>`,
  styleUrls: ['./app.scss']
})
export class App {}
