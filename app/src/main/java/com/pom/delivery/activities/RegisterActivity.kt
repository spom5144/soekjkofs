package com.pom.delivery.activities

import android.os.Bundle
import android.widget.ArrayAdapter
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.pom.delivery.R
import com.pom.delivery.database.DatabaseHelper
import com.pom.delivery.databinding.ActivityRegisterBinding

class RegisterActivity : AppCompatActivity() {

    private lateinit var binding: ActivityRegisterBinding
    private lateinit var db: DatabaseHelper

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityRegisterBinding.inflate(layoutInflater)
        setContentView(binding.root)

        db = DatabaseHelper(this)

        val roles = arrayOf("customer", "rider")
        val adapter = ArrayAdapter(this, android.R.layout.simple_spinner_dropdown_item, arrayOf("ลูกค้า", "ไรเดอร์"))
        binding.spinnerRole.adapter = adapter

        binding.btnRegister.setOnClickListener {
            val name = binding.etName.text.toString().trim()
            val phone = binding.etPhone.text.toString().trim()
            val password = binding.etPassword.text.toString().trim()
            val confirmPassword = binding.etConfirmPassword.text.toString().trim()
            val role = roles[binding.spinnerRole.selectedItemPosition]

            if (name.isEmpty() || phone.isEmpty() || password.isEmpty()) {
                Toast.makeText(this, "กรุณากรอกข้อมูลให้ครบ", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            if (password != confirmPassword) {
                Toast.makeText(this, "รหัสผ่านไม่ตรงกัน", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            if (phone.length < 10) {
                Toast.makeText(this, "เบอร์โทรต้องมีอย่างน้อย 10 หลัก", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            val result = db.registerUser(name, phone, password, role)
            if (result > 0) {
                Toast.makeText(this, "ลงทะเบียนสำเร็จ!", Toast.LENGTH_SHORT).show()
                finish()
            } else {
                Toast.makeText(this, "เบอร์โทรนี้ถูกใช้แล้ว", Toast.LENGTH_SHORT).show()
            }
        }

        binding.btnBack.setOnClickListener { finish() }
    }
}
