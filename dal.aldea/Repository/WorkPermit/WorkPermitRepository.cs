using biz.premier.Entities;
using biz.premier.Repository.WorkPermit;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.WorkPermit
{
    public class WorkPermitRepository : GenericRepository<biz.premier.Entities.WorkPermit>, IWorkPermitRepository
    {
        public WorkPermitRepository (Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.WorkPermit GetCustomWorkPermit(int key)
        {
            biz.premier.Entities.WorkPermit review = _context.WorkPermits
                .Include(x => x.ReminderWorkPermits)
                .Include(x => x.DocumentWorkPermits)
                .Include(x => x.CommentsWorkPermits)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .Where(x => x.Id == key)
                .FirstOrDefault();
            return review;
        }

        public biz.premier.Entities.WorkPermit UpdateCustom(biz.premier.Entities.WorkPermit workPermit)
        {
            if (workPermit == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.WorkPermit>()
                .Include(i => i.CommentsWorkPermits)
                .Single(s => s.Id == workPermit.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(workPermit);
                foreach (var package in workPermit.CommentsWorkPermits)
                {
                    var comments = exist.CommentsWorkPermits.Where(p => p.Id == package.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        package.User = null;
                        exist.CommentsWorkPermits.Add(package);
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
