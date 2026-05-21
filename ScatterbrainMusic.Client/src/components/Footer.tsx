import { Link } from 'react-router-dom';
import logo from '../assets/logo.png';
import './Footer.css';

export default function Footer() {
  return (
    <footer className="footer">
      <div className="container footer__inner">
        <div className="footer__brand">
          <img src={logo} alt="Scatterbrain Music" className="footer__logo" />
          <p>Gear · Vinyl · Community<br />Since 2024 · Orlando, FL</p>
        </div>
        <div className="footer__links">
          <h4>Shop</h4>
          <Link to="/shop?category=Guitars">Guitars</Link>
          <Link to="/shop?category=Vinyl">Vinyl</Link>
          <Link to="/shop?category=Gear">Gear</Link>
          <Link to="/shop?category=Accessories">Accessories</Link>
        </div>
        <div className="footer__links">
          <h4>Info</h4>
          <a href="#">About Us</a>
          <a href="#">Contact</a>
          <a href="#">Shipping</a>
          <a href="#">Returns</a>
        </div>
        <div className="footer__contact">
          <h4>Stay in the Loop</h4>
          <p>New arrivals every week. Vintage gems. Deals worth knowing about.</p>
          <div className="footer__subscribe">
            <input type="email" placeholder="your@email.com" aria-label="Email" />
            <button className="btn btn-orange">Join</button>
          </div>
        </div>
      </div>
      <div className="footer__bottom">
        <p>© 2024 Scatterbrain Music. All rights reserved.</p>
      </div>
    </footer>
  );
}
