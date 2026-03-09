package com.pom.delivery.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageButton
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.pom.delivery.R
import com.pom.delivery.models.CartItem

class CartAdapter(
    private var items: List<CartItem>,
    private val onRemove: (CartItem) -> Unit,
    private val onQuantityChange: (CartItem, Int) -> Unit
) : RecyclerView.Adapter<CartAdapter.ViewHolder>() {

    class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val tvName: TextView = view.findViewById(R.id.tvCartItemName)
        val tvSize: TextView = view.findViewById(R.id.tvCartItemSize)
        val tvQuantity: TextView = view.findViewById(R.id.tvCartItemQuantity)
        val tvPrice: TextView = view.findViewById(R.id.tvCartItemPrice)
        val btnPlus: ImageButton = view.findViewById(R.id.btnPlus)
        val btnMinus: ImageButton = view.findViewById(R.id.btnMinus)
        val btnRemove: ImageButton = view.findViewById(R.id.btnRemove)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_cart, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val item = items[position]
        holder.tvName.text = item.menuItem.name
        holder.tvSize.text = item.sizeType
        holder.tvQuantity.text = item.quantity.toString()
        holder.tvPrice.text = "฿${String.format("%.0f", item.totalPrice)}"

        holder.btnPlus.setOnClickListener {
            onQuantityChange(item, item.quantity + 1)
        }
        holder.btnMinus.setOnClickListener {
            onQuantityChange(item, item.quantity - 1)
        }
        holder.btnRemove.setOnClickListener {
            onRemove(item)
        }
    }

    override fun getItemCount() = items.size

    fun updateData(newItems: List<CartItem>) {
        items = newItems
        notifyDataSetChanged()
    }
}
