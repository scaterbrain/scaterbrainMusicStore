import React, { createContext, useContext, useEffect, useState } from 'react';
import { Cart } from '../types';
import { api } from '../hooks/useApi';

interface CartContextType {
  cart: Cart | null;
  loading: boolean;
  addToCart: (productId: number, quantity?: number) => Promise<void>;
  removeFromCart: (productId: number) => Promise<void>;
  updateQuantity: (productId: number, quantity: number) => Promise<void>;
  clearCart: () => Promise<void>;
  isOpen: boolean;
  openCart: () => void;
  closeCart: () => void;
}

const CartContext = createContext<CartContextType | null>(null);

export function CartProvider({ children }: { children: React.ReactNode }) {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);

  useEffect(() => {
    api.cart.get().then(setCart).catch(console.error);
  }, []);

  const addToCart = async (productId: number, quantity = 1) => {
    setLoading(true);
    try {
      const updated = await api.cart.add(productId, quantity);
      setCart(updated);
      setIsOpen(true);
    } finally {
      setLoading(false);
    }
  };

  const removeFromCart = async (productId: number) => {
    const updated = await api.cart.remove(productId);
    setCart(updated);
  };

  const updateQuantity = async (productId: number, quantity: number) => {
    const updated = await api.cart.update(productId, quantity);
    setCart(updated);
  };

  const clearCart = async () => {
    await api.cart.clear();
    setCart(prev => prev ? { ...prev, items: [], total: 0, itemCount: 0 } : null);
  };

  return (
    <CartContext.Provider value={{
      cart, loading, addToCart, removeFromCart, updateQuantity, clearCart,
      isOpen, openCart: () => setIsOpen(true), closeCart: () => setIsOpen(false)
    }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used inside CartProvider');
  return ctx;
}
