using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR_Platform.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public decimal ? HoursWorked { get; set; }

        public Employee? Employee { get; set; }
    }
}
