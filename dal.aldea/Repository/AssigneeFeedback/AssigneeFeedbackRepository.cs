using biz.premier.Repository.AssigneeFeedback;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.AssigneeFeedback
{
    public class AssigneeFeedbackRepository : GenericRepository<biz.premier.Entities.AssigneeFeedback>, IAssigneeFeedbackRepository
    {
        public AssigneeFeedbackRepository(Db_PremierContext context) : base(context) { }

        public IActionResult GetAllCustom(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            var query = _context.AssigneeFeedbacks
                //.Where(x => x.ServiceOrderServices.ServiceOrder.ServiceRecordId == key)
                .Select(s => new
                {
                    s.Id,
                    s.Rating,
                    service = s.ServiceOrderServices.StandaloneServiceWorkOrders.Any()
                        ? s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault(d => d.WorkOrderServiceId == s.ServiceOrderServicesId).Service.Service
                        + "/" + s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault(d => d.WorkOrderServiceId == s.ServiceOrderServicesId).ServiceNumber
                        : s.ServiceOrderServices.BundledServices.Any()
                        ? s.ServiceOrderServices.BundledServices.FirstOrDefault(d => d.WorkServicesId == s.ServiceOrderServicesId).Service.Service
                        + "/" + s.ServiceOrderServices.BundledServices.FirstOrDefault(d => d.WorkServicesId == s.ServiceOrderServicesId).ServiceNumber
                        : "--",
                    s.Feedback
                }).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new ObjectResult(query);
            //return null;
        }

        public int TotalFeed()
        {
            var query = _context.AssigneeFeedbacks.Count();
                //.Where(x => x.ServiceOrderServices.ServiceOrder.ServiceRecordId == key)
            return query;
            //return null;
        }
    }
}
