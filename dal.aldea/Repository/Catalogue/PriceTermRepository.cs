using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class PriceTermRepository : GenericRepository<biz.premier.Entities.CatPriceTermsDeal>, IPriceTermRepository
    {
        public PriceTermRepository(Db_PremierContext context) : base(context) { }
    }
}
