using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Globalization;
using AracServisTakipSistemi.Application.Interfaces;

namespace AracServisTakipSistemi.Infrastructure.Services;

public class NominatimGeocodingServisi : IGeocodingServisi
{
    private readonly HttpClient _httpClient;

    public NominatimGeocodingServisi(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AracServisTakipSistemi/1.0");
        }
    }

    public async Task<GeocodingSonucu> AdresteneKoordinatBulAsync(string adres)
    {
        try
        {
            var sorgu = Uri.EscapeDataString($"{adres}, Türkiye");
            var url = $"https://nominatim.openstreetmap.org/search?q={sorgu}&format=json&limit=1";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new GeocodingSonucu
                {
                    BasariliMi = false,
                    HataMesaji = $"Geocoding servisi {(int)response.StatusCode} kodu döndürdü."
                };
            }

            var icerik = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(icerik);

            if (json.RootElement.GetArrayLength() == 0)
            {
                return new GeocodingSonucu
                {
                    BasariliMi = false,
                    HataMesaji = "Bu adres için koordinat bulunamadı."
                };
            }

            var ilkSonuc = json.RootElement[0];
            var enlem = double.Parse(ilkSonuc.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture);
            var boylam = double.Parse(ilkSonuc.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture);

            return new GeocodingSonucu
            {
                BasariliMi = true,
                Enlem = enlem,
                Boylam = boylam
            };
        }
        catch (TaskCanceledException)
        {
            return new GeocodingSonucu { BasariliMi = false, HataMesaji = "Geocoding servisi zaman aşımına uğradı." };
        }
        catch (Exception ex)
        {
            return new GeocodingSonucu { BasariliMi = false, HataMesaji = $"Geocoding hatası: {ex.Message}" };
        }
    }
}