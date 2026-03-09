package com.pom.delivery.activities

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.provider.MediaStore
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.pom.delivery.R
import com.pom.delivery.database.DatabaseHelper
import com.pom.delivery.databinding.ActivityAddMenuBinding
import com.pom.delivery.models.MenuItem
import java.io.File
import java.io.FileOutputStream

class AddMenuActivity : AppCompatActivity() {

    private lateinit var binding: ActivityAddMenuBinding
    private lateinit var db: DatabaseHelper
    private var selectedImagePath: String = ""
    private val PICK_IMAGE = 100

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityAddMenuBinding.inflate(layoutInflater)
        setContentView(binding.root)

        db = DatabaseHelper(this)

        binding.btnSelectImage.setOnClickListener {
            val intent = Intent(Intent.ACTION_PICK, MediaStore.Images.Media.EXTERNAL_CONTENT_URI)
            startActivityForResult(intent, PICK_IMAGE)
        }

        binding.btnSave.setOnClickListener {
            val name = binding.etFoodName.text.toString().trim()
            val desc = binding.etDescription.text.toString().trim()
            val priceNormal = binding.etPriceNormal.text.toString().toDoubleOrNull()
            val priceSpecial = binding.etPriceSpecial.text.toString().toDoubleOrNull()

            if (name.isEmpty() || priceNormal == null || priceSpecial == null) {
                Toast.makeText(this, "กรุณากรอกข้อมูลให้ครบ", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            val item = MenuItem(
                name = name,
                description = desc,
                priceNormal = priceNormal,
                priceSpecial = priceSpecial,
                imagePath = selectedImagePath
            )

            val result = db.addMenuItem(item)
            if (result > 0) {
                Toast.makeText(this, "เพิ่มเมนู $name สำเร็จ!", Toast.LENGTH_SHORT).show()
                finish()
            } else {
                Toast.makeText(this, "เกิดข้อผิดพลาด", Toast.LENGTH_SHORT).show()
            }
        }

        binding.btnBack.setOnClickListener { finish() }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == PICK_IMAGE && resultCode == Activity.RESULT_OK && data != null) {
            val uri = data.data
            if (uri != null) {
                selectedImagePath = saveImageToInternal(uri)
                binding.ivPreview.setImageURI(uri)
                binding.tvImageStatus.text = "เลือกรูปภาพแล้ว"
            }
        }
    }

    private fun saveImageToInternal(uri: Uri): String {
        try {
            val inputStream = contentResolver.openInputStream(uri) ?: return ""
            val file = File(filesDir, "menu_${System.currentTimeMillis()}.jpg")
            val outputStream = FileOutputStream(file)
            inputStream.copyTo(outputStream)
            inputStream.close()
            outputStream.close()
            return file.absolutePath
        } catch (e: Exception) {
            return ""
        }
    }
}
