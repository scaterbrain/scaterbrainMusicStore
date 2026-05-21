export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  category: string;
  imageUrl: string;
  stockQuantity: number;
  isFeatured: boolean;
  rating: number;
  reviewCount: number;
  brand: string;
  condition: 'New' | 'Used' | 'Vintage';
}

export interface CartItem {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  imageUrl: string;
}

export interface Cart {
  sessionId: string;
  items: CartItem[];
  total: number;
  itemCount: number;
}

export interface ProductsResponse {
  products: Product[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export type Category = 'All' | 'Guitars' | 'Vinyl' | 'Gear' | 'Accessories';
