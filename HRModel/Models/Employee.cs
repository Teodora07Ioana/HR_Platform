using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HR_Platform.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        public string Position { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive.")]
        public decimal Salary { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        public Department? Department { get; set; }
        //public ICollection<LeaveRequest>? LeaveRequests { get; set; }
        //public ICollection<Attendance>? Attendances { get; set; }
        public ICollection<EmployeeTraining>? EmployeeTrainings { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        public string FullName => $"{FirstName} {LastName}";

    }

}
