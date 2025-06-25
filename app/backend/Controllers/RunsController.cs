using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers{
    [Route("api/games/{gameId}/[controller]")]
    [ApiController]
    public class RunsController : ControllerBase{
        private readonly AppDbContext _context;
        public RunsController(AppDbContext context){
            _context = context;
        }

        // GET: api/games/5/runs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpeedrunRecord>>> GetRuns(int gameId){
            return await _context.SpeedrunRecords
                .Where(r => r.GameId == gameId && r.IsApproved)
                .OrderBy(r => r.Time)
                .ToListAsync();
        }

        // POST: api/games/5/runs
        // TODO: Proteger este endpoint para função de "runner"
        [HttpPost]
        public async Task<ActionResult<SpeedrunRecord>> PostRun(int gameId, SpeedrunRecord record){
            record.GameId = gameId;
            record.DateSubmitted = DateTime.UtcNow;
            record.IsApproved = false;
            _context.SpeedrunRecords.Add(record);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRuns), new { gameId = gameId }, record);
        }
    }
}