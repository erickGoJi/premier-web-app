using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class GeneralContractPricingInfoRepository : GenericRepository<biz.premier.Entities.GeneralContractPricingInfo>, IGeneralContractPricingInfoRepository
    {
        public GeneralContractPricingInfoRepository(Db_PremierContext context) : base(context) { }
    }
}
