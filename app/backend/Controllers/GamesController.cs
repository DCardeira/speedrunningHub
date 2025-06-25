using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase{
        private readonly AppDbContext _context;
        public GamesController(AppDbContext context){
            _context = context;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames(){
            return await _context.Games
                .Include(g => g.GameImages)
                .ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id){
            var game = await _context.Games
                .Include(g => g.SpeedrunRecords.Where(r => r.IsApproved))
                .Include(g => g.Guides.Where(gd => gd.IsApproved))
                .Include(g => g.GameImages)
                .FirstOrDefaultAsync(g => g.GameId == id);

            if (game == null)
                return NotFound();

            return game;
        }

        // POST: api/Games
        // TODO: Proteger este endpoint para função de moderador
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(Game game){
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = game.GameId }, game);
        }
    }
}