using biz.premier.Entities;
using biz.premier.Repository.ResidencyPermit;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ResidencyPermit
{
    public class ReminderResidencyPermitRepository: GenericRepository<ReminderResidencyPermit>, IReminderResidencyPermitRepository
    {
        public ReminderResidencyPermitRepository(Db_PremierContext context) : base(context) { }
    }
}
