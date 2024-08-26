using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class ReportTypeRepository : GenericRepository<CatReportType>, IReportTypeRepository
    {
        public ReportTypeRepository(Db_PremierContext context) : base(context) { }
}
}
