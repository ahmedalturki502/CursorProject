import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {
  // Simple home component that works without backend
  loading = false;
  error = '';
  featuredProducts: any[] = [];

  constructor() {
    // Home page will display even without backend connection
  }
}
