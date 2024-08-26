using System.Collections.Generic;
using biz.premier.Entities;
using biz.premier.Models;

namespace biz.premier.Repository.TenancyManagement
{
    public interface ITenancyManagementRepository : IGenericRepository<Entities.TenancyManagement>
    {
        Entities.TenancyManagement GetCustom(int key);
        List<ReportAnEventTable> GetReportAnEventTable(int key);
        Entities.TenancyManagement UpdateCustom(Entities.TenancyManagement tenancyManagement, int key);
        bool DeleteReminder(int key);
        bool DeleteDocument(DocumentTenancyManagement key);
        DocumentTenancyManagement FindDocument(int key);
    }
}