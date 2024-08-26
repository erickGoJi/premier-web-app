using biz.premier.Entities;
using biz.premier.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.ReportDay
{
    public interface IReportDayRepository: IGenericRepository<Entities.ReportDay>
    {
        Entities.ReportDay UpdateCustom(Entities.ReportDay reportDay, int key);
        List<Entities.ReportDay> GetReportDay(int sr, int? serviceLine, int? program, DateTime? initialReportDate, DateTime? finalReportDate, int? totalTimeAuthorized);
        string GetServiceNameByWorOrder(int idService, int PartnerId);
        int CountByServiceRecord(int key);
        ReportSummary GetTimeRemaining(int service, int workOrder);
        Tuple<string, string> GetNumber(int workOrder, int service);
        int GetCategory(int workOrder, int service);
        ConclusionServiceReportDay AddConclusion(ConclusionServiceReportDay day);
        bool DeleteConclusion(int key);
        int GetActivityReportsByService(int service, int workorder);
        ActionResult GetTotalesActivityReports(int sr);
        int GetTimeRemaindingPublic(int service);
        void createExcelReportDay(int sr, string path);
    }
}
