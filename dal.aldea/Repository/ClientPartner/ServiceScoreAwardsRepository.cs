using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class ServiceScoreAwardsRepository : GenericRepository<biz.premier.Entities.ServiceScoreAward>, IServiceScoreAwardsRepository
    {
        public ServiceScoreAwardsRepository(Db_PremierContext context) : base(context) { }
    }
}
