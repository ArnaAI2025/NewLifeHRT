using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Repositories;
using System.Linq.Expressions;

public class LeadRepository : Repository<Lead, ClinicDbContext>, ILeadRepository
{
    private readonly ClinicDbContext _dbContext;

    public LeadRepository(ClinicDbContext context) : base(context)
    {
        _dbContext = context; 
    }

    
}
