using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Product : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string ProductID { get; set; }
        public bool? LabCorp { get; set; } = false;
        public bool? IsColdStorageProduct { get; set; } = false;
        public Guid? ParentId { get; set; }
        public virtual Product? Parent { get; set; }
        public string? Description { get; set; }
        public bool? Scheduled { get; set; }
        public string? WebProductName { get; set; }
        public string? Protocol { get; set; }
        public bool? WebPopularMedicine { get; set; }

        public int? WebFormId { get; set; }
        public virtual ProductWebForm? WebForm { get; set; }

        public bool? EnableCalculator { get; set; }
        public string? WebStrengths { get; set; }
        public bool? NewEnableCalculator { get; set; }
        public string? WebCost { get; set; }
        public bool? PBPEnable { get; set; }
        public string? WebProductDescription { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int? TypeId { get; set; }
        public virtual ProductType? Type { get; set; }

        public int? Category1Id { get; set; }
        public virtual ProductCategory? Category1 { get; set; }

        public int? Category2Id { get; set; }
        public virtual ProductCategory? Category2 { get; set; }

        public int? Category3Id { get; set; }
        public virtual ProductCategory? Category3 { get; set; }

        public string? LabCode { get; set; }

        public int StatusId { get; set; }
        public virtual ProductStatus Status { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public virtual ICollection<ProductStrength> ProductStrengths { get; set; } = new List<ProductStrength>();
        public virtual ICollection<CommisionRate> CommisionRates { get; set; } = new List<CommisionRate>();

        public virtual ICollection<ProductPharmacyPriceListItem> PriceListItems { get; set; } = new List<ProductPharmacyPriceListItem>();
        public virtual ICollection<ProposalDetail> ProposalDetails { get; set; } = new List<ProposalDetail>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();


        public Product() { }
        public Product(string productID, string name, bool? isColdStorageProduct, bool? isLabCorp, string? labCode, Guid? parentId, int? typeId, int? category1Id, int? category2Id,
            int? category3Id, string? productDescription, string? protocol, bool? isScheduled, string? webProductName, string? webProductDescription, bool? isWebPopularMedicine,
            int? webFormId, string? webStrength, string? webCost, bool? isEnabledCalculator, bool? isNewEnabledCalculator, bool? isPBPEnabled, int statusId, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            ProductID = productID;
            Name = name;
            LabCorp = isLabCorp;
            IsColdStorageProduct = isColdStorageProduct != null ? isColdStorageProduct.Value : false;
            LabCode = labCode;
            ParentId = parentId;
            TypeId = typeId;
            Category1Id = category1Id;
            Category2Id = category2Id;
            Category3Id = category3Id;
            Description = productDescription;
            Protocol = protocol;
            Scheduled = isScheduled;
            WebProductName = webProductName;
            WebProductDescription = webProductDescription;
            WebPopularMedicine = isWebPopularMedicine;
            WebFormId = webFormId;
            WebStrengths = webStrength;
            WebCost = webCost;
            EnableCalculator = isEnabledCalculator;
            NewEnableCalculator = isNewEnabledCalculator;
            PBPEnable = isPBPEnabled;
            StatusId = statusId;
        }
        public class ProductConfiguration : IEntityTypeConfiguration<Product>
        {
            public void Configure(EntityTypeBuilder<Product> entity)
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(p => p.ProductID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.LabCorp)
                    .HasDefaultValue(false);

                entity.Property(p => p.Description)
                    .HasMaxLength(500);

                entity.Property(p => p.WebProductName)
                    .HasMaxLength(200);

                entity.Property(p => p.Protocol)
                    .HasMaxLength(100);

                entity.Property(p => p.WebStrengths)
                    .HasMaxLength(100);

                entity.Property(p => p.WebCost)
                    .HasMaxLength(100);

                entity.Property(p => p.WebProductDescription)
                    .HasMaxLength(500);

                entity.Property(p => p.LabCode)
                    .HasMaxLength(100);

                entity.Property(u => u.IsDeleted).HasDefaultValue(false);

                entity.HasQueryFilter(u => !u.IsDeleted);

                // Self-referencing relationship for Parent
                entity.HasOne(p => p.Parent)
                    .WithMany(p => p.Products)
                    .HasForeignKey(p => p.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign key relationships
                entity.HasOne(p => p.WebForm)
                    .WithMany(p => p.Products)
                    .HasForeignKey(p => p.WebFormId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Type)
                    .WithMany(p => p.Products)
                    .HasForeignKey(p => p.TypeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Category1)
                    .WithMany(p => p.Category1Products)
                    .HasForeignKey(p => p.Category1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Category2)
                    .WithMany(p => p.Category2Products)
                    .HasForeignKey(p => p.Category2Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Category3)
                    .WithMany(p => p.Category3Products)
                    .HasForeignKey(p => p.Category3Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Status)
                    .WithMany(p => p.Products)
                    .HasForeignKey(p => p.StatusId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasMany(p => p.PriceListItems)
                    .WithOne(ppli => ppli.Product)
                    .HasForeignKey(ppli => ppli.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.CommisionRates)
                    .WithOne(ppli => ppli.Product)
                    .HasForeignKey(ppli => ppli.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

    }
}
