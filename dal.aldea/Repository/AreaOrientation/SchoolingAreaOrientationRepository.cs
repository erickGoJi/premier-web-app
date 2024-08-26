using biz.premier.Entities;
using biz.premier.Repository.AreaOrientation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.AreaOrientation
{
    public class SchoolingAreaOrientationRepository : GenericRepository<SchoolingAreaOrientation>, ISchoolingAreaOrientationRepository
    {
        public SchoolingAreaOrientationRepository(Db_PremierContext context) : base(context) { }
    }
}
