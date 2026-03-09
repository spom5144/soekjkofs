package com.pom.delivery.models

data class CartItem(
    val id: Long = 0,
    val menuItem: MenuItem,
    val quantity: Int = 1,
    val sizeType: String = "ธรรมดา",
    val totalPrice: Double = 0.0
)
