using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using YourNamespace.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarkodMed.Models;
using BarkodMed.Services;
using System.Web.Mvc;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TetkitController : ControllerBase
    {
        private readonly TetkikService _tetkikService;

        public TetkitController(TetkikService tetkikService)
        {
            _tetkikService = tetkikService;
        }

        [HttpGet("process")]
        public async Task<IActionResult> ProcessTetkikData()
        {
            try
            {
                var tetkik = await _tetkikService.GetTetkikDataAsync();

                // Verileri işleyin
                var results = new List<Tkimlik>();

                var barkodGruplari = tetkik.Data.GroupBy(t => t.X_Barkod);

                foreach (var grup in barkodGruplari)
                {
                    var ilkKayıt = grup.FirstOrDefault();
                    if (ilkKayıt == null)
                    {
                        continue;
                    }

                    string cinsiyet = ilkKayıt.X_Cinsiyeti == "Kadin" ? "K" : "E";

                    DateTime dogumTarihi;
                    if (!DateTime.TryParseExact(ilkKayıt.X_Dogum_Tarihi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dogumTarihi))
                    {
                        
                        dogumTarihi = DateTime.MinValue;
                    }

                    var kimlik = new Tkimlik
                    {
                        Tckimlik_No = ilkKayıt.X_Hasta_Kimlikno,
                        Adi = ilkKayıt.X_Hasta_Adi,
                        Soyadi = ilkKayıt.X_Hasta_Soyad,
                        Email = "ilayda@gmail.com",
                        Anne_Adi = ilkKayıt.X_Hasta_Anne_Adi,
                        Baba_Adi = ilkKayıt.X_Hasta_Baba_Adi,
                        DogumTarihi = dogumTarihi.ToString("dd.MM.yyyy"),
                        Cins = cinsiyet
                    };

                    var istekler = grup.Select((item, index) => new Tistekler
                    {
                        Kodu = Convert.ToDouble(item.X_Kodu),
                        Sira_No = index + 1,
                        Tarih = DateTime.Now.ToString("dd.MM.yyyy"),
                        Exkodu = "0",
                        Aciklama = "",
                        Girisyapan = "service",
                        Acilmi = "F"
                    }).ToList();

                    var istekGiris = new Tistekgiris
                    {
                        IsteyenBarkod = 0,
                        Aciklama = "",
                        Giris_Tipi = "TUMU",
                        Kimlik = kimlik,
                        Tckimlik_No = Convert.ToDouble(ilkKayıt.X_Hasta_Kimlikno),
                        Dosya_No = 0,
                        Istekler = istekler.ToArray(),
                        Istek_Tarihi = DateTime.Now.ToString("dd.MM.yyyy")
                    };

                    

                    results.Add(kimlik);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İşlem sırasında bir hata oluştu: {ex.Message}");
            }
        }
    }
}
