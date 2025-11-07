
namespace NewLifeHRT.Domain.Entities
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set ; }

        public BaseEntity() { }
        public BaseEntity (string? createdBy, DateTime createdAt)
        {
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }

        public BaseEntity(DateTime createdAt)
        {
            CreatedAt = createdAt;
        }
    }
}
