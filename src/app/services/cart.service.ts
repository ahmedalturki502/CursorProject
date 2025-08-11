import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  CartDto, 
  CartItemDto, 
  AddToCartRequest, 
  UpdateCartItemRequest 
} from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = `${environment.apiUrl}/api/cart`;
  private cartSubject = new BehaviorSubject<CartDto | null>(null);
  public cart$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) { }

  // Get current cart
  getCart(): Observable<CartDto> {
    return this.http.get<CartDto>(this.apiUrl)
      .pipe(
        tap(cart => this.cartSubject.next(cart))
      );
  }

  // Add product to cart
  addToCart(request: AddToCartRequest): Observable<CartDto> {
    return this.http.post<CartDto>(`${this.apiUrl}/add`, request)
      .pipe(
        tap(cart => this.cartSubject.next(cart))
      );
  }

  // Update cart item quantity
  updateCartItem(itemId: number, request: UpdateCartItemRequest): Observable<CartDto> {
    return this.http.put<CartDto>(`${this.apiUrl}/items/${itemId}`, request)
      .pipe(
        tap(cart => this.cartSubject.next(cart))
      );
  }

  // Remove item from cart
  removeCartItem(itemId: number): Observable<CartDto> {
    return this.http.delete<CartDto>(`${this.apiUrl}/items/${itemId}`)
      .pipe(
        tap(cart => this.cartSubject.next(cart))
      );
  }

  // Clear entire cart
  clearCart(): Observable<CartDto> {
    return this.http.delete<CartDto>(`${this.apiUrl}/clear`)
      .pipe(
        tap(cart => this.cartSubject.next(cart))
      );
  }

  // Get current cart value
  getCurrentCart(): CartDto | null {
    return this.cartSubject.value;
  }

  // Get cart item count
  getCartItemCount(): number {
    const cart = this.cartSubject.value;
    return cart?.totalItems || 0;
  }

  // Get cart total amount
  getCartTotal(): number {
    const cart = this.cartSubject.value;
    return cart?.totalAmount || 0;
  }

  // Check if cart has items
  hasItems(): boolean {
    const cart = this.cartSubject.value;
    return (cart?.items?.length || 0) > 0;
  }

  // Refresh cart data
  refreshCart(): void {
    this.getCart().subscribe();
  }
}
