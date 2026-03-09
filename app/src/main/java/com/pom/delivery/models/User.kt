package com.pom.delivery.models

data class User(
    val id: Long = 0,
    val name: String,
    val phone: String,
    val password: String,
    val role: String = "customer"
)
