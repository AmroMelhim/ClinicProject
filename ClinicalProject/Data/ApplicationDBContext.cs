using ClinicProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicProject.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Specialization>().HasData(
              new {Id=(long)1,SpecializationName="Sp1"},
              new {Id=(long)2,SpecializationName="Sp2"}
                );

            modelBuilder.Entity<AppointmentType>().HasData(
              new { Id = (long)1, AppointType = "Check-Up" }
                );
        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<AppointmentType> AppointmentTypes{ get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }




    }
}
