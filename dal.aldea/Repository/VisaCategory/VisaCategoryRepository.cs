using biz.premier.Entities;
using biz.premier.Repository.VisaCategory;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.VisaCategory
{
    public class VisaCategoryRepository : GenericRepository<CatVisaCategory>, IVisaCategoryRepository
    {
        public VisaCategoryRepository(Db_PremierContext context) : base(context) { }
    }
}
