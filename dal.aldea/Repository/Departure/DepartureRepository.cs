using biz.premier.Entities;
using biz.premier.Repository.Departure;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Departure
{
    public class DepartureRepository :GenericRepository<biz.premier.Entities.Departure>, IDepartureRepository
    {
        public DepartureRepository(Db_PremierContext context):base(context) { }

        public biz.premier.Entities.Departure GetCustom(int key)
        {
            var query = _context.Departures
                .Include(i => i.DocumentDepartures)
                .Include(i => i.ReminderDepartures)
                .Include(i => i.DepartureAssistanceWiths)
                .Include(i => i.CommentDepartures)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .SingleOrDefault(s => s.Id == key);
            return query;
        }

        public biz.premier.Entities.Departure UpdateCustom(biz.premier.Entities.Departure departure, int key)
        {
            if (departure == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Departure>()
                .Include(i => i.CommentDepartures)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(departure);
                foreach (var comment in departure.CommentDepartures)
                {
                    var existingComment = exist.CommentDepartures.Where(p => p.Id == comment.Id).FirstOrDefault();
                    if (existingComment == null)
                    {
                        exist.CommentDepartures.Add(comment);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingComment).CurrentValues.SetValues(comment);
                    }
                }

                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId, exist.StatusId);
            return exist;
        }
    }
}
