using Microsoft.EntityFrameworkCore;
using YemekTarifiApi.Models;
using YemekTarifiApi.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using YemekTarifiApi.Dtos;
using Azure.Core;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Antiforgery;

using System;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// DbContext ve veritabanı bağlantısını yapılandıralım
builder.Services.AddDbContext<TarifDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS yapılandırması
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:7213") // CORS izin verilen domain
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
app.UseStaticFiles(); // wwwroot klasörünü aktif hale getirir

// Eğer özel bir klasör yapılandırması varsa
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resimler")),
    RequestPath = "/Resimler"
});
// Swagger yapılandırması


// Swagger aktif hale getirilsin sadece geliştirme ortamında
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



#region Tarif CRUD

app.MapPost("/tarif/ekle", async ([FromForm] TarifEkleDto dto, TarifDbContext db) =>
{
    if (dto.Resim == null)
        return Results.BadRequest("Resim dosyası yok.");

    string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resimler");
    if (!Directory.Exists(uploadsPath))//mevcut değilse oluştur
        Directory.CreateDirectory(uploadsPath);


    var fileName = Path.GetFileName(dto.Resim.FileName);  // pizza.jpg
    var filePath = Path.Combine(uploadsPath, fileName);


    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await dto.Resim.CopyToAsync(stream);
    }

    var tarif = new Tarif
    {
        Ad = dto.Ad,
        Malzemeler = dto.Malzemeler,
        Yapilis = dto.Yapilis,
        EkleyenKullanici = dto.EkleyenKullanici,
        PisirmeSuresi = dto.PisirmeSuresi,
        ResimYolu = "/Resimler/" + fileName,
        EklenmeTarihi = DateTime.Now,
        GuncellenmeTarihi = DateTime.Now
    };

    db.Tarifler.Add(tarif);
    await db.SaveChangesAsync();

    return Results.Created($"/tarif/{tarif.Id}", tarif);
})
.DisableAntiforgery() //CSRF koruması kapatılır
.WithTags("Tarif");



// GET: Tüm tarifleri listelemek
app.MapGet("/tarif/listele", async (TarifDbContext db) =>
{
    var tarifler = await db.Tarifler.ToListAsync();
    return tarifler.Any() ? Results.Ok(tarifler) : Results.NoContent();
}).WithTags("Tarif");

// GET: Belirli bir kategoriye ait tarifleri getir
app.MapGet("/tarif/kategori/{kategoriId}", async (TarifDbContext db, int kategoriId) =>
{
    var tarifler = await db.Tarifler
    
        .Where(t => t.Id == kategoriId)
        .ToListAsync();

    return tarifler.Any() ? Results.Ok(tarifler) : Results.NoContent();
}).WithTags("Tarif");

// GET: ID'ye göre tek bir tarif döndürmek
app.MapGet("/tarif/{id}", async (TarifDbContext db, int id) =>
{
    var tarif = await db.Tarifler.FirstOrDefaultAsync(t => t.Id == id);

    if (tarif == null)
        return Results.NotFound();

    return Results.Ok(tarif);
}).WithTags("Tarif");

