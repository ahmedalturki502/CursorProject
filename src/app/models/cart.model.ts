// Cart model interfaces to match the backend API
export interface Cart {
  id: number;
  userId: string;
  items: CartItem[];
  totalAmount: number;
  totalItems: number;
}

export interface CartItem {
  id: number;
  cartId: number;
  productId: number;
  quantity: number;
  product: ProductDto;
  price: number;
}

// Cart request models
export interface AddToCartRequest {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}

// Cart response models
export interface CartDto {
  id: number;
  items: CartItemDto[];
  totalAmount: number;
  totalItems: number;
}

export interface CartItemDto {
  id: number;
  productId: number;
  quantity: number;
  price: number;
  product: ProductDto;
}
