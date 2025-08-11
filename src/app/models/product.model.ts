// Product model interface to match the backend API
export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  stockQuantity: number;
  categoryId: number;
  category?: Category;
}

// Category model interface
export interface Category {
  id: number;
  name: string;
  products?: Product[];
}

// Product request models
export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  stockQuantity: number;
  categoryId: number;
}

export interface UpdateProductRequest {
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  stockQuantity: number;
  categoryId: number;
}

// Product response models
export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  stockQuantity: number;
  categoryId: number;
  category: CategoryDto;
}

export interface CategoryDto {
  id: number;
  name: string;
}

// Product list response with pagination
export interface ProductListResponse {
  products: ProductDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Product filter options
export interface ProductFilter {
  page?: number;
  pageSize?: number;
  categoryId?: number;
  search?: string;
  minPrice?: number;
  maxPrice?: number;
}
