using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class PaymentRecurrenceRepository : GenericRepository<biz.premier.Entities.PaymentRecurrence>, IPaymentRecurrenceRepository
    {
        public PaymentRecurrenceRepository(Db_PremierContext context) : base(context) { }
    }
}
