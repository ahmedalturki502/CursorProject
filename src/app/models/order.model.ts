// Order model interfaces to match the backend API
export interface Order {
  id: number;
  userId: string;
  orderDate: Date;
  totalAmount: number;
  shippingAddress: string;
  items: OrderItem[];
  user?: UserDto;
}

export interface OrderItem {
  id: number;
  orderId: number;
  productId: number;
  quantity: number;
  price: number;
  product: ProductDto;
}

// Order request models
export interface PlaceOrderRequest {
  shippingAddress: string;
}

// Order response models
export interface OrderDto {
  id: number;
  orderDate: Date;
  totalAmount: number;
  shippingAddress: string;
  items: OrderItemDto[];
}

export interface OrderItemDto {
  id: number;
  productId: number;
  quantity: number;
  price: number;
  product: ProductDto;
}

// Admin order response
export interface AdminOrderDto {
  id: number;
  orderDate: Date;
  totalAmount: number;
  shippingAddress: string;
  items: OrderItemDto[];
  user: UserDto;
}

// Order list response with pagination
export interface OrderListResponse {
  orders: OrderDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Order filter options
export interface OrderFilter {
  page?: number;
  pageSize?: number;
  customerEmail?: string;
  fromDate?: string;
  toDate?: string;
}
