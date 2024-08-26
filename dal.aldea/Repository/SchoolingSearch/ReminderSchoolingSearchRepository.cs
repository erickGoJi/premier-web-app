using biz.premier.Entities;
using biz.premier.Repository.SchoolingSearch;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.SchoolingSearch
{
    public class ReminderSchoolingSearchRepository : GenericRepository<ReminderSchoolingSearch>, IReminderSchoolingSearchRepository
    {
        public ReminderSchoolingSearchRepository(Db_PremierContext context) : base(context) { }
    }
}
