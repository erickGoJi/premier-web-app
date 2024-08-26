using biz.premier.Entities;
using biz.premier.Repository.PredicisionOrientation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.PredicisionOrientation
{
    public class ReminderPredicisionOrientationRepository : GenericRepository<ReminderPredecisionOrientation>, IReminderPredicisionOrientationRepository
    {
        public ReminderPredicisionOrientationRepository(Db_PremierContext context) : base(context) { }
    }
}
