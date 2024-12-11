using HR_Platform.Models;
using Microsoft.EntityFrameworkCore;

public class HRManagementContext : DbContext
{
    // Constructor
    public HRManagementContext(DbContextOptions<HRManagementContext> options)
        : base(options) { }

    // Definirea tabelelor din baza de date
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<TrainingProgram> TrainingPrograms { get; set; }
    public DbSet<EmployeeTraining> EmployeeTrainings { get; set; }



    // Configurarea relațiilor dintre tabele
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurarea relației many-to-many între Employee și TrainingProgram
        modelBuilder.Entity<EmployeeTraining>()
            .HasKey(et => new { et.EmployeeID, et.TrainingProgramID });

        modelBuilder.Entity<EmployeeTraining>()
            .HasOne(et => et.Employee)
            .WithMany(e => e.EmployeeTrainings)
            .HasForeignKey(et => et.EmployeeID);

        modelBuilder.Entity<EmployeeTraining>()
            .HasOne(et => et.TrainingProgram)
            .WithMany(tp => tp.EmployeeTrainings)
            .HasForeignKey(et => et.TrainingProgramID);
    


    }
}