app.MapPut("/tarif/guncelleform", async ([FromForm] TarifGuncelleDto dto, TarifDbContext db) =>
{
    var tarif = await db.Tarifler.FindAsync(dto.Id);
    if (tarif == null)
        return Results.NotFound("Tarif bulunamadı");

    //Tarif bilgilerini güncelle
    tarif.Ad = dto.Ad;
    tarif.Malzemeler = dto.Malzemeler;
    tarif.Yapilis = dto.Yapilis;
    tarif.PisirmeSuresi = dto.PisirmeSuresi;
    tarif.GuncellenmeTarihi = DateTime.Now;

    //Resim varsa kaydet
    if (dto.Resim != null && dto.Resim.Length > 0)
    {
        var dosyaAdi = Path.GetFileName(dto.Resim.FileName);
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resimler");
        var kayitYolu = Path.Combine(uploadsFolder, dosyaAdi);

        //eğer aynı isimde resim varsa _1 _2 gibi isimlendirilir.
        int sayac = 1;
        while (File.Exists(kayitYolu))
        {
            var isim = Path.GetFileNameWithoutExtension(dosyaAdi);
            var ext = Path.GetExtension(dosyaAdi);
            dosyaAdi = $"{isim}_{sayac}{ext}";
            kayitYolu = Path.Combine(uploadsFolder, dosyaAdi);
            sayac++;
        }

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        using var stream = new FileStream(kayitYolu, FileMode.Create);
        await dto.Resim.CopyToAsync(stream);

        tarif.ResimYolu = $"/Resimler/{dosyaAdi}";
    }

    //Eski kategorileri sil
    var eskiKategoriler = db.TarifKategoriler.Where(tk => tk.TarifId == dto.Id);
    db.TarifKategoriler.RemoveRange(eskiKategoriler);

    //Yeni kategorileri ekle
    if (dto.KategoriIdler != null && dto.KategoriIdler.Any())
    {
        foreach (var kategoriId in dto.KategoriIdler)
        {
            db.TarifKategoriler.Add(new TarifKategori
            {
                TarifId = dto.Id,
                KategoriId = kategoriId
            });
        }
    }

    await db.SaveChangesAsync();
    return Results.Ok(tarif);
})
.DisableAntiforgery()
.Accepts<TarifGuncelleDto>("multipart/form-data") //format belirlemek IFromFile için 
.WithTags("Tarif");


// DELETE: Bir tarifi silmek
app.MapDelete("/tarif/{id}", async (TarifDbContext db, int id) =>
{
    var tarif = await db.Tarifler.FindAsync(id);

    if (tarif == null)
        return Results.NotFound();

    db.Tarifler.Remove(tarif);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).WithTags("Tarif");

#endregion

#region kategori

// GET: Tüm kategorileri listelemek
app.MapGet("/kategori/listele", async (TarifDbContext db) =>
{
    var kategoriler = await db.Kategoriler.ToListAsync();
    return kategoriler.Any() ? Results.Ok(kategoriler) : Results.NoContent();
}).WithTags("Kategori");

// POST: Yeni bir kategori ekleme
/*app.MapPost("/kategori/ekle", async (TarifDbContext db, [FromBody] KategoriEkle kategoriDTO) =>
{
    // Yeni kategori nesnesini oluştur
    var kategori = new Kategori
    {
        Ad = kategoriDTO.Ad
    };

    // Veritabanına ekle
    db.Kategoriler.Add(kategori);
    await db.SaveChangesAsync();

    // Başarılı bir şekilde ekledikten sonra kategori bilgilerini döndür
    return Results.Created($"/kategori/{kategori.KategoriId}", kategori);
}).WithTags("Kategori");*/



// GET: ID'ye göre tek bir kategori döndürmek
/*app.MapGet("/kategori/{id}", async (TarifDbContext db, int id) =>
{
    var kategori = await db.Kategoriler.FirstOrDefaultAsync(k => k.KategoriId == id);

    if (kategori == null)
        return Results.NotFound();

    return Results.Ok(kategori);
}).WithTags("Kategori");*/

// PUT: Var olan bir kategoriyi güncellemek
/*app.MapPut("/kategori/{id}", async (TarifDbContext db, int id, [FromBody] KategoriEkle kategoriDTO) =>
{
    var mevcutKategori = await db.Kategoriler.FindAsync(id);

    if (mevcutKategori == null)
        return Results.NotFound();

    // Güncellenen kategori bilgilerini mevcut kategori üzerine yaz
    mevcutKategori.Ad = kategoriDTO.Ad;

    await db.SaveChangesAsync();

    return Results.Ok(mevcutKategori);
}).WithTags("Kategori");*/

// DELETE: Bir kategoriyi silmek
/*app.MapDelete("/kategori/{id}", async (TarifDbContext db, int id) =>
{
    var kategori = await db.Kategoriler.FindAsync(id);

    if (kategori == null)
        return Results.NotFound();

    db.Kategoriler.Remove(kategori);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).WithTags("Kategori");*/

#endregion kategori

#region kullanici

app.MapPost("/kullanici/giris", async (TarifDbContext db, [FromBody] KullaniciGirisDto girisDto) => 
{
    var kullanici = await db.Kullanicilar
        .FirstOrDefaultAsync(x => x.KullaniciAdi == girisDto.KullaniciAdi && x.Sifre == girisDto.Sifre); 

    if (kullanici == null)
        return Results.Unauthorized();

    return Results.Ok(kullanici);
}).WithTags("Kullanıcı");

// POST: Yeni bir kullanıcı ekleme
app.MapPost("/kullanici/ekle", async (TarifDbContext db, [FromBody] KullaniciEkle dto) =>
{
    var kullanici = new Kullanici
    {
        
        Sifre = dto.Sifre,
        KullaniciAdi = dto.KullaniciAdi,
        KullaniciSoyadi = dto.KullaniciSoyadi,
        Eposta = dto.Eposta,
        Telefon = dto.Telefon,
        DogumTarihi = dto.DogumTarihi,
        KayitTarihi = dto.KayitTarihi,
        Cinsiyet = dto.Cinsiyet
    };

    db.Kullanicilar.Add(kullanici);
    await db.SaveChangesAsync();

    return Results.Created($"/kullanici/{kullanici.KullaniciId}", kullanici);
}).WithTags("Kullanıcı");

// GET: ID'ye göre tek bir kullanıcı döndürmek
app.MapGet("/kullanici/{id}", async (TarifDbContext db, int id) =>
{
    var kullanici = await db.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciId == id);

    if (kullanici == null)
        return Results.NotFound();

    return Results.Ok(kullanici);
}).WithTags("Kullanıcı");

