using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;

public class PatientCreditCard : BaseEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string CardNumber { get; set; }
    public CreditCardTypeEnum CardType { get; set; }
    public MonthEnum Month { get; set; }
    public string Year { get; set; }
    public bool? IsDefaultCreditCard { get; set; }
    public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Patient Patient { get; set; }
    public class PatientCreditCardConfiguration : IEntityTypeConfiguration<PatientCreditCard>
    {
        public void Configure(EntityTypeBuilder<PatientCreditCard> builder)
        {

            builder.HasKey(cc => cc.Id);

            builder.Property(cc => cc.CardNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(cc => cc.Month)
                    .IsRequired()
                    .HasConversion<string>()  
                    .HasMaxLength(20);        

            builder.Property(cc => cc.CardType)
                   .IsRequired()
                   .HasConversion<string>()  
                   .HasMaxLength(50);        


            builder.Property(cc => cc.Year)
                   .IsRequired()
                   .HasMaxLength(4);

            builder.HasOne(cc => cc.Patient)
                   .WithMany(p => p.PatientCreditCards)
                   .HasForeignKey(cc => cc.PatientId);
        }
    }
}