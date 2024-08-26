using biz.premier.Entities;
using biz.premier.Repository.CorporateAssistance;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.CorporateAssitance
{
    public class CorporateAssistanceRepository : GenericRepository<biz.premier.Entities.CorporateAssistance>, ICorporateAssistanceRepository
    {
        public CorporateAssistanceRepository(Db_PremierContext context): base(context) { }

        public CorporateAssistance GetCorporateAssistanceById(int id)
        {
            biz.premier.Entities.CorporateAssistance review = _context.CorporateAssistances
                .Include(x => x.RemiderCorporateAssistances)
                .Include(x => x.DocumentCorporateAssistances)
                .Include(x => x.CommentCorporateAssistances)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .Where(x => x.Id == id)
                .FirstOrDefault();
            return review;
        }

        public CorporateAssistance UpdateCustom(CorporateAssistance corporateAssistance, int key)
        {
            if (corporateAssistance == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.CorporateAssistance>()
                .Include(i => i.CommentCorporateAssistances)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(corporateAssistance);
                foreach (var i in corporateAssistance.CommentCorporateAssistances)
                {
                    var comments = exist.CommentCorporateAssistances.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        i.User = null;
                        exist.CommentCorporateAssistances.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId.Value, exist.StatusId);
            return exist;
        }
    }
}
