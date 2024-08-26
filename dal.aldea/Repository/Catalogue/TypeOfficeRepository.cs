using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class TypeOfficeRepository : GenericRepository<biz.premier.Entities.TypeOffice>, ITypeOfficeRepository
    {
        public TypeOfficeRepository(Db_PremierContext context) : base(context) { }
    }
}
