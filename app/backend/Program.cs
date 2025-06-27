using Azure.Identity;                           // Azure Key Vault auth
using Azure.Security.KeyVault.Secrets;           // Key Vault client
using Azure.Storage.Blobs;                       // Blob Storage client
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpeedRunningHub.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configurar Azure Key Vault como fonte de configuração ---
var kvUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrEmpty(kvUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(kvUrl),
        new DefaultAzureCredential()
    );
}

// --- 2. Ler secrets do Key Vault (ou fallback para appsettings) ---
var connString = builder.Configuration["DefaultConnection"];
var jwtSecret   = builder.Configuration["JwtSettings:SecretKey"];
var blobConn    = builder.Configuration["BlobStorage:ConnectionString"];

// --- 3. Configurar EF Core com MySQL ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connString,
        new MySqlServerVersion(new Version(8, 0, 27))
    )
);

// --- 4. Configurar cliente Blob Storage ---
builder.Services.AddSingleton(_ => new BlobServiceClient(blobConn));

// --- 5. Configurar autenticação JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience            = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();