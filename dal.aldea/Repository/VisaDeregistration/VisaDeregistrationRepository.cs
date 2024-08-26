using biz.premier.Entities;
using biz.premier.Repository.VisaDeregistration;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.VisaDeregistration
{
    public class VisaDeregistrationRepository : GenericRepository<biz.premier.Entities.VisaDeregistration>, IVisaDeregistrationRepository
    {
        public VisaDeregistrationRepository(Db_PremierContext context): base(context) { }

        public biz.premier.Entities.VisaDeregistration GetCustomVisaDeregistration(int key)
        {
            biz.premier.Entities.VisaDeregistration review = _context.VisaDeregistrations
                .Include(x => x.ReminderVisaDeregistrations)
                .Include(x => x.DocumentVisaDeregistrations)
                .Include(x => x.CommentVisaDeregistrations)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .Where(x => x.Id == key)
                .FirstOrDefault();
            return review;
        }

        public biz.premier.Entities.VisaDeregistration UpdateCustom(biz.premier.Entities.VisaDeregistration visaDeregistration, int key)
        {
            if (visaDeregistration == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.VisaDeregistration>()
                .Include(i => i.CommentVisaDeregistrations)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(visaDeregistration);
                foreach (var package in visaDeregistration.CommentVisaDeregistrations)
                {
                    var comments = exist.CommentVisaDeregistrations.Where(p => p.Id == package.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        package.User = null;
                        exist.CommentVisaDeregistrations.Add(package);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(package);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId.Value,exist.StatusId.Value);
            return exist;
        }
    }
}
