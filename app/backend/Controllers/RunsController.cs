using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class RunsController : ControllerBase {
        private readonly AppDbContext _context;

        public RunsController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<SpeedrunRecord>> GetRuns() {
            return await _context.SpeedrunRecords.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SpeedrunRecord>> GetRun(int id) {
            var run = await _context.SpeedrunRecords.FirstOrDefaultAsync(r => r.RunId == id);
            if (run is null) return NotFound();
            return run;
        }

        [HttpPost]
        [Authorize(Roles = "Runner")]
        public async Task<ActionResult<SpeedrunRecord>> CreateRun(SpeedrunRecord run) {
            _context.SpeedrunRecords.Add(run);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRun), new { id = run.RunId }, run);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRun(int id, SpeedrunRecord run) {
            if (id != run.RunId) return BadRequest();
            _context.Entry(run).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

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