using biz.premier.Entities;
using biz.premier.Repository.Immigration;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Immigration
{
    public class ImmigrationEntryVisaRepository : GenericRepository<biz.premier.Entities.EntryVisa>, IImmigrationEntryVisaRepository
    {

        public ImmigrationEntryVisaRepository(Db_PremierContext context) : base(context)
        {

        }

        public DocumentEntryVisa DeleteDocument(int key)
        {
            var delete = _context.DocumentEntryVisas.Where(x => x.Id == key).FirstOrDefault();
            if (delete.Id != 0)
            {
                _context.DocumentEntryVisas.Remove(delete);
                _context.SaveChanges();
                return delete;
            }
            else
            {
                return null;
            }
        }

        public Boolean DeleteReminder(int key)
        {
            var delete = _context.ReminderEntryVisas.Where(x => x.Id == key).FirstOrDefault();
            if (delete.Id != 0)
            {
                _context.ReminderEntryVisas.Remove(delete);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public EntryVisa GetEntryVisaCustom(int key)
        {
            biz.premier.Entities.EntryVisa review = _context.EntryVisas
                .Include(x => x.ReminderEntryVisas)
                .Include(x => x.DocumentEntryVisas)
                .Include(x => x.CommentsEntryVisas)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .Where(x => x.Id == key)
                .FirstOrDefault();
            return review;
        }

        public EntryVisa UpdateCustom(EntryVisa visa)
        {
            if (visa == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.EntryVisa>()
                .Include(i => i.DocumentEntryVisas)
                .Include(i => i.ReminderEntryVisas)
                .Include(i => i.ExtensionEntryVisas)
                .Include(i => i.CommentsEntryVisas)
                .Single(s => s.Id == visa.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(visa);
                foreach (var i in visa.ReminderEntryVisas)
                {
                    var assigneeInfo = exist.ReminderEntryVisas.FirstOrDefault(w => w.Id == i.Id);
                    if (assigneeInfo == null)
                    {
                        exist.ReminderEntryVisas.Add(i);
                    }
                    else
                    {
                        _context.Entry(assigneeInfo).CurrentValues.SetValues(i);
                    }
                }

                foreach (var i in visa.DocumentEntryVisas)
                {
                    var assigneeInfo = exist.DocumentEntryVisas.FirstOrDefault(w => w.Id == i.Id);
                    if (assigneeInfo == null)
                    {
                        exist.DocumentEntryVisas.Add(i);
                    }
                    else
                    {
                        _context.Entry(assigneeInfo).CurrentValues.SetValues(i);
                    }
                }

                foreach(var i in visa.ExtensionEntryVisas)
                {
                    var extension = exist.ExtensionEntryVisas.FirstOrDefault(w => w.Id == i.Id);
                    if (extension == null)
                    {
                        exist.ExtensionEntryVisas.Add(i);
                    }
                    else
                    {
                        _context.Entry(extension).CurrentValues.SetValues(i);
                    }
                }

                foreach (var i in visa.CommentsEntryVisas)
                {
                    var comment = exist.CommentsEntryVisas.FirstOrDefault(w => w.Id == i.Id);
                    if (comment == null)
                    {
                        i.User = null;
                        exist.CommentsEntryVisas.Add(i);
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId, exist.StatusId);
            return exist;
        }
    }
}
