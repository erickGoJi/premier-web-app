using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class SuccessProbabilityRepository : GenericRepository<biz.premier.Entities.SuccessProbability>, ISuccessProbabilityRepository
    {
        public SuccessProbabilityRepository(Db_PremierContext context) : base(context) { }
    }
}
