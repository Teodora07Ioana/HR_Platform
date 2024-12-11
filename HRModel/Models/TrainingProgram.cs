namespace HR_Platform.Models
{
    public class TrainingProgram
    {
        public int TrainingProgramID { get; set; }
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<EmployeeTraining>? EmployeeTrainings { get; set; }
    }
}
