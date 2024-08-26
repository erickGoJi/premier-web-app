using biz.premier.Entities;
using biz.premier.Repository.Transportation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Transportation
{
    public class ReminderTransportationRepository : GenericRepository<ReminderTransportation>, IReminderTransportationRepository
    {
        public ReminderTransportationRepository(Db_PremierContext context) : base(context) { }
    }
}
