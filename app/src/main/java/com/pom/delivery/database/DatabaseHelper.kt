package com.pom.delivery.database

import android.content.ContentValues
import android.content.Context
import android.database.sqlite.SQLiteDatabase
import android.database.sqlite.SQLiteOpenHelper
import com.pom.delivery.models.CartItem
import com.pom.delivery.models.MenuItem
import com.pom.delivery.models.Order
import com.pom.delivery.models.User

class DatabaseHelper(context: Context) : SQLiteOpenHelper(context, DB_NAME, null, DB_VERSION) {

    companion object {
        private const val DB_NAME = "pom_delivery.db"
        private const val DB_VERSION = 1

        private const val TABLE_USERS = "users"
        private const val TABLE_MENU = "menu_items"
        private const val TABLE_CART = "cart_items"
        private const val TABLE_ORDERS = "orders"
    }

    override fun onCreate(db: SQLiteDatabase) {
        db.execSQL("""
            CREATE TABLE $TABLE_USERS (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                phone TEXT NOT NULL UNIQUE,
                password TEXT NOT NULL,
                role TEXT DEFAULT 'customer'
            )
        """)
        db.execSQL("""
            CREATE TABLE $TABLE_MENU (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                description TEXT,
                price_normal REAL NOT NULL,
                price_special REAL NOT NULL,
                image_path TEXT
            )
        """)
        db.execSQL("""
            CREATE TABLE $TABLE_CART (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                menu_id INTEGER NOT NULL,
                quantity INTEGER DEFAULT 1,
                size_type TEXT DEFAULT 'ธรรมดา',
                user_phone TEXT NOT NULL,
                FOREIGN KEY(menu_id) REFERENCES $TABLE_MENU(id)
            )
        """)
        db.execSQL("""
            CREATE TABLE $TABLE_ORDERS (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                customer_name TEXT NOT NULL,
                customer_phone TEXT NOT NULL,
                items TEXT NOT NULL,
                total_food_price REAL NOT NULL,
                delivery_fee REAL DEFAULT 0,
                total_price REAL DEFAULT 0,
                latitude REAL DEFAULT 0,
                longitude REAL DEFAULT 0,
                status TEXT DEFAULT 'pending',
                timestamp INTEGER
            )
        """)

        db.execSQL("INSERT INTO $TABLE_USERS (name, phone, password, role) VALUES ('ไรเดอร์ป้อม', '0999999999', '1234', 'rider')")

        insertSampleMenu(db)
    }

    private fun insertSampleMenu(db: SQLiteDatabase) {
        val sampleMenus = listOf(
            arrayOf("ข้าวผัดกระเพรา", "ข้าวผัดกระเพราหมูสับ ใส่ไข่ดาว หอมกรอบ อร่อยแบบไทยแท้", "45", "55", ""),
            arrayOf("ข้าวมันไก่", "ข้าวมันไก่ต้ม น้ำจิ้มรสเด็ด พร้อมน้ำซุป", "40", "50", ""),
            arrayOf("ส้มตำ", "ส้มตำไทย รสจัดจ้าน เผ็ดแซ่บ พร้อมข้าวเหนียว", "35", "45", ""),
            arrayOf("ก๋วยเตี๋ยว", "ก๋วยเตี๋ยวน้ำใส เส้นเล็ก หมูสับ ลูกชิ้น", "40", "55", ""),
            arrayOf("ข้าวคลุกกะปิ", "ข้าวคลุกกะปิ พร้อมเครื่องเคียงครบ", "45", "60", ""),
            arrayOf("ผัดไทย", "ผัดไทยกุ้งสด เส้นจันท์เหนียวนุ่ม รสชาติกลมกล่อม", "50", "65", ""),
            arrayOf("ข้าวหมูแดง", "ข้าวหมูแดง หมูกรอบ น้ำราดแดงหวาน", "45", "55", ""),
            arrayOf("ต้มยำกุ้ง", "ต้มยำกุ้งน้ำข้น รสเผ็ดร้อน กุ้งสดตัวใหญ่", "60", "80", ""),
            arrayOf("แกงเขียวหวาน", "แกงเขียวหวานไก่ กะทิเข้มข้น หอมเครื่องแกง", "45", "60", ""),
            arrayOf("ข้าวเหนียวหมูปิ้ง", "ข้าวเหนียวหมูปิ้ง หมูหมักนุ่ม ย่างจนหอม", "35", "50", "")
        )
        for (menu in sampleMenus) {
            val cv = ContentValues().apply {
                put("name", menu[0])
                put("description", menu[1])
                put("price_normal", menu[2].toDouble())
                put("price_special", menu[3].toDouble())
                put("image_path", menu[4])
            }
            db.insert(TABLE_MENU, null, cv)
        }
    }

