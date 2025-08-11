import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { ProductDto, CategoryDto } from '../../models/product.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent implements OnInit {
  featuredProducts: ProductDto[] = [];
  categories: CategoryDto[] = [];
  isAuthenticated = false;
  loading = true;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadFeaturedProducts();
    this.loadCategories();
    this.checkAuthStatus();
  }

  loadFeaturedProducts(): void {
    this.productService.getProducts({ pageSize: 4 }).subscribe({
      next: (response) => {
        this.featuredProducts = response.products;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading featured products:', error);
        this.loading = false;
      }
    });
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  checkAuthStatus(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
    });
  }

  addToCart(productId: number): void {
    if (!this.isAuthenticated) {
      // Redirect to login if not authenticated
      return;
    }

    this.cartService.addToCart({ productId, quantity: 1 }).subscribe({
      next: () => {
        console.log('Product added to cart successfully');
      },
      error: (error) => {
        console.error('Error adding product to cart:', error);
      }
    });
  }
}
