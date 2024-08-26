using System.Linq;
using biz.premier.Repository.HomeSale;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.HomeSale
{
    public class HomeSaleRepository : GenericRepository<biz.premier.Entities.HomeSale>, IHomeSaleRepository
    {
        public HomeSaleRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public biz.premier.Entities.HomeSale GetCustom(int key)
        {
            if (key == 0)
                return null;
            
            var homeSale = _context.HomeSales
                .Include(i => i.CommentHomeSales)
                .ThenInclude(i => i.CreatedByNavigation)
                .ThenInclude(i => i.Role)
                .Include(i => i.DocumentHomeSales)
                .ThenInclude(i => i.CountryOriginNavigation)
                .Include(i => i.DocumentHomeSales)
                .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.ReminderHomeSales)
                .Include(i => i.VisitHomeSales)
                .SingleOrDefault(x => x.Id == key);
            return homeSale;
        }

        public bool DeleteReminder(int key)
        {
            bool isSuccess = false;
            var find = _context.ReminderHomeSales.Find(key);
            if (find != null)
            {
                _context.ReminderHomeSales.Remove(find);
                isSuccess = true;
            }

            _context.SaveChanges();
            return isSuccess;
        }

        public bool DeleteDocument(int key)
        {
            bool isSuccess = false;
            var find = _context.DocumentHomeSales.Find(key);
            if (find != null)
            {
                _context.DocumentHomeSales.Remove(find);
                isSuccess = true;
            }

            _context.SaveChanges();
            return isSuccess;
        }

        public biz.premier.Entities.HomeSale UpdateCustom(biz.premier.Entities.HomeSale homeSale, int key)
        {
            if (homeSale == null)
                return null;
            var @default = _context.HomeSales
                .Include(i => i.CommentHomeSales)
                .Include(i => i.DocumentHomeSales)
                .Include(i => i.ReminderHomeSales)
                .Include(i => i.VisitHomeSales)
                .SingleOrDefault(x => x.Id == key);
            if (@default != null)
            {
                _context.Entry(@default).CurrentValues.SetValues(homeSale);
                // Comment
                foreach (var commentHomeSale in homeSale.CommentHomeSales)
                {
                    var comment = @default.CommentHomeSales.FirstOrDefault(x => x.Id == commentHomeSale.Id);
                    if (comment == null)
                    {
                        @default.CommentHomeSales.Add(commentHomeSale);
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(commentHomeSale);
                    }
                }
                // Document
                foreach (var documentHomeSale in homeSale.DocumentHomeSales)
                {
                    var document = @default.DocumentHomeSales.FirstOrDefault(x => x.Id == documentHomeSale.Id);
                    if (document == null)
                    {
                        @default.DocumentHomeSales.Add(documentHomeSale);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(documentHomeSale);
                    }
                }
                // Reminder
                foreach (var reminderHomeSale in homeSale.ReminderHomeSales)
                {
                    var reminder = @default.ReminderHomeSales.FirstOrDefault(x => x.Id == reminderHomeSale.Id);
                    if (reminder == null)
                    {
                        @default.ReminderHomeSales.Add(reminderHomeSale);
                    }
                    else
                    {
                        _context.Entry(reminder).CurrentValues.SetValues(reminderHomeSale);
                    }
                }
                // Visit Home Sales
                foreach (var visitHomeSale in homeSale.VisitHomeSales)
                {
                    var firstOrDefault = @default.VisitHomeSales.FirstOrDefault(x => x.Id == visitHomeSale.Id);
                    if (firstOrDefault == null)
                    {
                        @default.VisitHomeSales.Add(visitHomeSale);
                    }
                    else
                    {
                        _context.Entry(firstOrDefault).CurrentValues.SetValues(visitHomeSale);
                    }
                }
                
                _context.SaveChanges();
            }

            UpdateStatusServiceRecord(@default.WorkOrderServices.Value, @default.StatusId.Value);
            return @default;
        }
    }
}