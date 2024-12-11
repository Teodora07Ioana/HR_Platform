using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HR_Platform.Models;
using Microsoft.AspNetCore.SignalR;
using HR_Platform.Hubs;



namespace HR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly HRManagementContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public LeaveRequestsController(HRManagementContext context)
        {
            _context = context;
            //_hubContext = hubContext;
        }

      
        // GET: api/LeaveRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequests()
        {
            return await _context.LeaveRequests.Include(lr => lr.Employee).ToListAsync();
        }

        // GET: api/LeaveRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests
        .Include(lr => lr.Employee)
        .FirstOrDefaultAsync(lr => lr.LeaveRequestID == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return leaveRequest;
        }

        // PUT: api/LeaveRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveRequest(int id, LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.LeaveRequestID)
            {
                return BadRequest();
            }

            _context.Entry(leaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.LeaveRequests.Any(lr => lr.LeaveRequestID == id))
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


        // POST: api/LeaveRequests
       /* [HttpPost]
        public async Task<ActionResult<LeaveRequest>> PostLeaveRequest(LeaveRequest leaveRequest)
        {
            leaveRequest.Status = "Neaprobat";
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            // Notificare pentru manageri
            await _hubContext.Clients.Group("Managers")
                .SendAsync("ReceiveNotification", $"O nouă cerere de concediu a fost adăugată de angajatul cu ID {leaveRequest.EmployeeID}.");

            return CreatedAtAction("GetLeaveRequest", new { id = leaveRequest.LeaveRequestID }, leaveRequest);
        }
*/

          [HttpPost]

           public async Task<ActionResult<LeaveRequest>> PostLeaveRequest(LeaveRequest leaveRequest)
           {
               leaveRequest.Status = "Neaprobat";
               _context.LeaveRequests.Add(leaveRequest);
               await _context.SaveChangesAsync();

               // Notificare pentru manageri
               await _hubContext.Clients.Group("Managers")
                   .SendAsync("ReceiveNotification", $"O nouă cerere de concediu a fost adăugată de angajatul cu ID {leaveRequest.EmployeeID}.");

               return CreatedAtAction("GetLeaveRequest", new { id = leaveRequest.LeaveRequestID }, leaveRequest);
          }
   

        // DELETE: api/LeaveRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/LeaveRequests/Approve/5
        [HttpPut("Approve/{id}")]
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            leaveRequest.Status = "Aprobat";

            _context.Entry(leaveRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /*[HttpPut("Approve/{id}")]
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            leaveRequest.Status = "Aprobat";
            _context.Entry(leaveRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Trimite notificare către utilizator
            var employee = await _context.Employees.FindAsync(leaveRequest.EmployeeID);
            if (employee != null)
            {
                await _hubContext.Clients.User(employee.Email)
                    .SendAsync("ReceiveNotification", "Cererea ta de concediu a fost aprobată.");
            }

            return NoContent();
        }*/

        /*[HttpPut("Reject/{id}")]
        public async Task<IActionResult> RejectLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            leaveRequest.Status = "Respins";
            _context.Entry(leaveRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Trimite notificare către utilizator
            var employee = await _context.Employees.FindAsync(leaveRequest.EmployeeID);
            if (employee != null)
            {
                await _hubContext.Clients.User(employee.Email)
                    .SendAsync("ReceiveNotification", "Cererea ta de concediu a fost respinsă.");
            }

            return NoContent();
        }
*/

     // PUT: api/LeaveRequests/Reject/5
        [HttpPut("Reject/{id}")]
        public async Task<IActionResult> RejectLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            leaveRequest.Status = "Respins";

            _context.Entry(leaveRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        


    }
}