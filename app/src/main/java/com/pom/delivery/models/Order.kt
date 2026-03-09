package com.pom.delivery.models

data class Order(
    val id: Long = 0,
    val customerName: String,
    val customerPhone: String,
    val items: String,
    val totalFoodPrice: Double,
    val deliveryFee: Double = 0.0,
    val totalPrice: Double = 0.0,
    val latitude: Double = 0.0,
    val longitude: Double = 0.0,
    val status: String = "pending",
    val timestamp: Long = System.currentTimeMillis()
)
