using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpeedRunningHub.DTOs;
using SpeedRunningHub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpeedRunningHub.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;

        // Injeção de dependências para gestão de utilizadores, papéis e configurações.
        public AuthController(UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration configuration) {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // Endpoint para registar um novo utilizador.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto) {
            // Verifica se o nome de utilizador já existe na base de dados.
            var userExists = await _userManager.FindByNameAsync(registerDto.UserName);
            if (userExists != null)
                return BadRequest("O nome de utilizador já existe.");

            // Cria uma nova instância do utilizador.
            var user = new User {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString() // Necessário para o Identity.
            };

            // Usa o UserManager para criar o utilizador, o que automaticamente faz o hash da password.
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Atribui o papel "Runner" por defeito a todos os novos utilizadores.
            if (await _roleManager.RoleExistsAsync("Runner")) {
                await _userManager.AddToRoleAsync(user, "Runner");
            }

            return Ok("Utilizador criado com sucesso!");
        }

        // Endpoint para autenticar um utilizador e obter um token.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto) {
            // Procura o utilizador pelo nome.
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            // Verifica se o utilizador existe e se a password está correta.
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password)) {
                // Obtém os papéis (roles) associados ao utilizador.
                var userRoles = await _userManager.GetRolesAsync(user);

                if (user.UserName == null) 
                return StatusCode(500, "Username is missing for the user.");

                // Cria a lista de 'claims' para o token, que são informações sobre o utilizador.
                var authClaims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // Adiciona os papéis do utilizador às claims.
                foreach (var userRole in userRoles) {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var configJwtKey = _configuration["JwtSettings:SecretKey"];
                if (string.IsNullOrEmpty(configJwtKey))
                    return StatusCode(500, "JWT Key is not configured.");

                // Obtém a chave secreta para assinar o token a partir das configurações.
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configJwtKey));

                // Cria o token JWT com as claims, data de expiração e credenciais de assinatura.
                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                // Retorna uma resposta 200 OK com o token e informação básica do utilizador.
                return Ok(new AuthResponseDto {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    UserName = user.UserName,
                    Roles = userRoles
                });
            }
            // Se a autenticação falhar, retorna uma resposta 401 Unauthorized.
            return Unauthorized();
        }
    }
}