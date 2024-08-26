using biz.premier.Repository.TemporaryHousingCoordinaton;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.TemporaryHousingCoordinaton
{
    public class TemporaryHousingCoordinatonRepository : GenericRepository<biz.premier.Entities.TemporaryHousingCoordinaton>, ITemporaryHousingCoordinatonRepository
    {
        public TemporaryHousingCoordinatonRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.TemporaryHousingCoordinaton GetCustom(int key)
        {
            var query = _context.TemporaryHousingCoordinatons
                .Include(i => i.DocumentTemporaryHousingCoordinatons)
                .Include(i => i.ReminderTemporaryHousingCoordinatons)
                .Include(i => i.ExtensionTemporaryHousingCoordinatons)
                .Include(i => i.StayExtensionTemporaryHousings)
                .Include(i => i.CommentTemporaryHosuings)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Single(s => s.Id == key);
            return query;
        }

        public biz.premier.Entities.TemporaryHousingCoordinaton UpdateCustom(biz.premier.Entities.TemporaryHousingCoordinaton temporaryHousingCoordinaton, int key)
        {
            if (temporaryHousingCoordinaton == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.TemporaryHousingCoordinaton>()
                .Include(i => i.CommentTemporaryHosuings)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(temporaryHousingCoordinaton);
                foreach (var i in temporaryHousingCoordinaton.CommentTemporaryHosuings)
                {
                    var comment = exist.CommentTemporaryHosuings.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentTemporaryHosuings.Add(i);
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
