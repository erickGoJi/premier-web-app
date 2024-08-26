using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class SchoolStatusRepository : GenericRepository<biz.premier.Entities.CatSchoolStatus>, ISchoolStatusRepository
    {
        public SchoolStatusRepository(Db_PremierContext context) : base(context) { }
    }
}
