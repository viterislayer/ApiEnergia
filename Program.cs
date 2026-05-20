using ApiEnergia.DbContext;
using ApiEnergia.Interfaces;
using ApiEnergia.Repositories;
using ApiEnergia.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos Azure MySQL ─────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no configurada.");

builder.Services.AddDbContext<EnergiaDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
            mySqlOptions.CommandTimeout(30);
        }
    )
);

// ── Servicios de negocio ──────────────────────────────────────────────────────
builder.Services.AddScoped<IEnergiaService, EnergiaService>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ── Controladores ─────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ── OpenAPI + Scalar ──────────────────────────────────────────────────────────
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "API Energía Eléctrica";
        document.Info.Version = "v1";
        document.Info.Description = "Sistema de cobro y pago de servicio eléctrico. " +
                                    "Gestión de titulares, recibos y pagos.";
        return Task.CompletedTask;
    });
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ── Pipeline HTTP ─────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Scalar disponible en desarrollo Y producción
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "API Energía Eléctrica";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseAuthorization();

// / → nada
app.MapGet("/", () => Results.NotFound());

// /scalar → sirve el index.html del sitio web
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapControllers();

// ── Verificar conexión BD al iniciar ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EnergiaDbContext>();
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        Console.WriteLine(canConnect
            ? "✅  Conexión a Azure MySQL establecida correctamente."
            : "⚠️   No se pudo conectar a la base de datos.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌  Error de conexión: {ex.Message}");
    }
}

app.Run();