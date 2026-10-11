import React, { useState } from 'react'
import {
  Package,
  CheckCircle2,
  Clock,
  Sparkles,
  RefreshCw,
  Truck,
  ExternalLink
} from 'lucide-react'

export default function OrdersView({ orders, onRefresh, loading, addToast }) {
  const [simulatingId, setSimulatingId] = useState(null)

  const handleSimulatePayment = async (orderId) => {
    setSimulatingId(orderId)
    try {
      const res = await fetch(`/api/payments/simulate-success/${orderId}`, {
        method: 'POST'
      })
      const data = await res.json()
      if (res.ok) {
        addToast('success', 'Order Fulfilled', `Order ${orderId.substring(0, 8)} marked as Paid via webhook!`)
        onRefresh()
      } else {
        throw new Error(data.message || 'Simulation failed')
      }
    } catch (err) {
      addToast('error', 'Simulation Error', err.message)
    } finally {
      setSimulatingId(null)
    }
  }

  return (
    <div className="space-y-6 max-w-5xl mx-auto animate-in fade-in duration-200">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold tracking-tight text-white flex items-center space-x-2">
            <Package className="h-6 w-6 text-indigo-400" />
            <span>Order Fulfillment Ledger</span>
          </h2>
          <p className="text-xs text-slate-400 mt-1">
            Persisted in EF Core relational database with line item breakdown and Stripe transaction signatures.
          </p>
        </div>

        <button
          onClick={onRefresh}
          disabled={loading}
          className="inline-flex items-center space-x-2 px-3 py-1.5 rounded-xl bg-slate-900 border border-slate-800 text-slate-300 hover:text-white text-xs font-semibold transition-all"
        >
          <RefreshCw className={`h-3.5 w-3.5 ${loading ? 'animate-spin' : ''}`} />
          <span>Refresh</span>
        </button>
      </div>

      {!orders || orders.length === 0 ? (
        <div className="py-24 text-center space-y-3 bg-[#0E131F]/60 rounded-2xl border border-slate-800/80">
          <Clock className="h-10 w-10 text-slate-600 mx-auto" />
          <h3 className="text-sm font-semibold text-slate-300">No Orders Found</h3>
          <p className="text-xs text-slate-500">Add products to your cart and complete checkout to see transactions here.</p>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => (
            <div
              key={order.id}
              className="p-6 rounded-2xl bg-[#0E131F]/90 border border-slate-800/90 shadow-xl space-y-4"
            >
              <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2 border-b border-slate-800/80 pb-4">
                <div className="flex items-center space-x-3">
                  <span className="font-mono text-xs font-bold text-indigo-300">
                    Order #{order.id.substring(0, 8)}...
                  </span>
                  <span
                    className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-[10px] font-mono font-bold uppercase ${
                      order.status === 'Paid'
                        ? 'bg-emerald-500/15 text-emerald-400 border border-emerald-500/30'
                        : 'bg-amber-500/15 text-amber-400 border border-amber-500/30'
                    }`}
                  >
                    {order.status}
                  </span>
                </div>

                <div className="text-xs text-slate-400 font-mono">
                  {new Date(order.createdAtUtc).toLocaleString()}
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-xs">
                <div>
                  <span className="text-[10px] font-mono uppercase text-slate-500">Customer Details</span>
                  <div className="font-medium text-white mt-1">{order.customerName}</div>
                  <div className="text-slate-400">{order.customerEmail}</div>
                  <div className="text-slate-400 mt-1">{order.shippingAddress}</div>
                </div>

                <div className="md:text-right">
                  <span className="text-[10px] font-mono uppercase text-slate-500">Financial Summary</span>
                  <div className="text-slate-400 mt-1">Subtotal: ${order.subtotal.toFixed(2)}</div>
                  <div className="text-slate-400">Tax: ${order.tax.toFixed(2)} | Shipping: ${order.shippingFee.toFixed(2)}</div>
                  <div className="text-base font-extrabold text-white font-mono mt-1">
                    Total: ${order.totalAmount.toFixed(2)} {order.currency.toUpperCase()}
                  </div>
                </div>
              </div>

              <div className="border-t border-slate-800/80 pt-3">
                <div className="text-[10px] font-mono uppercase text-slate-500 mb-2">Ordered Items</div>
                <div className="space-y-1.5">
                  {order.items?.map((item) => (
                    <div key={item.id} className="flex justify-between text-xs text-slate-300">
                      <span>{item.productName} <span className="text-slate-500 font-mono">× {item.quantity}</span></span>
                      <span className="font-mono font-medium text-white">${item.totalPrice.toFixed(2)}</span>
                    </div>
                  ))}
                </div>
              </div>

              {order.status !== 'Paid' && (
                <div className="pt-2 flex justify-end">
                  <button
                    onClick={() => handleSimulatePayment(order.id)}
                    disabled={simulatingId === order.id}
                    className="inline-flex items-center space-x-1.5 px-3 py-1.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-bold shadow-md shadow-emerald-600/20 transition-all disabled:opacity-50"
                  >
                    {simulatingId === order.id ? (
                      <RefreshCw className="h-3.5 w-3.5 animate-spin" />
                    ) : (
                      <Sparkles className="h-3.5 w-3.5" />
                    )}
                    <span>Simulate Webhook Payment Confirmation</span>
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
