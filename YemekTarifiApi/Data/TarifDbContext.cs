using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YemekTarifiApi.Models;
namespace YemekTarifiApi.Data
{
    public class TarifDbContext : DbContext
    {
        public TarifDbContext(DbContextOptions<TarifDbContext> options) : base(options) { }

        public DbSet<Tarif> Tarifler { get; set; }
        public DbSet<Favori> Favoriler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Puan> Puanlar { get; set; }
        public DbSet<TarifKategori> TarifKategoriler { get; set; }
        public DbSet<HaftalikPlan> HaftalikPlanlar {  get; set; }
        public DbSet<Gun>Gunler { get; set; }
        public DbSet<Ogun>Ogunler { get; set; }
    }


}
