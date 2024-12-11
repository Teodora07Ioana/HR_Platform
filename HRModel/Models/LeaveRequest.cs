using System.ComponentModel.DataAnnotations;

namespace HR_Platform.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string? Status { get; set; }

        public Employee? Employee { get; set; }
    }

}

   