app.MapPost("/sifredegistir", async (SifreDegistirModel model, TarifDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(model.KullaniciAdi) ||
        string.IsNullOrWhiteSpace(model.EskiSifre) ||
        string.IsNullOrWhiteSpace(model.YeniSifre) ||
        string.IsNullOrWhiteSpace(model.YeniSifreTekrar))
    {
        return Results.BadRequest("Lütfen tüm alanları doldurun.");
    }

    if (model.YeniSifre != model.YeniSifreTekrar)
    {
        return Results.BadRequest("Yeni şifre ve tekrar şifre eşleşmiyor.");
    }

    var kullanici = await db.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == model.KullaniciAdi);
    if (kullanici == null)
        return Results.NotFound("Kullanıcı bulunamadı.");

    if (kullanici.Sifre != model.EskiSifre)
        return Results.BadRequest("Eski şifre yanlış.");

    kullanici.Sifre = model.YeniSifre;
    await db.SaveChangesAsync();

    return Results.Ok("Şifreniz başarıyla değiştirildi.");
}).WithTags("Kullanıcı");

// GET: Tüm kullanıcıları listelemek
app.MapGet("/kullanici/listele", async (TarifDbContext db) =>
{
    var kullanicilar = await db.Kullanicilar.ToListAsync();
    return kullanicilar.Any() ? Results.Ok(kullanicilar) : Results.NoContent();
}).WithTags("Kullanıcı");


// PUT: Var olan bir kullanıcıyı güncellemek
app.MapPut("/kullanici/{id}", async (TarifDbContext db, int id, [FromBody] KullaniciEkle kullaniciDTO) =>
{
    var mevcutKullanici = await db.Kullanicilar.FindAsync(id);

    if (mevcutKullanici == null)
        return Results.NotFound();

    // Güncellenen kullanıcı bilgilerini mevcut kullanıcı üzerine yaz
    mevcutKullanici.KullaniciAdi = kullaniciDTO.KullaniciAdi;
    mevcutKullanici.Sifre = kullaniciDTO.Sifre;

    await db.SaveChangesAsync();

    return Results.Ok(mevcutKullanici);
}).WithTags("Kullanıcı");

