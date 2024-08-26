using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository.HomePurchase;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.HomePurchase
{
    public class HomePurchaseRepository : GenericRepository<biz.premier.Entities.HomePurchase>, IHomePurchaseRepository
    {
        public HomePurchaseRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public biz.premier.Entities.HomePurchase GetCustom(int key)
        {
            if (key == 0)
                return null;
            
            var homePurchase = _context.HomePurchases
                .Include(i => i.CommentHomePurchases)
                .ThenInclude(i => i.CreatedByNavigation)
                .ThenInclude(i => i.Role)
                .Include(i => i.DocumentHomePurchases)
                .ThenInclude(i => i.CountryOriginNavigation)
                .Include(i => i.DocumentHomePurchases)
                .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.ReminderHomePurchases)
                .SingleOrDefault(x => x.Id == key);
            return homePurchase;
        }

        public biz.premier.Entities.HomePurchase UpdateCustom(biz.premier.Entities.HomePurchase homePurchase, int key)
        {
            if (homePurchase == null)
                return null;
            var @default = _context.HomePurchases
                .Include(i => i.CommentHomePurchases)
                .Include(i => i.DocumentHomePurchases)
                .Include(i => i.ReminderHomePurchases)
                .SingleOrDefault(x => x.Id == key);
            if (@default != null)
            {
                _context.Entry(@default).CurrentValues.SetValues(homePurchase);
                // Comment
                foreach (var commentHomePurchase in homePurchase.CommentHomePurchases)
                {
                    var comment = @default.CommentHomePurchases.FirstOrDefault(x => x.Id == commentHomePurchase.Id);
                    if (comment == null)
                    {
                        @default.CommentHomePurchases.Add(commentHomePurchase);
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(commentHomePurchase);
                    }
                }
                // Document
                foreach (var documentHomePurchase in homePurchase.DocumentHomePurchases)
                {
                    var document = @default.DocumentHomePurchases.FirstOrDefault(x => x.Id == documentHomePurchase.Id);
                    if (document == null)
                    {
                        @default.DocumentHomePurchases.Add(documentHomePurchase);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(documentHomePurchase);
                    }
                }
                // Reminder
                foreach (var reminderHomePurchase in homePurchase.ReminderHomePurchases)
                {
                    var reminder = @default.ReminderHomePurchases.FirstOrDefault(x => x.Id == reminderHomePurchase.Id);
                    if (reminder == null)
                    {
                        @default.ReminderHomePurchases.Add(reminderHomePurchase);
                    }
                    else
                    {
                        _context.Entry(reminder).CurrentValues.SetValues(reminderHomePurchase);
                    }
                }
                
                _context.SaveChanges();
            }

            UpdateStatusServiceRecord(@default.WorkOrderServices.Value, @default.StatusId.Value);
            return @default;
        }

        public bool DeleteDocument(int key)
        {
            bool isSuccess = false;
            var find = _context.DocumentHomePurchases.Find(key);
            if (find != null)
            {
                _context.DocumentHomePurchases.Remove(find);
                isSuccess = true;
            }

            _context.SaveChanges();
            return isSuccess;
        }

        public bool DeleteReminder(int key)
        {
            bool isSuccess = false;
            var find = _context.ReminderHomePurchases.Find(key);
            if (find != null)
            {
                _context.ReminderHomePurchases.Remove(find);
                isSuccess = true;
            }

            _context.SaveChanges();
            return isSuccess;
        }

        public DocumentHomePurchase FindDocument(int key)
        {
            var find = _context.DocumentHomePurchases.Find(key);
            return find;
        }
    }
}