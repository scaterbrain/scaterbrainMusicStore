import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Product } from '../types';
import { api } from '../hooks/useApi';
import ProductCard from '../components/ProductCard';
import logo from '../assets/logo.png';
import './Home.css';

const CATEGORIES = [
  { name: 'Guitars', emoji: '🎸', desc: 'Electric, acoustic & vintage' },
  { name: 'Vinyl', emoji: '🎵', desc: 'New pressings & rare originals' },
  { name: 'Gear', emoji: '🎛️', desc: 'Amps, pedals & turntables' },
  { name: 'Accessories', emoji: '🎤', desc: 'Strings, straps & care kits' },
];

export default function Home() {
  const [featured, setFeatured] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.products.featured().then(setFeatured).finally(() => setLoading(false));
  }, []);

  return (
    <main className="home page-enter">
      {/* Hero */}
      <section className="hero">
        <div className="hero__noise" />
        <div className="container hero__inner">
          <div className="hero__text">
            <span className="hero__eyebrow">Since 2024 · Orlando, FL</span>
            <h1 className="hero__title">
              Where Music Lives<br />
              <span className="hero__accent">in Your Head</span>
            </h1>
            <p className="hero__sub">
              Guitars, vinyl, gear, and everything else rattling around in a music lover's brain.
              Curated with obsession. Priced with honesty.
            </p>
            <div className="hero__cta">
              <Link to="/shop" className="btn btn-primary hero__btn">Shop Everything</Link>
              <Link to="/shop?category=Vinyl" className="btn btn-secondary hero__btn">Browse Vinyl →</Link>
            </div>
          </div>
          <div className="hero__logo-wrap">
            <img src={logo} alt="Scatterbrain Music" className="hero__logo" />
          </div>
        </div>
      </section>

      {/* Categories */}
      <section className="categories section">
        <div className="container">
          <h2 className="section-title">Shop by Category</h2>
          <div className="categories__grid">
            {CATEGORIES.map(cat => (
              <Link key={cat.name} to={`/shop?category=${cat.name}`} className="category-card">
                <span className="category-card__emoji">{cat.emoji}</span>
                <h3>{cat.name}</h3>
                <p>{cat.desc}</p>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* Featured */}
      <section className="featured section">
        <div className="container">
          <div className="section-header">
            <h2 className="section-title">Featured Picks</h2>
            <Link to="/shop" className="section-link">View all →</Link>
          </div>
          {loading ? (
            <div className="products-skeleton">
              {[...Array(3)].map((_, i) => <div key={i} className="skeleton-card" />)}
            </div>
          ) : (
            <div className="products-grid">
              {featured.map(p => <ProductCard key={p.id} product={p} />)}
            </div>
          )}
        </div>
      </section>

      {/* Banner */}
      <section className="banner">
        <div className="container banner__inner">
          <div>
            <h2>Sell or Trade Your Gear</h2>
            <p>Got something collecting dust? We buy, sell, and trade. Bring it in.</p>
          </div>
          <a href="#" className="btn btn-primary">Get in Touch →</a>
        </div>
      </section>
    </main>
  );
}
