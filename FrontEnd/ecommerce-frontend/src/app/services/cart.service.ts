import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Cart, CartResponse, AddToCartRequest, UpdateCartItemRequest } from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly apiUrl = environment.apiUrl;
  private cartSubject = new BehaviorSubject<Cart | null>(null);
  public cart$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {
    // Load cart on service initialization
    this.loadCart();
  }

  getCart(): Observable<Cart> {
    return this.http.get<CartResponse>(`${this.apiUrl}/api/cart`)
      .pipe(
        map(cartResponse => this.mapCartResponseToCart(cartResponse)),
        tap(cart => {
          this.cartSubject.next(cart);
        })
      );
  }

  addToCart(request: AddToCartRequest): Observable<Cart> {
    return this.http.post<CartResponse>(`${this.apiUrl}/api/cart/items`, request)
      .pipe(
        map(cartResponse => this.mapCartResponseToCart(cartResponse)),
        tap(cart => {
          this.cartSubject.next(cart);
        })
      );
  }

  updateCartItem(itemId: string, request: UpdateCartItemRequest): Observable<Cart> {
    return this.http.put<CartResponse>(`${this.apiUrl}/api/cart/items/${itemId}`, request)
      .pipe(
        map(cartResponse => this.mapCartResponseToCart(cartResponse)),
        tap(cart => {
          this.cartSubject.next(cart);
        })
      );
  }

  removeFromCart(itemId: string): Observable<Cart> {
    return this.http.delete<CartResponse>(`${this.apiUrl}/api/cart/items/${itemId}`)
      .pipe(
        map(cartResponse => this.mapCartResponseToCart(cartResponse)),
        tap(cart => {
          this.cartSubject.next(cart);
        })
      );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/cart`)
      .pipe(
        tap(() => {
          this.cartSubject.next(null);
        })
      );
  }

  getCurrentCart(): Cart | null {
    return this.cartSubject.value;
  }

  private mapCartResponseToCart(cartResponse: CartResponse): Cart {
    return {
      id: cartResponse.id,
      userId: cartResponse.userId,
      items: cartResponse.items.map(item => ({
        id: item.id,
        cartId: cartResponse.id,
        productId: item.productId,
        productName: item.productName,
        productPrice: item.productPrice,
        productImageUrl: item.productImageUrl,
        quantity: item.quantity,
        totalPrice: item.totalPrice
      })),
      totalAmount: cartResponse.totalAmount,
      itemCount: cartResponse.itemCount,
      createdAt: new Date(),
      updatedAt: new Date()
    };
  }

  private loadCart(): void {
    // Only load cart if user is logged in
    const token = localStorage.getItem('auth_token');
    if (token) {
      this.getCart().subscribe({
        error: () => {
          // Cart not found or error, set to null
          this.cartSubject.next(null);
        }
      });
    }
  }
}
