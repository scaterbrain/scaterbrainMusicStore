import { useCart } from '../hooks/useCart';
import './CartDrawer.css';

export default function CartDrawer() {
  const { cart, isOpen, closeCart, removeFromCart, updateQuantity, clearCart } = useCart();

  if (!isOpen) return null;

  return (
    <>
      <div className="cart-overlay" onClick={closeCart} />
      <aside className="cart-drawer">
        <div className="cart-drawer__header">
          <h2>Your Cart</h2>
          {(cart?.itemCount ?? 0) > 0 && (
            <span className="cart-drawer__count">{cart!.itemCount} item{cart!.itemCount !== 1 ? 's' : ''}</span>
          )}
          <button className="cart-drawer__close" onClick={closeCart} aria-label="Close cart">✕</button>
        </div>

        <div className="cart-drawer__body">
          {!cart || cart.items.length === 0 ? (
            <div className="cart-empty">
              <span className="cart-empty__icon">🎸</span>
              <p>Your cart is empty</p>
              <button className="btn btn-primary" onClick={closeCart}>Keep Shopping</button>
            </div>
          ) : (
            <>
              <ul className="cart-items">
                {cart.items.map(item => (
                  <li key={item.productId} className="cart-item">
                    <img src={item.imageUrl} alt={item.productName} />
                    <div className="cart-item__info">
                      <p className="cart-item__name">{item.productName}</p>
                      <p className="cart-item__price">${item.price.toFixed(2)}</p>
                      <div className="cart-item__qty">
                        <button onClick={() => updateQuantity(item.productId, item.quantity - 1)}>−</button>
                        <span>{item.quantity}</span>
                        <button onClick={() => updateQuantity(item.productId, item.quantity + 1)}>+</button>
                      </div>
                    </div>
                    <button
                      className="cart-item__remove"
                      onClick={() => removeFromCart(item.productId)}
                      aria-label="Remove"
                    >✕</button>
                  </li>
                ))}
              </ul>

              <div className="cart-drawer__footer">
                <div className="cart-total">
                  <span>Total</span>
                  <span className="cart-total__amount">${cart.total.toFixed(2)}</span>
                </div>
                <button className="btn btn-primary" style={{ width: '100%' }}>
                  Checkout →
                </button>
                <button className="btn btn-ghost" style={{ width: '100%', marginTop: '0.5rem' }} onClick={clearCart}>
                  Clear Cart
                </button>
              </div>
            </>
          )}
        </div>
      </aside>
    </>
  );
}
