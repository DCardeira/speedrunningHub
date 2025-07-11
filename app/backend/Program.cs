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
if (!builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(kvUrl)) {
    builder.Configuration.AddAzureKeyVault(
        new Uri(kvUrl),
        new DefaultAzureCredential()
    );
}

// --- 2. Ler secrets do Key Vault (ou fallback para appsettings) ---
var connString = builder.Configuration["DefaultConnection"];
var jwtSecret   = builder.Configuration["JwtSettings:SecretKey"];
var blobConn    = builder.Configuration["BlobStorage:ConnectionString"];

// read flags + connection‐string
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");

// --- 3. Configurar EF Core com MySQL ---
builder.Services.AddDbContext<AppDbContext>(options => {
    if (useInMemory) {
        options.UseInMemoryDatabase("SpeedrunDb");
    }
    else {
        if (string.IsNullOrEmpty(defaultConn))
            throw new ArgumentNullException("DefaultConnection", "Please set DefaultConnection in your configuration.");
        options.UseMySql(
            defaultConn,
            new MySqlServerVersion(new Version(8, 0, 27))
        );
    }
});

// --- 4. Configurar cliente Blob Storage ---
builder.Services.AddSingleton(_ => new BlobServiceClient(blobConn));

// --- 5. Configurar autenticação JWT ---
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
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
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();

// --- 6. Inicializar BD e inserir mock data ---
using (var scope = app.Services.CreateScope()) {
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.SeedAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();