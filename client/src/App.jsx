import React, { useState, useEffect, useCallback } from 'react'
import Header from './components/Header'
import ProductCard from './components/ProductCard'
import CartDrawer from './components/CartDrawer'
import OrdersView from './components/OrdersView'
import Toast from './components/Toast'
import { Sparkles, ArrowRight, Zap, ShieldCheck, Cpu } from 'lucide-react'

export default function App() {
  const [sessionId, setSessionId] = useState('')
  const [products, setProducts] = useState([])
  const [categories, setCategories] = useState([])
  const [selectedCategory, setSelectedCategory] = useState('all')
  const [searchQuery, setSearchQuery] = useState('')
  const [sortBy, setSortBy] = useState('featured')
  const [cart, setCart] = useState(null)
  const [isCartOpen, setIsCartOpen] = useState(false)
  const [orders, setOrders] = useState([])
  const [ordersLoading, setOrdersLoading] = useState(false)
  const [activeView, setActiveView] = useState('store')
  const [toasts, setToasts] = useState([])

  const addToast = useCallback((type, title, message) => {
    const id = Date.now() + Math.random().toString(36).substring(2, 6)
    setToasts((prev) => [...prev, { id, type, title, message }])
    setTimeout(() => {
      setToasts((prev) => prev.filter((t) => t.id !== id))
    }, 4500)
  }, [])

  const removeToast = useCallback((id) => {
    setToasts((prev) => prev.filter((t) => t.id !== id))
  }, [])

  useEffect(() => {
    let sid = localStorage.getItem('auratech_session_id')
    if (!sid) {
      sid = 'sess_' + Math.random().toString(36).substring(2, 12) + Date.now().toString(36)
      localStorage.setItem('auratech_session_id', sid)
    }
    setSessionId(sid)
  }, [])

  const fetchCart = useCallback(async (sid) => {
    if (!sid) return
    try {
      const res = await fetch(`/api/cart/${sid}`)
      if (res.ok) {
        const data = await res.json()
        setCart(data)
      }
    } catch {
    }
  }, [])

  const fetchCategories = useCallback(async () => {
    try {
      const res = await fetch('/api/products/categories')
      if (res.ok) {
        const data = await res.json()
        setCategories(data)
      }
    } catch {
    }
  }, [])

  const fetchProducts = useCallback(async () => {
    try {
      const params = new URLSearchParams()
      if (selectedCategory && selectedCategory !== 'all') {
        params.append('category', selectedCategory)
      }
      if (searchQuery) {
        params.append('search', searchQuery)
      }
      if (sortBy) {
        params.append('sort', sortBy)
      }

      const res = await fetch(`/api/products?${params.toString()}`)
      if (res.ok) {
        const data = await res.json()
        setProducts(data)
      }
    } catch {
    }
  }, [selectedCategory, searchQuery, sortBy])

  const fetchOrders = useCallback(async () => {
    setOrdersLoading(true)
    try {
      const res = await fetch('/api/orders')
      if (res.ok) {
        const data = await res.json()
        setOrders(data)
      }
    } catch {
    } finally {
      setOrdersLoading(false)
    }
  }, [])

  useEffect(() => {
    fetchCategories()
  }, [fetchCategories])

  useEffect(() => {
    fetchProducts()
  }, [fetchProducts])

  useEffect(() => {
    if (sessionId) {
      fetchCart(sessionId)
    }
  }, [sessionId, fetchCart])

  useEffect(() => {
    fetchOrders()

    const params = new URLSearchParams(window.location.search)
    const orderId = params.get('order_id')
    if (orderId) {
      addToast('success', 'Checkout Complete', 'Payment verification successful!')
      setActiveView('orders')
    }
  }, [fetchOrders, addToast])

  const handleAddToCart = async (productId, quantity) => {
    if (!sessionId) return
    try {
      const res = await fetch('/api/cart/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ productId, quantity, sessionId })
      })

      if (res.ok) {
        const updatedCart = await res.json()
        setCart(updatedCart)
        addToast('success', 'Cart Updated', 'Added gear to your cart.')
      } else {
        throw new Error('Failed to add item')
      }
    } catch (err) {
      addToast('error', 'Cart Error', err.message)
    }
  }

  const handleUpdateQuantity = async (itemId, quantity) => {
    try {
      const res = await fetch(`/api/cart/items/${itemId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ quantity })
      })
      if (res.ok) {
        const updatedCart = await res.json()
        setCart(updatedCart)
      }
    } catch {
    }
  }

  const handleRemoveItem = async (itemId) => {
    try {
      const res = await fetch(`/api/cart/items/${itemId}`, {
        method: 'DELETE'
      })
      if (res.ok) {
        const updatedCart = await res.json()
        setCart(updatedCart)
        addToast('info', 'Item Removed', 'Removed item from cart.')
      }
    } catch {
    }
  }

  return (
    <div className="min-h-screen bg-[#07090E] text-slate-100 flex flex-col font-sans selection:bg-indigo-500 selection:text-white">
      <Header
        cartCount={cart?.totalItems || 0}
        onOpenCart={() => setIsCartOpen(true)}
        activeView={activeView}
        setActiveView={setActiveView}
        searchQuery={searchQuery}
        setSearchQuery={setSearchQuery}
        categories={categories}
        selectedCategory={selectedCategory}
        setSelectedCategory={setSelectedCategory}
      />

      <main className="flex-1 max-w-7xl w-full mx-auto px-6 py-8">
        {activeView === 'store' ? (
          <div className="space-y-10">
            <section className="relative rounded-3xl overflow-hidden bg-gradient-to-r from-indigo-950/60 via-[#0E131F] to-[#0A0E18] border border-slate-800/80 p-8 md:p-12 shadow-2xl">
              <div className="relative z-10 max-w-2xl space-y-4">
                <div className="inline-flex items-center space-x-2 px-3 py-1 rounded-full bg-indigo-500/10 border border-indigo-500/20 text-indigo-400 text-xs font-semibold">
                  <Sparkles className="h-3.5 w-3.5" />
                  <span>Curated for Software Engineers & Designers</span>
                </div>
                <h1 className="text-3xl md:text-5xl font-extrabold tracking-tight text-white leading-tight">
                  Precision Hardware & Developer Gear
                </h1>
                <p className="text-sm md:text-base text-slate-400 leading-relaxed">
                  Engineered with Clean Architecture in ASP.NET Core, EF Core multi-item relational persistence, and Stripe payment integration.
                </p>
                <div className="pt-2 flex items-center space-x-4">
                  <button
                    onClick={() => setIsCartOpen(true)}
                    className="inline-flex items-center space-x-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-indigo-600 to-violet-600 hover:from-indigo-500 hover:to-violet-500 text-white font-bold text-xs shadow-lg shadow-indigo-600/30 transition-all"
                  >
                    <span>View Shopping Bag</span>
                    <ArrowRight className="h-4 w-4" />
                  </button>
                  <span className="text-xs font-mono text-slate-500">
                    Free shipping on orders over $200
                  </span>
                </div>
              </div>

              <div className="absolute right-0 top-0 bottom-0 w-1/3 bg-gradient-to-l from-indigo-600/10 to-transparent pointer-events-none" />
            </section>

            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-slate-800/80 pb-4">
              <div className="flex items-center space-x-2 text-xs text-slate-400 font-mono">
                <span>Showing</span>
                <span className="font-bold text-white">{products.length}</span>
                <span>products</span>
              </div>

              <div className="flex items-center space-x-2">
                <span className="text-xs text-slate-400 font-mono">Sort:</span>
                <select
                  value={sortBy}
                  onChange={(e) => setSortBy(e.target.value)}
                  className="px-3 py-1.5 rounded-xl bg-slate-900 border border-slate-800 text-white text-xs font-medium focus:outline-none focus:border-indigo-500"
                >
                  <option value="featured">Featured First</option>
                  <option value="price-asc">Price: Low to High</option>
                  <option value="price-desc">Price: High to Low</option>
                  <option value="rating">Highest Rated</option>
                </select>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
              {products.map((product) => (
                <ProductCard
                  key={product.id}
                  product={product}
                  onAddToCart={handleAddToCart}
                />
              ))}
            </div>
          </div>
        ) : (
          <OrdersView
            orders={orders}
            onRefresh={fetchOrders}
            loading={ordersLoading}
            addToast={addToast}
          />
        )}
      </main>

      <footer className="border-t border-slate-800/80 bg-[#07090E] py-8 text-center text-xs text-slate-500 space-y-2">
        <div className="flex items-center justify-center space-x-6 text-slate-400 font-mono text-[11px]">
          <span>ASP.NET Core .NET 7/8</span>
          <span>•</span>
          <span>EF Core Relational Catalog</span>
          <span>•</span>
          <span>Stripe Multi-Item Checkout</span>
          <span>•</span>
          <span>Vite + React 18</span>
        </div>
        <p className="text-slate-600">
          AuraTech Gear • Fullstack .NET E-Commerce Architecture Showcase
        </p>
      </footer>

      <CartDrawer
        isOpen={isCartOpen}
        onClose={() => setIsCartOpen(false)}
        cart={cart}
        sessionId={sessionId}
        onUpdateQuantity={handleUpdateQuantity}
        onRemoveItem={handleRemoveItem}
        addToast={addToast}
        onOrderSuccess={() => {
          fetchOrders()
          if (sessionId) fetchCart(sessionId)
        }}
      />

      <Toast toasts={toasts} onDismiss={removeToast} />
    </div>
  )
}
