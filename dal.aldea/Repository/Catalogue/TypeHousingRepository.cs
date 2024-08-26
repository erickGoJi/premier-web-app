using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class TypeHousingRepository : GenericRepository<biz.premier.Entities.CatTypeHousing>, ITypeHousingRepository
    {
        public TypeHousingRepository(Db_PremierContext contex) : base(contex) { }
    }
}
