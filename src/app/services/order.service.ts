import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  OrderDto, 
  OrderListResponse, 
  OrderFilter,
  PlaceOrderRequest,
  AdminOrderDto 
} from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/api/orders`;

  constructor(private http: HttpClient) { }

  // Place a new order
  placeOrder(request: PlaceOrderRequest): Observable<OrderDto> {
    return this.http.post<OrderDto>(this.apiUrl, request);
  }

  // Get user's orders
  getUserOrders(filter?: OrderFilter): Observable<OrderListResponse> {
    let params = new HttpParams();
    
    if (filter?.page) params = params.set('page', filter.page.toString());
    if (filter?.pageSize) params = params.set('pageSize', filter.pageSize.toString());

    return this.http.get<OrderListResponse>(this.apiUrl, { params });
  }

  // Get specific order by ID
  getOrder(id: number): Observable<OrderDto> {
    return this.http.get<OrderDto>(`${this.apiUrl}/${id}`);
  }

  // Get all orders (Admin only)
  getAllOrders(filter?: OrderFilter): Observable<OrderListResponse> {
    let params = new HttpParams();
    
    if (filter?.page) params = params.set('page', filter.page.toString());
    if (filter?.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    if (filter?.customerEmail) params = params.set('customerEmail', filter.customerEmail);
    if (filter?.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter?.toDate) params = params.set('toDate', filter.toDate);

    return this.http.get<OrderListResponse>(`${this.apiUrl}/admin/all`, { params });
  }
}
