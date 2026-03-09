package com.pom.delivery.activities

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.SupportMapFragment
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.MarkerOptions
import com.pom.delivery.R
import com.pom.delivery.databinding.ActivityMapBinding

class MapActivity : AppCompatActivity(), OnMapReadyCallback {

    private lateinit var binding: ActivityMapBinding
    private var lat = 0.0
    private var lng = 0.0
    private var customerName = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMapBinding.inflate(layoutInflater)
        setContentView(binding.root)

        lat = intent.getDoubleExtra("lat", 13.7563)
        lng = intent.getDoubleExtra("lng", 100.5018)
        customerName = intent.getStringExtra("customer_name") ?: "ลูกค้า"

        val mapFragment = supportFragmentManager.findFragmentById(R.id.map) as? SupportMapFragment
        mapFragment?.getMapAsync(this)

        binding.btnBack.setOnClickListener { finish() }
    }

    override fun onMapReady(googleMap: GoogleMap) {
        val customerLocation = LatLng(lat, lng)
        googleMap.addMarker(MarkerOptions().position(customerLocation).title("ตำแหน่งลูกค้า: $customerName"))
        googleMap.moveCamera(CameraUpdateFactory.newLatLngZoom(customerLocation, 15f))
        googleMap.uiSettings.isZoomControlsEnabled = true
        googleMap.uiSettings.isMyLocationButtonEnabled = true
    }
}
