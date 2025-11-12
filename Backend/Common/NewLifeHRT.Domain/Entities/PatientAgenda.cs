using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class PatientAgenda : BaseEntity<Guid>
    {
        public int AgendaId { get; set; }
        public Guid PatientId { get; set; }
        public virtual Agenda Agenda { get; set; }
        public virtual Patient Patient { get; set; }



        public class PatientAgendaConfiguration : IEntityTypeConfiguration<PatientAgenda>
        {
            public void Configure(EntityTypeBuilder<PatientAgenda> builder)
            {
                builder.HasKey(e => e.Id);

                builder.Property(e => e.Id).IsRequired();

                builder.Property(e => e.AgendaId).IsRequired();
                builder.Property(e => e.PatientId).IsRequired();


                builder.HasOne(e => e.Agenda)
                       .WithMany(a => a.PatientAgendas)
                       .HasForeignKey(e => e.AgendaId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(e => e.Patient)
                       .WithMany(p => p.PatientAgendas)
                       .HasForeignKey(e => e.PatientId)
                       .OnDelete(DeleteBehavior.Cascade);


            }
        }
    }
}
