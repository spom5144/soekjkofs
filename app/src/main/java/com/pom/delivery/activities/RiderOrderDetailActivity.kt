package com.pom.delivery.activities

import android.Manifest
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import com.pom.delivery.R
import com.pom.delivery.database.DatabaseHelper
import com.pom.delivery.databinding.ActivityRiderOrderDetailBinding
import com.pom.delivery.models.Order

class RiderOrderDetailActivity : AppCompatActivity() {

    private lateinit var binding: ActivityRiderOrderDetailBinding
    private lateinit var db: DatabaseHelper
    private var order: Order? = null
    private var deliveryFee = 0.0
    private val CALL_PERMISSION = 2001

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityRiderOrderDetailBinding.inflate(layoutInflater)
        setContentView(binding.root)

        db = DatabaseHelper(this)
        val orderId = intent.getLongExtra("order_id", -1)

        if (orderId == -1L) {
            finish()
            return
        }

        order = db.getOrderById(orderId)
        if (order == null) {
            Toast.makeText(this, "ไม่พบออเดอร์", Toast.LENGTH_SHORT).show()
            finish()
            return
        }

        displayOrder()
        setupListeners()
    }

    private fun displayOrder() {
        val o = order ?: return
        binding.tvOrderId.text = "ออเดอร์ #${o.id}"
        binding.tvCustomerName.text = "ชื่อลูกค้า: ${o.customerName}"
        binding.tvCustomerPhone.text = "เบอร์โทร: ${o.customerPhone}"
        binding.tvOrderItems.text = o.items
        binding.tvFoodTotal.text = "รวมค่าอาหาร: ฿${String.format("%.0f", o.totalFoodPrice)}"
        updateTotals()
    }

    private fun setupListeners() {
        binding.rbNear.setOnCheckedChangeListener { _, isChecked ->
            if (isChecked) {
                deliveryFee = 40.0
                binding.tvDeliveryFee.text = "ค่าส่ง (ทางใกล้): ฿40"
                updateTotals()
            }
        }

        binding.rbFar.setOnCheckedChangeListener { _, isChecked ->
            if (isChecked) {
                deliveryFee = 75.0
                binding.tvDeliveryFee.text = "ค่าส่ง (ทางไกล): ฿75"
                updateTotals()
            }
        }

        binding.etCustomerPaid.addTextChangedListener(object : TextWatcher {
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}
            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {}
            override fun afterTextChanged(s: Editable?) {
                calculateChange()
            }
        })

        binding.btnCallCustomer.setOnClickListener {
            val phone = order?.customerPhone ?: return@setOnClickListener
            if (ActivityCompat.checkSelfPermission(this, Manifest.permission.CALL_PHONE) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.CALL_PHONE), CALL_PERMISSION)
            } else {
                val callIntent = Intent(Intent.ACTION_CALL, Uri.parse("tel:$phone"))
                startActivity(callIntent)
            }
        }

        binding.btnNavigate.setOnClickListener {
            val o = order ?: return@setOnClickListener
            if (o.latitude != 0.0 && o.longitude != 0.0) {
                val uri = Uri.parse("google.navigation:q=${o.latitude},${o.longitude}")
                val mapIntent = Intent(Intent.ACTION_VIEW, uri)
                mapIntent.setPackage("com.google.android.apps.maps")
                if (mapIntent.resolveActivity(packageManager) != null) {
                    startActivity(mapIntent)
                } else {
                    val webUri = Uri.parse("https://www.google.com/maps/dir/?api=1&destination=${o.latitude},${o.longitude}")
                    startActivity(Intent(Intent.ACTION_VIEW, webUri))
                }
            } else {
                Toast.makeText(this, "ไม่มีตำแหน่งลูกค้า", Toast.LENGTH_SHORT).show()
            }
        }

        binding.btnViewMap.setOnClickListener {
            val o = order ?: return@setOnClickListener
            val intent = Intent(this, MapActivity::class.java)
            intent.putExtra("lat", o.latitude)
            intent.putExtra("lng", o.longitude)
            intent.putExtra("customer_name", o.customerName)
            startActivity(intent)
        }

        binding.btnAcceptOrder.setOnClickListener {
            if (deliveryFee == 0.0) {
                Toast.makeText(this, "กรุณาเลือกทางใกล้หรือทางไกล", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            val o = order ?: return@setOnClickListener
            val totalPrice = o.totalFoodPrice + deliveryFee

            AlertDialog.Builder(this)
                .setTitle("รับออเดอร์")
                .setMessage("ยืนยันรับออเดอร์ #${o.id}?\n\nรวมค่าอาหาร: ฿${String.format("%.0f", o.totalFoodPrice)}\nค่าส่ง: ฿${String.format("%.0f", deliveryFee)}\nรวมทั้งหมด: ฿${String.format("%.0f", totalPrice)}")
                .setPositiveButton("รับออเดอร์") { _, _ ->
                    db.updateOrderStatus(o.id, "accepted", deliveryFee, totalPrice)
                    Toast.makeText(this, "รับออเดอร์ #${o.id} แล้ว!", Toast.LENGTH_SHORT).show()
                    finish()
                }
                .setNegativeButton("ยกเลิก", null)
                .show()
        }

        binding.btnBack.setOnClickListener { finish() }
    }

    private fun updateTotals() {
        val o = order ?: return
        val total = o.totalFoodPrice + deliveryFee
        binding.tvGrandTotal.text = "รวมทั้งหมด: ฿${String.format("%.0f", total)}"
        calculateChange()
    }

    private fun calculateChange() {
        val o = order ?: return
        val total = o.totalFoodPrice + deliveryFee
        val paidText = binding.etCustomerPaid.text.toString()
        val paid = paidText.toDoubleOrNull() ?: 0.0
        val change = paid - total
        if (change >= 0) {
            binding.tvChange.text = "เงินทอน: ฿${String.format("%.0f", change)}"
            binding.tvChange.setTextColor(getColor(android.R.color.holo_green_dark))
        } else {
            binding.tvChange.text = "ยังขาดอีก: ฿${String.format("%.0f", -change)}"
            binding.tvChange.setTextColor(getColor(android.R.color.holo_red_dark))
        }
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == CALL_PERMISSION && grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            val phone = order?.customerPhone ?: return
            val callIntent = Intent(Intent.ACTION_CALL, Uri.parse("tel:$phone"))
            startActivity(callIntent)
        }
    }
}
