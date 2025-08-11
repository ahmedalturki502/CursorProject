import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  Product, 
  Category, 
  ProductDto, 
  CreateProductDto, 
  UpdateProductDto, 
  ProductFilterOptions, 
  ProductListResponse 
} from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getProducts(page: number = 1, pageSize: number = 10, filters?: ProductFilterOptions): Observable<ProductListResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
      if (filters.categoryId) {
        params = params.set('categoryId', filters.categoryId);
      }
      if (filters.minPrice !== undefined) {
        params = params.set('minPrice', filters.minPrice.toString());
      }
      if (filters.maxPrice !== undefined) {
        params = params.set('maxPrice', filters.maxPrice.toString());
      }
      if (filters.inStock !== undefined) {
        params = params.set('inStock', filters.inStock.toString());
      }
      if (filters.sortBy) {
        params = params.set('sortBy', filters.sortBy);
      }
      if (filters.sortOrder) {
        params = params.set('sortOrder', filters.sortOrder);
      }
    }

    return this.http.get<ProductListResponse>(`${this.apiUrl}/api/products`, { params });
  }

  getProduct(id: string): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/api/products/${id}`);
  }

  createProduct(product: CreateProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(`${this.apiUrl}/api/products`, product);
  }

  updateProduct(id: string, product: UpdateProductDto): Observable<ProductDto> {
    return this.http.put<ProductDto>(`${this.apiUrl}/api/products/${id}`, product);
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/products/${id}`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/api/categories`);
  }

  getCategory(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/api/categories/${id}`);
  }

  createCategory(category: { name: string; description?: string }): Observable<Category> {
    return this.http.post<Category>(`${this.apiUrl}/api/categories`, category);
  }

  updateCategory(id: string, category: { name?: string; description?: string; isActive?: boolean }): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/api/categories/${id}`, category);
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/categories/${id}`);
  }
}
