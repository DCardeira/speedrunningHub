using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.DTOs;
using SpeedRunningHub.Models;
using System.Security.Claims;

namespace SpeedRunningHub.Controllers {
    [ApiController]
    // A rota agora está aninhada sob os jogos, como planeado.
    [Route("api/games/{gameId}/runs")]
    public class RunsController : ControllerBase {
        private readonly AppDbContext _context;

        public RunsController(AppDbContext context) {
            _context = context;
        }

        // Obtém todas as corridas aprovadas para um jogo específico.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpeedrunRecord>>> GetRunsForGame(int gameId) {
            var runs = await _context.SpeedrunRecords
                .Where(r => r.GameId == gameId && r.IsApproved)
                .Include(r => r.User) // Inclui a informação do utilizador.
                .OrderBy(r => r.Time)
                .ToListAsync();
            return Ok(runs);
        }

        // Obtém uma corrida específica pelo seu ID.
        [HttpGet("{runId:int}", Name = "GetRunById")]
        public async Task<ActionResult<SpeedrunRecord>> GetRun(int gameId, int runId) {
            var run = await _context.SpeedrunRecords
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.GameId == gameId && r.RunId == runId);

            if (run == null) return NotFound();
            
            // Se a corrida não estiver aprovada, apenas o próprio utilizador ou um moderador a pode ver.
            if (!run.IsApproved) {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                if (run.UserId != currentUserId && !userRoles.Contains("Moderator")) {
                    return Forbid(); // Retorna 403 Forbidden se não tiver permissão.
                }
            }

            return Ok(run);
        }

        // Cria uma nova submissão de corrida. Apenas para utilizadores com papel "Runner" ou "Moderator".
        [HttpPost]
        [Authorize(Roles = "Runner,Moderator")]
        public async Task<ActionResult<SpeedrunRecord>> CreateRun(int gameId, [FromBody] RunCreateDto runDto) {
            if (!await _context.Games.AnyAsync(g => g.GameId == gameId))
                return NotFound("Jogo não encontrado.");

            // Obtém o ID do utilizador autenticado a partir do token.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var newRun = new SpeedrunRecord {
                GameId = gameId,
                UserId = userId,
                Time = runDto.Time,
                VideoLink = runDto.VideoLink,
                DateSubmitted = DateTime.UtcNow,
                IsApproved = false // As novas corridas começam como não aprovadas.
            };

            _context.SpeedrunRecords.Add(newRun);
            await _context.SaveChangesAsync();

            // Retorna uma resposta 201 Created com a localização do novo recurso.
            return CreatedAtRoute("GetRunById", new { gameId = newRun.GameId, runId = newRun.RunId }, newRun);
        }

        // Aprova uma corrida. Apenas para moderadores.
        [HttpPut("{runId}/approve")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> ApproveRun(int gameId, int runId) {
            var run = await _context.SpeedrunRecords.FirstOrDefaultAsync(r => r.GameId == gameId && r.RunId == runId);
            if (run == null) return NotFound();

            run.IsApproved = true;
            await _context.SaveChangesAsync();
            return NoContent(); // Retorna 204 No Content.
        }

        // Apaga uma corrida. Apenas para moderadores.
        [HttpDelete("{runId}")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> DeleteRun(int gameId, int runId) {
            var run = await _context.SpeedrunRecords.FirstOrDefaultAsync(r => r.GameId == gameId && r.RunId == runId);
            if (run == null) return NotFound();

            _context.SpeedrunRecords.Remove(run);
            await _context.SaveChangesAsync();
            return NoContent(); // Retorna 204 No Content.
        }
    }
}