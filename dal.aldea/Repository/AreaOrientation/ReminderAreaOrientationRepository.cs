using biz.premier.Entities;
using biz.premier.Repository.AreaOrientation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.AreaOrientation
{
    public class ReminderAreaOrientationRepository : GenericRepository<ReminderAreaOrientation>, IReminderAreaOrientationRepository
    {
        public ReminderAreaOrientationRepository(Db_PremierContext context) : base(context) { }
    }
}
