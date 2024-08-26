using biz.premier.Repository.Call;
using dal.premier.DBContext;
using biz.premier.Models.Call;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace dal.premier.Repository.Call
{
    public class CallRepository : GenericRepository<biz.premier.Entities.Call>, ICallRepository
    {
        public CallRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetworkOrderBySR(int service_record_id, int service_line_id)
        {
            var service = _context.WorkOrders
                .Where(e => e.ServiceRecordId == service_record_id && e.ServiceLineId == service_line_id)
                .Select(k => new
                {
                    k.Id,
                    k.NumberWorkOrder
                }).OrderBy(n => n.NumberWorkOrder).ToList();

            return new ObjectResult(service);
        }

        public ActionResult GetServicesBySO(int service_record_id, int service_line_id)
        {
            var service = _context.StandaloneServiceWorkOrders
                .Where(e => e.WorkOrder.ServiceRecordId == service_record_id && e.WorkOrder.ServiceLineId == service_line_id)
                .Select(k => new
                {
                    //k.Id,
                    Id = k.WorkOrderService.Id,
                    k.ServiceNumber,
                    k.Category.Category
                }).OrderBy(n => n.Id).ToList();

            var service2 = _context.BundledServices.
                Where(e => e.BundledServiceOrder.WorkOrder.ServiceRecordId == service_record_id && e.BundledServiceOrder.WorkOrder.ServiceLineId == service_line_id)
                .Select(k => new
                {
                    //k.Id,
                    Id = k.WorkServicesId.Value,
                    k.ServiceNumber,
                    k.Category.Category
                }).OrderBy(n => n.Id).ToList();

            return new ObjectResult(service.Union(service2.ToList()));
        }

        public List<CallByServiceRecord> GetCallByServiceRecord(int service_record_id, int service_line_id)
        {
            var service = _context.Calls
                .Include(x => x.CallerNavigation)
                .Include(x => x.CalleNavigation)
                .Include(x => x.DurationNavigation)
                .Where(e => e.ServiceRecordId == service_record_id && e.ServiceLineId == service_line_id)
                .Select(k => new CallByServiceRecord
                {
                    Id = k.Id,
                    ServiceLine = k.ServiceLine.ServiceLine,
                    WorkOrder = k.WorkOrder.NumberWorkOrder,
                    ServiceId = k.ServiceId.ToString(),
                    ServiceRecordId = k.ServiceRecordId,
                    Caller = k.CallerNavigation.Name + " " + k.CallerNavigation.LastName,
                    Calle = k.CallerNavigation.Name + " " + k.CallerNavigation.LastName,
                    Date = k.Date,
                    Time = k.Time,
                    Escalate = k.Escalate.Value,
                    Duration = k.DurationNavigation.Duration,
                    WelcomeCall = k.WelcomeCall,
                    Service = k.Service.BundledServices.Any() ? k.Service.BundledServices.Where(x => x.WorkServicesId == k.ServiceId).FirstOrDefault().Category.Category 
                        : k.Service.StandaloneServiceWorkOrders.Where(x => x.WorkOrderServiceId == k.ServiceId).FirstOrDefault().Category.Category,                    
                    Comments = k.Comments

                }).OrderBy(n => n.Id).ToList();

            return service;
        }

        public biz.premier.Entities.Call UpdateCustom(biz.premier.Entities.Call call)
        {
            if (call == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Call>()
                .Include(i => i.CallAssistants)
                .SingleOrDefault(s => s.Id == call.Id);
            if(exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(call);
                exist.CallAssistants.Clear();
                _context.SaveChanges();
                foreach (var o in call.CallAssistants)
                {
                    exist.CallAssistants.Add(o);
                }
                _context.SaveChanges();
            }
            return exist;
        }
    }
}
