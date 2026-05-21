import { Cart, Product, ProductsResponse } from '../types';

const BASE = '/api';

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!res.ok) throw new Error(`API error ${res.status}: ${await res.text()}`);
  if (res.status === 204) return undefined as T;
  return res.json();
}

export const api = {
  products: {
    list: (params?: { category?: string; search?: string; page?: number; pageSize?: number }) => {
      const q = new URLSearchParams();
      if (params?.category && params.category !== 'All') q.set('category', params.category);
      if (params?.search) q.set('search', params.search);
      if (params?.page) q.set('page', String(params.page));
      if (params?.pageSize) q.set('pageSize', String(params.pageSize));
      return request<ProductsResponse>(`/products?${q}`);
    },
    get: (id: number) => request<Product>(`/products/${id}`),
    featured: () => request<Product[]>('/products/featured'),
    categories: () => request<string[]>('/products/categories'),
  },
  cart: {
    get: () => request<Cart>('/cart'),
    add: (productId: number, quantity = 1) =>
      request<Cart>('/cart/add', { method: 'POST', body: JSON.stringify({ productId, quantity }) }),
    update: (productId: number, quantity: number) =>
      request<Cart>('/cart/update', { method: 'PUT', body: JSON.stringify({ productId, quantity }) }),
    remove: (productId: number) =>
      request<Cart>(`/cart/${productId}`, { method: 'DELETE' }),
    clear: () =>
      request<void>('/cart', { method: 'DELETE' }),
  },
};
