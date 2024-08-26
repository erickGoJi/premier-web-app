using biz.premier.Entities;

namespace biz.premier.Repository.TenancyManagement
{
    public interface IReportAnEventRepository : IGenericRepository<ReportAnEvent>
    {
        ReportAnEvent UpdateCustom(ReportAnEvent @event, int key);
        bool DeleteAssignedPhoto(int key);
        bool DeleteSupplierConsultantPhoto(int key);
    }
}