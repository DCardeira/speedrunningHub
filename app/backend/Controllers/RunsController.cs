using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers {
    // Controlador responsável pela gestão dos registos de speedruns
    [ApiController]
    [Route("api/[controller]")]
    public class RunsController : ControllerBase {
        // Contexto da base de dados
        private readonly AppDbContext _context;

        // Construtor: injeta o contexto
        public RunsController(AppDbContext context) {
            _context = context;
        }

        // Obtém todos os registos de speedruns
        [HttpGet]
        public async Task<IEnumerable<SpeedrunRecord>> GetRuns() {
            return await _context.SpeedrunRecords.ToListAsync();
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
        [Authorize(Roles = "Runner")]
        public async Task<ActionResult<SpeedrunRecord>> CreateRun(SpeedrunRecord run) {
            _context.SpeedrunRecords.Add(run);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRun), new { id = run.RunId }, run);
        }

        // Atualiza um registo de speedrun existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRun(int id, SpeedrunRecord run) {
            if (id != run.RunId) return BadRequest();
            _context.Entry(run).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Remove um registo de speedrun
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRun(int id) {
            var run = await _context.SpeedrunRecords.FindAsync(id);
            if (run is null) return NotFound();
            _context.SpeedrunRecords.Remove(run);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}