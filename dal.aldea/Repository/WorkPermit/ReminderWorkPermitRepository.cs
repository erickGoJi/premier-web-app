using biz.premier.Entities;
using biz.premier.Repository.WorkPermit;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.WorkPermit
{
    public class ReminderWorkPermitRepository : GenericRepository<ReminderWorkPermit>, IReminderWorkPermitRepository
    {
        public ReminderWorkPermitRepository(Db_PremierContext context) : base(context) { }
    }
}
