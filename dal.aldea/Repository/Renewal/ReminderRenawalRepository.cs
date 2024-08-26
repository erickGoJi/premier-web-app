using biz.premier.Entities;
using biz.premier.Repository.Renewal;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Renewal
{
    public class ReminderRenawalRepository : GenericRepository<ReminderRenewal>, IReminderRenawalRepository
    {
        public ReminderRenawalRepository(Db_PremierContext context) : base(context) { }
    }
}
