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
    public class AttendancesController : ControllerBase
    {
        private readonly HRManagementContext _context;

        public AttendancesController(HRManagementContext context)
        {
            _context = context;
        }

        // GET: api/Attendances
        [HttpGet]
        [HttpGet("user/{email}")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetUserAttendances(string email)
        {
            var attendances = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Employee.Email == email)
                .ToListAsync();

            return attendances;
        }


        // GET: api/Attendances/5
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
        {
            if (_context.Attendances == null)
            {
                return NotFound();
            }

            return await _context.Attendances
                .Include(a => a.Employee) // Include angajatul asociat pontajului
                .ToListAsync();
        }


        // POST: api/Attendances
        [HttpPost]
        public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendance)
        {
            if (_context.Attendances == null)
            {
                return Problem("Entity set 'HRManagementContext.Attendances' is null.");
            }

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttendance", new { id = attendance.AttendanceID }, attendance);
        }

        // PUT: api/Attendances/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, Attendance attendance)
        {
            if (id != attendance.AttendanceID)
            {
                return BadRequest();
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Attendances.Any(e => e.AttendanceID == id))
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

        // DELETE: api/Attendances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            if (_context.Attendances == null)
            {
                return NotFound();
            }
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
