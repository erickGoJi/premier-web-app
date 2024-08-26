using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.SettlingIn;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.SettlingIn
{
    public class SettlingInRepository : GenericRepository<biz.premier.Entities.SettlingIn>, ISettlingInRepository
    {
        public SettlingInRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.SettlingIn GetCustom(int key)
        {
            var query = _context.SettlingIns
                .Include(i => i.ReminderSettlingIns)
                .Include(i => i.DocumentSettlingIns)
                .Include(i => i.CommentSettlingIns)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Include(i => i.ExtensionSettlingIns)
                .Include(i => i.WorkOrderServices.RequestPayments)
                .Include(i => i.ChildCareSupplierPartnerNavigation)
                .Include(i => i.CleaningServicesSupplierPartnerNavigation)
                .SingleOrDefault(s => s.Id == key);
            return query;
        }

        public biz.premier.Entities.SettlingIn UpdateCustom(biz.premier.Entities.SettlingIn settlingIn, int key)
        {
            if (settlingIn == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.SettlingIn>()
                .Include(i => i.CommentSettlingIns)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(settlingIn);
                foreach (var i in settlingIn.CommentSettlingIns)
                {
                    var comment = exist.CommentSettlingIns.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentSettlingIns.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId,exist.StatusId);
            return exist;
        }
    }
}
