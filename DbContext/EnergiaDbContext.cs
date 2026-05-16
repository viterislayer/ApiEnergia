using ApiEnergia.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiEnergia.DbContext
{
    public class EnergiaDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public EnergiaDbContext(DbContextOptions<EnergiaDbContext> options) : base(options) { }

        public DbSet<PagosProcesados> PagosProcesados { get; set; }
        public DbSet<ReciboLuz> ReciboLuz { get; set; }
        public DbSet<TitularServicio> TitularServicio { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── pagos_procesados ──────────────────────────────────────────────
            modelBuilder.Entity<PagosProcesados>(entity =>
            {
                entity.ToTable("pagos_procesados");
                entity.HasKey(e => e.IdPago);
                entity.Property(e => e.IdPago)
                      .HasColumnName("id_pago")
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.NumeroContador)
                      .HasColumnName("numero_contador")
                      .HasMaxLength(30)
                      .IsRequired();
                entity.Property(e => e.Monto)
                      .HasColumnName("monto")
                      .HasColumnType("decimal(10,2)")
                      .IsRequired();
                entity.Property(e => e.FechaCobro)
                      .HasColumnName("fecha_cobro")
                      .IsRequired();
                entity.Property(e => e.CanalPago)
                      .HasColumnName("canal_pago")
                      .HasMaxLength(30)
                      .IsRequired();
                entity.Property(e => e.CodigoAutorizacionBanco)
                      .HasColumnName("codigo_autorizacion_banco")
                      .HasMaxLength(50);
            });

            // ── recibo_luz ────────────────────────────────────────────────────
            modelBuilder.Entity<ReciboLuz>(entity =>
            {
                entity.ToTable("recibo_luz");
                entity.HasKey(e => e.IdRecibo);
                entity.Property(e => e.IdRecibo)
                      .HasColumnName("id_recibo")
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.NumeroContador)
                      .HasColumnName("numero_contador")
                      .HasMaxLength(30)
                      .IsRequired();
                entity.Property(e => e.SaldoPendiente)
                      .HasColumnName("saldo_pendiente")
                      .HasColumnType("decimal(10,2)")
                      .IsRequired();
                entity.Property(e => e.FechaEmision)
                      .HasColumnName("fecha_emision")
                      .IsRequired();
            });

            // ── titular_servicio ──────────────────────────────────────────────
            modelBuilder.Entity<TitularServicio>(entity =>
            {
                entity.ToTable("titular_servicio");
                entity.HasKey(e => e.IdTitular);
                entity.Property(e => e.IdTitular)
                      .HasColumnName("id_titular")
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.NumeroContador)
                      .HasColumnName("numero_contador")
                      .HasMaxLength(30)
                      .IsRequired();
                entity.Property(e => e.NombreResponsable)
                      .HasColumnName("nombre_responsable")
                      .HasMaxLength(150)
                      .IsRequired();
                entity.Property(e => e.DireccionInmueble)
                      .HasColumnName("direccion_inmueble")
                      .HasMaxLength(255)
                      .IsRequired();
            });
        }
    }
}