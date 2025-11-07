using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class State : BaseEntity<int>
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public virtual Country Country { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        public virtual ICollection<LicenseInformation> LicenseInformations { get; set; } = new List<LicenseInformation>();
        public class StateConfiguration : IEntityTypeConfiguration<State>
        {
            public void Configure(EntityTypeBuilder<State> builder)
            {
                builder.HasKey(o => o.Id);
                builder.Property(a => a.Name)
                       .IsRequired(true)
                       .HasMaxLength(100);
                builder.HasOne(s => s.Country)
                   .WithMany(c => c.States) 
                   .HasForeignKey(s => s.CountryId)
                   .OnDelete(DeleteBehavior.Restrict);


            }
        }
    }
}
