using biz.premier.Entities;
using biz.premier.Repository.ResidencyPermit;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.ResidencyPermit
{
    public class ResidencyPermitRepository : GenericRepository<biz.premier.Entities.ResidencyPermit>, IResidencyPermitRepository
    {
        public ResidencyPermitRepository(Db_PremierContext context): base(context) { }

        public biz.premier.Entities.ResidencyPermit GetResidencyPermitCustom(int key)
        {
            biz.premier.Entities.ResidencyPermit review = _context.ResidencyPermits
                .Include(x => x.ReminderResidencyPermits)
                .Include(x => x.DocumentResidencyPermits)
                .Include(x => x.CommentResidencyPermits)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .Where(x => x.Id == key)
                .FirstOrDefault();
            return review;
        }

        public biz.premier.Entities.ResidencyPermit UpdateCustom(biz.premier.Entities.ResidencyPermit residencyPermit, int key)
        {
            if (residencyPermit == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.ResidencyPermit>()
                .Include(i => i.CommentResidencyPermits)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(residencyPermit);
                foreach (var package in residencyPermit.CommentResidencyPermits)
                {
                    var comments = exist.CommentResidencyPermits.Where(p => p.Id == package.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        package.User = null;
                        exist.CommentResidencyPermits.Add(package);
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
