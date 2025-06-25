using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddControllers();
// Configurar EF Core com o MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 27))));

// Autentificação e configuração da Identidade (tbd)

var app = builder.Build();

// Configurar o pipeline de solicitação HTTP
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();