import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { UserDto } from '../../models/user.model';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})
export class LayoutComponent implements OnInit {
  isAuthenticated = false;
  currentUser: UserDto | null = null;
  cartItemCount = 0;

  constructor(
    private authService: AuthService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    // Subscribe to authentication state
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.isAuthenticated = !!user;
      
      // Load cart if user is authenticated
      if (this.isAuthenticated) {
        this.loadCart();
      }
    });

    // Subscribe to cart updates
    this.cartService.cart$.subscribe(cart => {
      this.cartItemCount = cart?.totalItems || 0;
    });
  }

  logout(): void {
    this.authService.logout().subscribe(() => {
      // User will be redirected by the interceptor
    });
  }

  private loadCart(): void {
    this.cartService.getCart().subscribe();
  }
}
