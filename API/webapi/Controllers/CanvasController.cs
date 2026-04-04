using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.DTOs;
using webapi.Models;
using System.Security.Claims;
using webapi.Repositories;

namespace webapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/canvas")]
    public class CanvasController : ControllerBase
    {
        private readonly ICanvasRepository _canvasRepository;

        public CanvasController(ICanvasRepository canvasRepository)
        {
            _canvasRepository = canvasRepository;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet("{canvasId}")]
        public async Task<ActionResult<CanvasDto>> GetCanvasById(int canvasId)
        {
            var canvas = await _canvasRepository.GetAsync(canvasId, GetUserId());
            if (canvas == null) return NotFound();

            return Ok(new CanvasDto
            {
                Id = canvas.Id,
                Title = canvas.Title,
                Notes = canvas.Notes.Select(n => new NoteDto {
                    Id = n.Id, Content = n.Content, X = n.X, Y = n.Y, Width = n.Width, Height = n.Height
                }).ToList()
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CanvasDto>>> GetUserCanvases()
        {
            var canvases = await _canvasRepository.GetAllByUserIdAsync(GetUserId());
            return Ok(canvases.Select(c => new CanvasDto { Id = c.Id, Title = c.Title }));
        }

        [HttpPost]
        public async Task<ActionResult<CanvasDto>> CreateCanvas(CreateCanvasDto dto)
        {
            var userId = GetUserId();
            var canvas = new Canvas { Title = dto.Title };
            await _canvasRepository.AddAsync(canvas, userId);
            await _canvasRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCanvasById), new { canvasId = canvas.Id }, 
                new CanvasDto { Id = canvas.Id, Title = canvas.Title, Notes = new List<NoteDto>() });
        }

        [HttpDelete("{canvasId}")]
        public async Task<IActionResult> DeleteCanvas(int canvasId)
        {
            await _canvasRepository.DeleteCanvasAsync(canvasId, GetUserId());
            await _canvasRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{canvasId}/sync")]
        public async Task<IActionResult> SyncCanvas(int canvasId, [FromBody] CanvasSyncDto dto)
        {
            var userId = GetUserId();
            await _canvasRepository.SyncNotesAsync(canvasId, userId, dto.Notes);
            await _canvasRepository.SaveChangesAsync();

            var updatedCanvas = await _canvasRepository.GetAsync(canvasId, userId);
            return Ok(updatedCanvas?.Notes.Select(n => new NoteDto {
                Id = n.Id, Content = n.Content, X = n.X, Y = n.Y, Width = n.Width, Height = n.Height
            }));
        }
    }
}