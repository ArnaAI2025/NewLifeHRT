using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities.Hospital;

namespace NewLifeHRT.Infrastructure.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }
        public DbSet<Clinic> Clinics { get; set; }
    }
}
