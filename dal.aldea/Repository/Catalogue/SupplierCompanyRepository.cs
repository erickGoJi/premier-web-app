using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class SupplierCompanyRepository : GenericRepository<CatSupplierCompany>, ISupplierCompanyRepository
    {
        public SupplierCompanyRepository(Db_PremierContext context): base(context) { } 
    }
}
