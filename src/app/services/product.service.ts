import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  ProductDto, 
  ProductListResponse, 
  ProductFilter,
  CreateProductRequest,
  UpdateProductRequest,
  CategoryDto 
} from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) { }

  // Get all products with optional filtering
  getProducts(filter?: ProductFilter): Observable<ProductListResponse> {
    let params = new HttpParams();
    
    if (filter?.page) params = params.set('page', filter.page.toString());
    if (filter?.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    if (filter?.categoryId) params = params.set('categoryId', filter.categoryId.toString());
    if (filter?.search) params = params.set('search', filter.search);
    if (filter?.minPrice) params = params.set('minPrice', filter.minPrice.toString());
    if (filter?.maxPrice) params = params.set('maxPrice', filter.maxPrice.toString());

    return this.http.get<ProductListResponse>(`${this.apiUrl}/products`, { params });
  }

  // Get product by ID
  getProduct(id: number): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/products/${id}`);
  }

  // Create new product (Admin only)
  createProduct(request: CreateProductRequest): Observable<ProductDto> {
    return this.http.post<ProductDto>(`${this.apiUrl}/products`, request);
  }

  // Update product (Admin only)
  updateProduct(id: number, request: UpdateProductRequest): Observable<ProductDto> {
    return this.http.put<ProductDto>(`${this.apiUrl}/products/${id}`, request);
  }

  // Delete product (Admin only)
  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/products/${id}`);
  }

  // Get all categories
  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(`${this.apiUrl}/categories`);
  }

  // Get category by ID
  getCategory(id: number): Observable<CategoryDto> {
    return this.http.get<CategoryDto>(`${this.apiUrl}/categories/${id}`);
  }

  // Create new category (Admin only)
  createCategory(name: string): Observable<CategoryDto> {
    return this.http.post<CategoryDto>(`${this.apiUrl}/categories`, { name });
  }

  // Update category (Admin only)
  updateCategory(id: number, name: string): Observable<CategoryDto> {
    return this.http.put<CategoryDto>(`${this.apiUrl}/categories/${id}`, { name });
  }

  // Delete category (Admin only)
  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/categories/${id}`);
  }
}
