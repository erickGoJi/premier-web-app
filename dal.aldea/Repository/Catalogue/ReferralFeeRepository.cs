using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class ReferralFeeRepository : GenericRepository<biz.premier.Entities.ReferralFee>, IReferralFeeRepository
    {
        public ReferralFeeRepository(Db_PremierContext context) : base(context) { }
    }
}
