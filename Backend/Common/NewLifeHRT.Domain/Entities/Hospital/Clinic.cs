
namespace NewLifeHRT.Domain.Entities.Hospital
{
    public class Clinic : BaseEntity
    {
        public Guid? TenantId { get; set; }
        public string ClinicName { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public string Database { get; set; }
        public string HostUrl { get; set; }
        public string JwtBearerAudience { get; set; }
        public string IdentityOptions { get; set; }
    }
}
