package com.pom.delivery.activities

import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import com.pom.delivery.R
import com.pom.delivery.adapters.OrderAdapter
import com.pom.delivery.database.DatabaseHelper
import com.pom.delivery.databinding.ActivityRiderBinding

class RiderActivity : AppCompatActivity() {

    private lateinit var binding: ActivityRiderBinding
    private lateinit var db: DatabaseHelper
    private lateinit var prefs: SharedPreferences
    private lateinit var adapter: OrderAdapter
    private val handler = Handler(Looper.getMainLooper())
    private val refreshRunnable = object : Runnable {
        override fun run() {
            refreshOrders()
            handler.postDelayed(this, 5000)
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityRiderBinding.inflate(layoutInflater)
        setContentView(binding.root)

        db = DatabaseHelper(this)
        prefs = getSharedPreferences("pom_prefs", MODE_PRIVATE)

        val riderName = prefs.getString("user_name", "ไรเดอร์")
        binding.tvRiderName.text = "ไรเดอร์: $riderName"

        binding.rvOrders.layoutManager = LinearLayoutManager(this)
        adapter = OrderAdapter(db.getPendingOrders()) { order ->
            val intent = Intent(this, RiderOrderDetailActivity::class.java)
            intent.putExtra("order_id", order.id)
            startActivity(intent)
        }
        binding.rvOrders.adapter = adapter

        binding.btnLogout.setOnClickListener {
            prefs.edit().clear().apply()
            startActivity(Intent(this, LoginActivity::class.java).apply {
                flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            })
            finish()
        }

        binding.btnRefresh.setOnClickListener { refreshOrders() }
    }

    override fun onResume() {
        super.onResume()
        refreshOrders()
        handler.postDelayed(refreshRunnable, 5000)
    }

    override fun onPause() {
        super.onPause()
        handler.removeCallbacks(refreshRunnable)
    }

    private fun refreshOrders() {
        val orders = db.getPendingOrders()
        adapter.updateData(orders)
        if (orders.isEmpty()) {
            binding.tvNoOrders.visibility = View.VISIBLE
            binding.rvOrders.visibility = View.GONE
        } else {
            binding.tvNoOrders.visibility = View.GONE
            binding.rvOrders.visibility = View.VISIBLE
        }
    }
}