// DELETE: Bir kullanıcıyı silmek
app.MapDelete("/kullanici/{id}", async (TarifDbContext db, int id) =>
{
    var kullanici = await db.Kullanicilar.FindAsync(id);

    if (kullanici == null)
        return Results.NotFound();

    db.Kullanicilar.Remove(kullanici);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).WithTags("Kullanıcı");


#endregion

#region Favori CRUD İşlemleri

// POST: Yeni bir favori ekleme
app.MapPost("/favori/ekle", async (TarifDbContext db, [FromBody] FavoriEkle favoriDTO) =>
{
    // Aynı kullanıcı-tarif kombinasyonu varsa ekleme
    var zatenVarMi = await db.Favoriler
        .AnyAsync(f => f.KullaniciId == favoriDTO.KullaniciId && f.TarifId == favoriDTO.TarifId);

    if (zatenVarMi)
    {
        return Results.Conflict("Bu tarif zaten favorilerde.");
    }
    var favori = new Favori
    {
        KullaniciId = favoriDTO.KullaniciId,
        TarifId = favoriDTO.TarifId,
        EklenmeTarihi = DateTime.Now
    };

    db.Favoriler.Add(favori);
    await db.SaveChangesAsync();

    return Results.Created($"/favori/{favori.KullaniciId}/{favori.TarifId}", favori);
}).WithTags("Favori");

// GET: Kullanıcıya ait favori tarifleri listeleme+
app.MapGet("/favori/kullanici/{kullaniciId}", async (TarifDbContext db, int kullaniciId) =>
{
    var favoriler = await (from f in db.Favoriler
                           join t in db.Tarifler on f.TarifId equals t.Id
                           where f.KullaniciId == kullaniciId
                           select new FavoriDto
                           {
                               KullaniciId = f.KullaniciId,
                               TarifId = f.TarifId,
                               TarifAdi = t.Ad, 
                               EklenmeTarihi = f.EklenmeTarihi

                           }).ToListAsync();

    return favoriler.Any() ? Results.Ok(favoriler) : Results.NoContent();
}).WithTags("Favori");



// DELETE: Bir favoriyi silme+
app.MapDelete("/favori/sil/{kullaniciId}/{tarifId}", async (TarifDbContext db, int kullaniciId, int tarifId) =>
{
    var favori = await db.Favoriler
                         .FirstOrDefaultAsync(f => f.KullaniciId == kullaniciId && f.TarifId == tarifId);

    if (favori == null)
        return Results.NotFound(); 

    db.Favoriler.Remove(favori);
    await db.SaveChangesAsync();

    return Results.Ok($"Favori tarif (Kullanıcı ID: {kullaniciId}, Tarif ID: {tarifId}) başarıyla silindi."); 
}).WithTags("Favori");


// GET: Tarifin favorilerine sahip olan kullanıcıları listeleme-
app.MapGet("/favori/tarif/{tarifId}", async (TarifDbContext db, int tarifId) =>
{
    var favoriler = await db.Favoriler
                            .Where(f => f.TarifId == tarifId)
                            .ToListAsync();

    return favoriler.Any() ? Results.Ok(favoriler) : Results.NoContent();
}).WithTags("Favori");



/*// GET: Tüm favorileri listele
app.MapGet("/favori/listele", async (TarifDbContext db) =>
{
    var favoriler = await db.Favoriler.ToListAsync();
    return favoriler.Any() ? Results.Ok(favoriler) : Results.NoContent();
}).WithTags("Favori");
*/

/*app.MapPut("/favori/guncelle/{kullaniciId}/{tarifId}", async (TarifDbContext db, int kullaniciId, int tarifId, [FromBody] FavoriEkle yeniFavori) =>
{
    var favori = await db.Favoriler
                         .FirstOrDefaultAsync(f => f.KullaniciId == kullaniciId && f.TarifId == tarifId);

    if (favori == null)
        return Results.NotFound("Güncellenecek favori bulunamadı.");

    // Yeni değerlerle güncelle
    favori.KullaniciId = yeniFavori.KullaniciId;
    favori.TarifId = yeniFavori.TarifId;

    await db.SaveChangesAsync();

    return Results.Ok($"Favori güncellendi: KullanıcıID={favori.KullaniciId}, TarifID={favori.TarifId}"); ;
}).WithTags("Favori");*/


#endregion


#region puan
// POST: Puan ekle
app.MapPost("/puan/ekle", async (TarifDbContext db, [FromBody] PuanEkle yeniPuan) =>
{
    // Opsiyonel olarak puan değeri kontrolü yapılabilir
    if (yeniPuan.Deger < 1 || yeniPuan.Deger > 5)
        return Results.BadRequest("Puan değeri 1 ile 5 arasında olmalıdır.");

    var puan = new Puan
    {
        TarifId = yeniPuan.TarifId,
        KullaniciId = yeniPuan.KullaniciId,
        Deger = yeniPuan.Deger,
        Yorum = yeniPuan.Yorum,
        EklenmeTarihi=DateTime.Now,
        GuncellenmeTarihi=DateTime.Now
    };

    db.Puanlar.Add(puan);
    await db.SaveChangesAsync();

    return Results.Created($"/puan/{puan.PuanId}", puan);
}).WithTags("Puan");

// GET: Tüm puanları listele
//tarifler sayfasında tüm kullanıcıların yaptığı puanlar
app.MapGet("/puan/listele", async (TarifDbContext db) =>
{
    var puanlar = await db.Puanlar
        .Include(p => p.Kullanici)  
        .Select(p => new
        {
            p.PuanId,
            p.TarifId,
            p.KullaniciId,
            KullaniciAdi = p.Kullanici.KullaniciAdi, 
            p.Deger,
            p.Yorum,
            p.EklenmeTarihi,
            p.GuncellenmeTarihi
        })
        .ToListAsync();
    return puanlar.Any() ? Results.Ok(puanlar) : Results.NoContent();
}).WithTags("Puan");

// GET: Puanları tarif adına göre ara 
app.MapGet("/puan/ara", async (TarifDbContext db, int? tarifId) =>
{
    var puanlar = await db.Puanlar
    .Include(p => p.Kullanici) 
    .Where(p => p.TarifId == tarifId) 
    .Select(p => new PuanDto
    {
        PuanId = p.PuanId,
        TarifId = p.TarifId,
        KullaniciId = p.KullaniciId,
        KullaniciAdi = p.Kullanici.KullaniciAdi,  
        Deger = p.Deger,
        Yorum = p.Yorum,
        EklenmeTarihi=p.EklenmeTarihi,
        GuncellenmeTarihi=p.GuncellenmeTarihi
    })
    .ToListAsync();


    return puanlar.Any() ? Results.Ok(puanlar) : Results.NoContent();
}).WithTags("Puan");

//yorumlarım sayfasında
app.MapGet("/puan/kullanici/{kullaniciId}/tarif/{tarifId}", async (TarifDbContext db, int kullaniciId, int tarifId) =>
{
    var puanlar = await db.Puanlar
         .Include(p => p.Kullanici)
         .Where(p => p.KullaniciId == kullaniciId && p.TarifId == tarifId)
         .Select(p => new
         {
             p.PuanId,
             p.TarifId,
             p.KullaniciId,
             KullaniciAdi = p.Kullanici.KullaniciAdi,
             p.Deger,
             p.Yorum,
             p.GuncellenmeTarihi
         })
         .ToListAsync();

    return puanlar.Any() ? Results.Ok(puanlar) : Results.NoContent();

}).WithTags("Puan");

// PUT: Puan güncelle
//yorumlarım sayfası
app.MapPut("/puan/guncelle/{id}", async (TarifDbContext db, int id, [FromBody] PuanEkle guncelPuan) =>
{
    var puan = await db.Puanlar.FindAsync(id);
    if (puan == null)
        return Results.NotFound("Puan kaydı bulunamadı.");

    if (guncelPuan.Deger < 1 || guncelPuan.Deger > 5)
        return Results.BadRequest("Puan değeri 1 ile 5 arasında olmalıdır.");

    puan.TarifId = guncelPuan.TarifId;
    puan.KullaniciId = guncelPuan.KullaniciId;
    puan.Deger = guncelPuan.Deger;
    puan.Yorum = guncelPuan.Yorum;
    puan.GuncellenmeTarihi = DateTime.Now;

    await db.SaveChangesAsync();
    return Results.Ok(puan);
}).WithTags("Puan");

// GET: Kullanıcıya ait puanlar (yorumlar dahil)
//yorumlarım sayfası kullanıcının yorumları
app.MapGet("/puan/kullanici/{kullaniciId}", async (TarifDbContext db, int kullaniciId) =>
{
    var puanlar = await db.Puanlar
    .Include(p => p.Tarif)
        .Where(p => p.KullaniciId == kullaniciId)
         .Select(p => new
         {
             p.PuanId,
             p.KullaniciId,
             p.TarifId,
             p.Deger,
             p.Yorum,
             TarifAdi = p.Tarif.Ad,
             p.EklenmeTarihi,
             p.GuncellenmeTarihi
         })
        .ToListAsync();


    return puanlar.Any() ? Results.Ok(puanlar) : Results.NoContent();
}).WithTags("Puan");

// DELETE: Puan sil
app.MapDelete("/puan/sil/", async (TarifDbContext db, int kullaniciId,int tarifId) =>
{
    var puan = await db.Puanlar
       .FirstOrDefaultAsync(p => p.KullaniciId == kullaniciId && p.TarifId == tarifId);
    if (puan == null)
        return Results.NotFound("İlgili puan kaydı bulunamadı.");

    db.Puanlar.Remove(puan);
    await db.SaveChangesAsync();

    return Results.Ok("Puan başarıyla silindi.");
}).WithTags("Puan");
#endregion

#region Tarif-kategori
// POST: Tarif-Kategori eşlemesi ekle
app.MapPost("/tarifkategori/ekle", async (TarifDbContext db, [FromBody] TarifKategoriEkle yeniEsleme) =>
{
    var esleme = new TarifKategori
    {
        TarifId = yeniEsleme.TarifId,
        KategoriId = yeniEsleme.KategoriId
    };

    db.TarifKategoriler.Add(esleme);
    await db.SaveChangesAsync();

    return Results.Created($"/tarifkategori/{esleme.TarifKategoriId}", esleme); 
}).WithTags("TarifKategori");

// GET: Tüm eşleşmeleri listele
/*app.MapGet("/tarifkategori/listeler", async (TarifDbContext db) =>
{
    var eslemeler = await db.TarifKategoriler.ToListAsync();
    return eslemeler.Any() ? Results.Ok(eslemeler) : Results.NoContent();
}).WithTags("TarifKategori");*/

//kategoriId ye göre tarifleri listele
/*app.MapGet("/api/tarifkategoriler/listele", (int kategoriId, TarifDbContext db) =>
{
    var tarifler = from tk in db.TarifKategoriler
                   join t in db.Tarifler on tk.TarifId equals t.Id
                   where tk.KategoriId == kategoriId
                   select new
                   {
                       t.Id,
                       t.Ad
                   };

    return Results.Ok(tarifler.ToList());
}).WithTags("TarifKategori");*/

// GET: Tarif'e göre kategorileri veya kategoriye göre tarifleri getir
/*app.MapGet("/tarifkategori/ara", async (TarifDbContext db, int? tarifId, int? kategoriId) =>
{
    var sorgu = db.TarifKategoriler.AsQueryable();

    if (tarifId.HasValue)
        sorgu = sorgu.Where(x => x.TarifId == tarifId);

    if (kategoriId.HasValue)
        sorgu = sorgu.Where(x => x.KategoriId == kategoriId);

    var eslemeler = await sorgu.ToListAsync();
    return eslemeler.Any() ? Results.Ok(eslemeler) : Results.NoContent();
}).WithTags("TarifKategori");*/

// Belirli bir kategoriye ait tarifleri detaylarıyla döndür
app.MapGet("/tarifkategori/listele", async (TarifDbContext db, int kategoriId) =>
{
    var tarifler = from tk in db.TarifKategoriler
                   join t in db.Tarifler on tk.TarifId equals t.Id
                   where tk.KategoriId == kategoriId
                   select new
                   {
                       t.Id,
                       t.Ad,
                       t.Malzemeler,
                       t.Yapilis,
                       t.EklenmeTarihi,
                       t.EkleyenKullanici
                   };

    var liste = await tarifler.ToListAsync();
    return liste.Any() ? Results.Ok(liste) : Results.NoContent();
}).WithTags("TarifKategori");

// PUT: Tarif-Kategori eşlemesini güncelle
app.MapPut("/tarifkategori/guncelle/{id}", async (TarifDbContext db, int id, [FromBody] TarifKategoriEkle guncel) =>
{
    var esleme = await db.TarifKategoriler.FindAsync(id);
    if (esleme == null)
        return Results.NotFound("Eşleme bulunamadı.");

    esleme.TarifId = guncel.TarifId;
    esleme.KategoriId = guncel.KategoriId;

    await db.SaveChangesAsync();
    return Results.Ok(esleme);
}).WithTags("TarifKategori");

// DELETE: Tarif-Kategori eşlemesini sil
app.MapDelete("/tarifkategori/sil/{id}", async (TarifDbContext db, int id) =>
{
    var esleme = await db.TarifKategoriler.FindAsync(id);
    if (esleme == null)
        return Results.NotFound("Eşleme bulunamadı.");

    db.TarifKategoriler.Remove(esleme);
    await db.SaveChangesAsync();

    return Results.Ok($"ID {id} eşleşmesi silindi.");
}).WithTags("TarifKategori");

#endregion

#region Haftalık Plan

// POST: Yeni haftalık plan ekle
app.MapPost("/haftalikplan/ekle", async (TarifDbContext db, [FromBody] HaftalikPlanEkle planDto) =>
{
    var plan = new HaftalikPlan
    {
        KullaniciId = planDto.KullaniciId,
        GunId = planDto.GunId,
        OgunId = planDto.OgunId,
        TarifId = planDto.TarifId,
        OlusturmaTarihi = DateTime.Now
    };

    db.HaftalikPlanlar.Add(plan);
    await db.SaveChangesAsync();
    return Results.Created($"/haftalikplan/{plan.Id}", plan);
}).WithTags("Haftalık Plan");

// GET: Belirli kullanıcının haftalık planlarını listele
app.MapGet("/haftalikplan/kullanici/{kullaniciId}", async (TarifDbContext db, int kullaniciId) =>
{
    var planlar = await db.HaftalikPlanlar
        .Include(p => p.Gun)
        .Include(p => p.Ogun)
        .Include(p => p.Tarif)
        .Where(p => p.KullaniciId == kullaniciId)
        .ToListAsync();

    var planDtoList = planlar.Select(p => new PlanDto
    {
        Id = p.Id,
        KullaniciId = p.KullaniciId,
        GunId = p.GunId,
        OgunId = p.OgunId,
        TarifId = p.TarifId,
        Gun = p.Gun.Ad,
        Ogun = p.Ogun.Ad,
        TarifAdi = p.Tarif.Ad
    }).ToList();

    return planDtoList.Any() ? Results.Ok(planDtoList) : Results.NoContent();
}).WithTags("Haftalık Plan");

// DELETE: Haftalık plan sil
app.MapDelete("/haftalikplan/{id}", async (TarifDbContext db, int id) =>
{
    var plan = await db.HaftalikPlanlar.FindAsync(id);
    if (plan == null) return Results.NotFound();

    db.HaftalikPlanlar.Remove(plan);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Haftalık Plan");



// GET: Tüm haftalık planları listele 
/*app.MapGet("/haftalikplan/listele", async (TarifDbContext db) =>
{
    var planlar = await db.HaftalikPlanlar
        .Include(p => p.Tarif) 
        .Include(p => p.Gun)    
        .Include(p => p.Ogun)   
        .ToListAsync();

    if (!planlar.Any())
        return Results.NoContent();


    var planDtoList = planlar.Select(p => new
    {
        p.Id,
        p.KullaniciId,
        p.GunId,
        GunAdi = p.Gun.Ad,    
        p.OgunId,
        OgunAdi = p.Ogun.Ad,  
        p.TarifId,
        TarifAdi = p.Tarif.Ad, 
        p.OlusturmaTarihi
    }).ToList();

    return Results.Ok(planDtoList);
}).WithTags("Haftalık Plan");*/

// GET: ID ile haftalık plan getir
/*app.MapGet("/haftalikplan/{id}", async (TarifDbContext db, int id) =>
{
    var plan = await db.HaftalikPlanlar.FindAsync(id);
    return plan == null ? Results.NotFound() : Results.Ok(plan);
}).WithTags("Haftalık Plan");*/

// PUT: Haftalık plan güncelle
/*app.MapPut("/haftalikplan/{id}", async (TarifDbContext db, int id, [FromBody] HaftalikPlanEkle planDto) =>
{
    var mevcutPlan = await db.HaftalikPlanlar.FindAsync(id);
    if (mevcutPlan == null) return Results.NotFound();

    mevcutPlan.KullaniciId = planDto.KullaniciId;
    mevcutPlan.GunId = planDto.GunId;
    mevcutPlan.OgunId = planDto.OgunId;
    mevcutPlan.TarifId = planDto.TarifId;

    await db.SaveChangesAsync();
    return Results.Ok(mevcutPlan);
}).WithTags("Haftalık Plan");*/


#endregion

#region Gün

// POST: Yeni gün ekle
app.MapPost("/gun/ekle", async (TarifDbContext db, [FromBody] GunEkle gunDto) =>
{


    var gun = new Gun { Ad = gunDto.Ad };

    db.Gunler.Add(gun);
    await db.SaveChangesAsync();
    return Results.Created($"/gun/{gun.Id}", gun);
}).WithTags("Gün");

// GET: Tüm günleri listele
app.MapGet("/gun/listele", async (TarifDbContext db) =>
{
    var gunler = await db.Gunler.ToListAsync();
    return gunler.Any() ? Results.Ok(gunler) : Results.NoContent();
}).WithTags("Gün");


// GET: ID ile gün getir
app.MapGet("/gun/{id}", async (TarifDbContext db, int id) =>
{
    var gun = await db.Gunler.FindAsync(id);
    return gun == null ? Results.NotFound() : Results.Ok(gun);
}).WithTags("Gün");

// PUT: Gün güncelle
app.MapPut("/gun/{id}", async (TarifDbContext db, int id, [FromBody] GunEkle gunDto) =>
{
    var mevcutGun = await db.Gunler.FindAsync(id);
    if (mevcutGun == null) return Results.NotFound();

    mevcutGun.Ad = gunDto.Ad;
    await db.SaveChangesAsync();
    return Results.Ok(mevcutGun);
}).WithTags("Gün");

// DELETE: Gün sil
app.MapDelete("/gun/{id}", async (TarifDbContext db, int id) =>
{
    var gun = await db.Gunler.FindAsync(id);
    if (gun == null) return Results.NotFound();

    db.Gunler.Remove(gun);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Gün");

#endregion

#region Öğün

// POST: Yeni öğün ekle
app.MapPost("/ogun/ekle", async (TarifDbContext db, [FromBody] OgunEkle ogunDto) =>
{
    var ogun = new Ogun { Ad = ogunDto.Ad };
    db.Ogunler.Add(ogun);
    await db.SaveChangesAsync();
    return Results.Created($"/ogun/{ogun.Id}", ogun);
}).WithTags("Öğün");

// GET: Tüm öğünleri listele
app.MapGet("/ogun/listele", async (TarifDbContext db) =>
{
    var ogunler = await db.Ogunler.ToListAsync();
    return ogunler.Any() ? Results.Ok(ogunler) : Results.NoContent();
}).WithTags("Öğün");


// GET: ID ile öğün getir
app.MapGet("/ogun/{id}", async (TarifDbContext db, int id) =>
{
    var ogun = await db.Ogunler.FindAsync(id);
    return ogun == null ? Results.NotFound() : Results.Ok(ogun);
}).WithTags("Öğün");

// PUT: Öğün güncelle
app.MapPut("/ogun/{id}", async (TarifDbContext db, int id, [FromBody] OgunEkle ogunDto) =>
{
    var mevcutOgun = await db.Ogunler.FindAsync(id);
    if (mevcutOgun == null) return Results.NotFound();

    mevcutOgun.Ad = ogunDto.Ad;
    await db.SaveChangesAsync();
    return Results.Ok(mevcutOgun);
}).WithTags("Öğün");

// DELETE: Öğün sil
app.MapDelete("/ogun/{id}", async (TarifDbContext db, int id) =>
{
    var ogun = await db.Ogunler.FindAsync(id);
    if (ogun == null) return Results.NotFound();

    db.Ogunler.Remove(ogun);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Öğün");

#endregion






app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

