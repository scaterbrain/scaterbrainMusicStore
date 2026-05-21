import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Product } from '../types';
import { api } from '../hooks/useApi';
import { useCart } from '../hooks/useCart';
import './ProductDetail.css';

function Stars({ rating }: { rating: number }) {
  return (
    <span className="stars">
      {[1,2,3,4,5].map(i => (
        <span key={i} style={{ opacity: i <= Math.round(rating) ? 1 : 0.3 }}>★</span>
      ))}
    </span>
  );
}

export default function ProductDetail() {
  const { id } = useParams<{ id: string }>();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [qty, setQty] = useState(1);
  const [added, setAdded] = useState(false);
  const { addToCart } = useCart();

  useEffect(() => {
    if (!id) return;
    api.products.get(Number(id)).then(setProduct).finally(() => setLoading(false));
  }, [id]);

  const handleAdd = async () => {
    if (!product) return;
    await addToCart(product.id, qty);
    setAdded(true);
    setTimeout(() => setAdded(false), 2000);
  };

  if (loading) return <div className="detail-loading container"><div className="skeleton-card" style={{ height: 500 }} /></div>;
  if (!product) return <div className="container" style={{ padding: '4rem 1.5rem', textAlign: 'center' }}><p>Product not found. <Link to="/shop">Back to shop</Link></p></div>;

  const conditionClass = product.condition === 'New' ? 'badge-new'
    : product.condition === 'Vintage' ? 'badge-vintage' : 'badge-used';

  return (
    <main className="product-detail page-enter">
      <div className="container">
        <nav className="breadcrumb">
          <Link to="/">Home</Link> / <Link to="/shop">Shop</Link> /
          <Link to={`/shop?category=${product.category}`}>{product.category}</Link> /
          <span>{product.name}</span>
        </nav>

        <div className="product-detail__grid">
          <div className="product-detail__img-wrap">
            <img src={product.imageUrl} alt={product.name} />
            {product.isFeatured && <span className="product-detail__featured">⭐ Featured Pick</span>}
          </div>

          <div className="product-detail__info">
            <div className="product-detail__meta">
              <span className="product-brand">{product.brand}</span>
              <span className={`badge ${conditionClass}`}>{product.condition}</span>
            </div>

            <h1 className="product-detail__name">{product.name}</h1>

            <div className="product-detail__rating">
              <Stars rating={product.rating} />
              <span>{product.rating.toFixed(1)}</span>
              <span className="review-count">({product.reviewCount} reviews)</span>
            </div>

            <p className="product-detail__price">
              ${product.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}
            </p>

            <p className="product-detail__desc">{product.description}</p>

            <div className="product-detail__stock">
              {product.stockQuantity === 0
                ? <span className="out-of-stock">Out of Stock</span>
                : product.stockQuantity <= 3
                  ? <span className="low-stock">⚡ Only {product.stockQuantity} remaining</span>
                  : <span className="in-stock">✓ In Stock ({product.stockQuantity} available)</span>
              }
            </div>

            {product.stockQuantity > 0 && (
              <div className="product-detail__actions">
                <div className="qty-selector">
                  <button onClick={() => setQty(q => Math.max(1, q - 1))}>−</button>
                  <span>{qty}</span>
                  <button onClick={() => setQty(q => Math.min(product.stockQuantity, q + 1))}>+</button>
                </div>
                <button
                  className={`btn btn-primary add-btn ${added ? 'added' : ''}`}
                  onClick={handleAdd}
                >
                  {added ? '✓ Added to Cart!' : 'Add to Cart'}
                </button>
              </div>
            )}

            <dl className="product-detail__specs">
              <div><dt>Category</dt><dd>{product.category}</dd></div>
              <div><dt>Brand</dt><dd>{product.brand}</dd></div>
              <div><dt>Condition</dt><dd>{product.condition}</dd></div>
            </dl>
          </div>
        </div>
      </div>
    </main>
  );
}
