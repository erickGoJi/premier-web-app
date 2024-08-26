using biz.premier.Entities;
using biz.premier.Repository.TypeService;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.TypeService
{
    public class TypeServiceRepository : GenericRepository<CatTypeService>, ITypeServiceRepository
    {
        public TypeServiceRepository(Db_PremierContext context) : base(context) { }
    }
}
