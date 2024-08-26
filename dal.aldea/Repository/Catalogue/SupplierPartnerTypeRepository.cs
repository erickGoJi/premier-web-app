using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class SupplierPartnerTypeRepository : GenericRepository<CatSupplierPartnerType>, ISupplierPartnerTypeRepository
    {
        public SupplierPartnerTypeRepository(Db_PremierContext context) : base(context) { }
    }
}
