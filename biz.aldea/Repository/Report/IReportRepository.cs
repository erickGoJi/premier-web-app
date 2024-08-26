using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Report
{
    public interface IReportRepository : IGenericRepository<Entities.Report>
    {
        List<Entities.Filter> AddOrEditFilters(List<Entities.Filter> filters, int report);
        List<Entities.Column> AddOrEditColumns(List<Entities.Column> columns, int report);
        List<Entities.Filter> AddFiltersOpertionals(List<Entities.Filter> filters, int report, int reportType);
        ActionResult GetCustom(int user, int report);
        ActionResult ReturnReport();
    }
}
