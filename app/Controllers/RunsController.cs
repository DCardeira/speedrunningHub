using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.DTOs;
using SpeedRunningHub.Models;
using System.Security.Claims;

namespace SpeedRunningHub.Controllers {
    // Controlador responsável pela gestão dos registos de speedruns
    [ApiController]
    // A rota agora está aninhada sob os jogos, como planeado.
    [Route("api/games/{gameId}/runs")]
    public class RunsController : ControllerBase {
        // Contexto da base de dados
        private readonly AppDbContext _context;

        // Construtor: injeta o contexto
        public RunsController(AppDbContext context) {
            _context = context;
        }

        // Obtém todos os registos de speedruns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpeedrunRecord>>> GetRunsForGame(int gameId) {
            var runs = await _context.SpeedrunRecords
                .Where(r => r.GameId == gameId && r.IsApproved)
                .Include(r => r.User) // Inclui a informação do utilizador.
                .OrderBy(r => r.Time)
                .ToListAsync();
            return Ok(runs);
        }

        // Obtém um registo de speedrun pelo seu ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SpeedrunRecord>> GetRun(int id) {
            var run = await _context.SpeedrunRecords.FirstOrDefaultAsync(r => r.RunId == id);
            if (run is null) return NotFound();
            return run;
        }

        // Cria um novo registo de speedrun (apenas utilizadores com o papel 'Runner')
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

        // Atualiza um registo de speedrun existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRun(int id, SpeedrunRecord run) {
            if (id != run.RunId) return BadRequest();
            _context.Entry(run).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent(); // Retorna 204 No Content.
        }

        // Remove um registo de speedrun
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRun(int id) {
            var run = await _context.SpeedrunRecords.FindAsync(id);
            if (run is null) return NotFound();
            _context.SpeedrunRecords.Remove(run);
            await _context.SaveChangesAsync();
            return NoContent(); // Retorna 204 No Content.
        }
    }
}