    override fun onUpgrade(db: SQLiteDatabase, oldVersion: Int, newVersion: Int) {
        db.execSQL("DROP TABLE IF EXISTS $TABLE_CART")
        db.execSQL("DROP TABLE IF EXISTS $TABLE_ORDERS")
        db.execSQL("DROP TABLE IF EXISTS $TABLE_MENU")
        db.execSQL("DROP TABLE IF EXISTS $TABLE_USERS")
        onCreate(db)
    }

    fun registerUser(name: String, phone: String, password: String, role: String = "customer"): Long {
        val db = writableDatabase
        val cv = ContentValues().apply {
            put("name", name)
            put("phone", phone)
            put("password", password)
            put("role", role)
        }
        return db.insert(TABLE_USERS, null, cv)
    }

    fun loginUser(phone: String, password: String): User? {
        val db = readableDatabase
        val cursor = db.rawQuery(
            "SELECT * FROM $TABLE_USERS WHERE phone = ? AND password = ?",
            arrayOf(phone, password)
        )
        return if (cursor.moveToFirst()) {
            val user = User(
                id = cursor.getLong(cursor.getColumnIndexOrThrow("id")),
                name = cursor.getString(cursor.getColumnIndexOrThrow("name")),
                phone = cursor.getString(cursor.getColumnIndexOrThrow("phone")),
                password = cursor.getString(cursor.getColumnIndexOrThrow("password")),
                role = cursor.getString(cursor.getColumnIndexOrThrow("role"))
            )
            cursor.close()
            user
        } else {
            cursor.close()
            null
        }
    }

    fun getAllMenuItems(): List<MenuItem> {
        val list = mutableListOf<MenuItem>()
        val db = readableDatabase
        val cursor = db.rawQuery("SELECT * FROM $TABLE_MENU", null)
        while (cursor.moveToNext()) {
            list.add(
                MenuItem(
                    id = cursor.getLong(cursor.getColumnIndexOrThrow("id")),
                    name = cursor.getString(cursor.getColumnIndexOrThrow("name")),
                    description = cursor.getString(cursor.getColumnIndexOrThrow("description")),
                    priceNormal = cursor.getDouble(cursor.getColumnIndexOrThrow("price_normal")),
                    priceSpecial = cursor.getDouble(cursor.getColumnIndexOrThrow("price_special")),
                    imagePath = cursor.getString(cursor.getColumnIndexOrThrow("image_path")) ?: ""
                )
            )
        }
        cursor.close()
        return list
    }

    fun addMenuItem(item: MenuItem): Long {
        val db = writableDatabase
        val cv = ContentValues().apply {
            put("name", item.name)
            put("description", item.description)
            put("price_normal", item.priceNormal)
            put("price_special", item.priceSpecial)
            put("image_path", item.imagePath)
        }
        return db.insert(TABLE_MENU, null, cv)
    }

    fun addToCart(menuId: Long, quantity: Int, sizeType: String, userPhone: String): Long {
        val db = writableDatabase
        val cursor = db.rawQuery(
            "SELECT * FROM $TABLE_CART WHERE menu_id = ? AND user_phone = ? AND size_type = ?",
            arrayOf(menuId.toString(), userPhone, sizeType)
        )
        return if (cursor.moveToFirst()) {
            val currentQty = cursor.getInt(cursor.getColumnIndexOrThrow("quantity"))
            val id = cursor.getLong(cursor.getColumnIndexOrThrow("id"))
            cursor.close()
            val cv = ContentValues().apply {
                put("quantity", currentQty + quantity)
            }
            db.update(TABLE_CART, cv, "id = ?", arrayOf(id.toString())).toLong()
        } else {
            cursor.close()
            val cv = ContentValues().apply {
                put("menu_id", menuId)
                put("quantity", quantity)
                put("size_type", sizeType)
                put("user_phone", userPhone)
            }
            db.insert(TABLE_CART, null, cv)
        }
    }

