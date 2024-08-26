using biz.premier.Repository.ReportDay;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ReportDay
{
    public class ServiceReportDayRepository : GenericRepository<biz.premier.Entities.ServiceReportDay>, IServiceReportDayRepository
    {
        public ServiceReportDayRepository(Db_PremierContext context) : base(context) { }
    }
}
