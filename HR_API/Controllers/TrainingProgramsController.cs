using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HR_Platform.Models;

namespace HR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingProgramsController : ControllerBase
    {
        private readonly HRManagementContext _context;

        public TrainingProgramsController(HRManagementContext context)
        {
            _context = context;
        }

        


        // GET: api/TrainingPrograms
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingProgram>>> GetTrainingPrograms(string? searchString, string? sortOrder)
        {
            if (_context.TrainingPrograms == null)
            {
                return NotFound();
            }

            var programs = from p in _context.TrainingPrograms select p;

            // Filtering by search string
            if (!string.IsNullOrEmpty(searchString))
            {
                programs = programs.Where(p => p.ProgramName.Contains(searchString) || p.Description.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    programs = programs.OrderByDescending(p => p.ProgramName);
                    break;
                case "start_date":
                    programs = programs.OrderBy(p => p.StartDate);
                    break;
                case "end_date":
                    programs = programs.OrderBy(p => p.EndDate);
                    break;
                default:
                    programs = programs.OrderBy(p => p.ProgramName);
                    break;
            }

            return await programs.ToListAsync();
        }


        // GET: api/TrainingPrograms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingProgram>> GetTrainingProgram(int id)
        {
            if (_context.TrainingPrograms == null)
            {
                return NotFound();
            }

            var trainingProgram = await _context.TrainingPrograms.FindAsync(id);

            if (trainingProgram == null)
            {
                return NotFound();
            }

            return trainingProgram;
        }

        // POST: api/TrainingPrograms
        [HttpPost]
        public async Task<ActionResult<TrainingProgram>> PostTrainingProgram(TrainingProgram trainingProgram)
        {
            if (_context.TrainingPrograms == null)
            {
                return Problem("Entity set 'HRManagementContext.TrainingPrograms' is null.");
            }

            _context.TrainingPrograms.Add(trainingProgram);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrainingProgram", new { id = trainingProgram.TrainingProgramID }, trainingProgram);
        }

        // PUT: api/TrainingPrograms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrainingProgram(int id, TrainingProgram trainingProgram)
        {
            if (id != trainingProgram.TrainingProgramID)
            {
                return BadRequest();
            }

            _context.Entry(trainingProgram).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TrainingPrograms.Any(tp => tp.TrainingProgramID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/TrainingPrograms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainingProgram(int id)
        {
            if (_context.TrainingPrograms == null)
            {
                return NotFound();
            }

            var trainingProgram = await _context.TrainingPrograms.FindAsync(id);
            if (trainingProgram == null)
            {
                return NotFound();
            }

            _context.TrainingPrograms.Remove(trainingProgram);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Enroll")]
        public async Task<IActionResult> Enroll([FromBody] EmployeeTraining enrollment)
        {
            if (enrollment == null)
            {
                return BadRequest("Date invalide pentru înscriere.");
            }

            _context.EmployeeTrainings.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
