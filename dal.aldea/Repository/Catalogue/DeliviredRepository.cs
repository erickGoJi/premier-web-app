using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class DeliviredRepository : GenericRepository<CatDeliviredIn>, IDeliviredRepository
    {
        public DeliviredRepository(Db_PremierContext context) : base(context)
        {

        }
    }
}