    fun getCartItems(userPhone: String): List<CartItem> {
        val list = mutableListOf<CartItem>()
        val db = readableDatabase
        val cursor = db.rawQuery(
            """SELECT c.id, c.quantity, c.size_type, m.id as menu_id, m.name, m.description,
               m.price_normal, m.price_special, m.image_path
               FROM $TABLE_CART c JOIN $TABLE_MENU m ON c.menu_id = m.id
               WHERE c.user_phone = ?""",
            arrayOf(userPhone)
        )
        while (cursor.moveToNext()) {
            val menuItem = MenuItem(
                id = cursor.getLong(cursor.getColumnIndexOrThrow("menu_id")),
                name = cursor.getString(cursor.getColumnIndexOrThrow("name")),
                description = cursor.getString(cursor.getColumnIndexOrThrow("description")),
                priceNormal = cursor.getDouble(cursor.getColumnIndexOrThrow("price_normal")),
                priceSpecial = cursor.getDouble(cursor.getColumnIndexOrThrow("price_special")),
                imagePath = cursor.getString(cursor.getColumnIndexOrThrow("image_path")) ?: ""
            )
            val sizeType = cursor.getString(cursor.getColumnIndexOrThrow("size_type"))
            val qty = cursor.getInt(cursor.getColumnIndexOrThrow("quantity"))
            val price = if (sizeType == "พิเศษ") menuItem.priceSpecial else menuItem.priceNormal
            list.add(
                CartItem(
                    id = cursor.getLong(cursor.getColumnIndexOrThrow("id")),
                    menuItem = menuItem,
                    quantity = qty,
                    sizeType = sizeType,
                    totalPrice = price * qty
                )
            )
        }
        cursor.close()
        return list
    }

    fun removeCartItem(cartId: Long) {
        writableDatabase.delete(TABLE_CART, "id = ?", arrayOf(cartId.toString()))
    }

    fun clearCart(userPhone: String) {
        writableDatabase.delete(TABLE_CART, "user_phone = ?", arrayOf(userPhone))
    }

    fun updateCartItemQuantity(cartId: Long, quantity: Int) {
        val cv = ContentValues().apply { put("quantity", quantity) }
        writableDatabase.update(TABLE_CART, cv, "id = ?", arrayOf(cartId.toString()))
    }

    fun createOrder(order: Order): Long {
        val db = writableDatabase
        val cv = ContentValues().apply {
            put("customer_name", order.customerName)
            put("customer_phone", order.customerPhone)
            put("items", order.items)
            put("total_food_price", order.totalFoodPrice)
            put("delivery_fee", order.deliveryFee)
            put("total_price", order.totalPrice)
            put("latitude", order.latitude)
            put("longitude", order.longitude)
            put("status", order.status)
            put("timestamp", order.timestamp)
        }
        return db.insert(TABLE_ORDERS, null, cv)
    }

    fun getPendingOrders(): List<Order> {
        val list = mutableListOf<Order>()
        val db = readableDatabase
        val cursor = db.rawQuery("SELECT * FROM $TABLE_ORDERS WHERE status = 'pending' ORDER BY timestamp DESC", null)
        while (cursor.moveToNext()) {
            list.add(cursorToOrder(cursor))
        }
        cursor.close()
        return list
    }

    fun getAllOrders(): List<Order> {
        val list = mutableListOf<Order>()
        val db = readableDatabase
        val cursor = db.rawQuery("SELECT * FROM $TABLE_ORDERS ORDER BY timestamp DESC", null)
        while (cursor.moveToNext()) {
            list.add(cursorToOrder(cursor))
        }
        cursor.close()
        return list
    }

    fun updateOrderStatus(orderId: Long, status: String, deliveryFee: Double = 0.0, totalPrice: Double = 0.0) {
        val cv = ContentValues().apply {
            put("status", status)
            put("delivery_fee", deliveryFee)
            put("total_price", totalPrice)
        }
        writableDatabase.update(TABLE_ORDERS, cv, "id = ?", arrayOf(orderId.toString()))
    }

    fun getOrderById(orderId: Long): Order? {
        val db = readableDatabase
        val cursor = db.rawQuery("SELECT * FROM $TABLE_ORDERS WHERE id = ?", arrayOf(orderId.toString()))
        return if (cursor.moveToFirst()) {
            val order = cursorToOrder(cursor)
            cursor.close()
            order
        } else {
            cursor.close()
            null
        }
    }

    private fun cursorToOrder(cursor: android.database.Cursor): Order {
        return Order(
            id = cursor.getLong(cursor.getColumnIndexOrThrow("id")),
            customerName = cursor.getString(cursor.getColumnIndexOrThrow("customer_name")),
            customerPhone = cursor.getString(cursor.getColumnIndexOrThrow("customer_phone")),
            items = cursor.getString(cursor.getColumnIndexOrThrow("items")),
            totalFoodPrice = cursor.getDouble(cursor.getColumnIndexOrThrow("total_food_price")),
            deliveryFee = cursor.getDouble(cursor.getColumnIndexOrThrow("delivery_fee")),
            totalPrice = cursor.getDouble(cursor.getColumnIndexOrThrow("total_price")),
            latitude = cursor.getDouble(cursor.getColumnIndexOrThrow("latitude")),
            longitude = cursor.getDouble(cursor.getColumnIndexOrThrow("longitude")),
            status = cursor.getString(cursor.getColumnIndexOrThrow("status")),
            timestamp = cursor.getLong(cursor.getColumnIndexOrThrow("timestamp"))
        )
    }
}
