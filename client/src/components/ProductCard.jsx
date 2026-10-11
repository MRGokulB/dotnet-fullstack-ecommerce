import React, { useState } from 'react'
import { Star, ShoppingBag, Check, Zap } from 'lucide-react'

export default function ProductCard({ product, onAddToCart }) {
  const [adding, setAdding] = useState(false)

  const handleAdd = async () => {
    setAdding(true)
    await onAddToCart(product.id, 1)
    setTimeout(() => setAdding(false), 800)
  }

  return (
    <div className="group rounded-2xl bg-[#0E131F]/80 border border-slate-800/80 hover:border-indigo-500/50 hover:bg-[#121828]/90 transition-all duration-300 flex flex-col justify-between overflow-hidden shadow-xl hover:shadow-2xl hover:shadow-indigo-950/40">
      <div className="relative aspect-[4/3] overflow-hidden bg-slate-900">
        <img
          src={product.imageUrl}
          alt={product.name}
          className="w-full h-full object-cover object-center group-hover:scale-105 transition-transform duration-500"
          loading="lazy"
        />

        {product.isFeatured && (
          <div className="absolute top-3 left-3 px-2.5 py-0.5 rounded-full bg-indigo-600/90 backdrop-blur-md text-white text-[10px] font-bold uppercase tracking-wider flex items-center space-x-1 shadow-md">
            <Zap className="h-3 w-3 fill-current" />
            <span>Featured</span>
          </div>
        )}

        <div className="absolute bottom-3 left-3 px-2 py-0.5 rounded-md bg-black/60 backdrop-blur-md text-white text-[10px] font-mono flex items-center space-x-1">
          <Star className="h-3 w-3 text-amber-400 fill-amber-400" />
          <span className="font-bold">{product.rating.toFixed(1)}</span>
          <span className="text-slate-400">({product.reviewCount})</span>
        </div>
      </div>

      <div className="p-5 flex-1 flex flex-col justify-between space-y-4">
        <div className="space-y-1.5">
          <span className="text-[10px] font-mono uppercase tracking-wider text-indigo-400 font-semibold">
            {product.categoryName || 'Developer Gear'}
          </span>
          <h3 className="font-bold text-sm text-white line-clamp-1 group-hover:text-indigo-300 transition-colors">
            {product.name}
          </h3>
          <p className="text-xs text-slate-400 line-clamp-2 leading-relaxed">
            {product.description}
          </p>
        </div>

        <div className="pt-3 border-t border-slate-800/80 flex items-center justify-between">
          <div>
            <div className="text-[10px] text-slate-500 font-mono">Price USD</div>
            <div className="text-lg font-extrabold text-white font-mono">
              ${product.price.toFixed(2)}
            </div>
          </div>

          <button
            onClick={handleAdd}
            disabled={adding}
            className={`px-3.5 py-2 rounded-xl text-xs font-bold transition-all flex items-center space-x-1.5 shadow-md ${
              adding
                ? 'bg-emerald-600 text-white shadow-emerald-600/30'
                : 'bg-indigo-600 hover:bg-indigo-500 text-white shadow-indigo-600/25'
            }`}
          >
            {adding ? (
              <>
                <Check className="h-3.5 w-3.5" />
                <span>Added</span>
              </>
            ) : (
              <>
                <ShoppingBag className="h-3.5 w-3.5" />
                <span>Add to Cart</span>
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  )
}
