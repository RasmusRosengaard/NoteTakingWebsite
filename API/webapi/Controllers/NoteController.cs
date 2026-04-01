using Microsoft.AspNetCore.Mvc;
using webapi.DTOs;
using webapi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/notes")]
    public class NoteController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;
        private readonly IConfiguration _configuration;

        public NoteController(INoteRepository noteRepository, IConfiguration configuration)
        {
            _noteRepository = noteRepository;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNoteById(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var note = await _noteRepository.GetAsync(id, userId);

            if (note == null) return NotFound();

            return Ok(note);
        }

    }
}