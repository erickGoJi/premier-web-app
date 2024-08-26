using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class PropertySectionRepository : GenericRepository<CatPropertySection>, IPropertySectionRepository
    {
        public PropertySectionRepository(Db_PremierContext context) : base(context) { }
    }
}
