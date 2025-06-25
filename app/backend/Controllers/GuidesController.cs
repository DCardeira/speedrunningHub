using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers{
    [Route("api/games/{gameId}/[controller]")]
    [ApiController]
    public class GuidesController : ControllerBase{
        private readonly AppDbContext _context;
        public GuidesController(AppDbContext context){
            _context = context;
        }

        // GET: api/games/5/guides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guide>>> GetGuides(int gameId){
            return await _context.Guides
                .Where(g => g.GameId == gameId && g.IsApproved)
                .ToListAsync();
        }

        // POST: api/games/5/guides
        // TODO: Proteger este endpoint para função de "runner"
        [HttpPost]
        public async Task<ActionResult<Guide>> PostGuide(int gameId, Guide guide){
            guide.GameId = gameId;
            guide.DateCreated = DateTime.UtcNow;
            guide.IsApproved = false;
            _context.Guides.Add(guide);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuides), new { gameId = gameId }, guide);
        }
    }
}