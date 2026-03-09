package com.pom.delivery.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.pom.delivery.R
import com.pom.delivery.models.MenuItem
import java.io.File

class MenuAdapter(
    private var items: List<MenuItem>,
    private val onClick: (MenuItem) -> Unit
) : RecyclerView.Adapter<MenuAdapter.ViewHolder>() {

    class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val ivFood: ImageView = view.findViewById(R.id.ivFood)
        val tvName: TextView = view.findViewById(R.id.tvFoodName)
        val tvDesc: TextView = view.findViewById(R.id.tvFoodDesc)
        val tvPriceNormal: TextView = view.findViewById(R.id.tvPriceNormal)
        val tvPriceSpecial: TextView = view.findViewById(R.id.tvPriceSpecial)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_menu, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val item = items[position]
        holder.tvName.text = item.name
        holder.tvDesc.text = item.description
        holder.tvPriceNormal.text = "ธรรมดา ฿${String.format("%.0f", item.priceNormal)}"
        holder.tvPriceSpecial.text = "พิเศษ ฿${String.format("%.0f", item.priceSpecial)}"

        if (item.imagePath.isNotEmpty() && File(item.imagePath).exists()) {
            Glide.with(holder.itemView.context)
                .load(File(item.imagePath))
                .centerCrop()
                .into(holder.ivFood)
        } else {
            holder.ivFood.setImageResource(R.drawable.ic_food_placeholder)
        }

        holder.itemView.setOnClickListener { onClick(item) }
    }

    override fun getItemCount() = items.size

    fun updateData(newItems: List<MenuItem>) {
        items = newItems
        notifyDataSetChanged()
    }
}
