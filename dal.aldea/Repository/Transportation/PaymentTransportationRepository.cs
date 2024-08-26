using biz.premier.Entities;
using biz.premier.Repository.Transportation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Transportation
{
    public class PaymentTransportationRepository : GenericRepository<PaymentTransportation>, IPaymentTransportationRepository
    {
        public PaymentTransportationRepository(Db_PremierContext context) : base(context) { }
    }
}
