import React from 'react'
import {
  ShoppingBag,
  Search,
  Package,
  Layers,
  Sparkles,
  SlidersHorizontal
} from 'lucide-react'

export default function Header({
  cartCount,
  onOpenCart,
  activeView,
  setActiveView,
  searchQuery,
  setSearchQuery,
  categories,
  selectedCategory,
  setSelectedCategory
}) {
  return (
    <header className="border-b border-slate-800/80 bg-[#07090E]/90 backdrop-blur-xl sticky top-0 z-40">
      <div className="max-w-7xl mx-auto px-6 h-16 flex items-center justify-between">
        <div className="flex items-center space-x-8">
          <div
            onClick={() => setActiveView('store')}
            className="flex items-center space-x-3 cursor-pointer group"
          >
            <div className="h-9 w-9 rounded-xl bg-gradient-to-tr from-indigo-600 via-indigo-500 to-violet-500 flex items-center justify-center shadow-lg shadow-indigo-500/25 group-hover:scale-105 transition-transform">
              <Layers className="h-5 w-5 text-white" />
            </div>
            <div>
              <span className="font-extrabold text-base tracking-tight bg-gradient-to-r from-white via-slate-200 to-slate-400 bg-clip-text text-transparent">
                AuraTech Gear
              </span>
              <span className="hidden sm:inline-block text-[10px] ml-2 px-2 py-0.5 rounded-full bg-indigo-500/10 text-indigo-400 border border-indigo-500/20 font-mono">
                .NET E-Commerce
              </span>
            </div>
          </div>

          <div className="hidden md:flex items-center relative w-72">
            <Search className="absolute left-3 top-2.5 h-4 w-4 text-slate-500" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Search gear, switches, audio..."
              className="w-full pl-9 pr-4 py-1.5 rounded-xl bg-slate-900/90 border border-slate-800 text-white placeholder-slate-500 text-xs focus:outline-none focus:border-indigo-500 transition-colors"
            />
          </div>
        </div>

        <div className="flex items-center space-x-3">
          <button
            onClick={() => setActiveView(activeView === 'store' ? 'orders' : 'store')}
            className={`inline-flex items-center space-x-1.5 px-3 py-1.5 rounded-xl text-xs font-semibold border transition-all ${
              activeView === 'orders'
                ? 'bg-indigo-600/20 text-indigo-300 border-indigo-500/40'
                : 'bg-slate-900 border-slate-800 text-slate-400 hover:text-white'
            }`}
          >
            <Package className="h-3.5 w-3.5" />
            <span>{activeView === 'orders' ? 'Back to Store' : 'My Orders'}</span>
          </button>

          <button
            onClick={onOpenCart}
            className="relative inline-flex items-center space-x-2 px-3.5 py-1.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-bold shadow-lg shadow-indigo-600/25 transition-all"
          >
            <ShoppingBag className="h-4 w-4" />
            <span>Cart</span>
            {cartCount > 0 && (
              <span className="h-5 w-5 rounded-full bg-white text-indigo-900 text-[10px] font-extrabold flex items-center justify-center">
                {cartCount}
              </span>
            )}
          </button>
        </div>
      </div>

      {activeView === 'store' && (
        <div className="border-t border-slate-800/60 bg-[#0B0F17]/40 px-6 py-2.5">
          <div className="max-w-7xl mx-auto flex items-center space-x-2 overflow-x-auto scrollbar-none">
            <button
              onClick={() => setSelectedCategory('all')}
              className={`whitespace-nowrap px-3 py-1 rounded-lg text-xs font-medium transition-all ${
                selectedCategory === 'all'
                  ? 'bg-indigo-600 text-white shadow-sm'
                  : 'bg-slate-900/80 border border-slate-800/80 text-slate-400 hover:text-white'
              }`}
            >
              All Gear
            </button>
            {categories.map((c) => (
              <button
                key={c.id}
                onClick={() => setSelectedCategory(c.slug)}
                className={`whitespace-nowrap px-3 py-1 rounded-lg text-xs font-medium transition-all ${
                  selectedCategory === c.slug
                    ? 'bg-indigo-600 text-white shadow-sm'
                    : 'bg-slate-900/80 border border-slate-800/80 text-slate-400 hover:text-white'
                }`}
              >
                {c.name}
              </button>
            ))}
          </div>
        </div>
      )}
    </header>
  )
}
