using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Country : BaseEntity<int>
    {
        public string Name { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<State> States { get; set; } = new List<State>();


        public class CountryConfiguration : IEntityTypeConfiguration<Country>
        {
            public void Configure(EntityTypeBuilder<Country> builder)
            {
                builder.HasKey(o => o.Id);
                builder.Property(a => a.Name)
                       .IsRequired()
                       .HasMaxLength(100);

            }
        }
    }
}
