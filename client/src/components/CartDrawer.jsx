import React, { useState } from 'react'
import {
  X,
  Trash2,
  Plus,
  Minus,
  ShoppingBag,
  CreditCard,
  ArrowRight,
  RefreshCw,
  Sparkles,
  Truck
} from 'lucide-react'

export default function CartDrawer({
  isOpen,
  onClose,
  cart,
  onUpdateQuantity,
  onRemoveItem,
  sessionId,
  addToast,
  onOrderSuccess
}) {
  const [customerEmail, setCustomerEmail] = useState('developer@enterprise.com')
  const [customerName, setCustomerName] = useState('Gokul B')
  const [shippingAddress, setShippingAddress] = useState('100 Silicon Way, Tech Park, Suite 400')
  const [loading, setLoading] = useState(false)
  const [lastOrder, setLastOrder] = useState(null)
  const [simulating, setSimulating] = useState(false)

  if (!isOpen) return null

  const subtotal = cart?.subtotal || 0
  const tax = Math.round(subtotal * 0.08 * 100) / 100
  const shipping = subtotal > 200 || subtotal === 0 ? 0 : 15
  const total = subtotal + tax + shipping

  const freeShippingThreshold = 200
  const freeShippingProgress = Math.min((subtotal / freeShippingThreshold) * 100, 100)

  const handleCheckout = async (e) => {
    e.preventDefault()
    if (!cart?.items || cart.items.length === 0) {
      addToast('error', 'Empty Cart', 'Please add items to your cart before checking out.')
      return
    }

    setLoading(true)

    try {
      const response = await fetch('/api/orders/checkout-cart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          sessionId,
          customerEmail,
          customerName,
          shippingAddress
        })
      })

      const data = await response.json()
      if (!response.ok) {
        throw new Error(data.message || data.title || 'Checkout session failed')
      }

      setLastOrder(data)
      onOrderSuccess()
      addToast('success', 'Order Dispatched', `Created order #${data.orderId.substring(0, 8)}...`)

      if (data.checkoutUrl) {
        window.location.href = data.checkoutUrl
      }
    } catch (err) {
      addToast('error', 'Checkout Error', err.message)
    } finally {
      setLoading(false)
    }
  }

  const handleSimulatePayment = async (orderId) => {
    setSimulating(true)
    try {
      const res = await fetch(`/api/payments/simulate-success/${orderId}`, {
        method: 'POST'
      })
      const data = await res.json()
      if (res.ok) {
        addToast('success', 'Payment Verified', 'Order paid via simulated webhook!')
        onOrderSuccess()
        onClose()
      } else {
        throw new Error(data.message || 'Simulation failed')
      }
    } catch (err) {
      addToast('error', 'Simulation Error', err.message)
    } finally {
      setSimulating(false)
    }
  }

  return (
    <div className="fixed inset-0 z-50 overflow-hidden bg-black/70 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="absolute inset-y-0 right-0 max-w-full flex pl-10">
        <div className="w-screen max-w-md bg-[#0A0D15] border-l border-slate-800 shadow-2xl flex flex-col justify-between">
          <div className="p-6 border-b border-slate-800/80 flex items-center justify-between">
            <div className="flex items-center space-x-2">
              <ShoppingBag className="h-5 w-5 text-indigo-400" />
              <h2 className="text-base font-bold text-white">Your Shopping Cart</h2>
              <span className="text-xs font-mono px-2 py-0.5 rounded-full bg-slate-800 text-slate-300">
                {cart?.totalItems || 0}
              </span>
            </div>
            <button
              onClick={onClose}
              className="p-1 rounded-lg text-slate-400 hover:text-white transition-colors"
            >
              <X className="h-5 w-5" />
            </button>
          </div>

          <div className="px-6 py-3 bg-[#0E131F]/90 border-b border-slate-800/60">
            <div className="flex items-center justify-between text-[11px] mb-1.5">
              <span className="text-slate-300 flex items-center space-x-1.5">
                <Truck className="h-3.5 w-3.5 text-indigo-400" />
                <span>
                  {subtotal >= freeShippingThreshold
                    ? 'Free Express Shipping Unlocked!'
                    : `Add $${(freeShippingThreshold - subtotal).toFixed(2)} more for Free Shipping`}
                </span>
              </span>
              <span className="font-mono text-slate-400">{Math.round(freeShippingProgress)}%</span>
            </div>
            <div className="h-1.5 rounded-full bg-slate-800 overflow-hidden">
              <div
                style={{ width: `${freeShippingProgress}%` }}
                className="h-full bg-gradient-to-r from-indigo-500 to-emerald-400 transition-all duration-300"
              />
            </div>
          </div>

          <div className="flex-1 overflow-y-auto p-6 space-y-4">
            {!cart?.items || cart.items.length === 0 ? (
              <div className="py-20 text-center space-y-3">
                <ShoppingBag className="h-10 w-10 text-slate-600 mx-auto" />
                <p className="text-sm font-semibold text-slate-300">Your cart is currently empty</p>
                <p className="text-xs text-slate-500">Explore our curated developer hardware and add gear to begin.</p>
              </div>
            ) : (
              cart.items.map((item) => (
                <div
                  key={item.id}
                  className="p-3.5 rounded-xl bg-slate-900/60 border border-slate-800/80 flex items-center space-x-3.5"
                >
                  {item.productImageUrl && (
                    <img
                      src={item.productImageUrl}
                      alt={item.productName}
                      className="h-16 w-16 rounded-lg object-cover bg-slate-800 flex-shrink-0"
                    />
                  )}

                  <div className="flex-1 min-w-0">
                    <h4 className="text-xs font-bold text-white truncate">{item.productName}</h4>
                    <div className="text-xs font-mono font-semibold text-indigo-300 mt-0.5">
                      ${item.unitPrice.toFixed(2)}
                    </div>

                    <div className="flex items-center space-x-2 mt-2">
                      <div className="inline-flex items-center border border-slate-700 rounded-lg bg-slate-950">
                        <button
                          onClick={() => onUpdateQuantity(item.id, item.quantity - 1)}
                          className="p-1 hover:text-white text-slate-400"
                        >
                          <Minus className="h-3 w-3" />
                        </button>
                        <span className="px-2 text-xs font-mono font-bold text-white">{item.quantity}</span>
                        <button
                          onClick={() => onUpdateQuantity(item.id, item.quantity + 1)}
                          className="p-1 hover:text-white text-slate-400"
                        >
                          <Plus className="h-3 w-3" />
                        </button>
                      </div>

                      <button
                        onClick={() => onRemoveItem(item.id)}
                        className="text-slate-500 hover:text-rose-400 p-1 transition-colors"
                      >
                        <Trash2 className="h-3.5 w-3.5" />
                      </button>
                    </div>
                  </div>

                  <div className="text-xs font-mono font-bold text-white text-right">
                    ${item.lineTotal.toFixed(2)}
                  </div>
                </div>
              ))
            )}
          </div>

          {cart?.items && cart.items.length > 0 && (
            <div className="p-6 border-t border-slate-800/80 bg-[#07090E] space-y-4">
              <div className="space-y-1.5 text-xs text-slate-400">
                <div className="flex justify-between">
                  <span>Subtotal</span>
                  <span className="text-white font-mono">${subtotal.toFixed(2)}</span>
                </div>
                <div className="flex justify-between">
                  <span>Estimated Tax (8%)</span>
                  <span className="text-white font-mono">${tax.toFixed(2)}</span>
                </div>
                <div className="flex justify-between">
                  <span>Shipping</span>
                  <span className="text-white font-mono">{shipping === 0 ? 'FREE' : `$${shipping.toFixed(2)}`}</span>
                </div>
                <div className="flex justify-between text-sm font-bold text-white pt-2 border-t border-slate-800">
                  <span>Total Amount</span>
                  <span className="text-indigo-400 font-mono">${total.toFixed(2)} USD</span>
                </div>
              </div>

              <form onSubmit={handleCheckout} className="space-y-3 pt-2">
                <div>
                  <input
                    type="email"
                    required
                    value={customerEmail}
                    onChange={(e) => setCustomerEmail(e.target.value)}
                    placeholder="Email address for receipt"
                    className="w-full px-3 py-2 rounded-xl bg-slate-900 border border-slate-800 text-white placeholder-slate-500 text-xs focus:outline-none focus:border-indigo-500"
                  />
                </div>

                <button
                  type="submit"
                  disabled={loading}
                  className="w-full py-2.5 rounded-xl bg-gradient-to-r from-indigo-600 to-violet-600 hover:from-indigo-500 hover:to-violet-500 text-white font-bold text-xs shadow-lg shadow-indigo-600/30 flex items-center justify-center space-x-2 transition-all disabled:opacity-50"
                >
                  {loading ? (
                    <RefreshCw className="h-4 w-4 animate-spin" />
                  ) : (
                    <>
                      <CreditCard className="h-4 w-4" />
                      <span>Checkout with Stripe</span>
                      <ArrowRight className="h-4 w-4" />
                    </>
                  )}
                </button>
              </form>

              {lastOrder && (
                <div className="p-3 rounded-xl bg-indigo-950/40 border border-indigo-500/30 space-y-2">
                  <div className="text-[11px] text-indigo-300">
                    Order created: <span className="font-mono text-white">#{lastOrder.orderId.substring(0, 8)}...</span>
                  </div>
                  <button
                    type="button"
                    disabled={simulating}
                    onClick={() => handleSimulatePayment(lastOrder.orderId)}
                    className="w-full py-1.5 rounded-lg bg-emerald-600 hover:bg-emerald-500 text-white text-[11px] font-bold flex items-center justify-center space-x-1.5 shadow-md transition-all disabled:opacity-50"
                  >
                    {simulating ? (
                      <RefreshCw className="h-3 w-3 animate-spin" />
                    ) : (
                      <Sparkles className="h-3 w-3" />
                    )}
                    <span>Simulate Instant Webhook Success</span>
                  </button>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
