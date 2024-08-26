using biz.premier.Entities;
using biz.premier.Repository.Transportation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Transportation
{
    public class TransportationPickupRepository : GenericRepository<TransportPickup>, ITransportationPickupRepository
    {
        public TransportationPickupRepository(Db_PremierContext context) : base(context) { }
    }
}
