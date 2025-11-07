using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class MessageRepository : Repository<Message, ClinicDbContext>, IMessageRepository
    {
        public MessageRepository(ClinicDbContext context) : base(context) { }
    }
}
