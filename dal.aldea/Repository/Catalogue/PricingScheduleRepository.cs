using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace dal.premier.Repository.Catalogue
{
    public class PricingScheduleRepository : GenericRepository<biz.premier.Entities.PricingSchedule>, IPricingScheduleRepository
    {
        public PricingScheduleRepository(Db_PremierContext context) : base(context) { }

        public List<biz.premier.Entities.PricingType> GetPricingType()
        {
            return _context.PricingTypes.ToList();
        }
    }
}
