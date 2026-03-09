package com.pom.delivery.activities

import android.Manifest
import android.content.Intent
import android.content.SharedPreferences
import android.content.pm.PackageManager
import android.location.Location
import android.os.Bundle
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.recyclerview.widget.LinearLayoutManager
import com.google.android.gms.location.FusedLocationProviderClient
import com.google.android.gms.location.LocationServices
import com.pom.delivery.R
import com.pom.delivery.adapters.CartAdapter
import com.pom.delivery.database.DatabaseHelper
import com.pom.delivery.databinding.ActivityCartBinding
import com.pom.delivery.models.CartItem
import com.pom.delivery.models.Order

class CartActivity : AppCompatActivity() {

    private lateinit var binding: ActivityCartBinding
    private lateinit var db: DatabaseHelper
    private lateinit var prefs: SharedPreferences
    private lateinit var adapter: CartAdapter
    private lateinit var fusedLocationClient: FusedLocationProviderClient
    private var currentLat = 0.0
    private var currentLng = 0.0
    private val LOCATION_PERMISSION = 1001

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityCartBinding.inflate(layoutInflater)
        setContentView(binding.root)

        db = DatabaseHelper(this)
        prefs = getSharedPreferences("pom_prefs", MODE_PRIVATE)
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(this)

        val phone = prefs.getString("user_phone", "") ?: ""

        binding.rvCart.layoutManager = LinearLayoutManager(this)
        adapter = CartAdapter(
            db.getCartItems(phone),
            onRemove = { cartItem ->
                db.removeCartItem(cartItem.id)
                refreshCart()
            },
            onQuantityChange = { cartItem, newQty ->
                if (newQty <= 0) {
                    db.removeCartItem(cartItem.id)
                } else {
                    db.updateCartItemQuantity(cartItem.id, newQty)
                }
                refreshCart()
            }
        )
        binding.rvCart.adapter = adapter
        refreshCart()

        binding.btnOrder.setOnClickListener {
            val cartItems = db.getCartItems(phone)
            if (cartItems.isEmpty()) {
                Toast.makeText(this, "ตะกร้าว่าง", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            requestLocationAndOrder(cartItems)
        }

        binding.btnClearCart.setOnClickListener {
            AlertDialog.Builder(this)
                .setTitle("ยกเลิกทั้งหมด")
                .setMessage("ต้องการลบอาหารทั้งหมดในตะกร้า?")
                .setPositiveButton("ยืนยัน") { _, _ ->
                    db.clearCart(phone)
                    refreshCart()
                    Toast.makeText(this, "ลบอาหารทั้งหมดแล้ว", Toast.LENGTH_SHORT).show()
                }
                .setNegativeButton("ยกเลิก", null)
                .show()
        }

        binding.btnBack.setOnClickListener { finish() }
    }

    private fun refreshCart() {
        val phone = prefs.getString("user_phone", "") ?: ""
        val items = db.getCartItems(phone)
        adapter.updateData(items)
        val total = items.sumOf { it.totalPrice }
        binding.tvTotal.text = "รวม: ฿${String.format("%.0f", total)}"
        if (items.isEmpty()) {
            binding.tvEmpty.visibility = View.VISIBLE
            binding.rvCart.visibility = View.GONE
        } else {
            binding.tvEmpty.visibility = View.GONE
            binding.rvCart.visibility = View.VISIBLE
        }
    }

    private fun requestLocationAndOrder(cartItems: List<CartItem>) {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.ACCESS_FINE_LOCATION), LOCATION_PERMISSION)
            return
        }
        fusedLocationClient.lastLocation.addOnSuccessListener { location: Location? ->
            currentLat = location?.latitude ?: 13.7563
            currentLng = location?.longitude ?: 100.5018
            submitOrder(cartItems)
        }.addOnFailureListener {
            currentLat = 13.7563
            currentLng = 100.5018
            submitOrder(cartItems)
        }
    }

    private fun submitOrder(cartItems: List<CartItem>) {
        val phone = prefs.getString("user_phone", "") ?: ""
        val name = prefs.getString("user_name", "ลูกค้า") ?: "ลูกค้า"

        val itemsText = cartItems.joinToString("\n") { item ->
            "${item.menuItem.name} (${item.sizeType}) x${item.quantity} = ฿${String.format("%.0f", item.totalPrice)}"
        }
        val totalFood = cartItems.sumOf { it.totalPrice }

        val order = Order(
            customerName = name,
            customerPhone = phone,
            items = itemsText,
            totalFoodPrice = totalFood,
            latitude = currentLat,
            longitude = currentLng,
            status = "pending"
        )

        val orderId = db.createOrder(order)
        if (orderId > 0) {
            db.clearCart(phone)
            refreshCart()
            AlertDialog.Builder(this)
                .setTitle("สั่งอาหารสำเร็จ!")
                .setMessage("ออเดอร์ #$orderId\n\n$itemsText\n\nรวมค่าอาหาร: ฿${String.format("%.0f", totalFood)}\n\nรอไรเดอร์รับออเดอร์...")
                .setPositiveButton("ตกลง") { _, _ -> finish() }
                .setCancelable(false)
                .show()
        } else {
            Toast.makeText(this, "เกิดข้อผิดพลาด", Toast.LENGTH_SHORT).show()
        }
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == LOCATION_PERMISSION && grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            val phone = prefs.getString("user_phone", "") ?: ""
            requestLocationAndOrder(db.getCartItems(phone))
        }
    }
}
