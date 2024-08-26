using biz.premier.Entities;
using biz.premier.Repository.WorkOrder;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.WorkOrder
{
    public class SchoolListRepository : GenericRepository<SchoolsList>, ISchoolListRepository
    {
        public SchoolListRepository(Db_PremierContext context) : base(context) { }

    }
}
