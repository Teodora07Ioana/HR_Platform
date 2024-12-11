namespace HR_Platform.Models
{
    public class EmployeeTraining
    {
            public int EmployeeID { get; set; }
            public int TrainingProgramID { get; set; }

            public Employee Employee { get; set; }
            public TrainingProgram TrainingProgram { get; set; }

    }
}
