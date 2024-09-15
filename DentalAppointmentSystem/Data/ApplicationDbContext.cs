using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Dentist> Dentists { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PatientDetails> PatientDetails { get; set; }
    public DbSet<OpeningHours> OpeningHours { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Prices> Prices { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<WaitingList> WaitingList { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // تعديل حقل Price لتحديد الدقة والحجم المناسبين
        modelBuilder.Entity<Prices>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)"); // 18 رقم إجمالي، 2 عدد الأرقام بعد الفاصلة
    }

}