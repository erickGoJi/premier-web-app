using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class CompanyTypeRepository : GenericRepository<biz.premier.Entities.CompanyType>, ICompanyTypeRepository
    {
        public CompanyTypeRepository(Db_PremierContext context) : base(context) { }
    }
}
