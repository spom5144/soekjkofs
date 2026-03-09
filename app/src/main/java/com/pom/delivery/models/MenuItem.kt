package com.pom.delivery.models

data class MenuItem(
    val id: Long = 0,
    val name: String,
    val description: String,
    val priceNormal: Double,
    val priceSpecial: Double,
    val imagePath: String = ""
)
