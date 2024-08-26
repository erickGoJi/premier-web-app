using biz.premier.Entities;
using biz.premier.Repository.PredicisionOrientation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.PredicisionOrientation
{
    public class SchoolingRepository : GenericRepository<Schooling>, ISchoolingRepository
    {
        public SchoolingRepository(Db_PremierContext context) : base(context) { }
    }
}
