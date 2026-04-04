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

        public Dictionary<string, string> Claims { get; set; }

        public CanvasController(ICanvasRepository canvasRepository)
        {
            _canvasRepository = canvasRepository;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }

        [HttpGet("{canvasId}")]
        public async Task<ActionResult<CanvasDto>> GetCanvasById(int canvasId)
        {
            var userId = GetUserId();
            var canvas = await _canvasRepository.GetAsync(canvasId, userId);

            if (canvas == null) return NotFound();

            var result = new CanvasDto
            {
                Id = canvas.Id,
                Title = canvas.Title,
                Notes = canvas.Notes.Select(n => new NoteDto
                {
                    Id = n.Id,
                    Content = n.Content,
                    X = n.X,
                    Y = n.Y,
                    Width = n.Width,
                    Height = n.Height
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CanvasDto>>> GetUserCanvases()
        {
            var userId = GetUserId();
            var canvases = await _canvasRepository.GetAllByUserIdAsync(userId);

            var result = canvases.Select(c => new CanvasDto
            {
                Id = c.Id,
                Title = c.Title
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CanvasDto>> CreateCanvas(CreateCanvasDto dto)
        {
            var userId = GetUserId();
            var canvas = new Canvas { Title = dto.Title };

            await _canvasRepository.AddAsync(canvas, userId);
            await _canvasRepository.SaveChangesAsync();

            var result = new CanvasDto
            {
                Id = canvas.Id,
                Title = canvas.Title,
                Notes = new List<NoteDto>()
            };

            return CreatedAtAction(nameof(GetCanvasById), new { canvasId = canvas.Id }, result);
        }

        [HttpDelete("{canvasId}")]
        public async Task<IActionResult> DeleteCanvas(int canvasId)
        {
            var userId = GetUserId();
            
            await _canvasRepository.DeleteCanvasAsync(canvasId, userId);
            await _canvasRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{canvasId}/notes")]
        public async Task<ActionResult<NoteDto>> AddNote(int canvasId, CreateNoteDto dto)
        {
            var userId = GetUserId();

            var note = new Note
            {
                Content = dto.Content,
                X = dto.X,
                Y = dto.Y,
                Width = dto.Width,   
                Height = dto.Height  
            };

            var createdNote = await _canvasRepository.AddNoteAsync(canvasId, userId, note);
            if (createdNote == null)
                return Forbid();

            await _canvasRepository.SaveChangesAsync();

            return Ok(new NoteDto
            {
                Id = createdNote.Id,
                Content = createdNote.Content,
                X = createdNote.X,
                Y = createdNote.Y,
                Width = createdNote.Width,
                Height = createdNote.Height
            });
        }

        [HttpPut("{canvasId}/notes/{noteId}")]
        public async Task<ActionResult<NoteDto>> UpdateNote(int canvasId, int noteId, CreateNoteDto dto)
        {
            var userId = GetUserId();
            var updatedNote = await _canvasRepository.UpdateNoteAsync(canvasId, noteId, userId, dto);

            if (updatedNote == null) return NotFound();

            await _canvasRepository.SaveChangesAsync();

            return Ok(new NoteDto
            {
                Id = updatedNote.Id,
                Content = updatedNote.Content,
                X = updatedNote.X,
                Y = updatedNote.Y,
                Width = updatedNote.Width,
                Height = updatedNote.Height
            });
        }

        [HttpPut("{canvasId}/sync")]
        public async Task<IActionResult> SyncCanvas(int canvasId, [FromBody] CanvasSyncDto dto)
        {
            var userId = GetUserId();
            
            await _canvasRepository.SyncNotesAsync(canvasId, userId, dto.Notes);
            await _canvasRepository.SaveChangesAsync();

            var updatedCanvas = await _canvasRepository.GetAsync(canvasId, userId);
            return Ok(updatedCanvas?.Notes.Select(n => new NoteDto {
                Id = n.Id,
                Content = n.Content,
                X = n.X,
                Y = n.Y,
                Width = n.Width,
                Height = n.Height
            }));
        }
    }
}