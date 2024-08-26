using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Catalogue
{
    public interface IPricingScheduleRepository : IGenericRepository<biz.premier.Entities.PricingSchedule>
    {
        List<biz.premier.Entities.PricingType> GetPricingType();
    }
}
