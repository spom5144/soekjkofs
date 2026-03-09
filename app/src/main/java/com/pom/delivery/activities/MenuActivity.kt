package com.pom.delivery.activities

import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.GridLayoutManager
import com.pom.delivery.R
import com.pom.delivery.adapters.MenuAdapter
import com.pom.delivery.database.DatabaseHelper
import com.pom.delivery.databinding.ActivityMenuBinding
import com.pom.delivery.models.MenuItem

class MenuActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMenuBinding
    private lateinit var db: DatabaseHelper
    private lateinit var prefs: SharedPreferences
    private lateinit var adapter: MenuAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMenuBinding.inflate(layoutInflater)
        setContentView(binding.root)

        db = DatabaseHelper(this)
        prefs = getSharedPreferences("pom_prefs", MODE_PRIVATE)

        val userName = prefs.getString("user_name", "ลูกค้า")
        binding.tvWelcome.text = "สวัสดี, $userName"

        binding.rvMenu.layoutManager = GridLayoutManager(this, 2)
        adapter = MenuAdapter(db.getAllMenuItems()) { menuItem ->
            showAddToCartDialog(menuItem)
        }
        binding.rvMenu.adapter = adapter

        binding.fabCart.setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
        }

        binding.fabAddMenu.setOnClickListener {
            startActivity(Intent(this, AddMenuActivity::class.java))
        }

        binding.btnLogout.setOnClickListener {
            prefs.edit().clear().apply()
            startActivity(Intent(this, LoginActivity::class.java))
            finish()
        }
    }

    override fun onResume() {
        super.onResume()
        adapter.updateData(db.getAllMenuItems())
    }

    private fun showAddToCartDialog(menuItem: MenuItem) {
        val options = arrayOf(
            "ธรรมดา - ฿${String.format("%.0f", menuItem.priceNormal)}",
            "พิเศษ - ฿${String.format("%.0f", menuItem.priceSpecial)}"
        )
        var selectedIndex = 0

        AlertDialog.Builder(this)
            .setTitle("เพิ่ม ${menuItem.name} ลงตะกร้า")
            .setSingleChoiceItems(options, 0) { _, which ->
                selectedIndex = which
            }
            .setPositiveButton("เพิ่ม") { _, _ ->
                val sizeType = if (selectedIndex == 0) "ธรรมดา" else "พิเศษ"
                val phone = prefs.getString("user_phone", "") ?: ""
                db.addToCart(menuItem.id, 1, sizeType, phone)
                Toast.makeText(this, "เพิ่ม ${menuItem.name} ($sizeType) ลงตะกร้าแล้ว", Toast.LENGTH_SHORT).show()
            }
            .setNegativeButton("ยกเลิก", null)
            .show()
    }
}
