import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../hooks/useCart';
import logo from '../assets/logo.png';
import './Navbar.css';

export default function Navbar() {
  const { cart, openCart } = useCart();
  const [search, setSearch] = useState('');
  const [menuOpen, setMenuOpen] = useState(false);
  const navigate = useNavigate();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (search.trim()) navigate(`/shop?search=${encodeURIComponent(search.trim())}`);
  };

  return (
    <header className="navbar">
      <div className="navbar-inner container">
        <Link to="/" className="navbar-brand">
          <img src={logo} alt="Scatterbrain Music" className="navbar-logo" />
        </Link>

        <nav className={`navbar-links ${menuOpen ? 'open' : ''}`}>
          <Link to="/shop" onClick={() => setMenuOpen(false)}>Shop</Link>
          <Link to="/shop?category=Guitars" onClick={() => setMenuOpen(false)}>Guitars</Link>
          <Link to="/shop?category=Vinyl" onClick={() => setMenuOpen(false)}>Vinyl</Link>
          <Link to="/shop?category=Gear" onClick={() => setMenuOpen(false)}>Gear</Link>
          <Link to="/shop?category=Accessories" onClick={() => setMenuOpen(false)}>Accessories</Link>
        </nav>

        <div className="navbar-actions">
          <form className="navbar-search" onSubmit={handleSearch}>
            <input
              type="text"
              placeholder="Search…"
              value={search}
              onChange={e => setSearch(e.target.value)}
              aria-label="Search products"
            />
            <button type="submit" aria-label="Search">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/>
              </svg>
            </button>
          </form>

          <button className="cart-btn" onClick={openCart} aria-label="Open cart">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4z"/>
              <line x1="3" y1="6" x2="21" y2="6"/>
              <path d="M16 10a4 4 0 0 1-8 0"/>
            </svg>
            {(cart?.itemCount ?? 0) > 0 && (
              <span className="cart-badge">{cart!.itemCount}</span>
            )}
          </button>

          <button
            className="hamburger"
            onClick={() => setMenuOpen(!menuOpen)}
            aria-label="Toggle menu"
          >
            <span /><span /><span />
          </button>
        </div>
      </div>
    </header>
  );
}
