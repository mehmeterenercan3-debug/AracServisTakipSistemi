namespace AracServisTakipSistemi.Web.Models;

public class AdminDashboardViewModel
{
    public int ToplamAktifPersonel { get; set; }
    public int BeklemedeKalanPersonel { get; set; }
    public int ToplamAktifArac { get; set; }
    public int BakimdakiAracSayisi { get; set; }
    public int ToplamBolge { get; set; }

    public List<GrafikVeriNoktasi> RiskDagilimi { get; set; } = new();
    public List<GrafikVeriNoktasi> BolgePersonelDagilimi { get; set; } = new();

    // Grafiğin "hangi araç" sorusuna cevap verebilmesi için — son 30 günün ham detayı
    public List<RiskDetayi> SonRiskHesaplamalari { get; set; } = new();
}

public class GrafikVeriNoktasi
{
    public string Etiket { get; set; } = string.Empty;
    public int Deger { get; set; }
}

public class RiskDetayi
{
    public string Plaka { get; set; } = string.Empty;
    public string Seviye { get; set; } = string.Empty;
    public decimal Puan { get; set; }
    public DateTime Tarih { get; set; }
}