using biz.premier.Repository.CorporateAssistance;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.CorporateAssitance
{
    public class ReminderCorporateAssistanceRepository : GenericRepository<biz.premier.Entities.RemiderCorporateAssistance>, IReminderCorporateAssistanceRepository
    {
        public ReminderCorporateAssistanceRepository(Db_PremierContext context) : base(context) { }
    }
}
