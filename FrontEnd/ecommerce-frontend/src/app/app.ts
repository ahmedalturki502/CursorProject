import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';
import { CartService } from './services/cart.service';
import { User } from './models/user.model';
import { Cart } from './models/cart.model';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  // Signal to track if user is logged in
  protected readonly isLoggedIn = signal<boolean>(false);
  
  // Signal to track current user
  protected readonly currentUser = signal<User | null>(null);
  
  // Signal to track cart item count
  protected readonly cartItemCount = signal<number>(0);

  constructor(
    private authService: AuthService,
    private cartService: CartService
  ) {}

  ngOnInit() {
    // Check authentication status on app initialization
    this.checkAuthStatus();
    
    // Subscribe to cart updates
    this.cartService.getCart().subscribe((cart: Cart) => {
      this.cartItemCount.set(cart.items.length);
    });
  }

  private checkAuthStatus() {
    const token = this.authService.getToken();
    if (token) {
      this.authService.validateToken().subscribe({
        next: (user: User) => {
          this.isLoggedIn.set(true);
          this.currentUser.set(user);
        },
        error: () => {
          this.authService.logout().subscribe();
          this.isLoggedIn.set(false);
          this.currentUser.set(null);
        }
      });
    }
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.isLoggedIn.set(false);
      this.currentUser.set(null);
      this.cartItemCount.set(0);
    });
  }
}
