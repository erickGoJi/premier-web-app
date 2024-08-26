using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository.Other;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Other
{
    public class OtherRepository : GenericRepository<biz.premier.Entities.Other>, IOtherRepository
    {
        public OtherRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public biz.premier.Entities.Other GetCustom(int key)
        {
            if (key == 0)
                return null;
            var find = _context.Others
                .Include(i => i.CommentOthers).ThenInclude(i => i.CreatedByNavigation).ThenInclude(i => i.Role)
                .Include(i => i.ReminderOthers)
                .Include(i => i.DocumentOthers)
                .SingleOrDefault(s => s.Id == key);
            return find;
        }

        public biz.premier.Entities.Other UpdateCustom(biz.premier.Entities.Other other, int key)
        {
            if (other == null)
                return null;
            var @default = _context.Others
                .Include(i => i.CommentOthers).ThenInclude(i => i.CreatedByNavigation).ThenInclude(i => i.UserType)
                .Include(i => i.ReminderOthers)
                .Include(i => i.DocumentOthers)
                .SingleOrDefault(s => s.Id == key);
            if (@default != null)
            {
                _context.Entry(@default).CurrentValues.SetValues(other);
                // Comment
                foreach (var commentOther in other.CommentOthers)
                {
                    var comment = @default.CommentOthers.FirstOrDefault(x => x.Id == commentOther.Id);
                    if (comment == null)
                    {
                        @default.CommentOthers.Add(commentOther);
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(commentOther);
                    }
                }
                // Document
                foreach (var documentOther in other.DocumentOthers)
                {
                    var document = @default.DocumentOthers.FirstOrDefault(x => x.Id == documentOther.Id);
                    if (document == null)
                    {
                        @default.DocumentOthers.Add(documentOther);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(documentOther);
                    }
                }
                // Reminder
                foreach (var reminderOther in other.ReminderOthers)
                {
                    var reminder = @default.ReminderOthers.FirstOrDefault(x => x.Id == reminderOther.Id);
                    if (reminder == null)
                    {
                        @default.ReminderOthers.Add(reminderOther);
                    }
                    else
                    {
                        _context.Entry(reminder).CurrentValues.SetValues(reminderOther);
                    }
                }
                
                _context.SaveChanges();
            }

            UpdateStatusServiceRecord(@default.WorkOrderServices.Value,@default.StatusId.Value);
            return @default;
        }

        public bool DeleteReminder(int key)
        {
            bool isSuccess = false;
            var findReminder = _context.ReminderOthers.Find(key);
            if (findReminder != null)
            {
                _context.ReminderOthers.Remove(findReminder);
                _context.SaveChanges();
                isSuccess = true;
            }
            return isSuccess;
        }

        public DocumentOther FindDocument(int key)
        {
            if (key == 0)
                return null;
            var findDocument = _context.DocumentOthers.Find(key);
            return findDocument;
        }

        public bool DeleteDocument(DocumentOther documentOther)
        {
            bool isSuccess = false;
            if (documentOther == null)
                return isSuccess = false;
            _context.DocumentOthers.Remove(documentOther);
            _context.SaveChanges();
            return isSuccess = true;
        }
    }
}