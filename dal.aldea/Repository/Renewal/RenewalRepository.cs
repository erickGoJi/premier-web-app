using biz.premier.Entities;
using biz.premier.Repository.Renewal;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Renewal
{
    public class RenewalRepository : GenericRepository<biz.premier.Entities.Renewal>, IRenewalRepository
    {
        public RenewalRepository(Db_PremierContext context): base(context) { }

        public biz.premier.Entities.Renewal GetRenewalById(int id)
        {
            biz.premier.Entities.Renewal review = _context.Renewals
                .Include(x => x.ReminderRenewals)
                .Include(x => x.DocumentRenewals)
                .Include(x => x.CommentRenewals)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .Where(x => x.Id == id)
                .FirstOrDefault();
            return review;
        }

        public biz.premier.Entities.Renewal UpdateCustom(biz.premier.Entities.Renewal renewal, int key)
        {
            if (renewal == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Renewal>()
                .Include(i => i.CommentRenewals)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(renewal);
                foreach (var i in renewal.CommentRenewals)
                {
                    var comments = exist.CommentRenewals.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        i.User = null;
                        exist.CommentRenewals.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId.Value,exist.StatusId.Value);
            return exist;
        }
    }
}
