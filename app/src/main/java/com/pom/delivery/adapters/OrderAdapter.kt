package com.pom.delivery.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.pom.delivery.R
import com.pom.delivery.models.Order
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

class OrderAdapter(
    private var items: List<Order>,
    private val onClick: (Order) -> Unit
) : RecyclerView.Adapter<OrderAdapter.ViewHolder>() {

    class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val tvOrderId: TextView = view.findViewById(R.id.tvOrderId)
        val tvCustomerName: TextView = view.findViewById(R.id.tvCustomerName)
        val tvFoodTotal: TextView = view.findViewById(R.id.tvFoodTotal)
        val tvTime: TextView = view.findViewById(R.id.tvOrderTime)
        val tvStatus: TextView = view.findViewById(R.id.tvOrderStatus)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_order, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val order = items[position]
        holder.tvOrderId.text = "ออเดอร์ #${order.id}"
        holder.tvCustomerName.text = order.customerName
        holder.tvFoodTotal.text = "฿${String.format("%.0f", order.totalFoodPrice)}"
        val sdf = SimpleDateFormat("HH:mm dd/MM/yyyy", Locale.getDefault())
        holder.tvTime.text = sdf.format(Date(order.timestamp))
        holder.tvStatus.text = when (order.status) {
            "pending" -> "รอรับ"
            "accepted" -> "รับแล้ว"
            "delivered" -> "ส่งแล้ว"
            else -> order.status
        }
        holder.itemView.setOnClickListener { onClick(order) }
    }

    override fun getItemCount() = items.size

    fun updateData(newItems: List<Order>) {
        items = newItems
        notifyDataSetChanged()
    }
}
