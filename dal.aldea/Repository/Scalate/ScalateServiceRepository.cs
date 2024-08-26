using biz.premier.Entities;
using biz.premier.Repository.Scalate;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Scalate
{
    public class ScalateServiceRepository : GenericRepository<ScalateService>, IScalateServiceRepository
    {
        public ScalateServiceRepository(Db_PremierContext context) : base(context) { }

        public ScalateService selectCustom(int key, int sr)
        {
            var query = _context.Set<ScalateService>()
                .Include(i => i.ScalateComments)
                    .ThenInclude(i => i.UserFrom)
                .Include(i => i.ScalateComments)
                    .ThenInclude(i => i.UserTo)
                .Include(i => i.ScalateDocuments)
                .Where(s => s.UserFromId == key | s.UserToId == key && s.ServiceRecordId == sr).FirstOrDefault();
            if (query != null)
            {
                return query;
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetEscalationCommunication(int service_line, int sr)
        {
            var query = _context.ScalateServices
                .Where(x => x.ServiceLineId == service_line && x.ServiceRecordId == sr)
                .Select(c => new
                {
                    c.Id,
                    UserFrom = c.UserFrom.Name + " " + c.UserFrom.LastName,
                    c.UserFromId,
                    To = c.UserTo.Name + " " + c.UserTo.LastName,
                    c.UserToId,
                    c.EscalationLevel.Value,
                    c.ServiceLine.ServiceLine,
                    c.ServiceLineId,
                    c.WorkOrder.NumberWorkOrder,
                    c.ServiceId,
                    service_name = _context.BundledServices.Any(x => x.WorkServicesId == c.ServiceId) ? _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == c.ServiceId).Service.Service
                                                          : _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == c.ServiceId).Service.Service,
                    //service_name = _context.CatServices.SingleOrDefault(s => s.Id == c.ServiceId).Service,
                    Status = c.Status == true ? "Active" : "Closed",
                    c.Escalation,
                    c.ClosedDate,
                    c.WorkOrderId

                }).ToList();
            if (query != null)
            {
                return new ObjectResult(query);
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetEscalationCommunicationById(int id)
        {
            var query = _context.ScalateServices
                .Where(x => x.Id == id)
                .Select(c => new
                {
                    c.Id,
                    UserFrom = c.UserFrom.Name + " " + c.UserFrom.LastName,
                    c.UserFromId,
                    To = c.UserTo.Name + " " + c.UserTo.LastName,
                    c.UserToId,
                    EscalationLevel = c.EscalationLevel.Value,
                    c.ServiceLine.ServiceLine,
                    c.ServiceLineId,
                    c.WorkOrderId,
                    c.WorkOrder.NumberWorkOrder,
                    c.ServiceId,
                    service_name = _context.BundledServices.Any(x => x.WorkServicesId == c.ServiceId) ? _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == c.ServiceId).Service.Service
                                                          : _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == c.ServiceId).Service.Service,
                    c.Status,
                    c.Escalation,
                    c.ClosedDate,
                    c.ScalateDocuments,
                    ScalateComments = c.ScalateComments.Select(n => new {
                        n.Id,
                        n.Comments,
                        n.ScalateServiceId,
                        n.Date,
                        name_user = n.UserTo.Name + " " + n.UserTo.LastName,
                        tittle_user = n.UserTo.UserType.Type,
                        avatar_user = n.UserTo.Avatar
                    }).ToList()

                }).ToList();
            if (query != null)
            {
                return new ObjectResult(query);
            }
            else
            {
                return null;
            }
        }
    }
}
