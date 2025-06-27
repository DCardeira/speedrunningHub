using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;
using System.Security.Claims;

namespace SpeedRunningHub.Controllers {
    [Route("api/games/{gameId}/[controller]")]
    [ApiController]
    public class GuidesController : ControllerBase {
        private readonly AppDbContext _context;
        private readonly BlobServiceClient _blobService;

        public GuidesController(AppDbContext context, BlobServiceClient blobService) {
            _context     = context;
            _blobService = blobService;
        }

        // GET: api/games/5/guides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guide>>> GetGuides(int gameId) {
            return await _context.Guides
                .Where(g => g.GameId == gameId && g.IsApproved)
                .Include(g => g.GuideImages)
                .ToListAsync();
        }

        // POST: api/games/5/guides
        [HttpPost]
        [Authorize(Roles = "Runner")]
        public async Task<ActionResult<Guide>> PostGuide(int gameId, Guide guide) {
            guide.GameId      = gameId;
            guide.DateCreated = DateTime.UtcNow;
            guide.IsApproved  = false;
            _context.Guides.Add(guide);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGuides), new { gameId = gameId }, guide);
        }

        // POST: api/games/5/guides/10/images
        [HttpPost("{guideId}/images")]
        [Authorize(Roles = "Runner")]
        public async Task<IActionResult> UploadGuideImage(int gameId, int guideId, IFormFile file) {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum ficheiro recebido.");

            var container = _blobService.GetBlobContainerClient("guide-images");
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobName = $"{guideId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = container.GetBlobClient(blobName);

            await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            var record = new GuideImage {
                GuideId          = guideId,
                FileName         = blobName,
                FilePath         = blobClient.Uri.ToString(),
                UploadedAt       = DateTime.UtcNow,
                UploadedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            _context.GuideImages.Add(record);
            await _context.SaveChangesAsync();

            return Ok(new { record.GuideImageId, record.FilePath });
        }
    }
}