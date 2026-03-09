package com.pom.delivery.activities

import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.pom.delivery.R
import com.pom.delivery.database.DatabaseHelper
import com.pom.delivery.databinding.ActivityLoginBinding

class LoginActivity : AppCompatActivity() {

    private lateinit var binding: ActivityLoginBinding
    private lateinit var db: DatabaseHelper
    private lateinit var prefs: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        db = DatabaseHelper(this)
        prefs = getSharedPreferences("pom_prefs", MODE_PRIVATE)

        val savedPhone = prefs.getString("user_phone", null)
        if (savedPhone != null) {
            val role = prefs.getString("user_role", "customer")
            navigateToMain(role ?: "customer")
            return
        }

        binding.btnLogin.setOnClickListener {
            val phone = binding.etPhone.text.toString().trim()
            val password = binding.etPassword.text.toString().trim()

            if (phone.isEmpty() || password.isEmpty()) {
                Toast.makeText(this, "กรุณากรอกเบอร์โทรและรหัสผ่าน", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            val user = db.loginUser(phone, password)
            if (user != null) {
                prefs.edit().apply {
                    putString("user_phone", user.phone)
                    putString("user_name", user.name)
                    putString("user_role", user.role)
                    apply()
                }
                Toast.makeText(this, "เข้าสู่ระบบสำเร็จ! ยินดีต้อนรับ ${user.name}", Toast.LENGTH_SHORT).show()
                navigateToMain(user.role)
            } else {
                Toast.makeText(this, "เบอร์โทรหรือรหัสผ่านไม่ถูกต้อง", Toast.LENGTH_SHORT).show()
            }
        }

        binding.btnRegister.setOnClickListener {
            startActivity(Intent(this, RegisterActivity::class.java))
        }
    }

    private fun navigateToMain(role: String) {
        val intent = if (role == "rider") {
            Intent(this, RiderActivity::class.java)
        } else {
            Intent(this, MenuActivity::class.java)
        }
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
        finish()
    }
}
