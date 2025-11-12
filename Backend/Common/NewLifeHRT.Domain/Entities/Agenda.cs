using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class Agenda : BaseEntity<int>
    {
        public string Code { get; set; }
        public string AgendaName { get; set; }
        public virtual   ICollection<PatientAgenda> PatientAgendas { get; set; }


        public class AgendaConfiguration : IEntityTypeConfiguration<Agenda>
        {
            public void Configure(EntityTypeBuilder<Agenda> builder)
            {

                builder.HasKey(a => a.Id);

                builder.Property(a => a.Code)
                       .IsRequired()
                       .HasMaxLength(50);

                builder.Property(a => a.AgendaName)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.HasMany(a => a.PatientAgendas)
                       .WithOne(pa => pa.Agenda)
                       .HasForeignKey(pa => pa.AgendaId);
            }
        }

    }
}
