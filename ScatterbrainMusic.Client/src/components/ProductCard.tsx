import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Product } from '../types';
import { useCart } from '../hooks/useCart';
import './ProductCard.css';

interface Props { product: Product; }

function Stars({ rating }: { rating: number }) {
  return (
    <span className="stars" aria-label={`${rating} stars`}>
      {[1,2,3,4,5].map(i => (
        <span key={i} style={{ opacity: i <= Math.round(rating) ? 1 : 0.3 }}>★</span>
      ))}
    </span>
  );
}

export default function ProductCard({ product }: Props) {
  const { addToCart, loading } = useCart();
  const [added, setAdded] = useState(false);

  const handleAdd = async (e: React.MouseEvent) => {
    e.preventDefault();
    await addToCart(product.id);
    setAdded(true);
    setTimeout(() => setAdded(false), 1800);
  };

  const conditionClass = product.condition === 'New' ? 'badge-new'
    : product.condition === 'Vintage' ? 'badge-vintage' : 'badge-used';

  return (
    <Link to={`/product/${product.id}`} className="product-card">
      <div className="product-card__img-wrap">
        <img src={product.imageUrl} alt={product.name} loading="lazy" />
        <span className={`badge ${conditionClass} product-card__badge`}>{product.condition}</span>
        {product.isFeatured && <span className="product-card__featured">Featured</span>}
      </div>

      <div className="product-card__body">
        <div className="product-card__meta">
          <span className="product-card__brand">{product.brand}</span>
          <span className="product-card__category">{product.category}</span>
        </div>

        <h3 className="product-card__name">{product.name}</h3>

        <div className="product-card__rating">
          <Stars rating={product.rating} />
          <span className="product-card__review-count">({product.reviewCount})</span>
        </div>

        <div className="product-card__footer">
          <span className="product-card__price">${product.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}</span>
          <button
            className={`btn btn-primary product-card__btn ${added ? 'added' : ''}`}
            onClick={handleAdd}
            disabled={loading || product.stockQuantity === 0}
          >
            {product.stockQuantity === 0 ? 'Out of Stock' : added ? '✓ Added!' : 'Add to Cart'}
          </button>
        </div>

        {product.stockQuantity <= 3 && product.stockQuantity > 0 && (
          <p className="product-card__low-stock">Only {product.stockQuantity} left!</p>
        )}
      </div>
    </Link>
  );
}
