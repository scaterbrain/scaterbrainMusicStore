import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Product } from '../types';
import { api } from '../hooks/useApi';
import ProductCard from '../components/ProductCard';
import './Shop.css';

const SORT_OPTIONS = ['Default', 'Price: Low to High', 'Price: High to Low', 'Rating'];

export default function Shop() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<string[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [sort, setSort] = useState('Default');

  const category = searchParams.get('category') || 'All';
  const search = searchParams.get('search') || '';
  const page = Number(searchParams.get('page') || 1);

  useEffect(() => {
    api.products.categories().then(cats => setCategories(['All', ...cats]));
  }, []);

  useEffect(() => {
    setLoading(true);
    api.products.list({ category: category === 'All' ? undefined : category, search, page })
      .then(res => {
        let sorted = [...res.products];
        if (sort === 'Price: Low to High') sorted.sort((a, b) => a.price - b.price);
        else if (sort === 'Price: High to Low') sorted.sort((a, b) => b.price - a.price);
        else if (sort === 'Rating') sorted.sort((a, b) => b.rating - a.rating);
        setProducts(sorted);
        setTotal(res.totalCount);
      })
      .finally(() => setLoading(false));
  }, [category, search, page, sort]);

  const setCategory = (cat: string) => {
    const p = new URLSearchParams(searchParams);
    if (cat === 'All') p.delete('category'); else p.set('category', cat);
    p.delete('page');
    setSearchParams(p);
  };

  return (
    <main className="shop page-enter">
      <div className="shop__hero">
        <div className="container">
          <h1 className="shop__title">
            {search ? `Search: "${search}"` : category === 'All' ? 'All Products' : category}
          </h1>
          <p className="shop__count">{total} item{total !== 1 ? 's' : ''}</p>
        </div>
      </div>

      <div className="container shop__body">
        {/* Sidebar */}
        <aside className="shop__sidebar">
          <h3>Categories</h3>
          <ul className="category-list">
            {categories.map(cat => (
              <li key={cat}>
                <button
                  className={`category-btn ${category === cat ? 'active' : ''}`}
                  onClick={() => setCategory(cat)}
                >
                  {cat}
                </button>
              </li>
            ))}
          </ul>
        </aside>

        {/* Main */}
        <div className="shop__main">
          <div className="shop__toolbar">
            <span className="shop__results">{total} results</span>
            <select
              className="sort-select"
              value={sort}
              onChange={e => setSort(e.target.value)}
              aria-label="Sort by"
            >
              {SORT_OPTIONS.map(o => <option key={o} value={o}>{o}</option>)}
            </select>
          </div>

          {loading ? (
            <div className="products-skeleton">
              {[...Array(6)].map((_, i) => <div key={i} className="skeleton-card" />)}
            </div>
          ) : products.length === 0 ? (
            <div className="shop__empty">
              <span>🎵</span>
              <p>No products found. Try a different search.</p>
            </div>
          ) : (
            <div className="products-grid">
              {products.map(p => <ProductCard key={p.id} product={p} />)}
            </div>
          )}
        </div>
      </div>
    </main>
  );
}
