﻿using MedicalVisits.Infrastructure.Services.Interfaces;
using MedicalVisits.Models;
using MedicalVisits.Models.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace MedicalVisits.Infrastructure.Services.GoogleMapsApi;

public class GGeocodingService : IGGeocodingService
{
    private readonly HttpClient _httpClient;
     public readonly IOptions<GoogleMapsServiceSettings> _settings;


    public GGeocodingService(HttpClient httpClient, IOptions<GoogleMapsServiceSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }
    
    public async Task<(double Latitude, double Longitude)> GeocodeAddressAsync(Address address)
    {
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address.ToString())}&key={_settings.Value.ApiKey}";

        try
        {
            // Запит до Google Geocoding API
            var response = await _httpClient.GetAsync(url);
        
            // Перевірка на успішний статус запиту
            response.EnsureSuccessStatusCode();

            // Зчитуємо відповідь
            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);

            // Перевірка на статус API
            if (data["status"]?.ToString() != "OK")
            {
                var errorMessage = data["status"]?.ToString() ?? "Unknown error";
                throw new Exception($"Geocoding API error: {errorMessage}");
            }

            // Отримуємо координати
            var location = data["results"]?[0]?["geometry"]?["location"];
            if (location == null)
                throw new Exception("Address not found");

            double latitude = (double)location["lat"];
            double longitude = (double)location["lng"];

            return (latitude, longitude);
        }
        catch (HttpRequestException ex)
        {
            // Помилка при підключенні до сервера
            throw new Exception("Error connecting to Geocoding API: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Ловимо інші помилки
            throw new Exception("An error occurred: " + ex.Message);
        }
    }
}