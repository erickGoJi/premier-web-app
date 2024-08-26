using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Repository.SchoolingSearch;

namespace dal.premier.Repository.SchoolingSearch
{
    public class PaymentSchoolingRepository : GenericRepository<biz.premier.Entities.PaymentSchoolingInformation>, IPaymentSchoolingRepository
    {
        public PaymentSchoolingRepository(Db_PremierContext context) : base(context) { }
    }
}
