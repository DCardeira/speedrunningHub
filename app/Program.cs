using Azure.Identity;                           // Azure Key Vault auth
using Azure.Security.KeyVault.Secrets;           // Key Vault client
using Azure.Storage.Blobs;                       // Blob Storage client
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;
using System.Text;

Console.WriteLine("--> A iniciar a aplicação...");

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("--> Builder criado. A configurar serviços...");

// --- 1. Configurar Azure Key Vault como fonte de configuração ---
var kvUrl = builder.Configuration["KeyVault:Url"];
if (!builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(kvUrl)) {
    builder.Configuration.AddAzureKeyVault(
        new Uri(kvUrl),
        new DefaultAzureCredential()
    );
}

// --- 2. Ler secrets do Key Vault (ou fallback para appsettings) ---
var jwtKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(jwtKey)) {
    throw new InvalidOperationException("Jwt:Key not configured in appsettings.json");
}
var blobConn = builder.Configuration["BlobStorage:ConnectionString"];

// read flags + connection‐string
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Ligação MySQL: {defaultConn}");

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

// --- 4. Configurar ASP.NET Core Identity ---
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// --- 5. Configurar cliente Blob Storage ---
builder.Services.AddSingleton(_ => new BlobServiceClient(blobConn));

// --- 6. Configurar autenticação JWT ---
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// --- 7. Configurar CORS (Cross-Origin Resource Sharing) ---
var frontendUrl = builder.Configuration["FrontendURL"];
if (string.IsNullOrEmpty(frontendUrl)) {
    throw new InvalidOperationException("FrontendURL is not configured.");
}
builder.Services.AddCors(options => {
    options.AddPolicy("AllowMyFrontend",
        policy => {
            // Permite pedidos da URL do frontend definida em appsettings.json.
            // É crucial para o desenvolvimento com servidores separados.
            policy.WithOrigins(frontendUrl)
                  .AllowAnyHeader() // Permite todos os cabeçalhos (ex: Authorization para JWT).
                  .AllowAnyMethod(); // Permite todos os métodos HTTP (GET, POST, PUT, DELETE).
        });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DbInitializer>();

Console.WriteLine("--> Serviços configurados. A construir a aplicação...");

var app = builder.Build();

Console.WriteLine("--> Aplicação construída. A configurar a pipeline de pedidos...");

// --- 8. Inicializar BD e inserir mock data ---
Console.WriteLine("--> A obter o DbInitializer...");
using (var scope = app.Services.CreateScope()) {
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    Console.WriteLine("--> A executar SeedAsync()...");
    await initializer.SeedAsync();
    Console.WriteLine("--> SeedAsync() concluído.");
}

// --- 9. Configurar a pipeline de pedidos HTTP ---

// Configura o middleware para servir ficheiros estáticos (ex: imagens de upload).
// A pasta 'wwwroot' continua a ser útil para este propósito.
app.UseStaticFiles();

app.UseHttpsRedirection();

// Ativa a política de CORS que definimos.
// Deve ser chamada antes de UseAuthentication/UseAuthorization.
app.UseCors("AllowMyFrontend");

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();