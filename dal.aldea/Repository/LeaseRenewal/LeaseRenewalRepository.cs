using System;
using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository.LeaseRenewal;
using dal.premier.DBContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.LeaseRenewal
{
    public class LeaseRenewalRepository : GenericRepository<biz.premier.Entities.LeaseRenewal>, ILeaseRenewalRepository
    {
        public LeaseRenewalRepository(Db_PremierContext context):base(context){}
        public biz.premier.Entities.LeaseRenewal GetCustom(int key)
        {
            if (key == 0)
                return null;
            
            var leaseRenewal = _context.LeaseRenewals
                .Include(i => i.CommentLeaseRenewals)
                    .ThenInclude(i => i.CreationByNavigation)
                        .ThenInclude(i => i.Role)
                .Include(i => i.DocumentLeaseRenewals)
                .ThenInclude(i => i.CountryOriginNavigation)
                .Include(i => i.DocumentLeaseRenewals)
                .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.ReminderLeaseRenewals)
                .SingleOrDefault(x => x.Id == key);
            return leaseRenewal;
        }

        public biz.premier.Entities.LeaseRenewal UpdateCustom(biz.premier.Entities.LeaseRenewal leaseRenewal)
        {
            if (leaseRenewal == null)
                return null;
            var @default = _context.LeaseRenewals
                .Include(i => i.CommentLeaseRenewals)
                .Include(i => i.DocumentLeaseRenewals)
                .Include(i => i.ReminderLeaseRenewals)
                .SingleOrDefault(x => x.Id == leaseRenewal.Id);
            if (@default != null)
            {
                _context.Entry(@default).CurrentValues.SetValues(leaseRenewal);
                // Comment
                foreach (var commentLeaseRenewal in leaseRenewal.CommentLeaseRenewals)
                {
                    var comment = @default.CommentLeaseRenewals.FirstOrDefault(x => x.Id == commentLeaseRenewal.Id);
                    if (comment == null)
                    {
                        @default.CommentLeaseRenewals.Add(commentLeaseRenewal);
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(commentLeaseRenewal);
                    }
                }
                // Document
                foreach (var documentLeaseRenewal in leaseRenewal.DocumentLeaseRenewals)
                {
                    var document = @default.DocumentLeaseRenewals.FirstOrDefault(x => x.Id == documentLeaseRenewal.Id);
                    if (document == null)
                    {
                        @default.DocumentLeaseRenewals.Add(documentLeaseRenewal);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(documentLeaseRenewal);
                    }
                }
                // Reminder
                foreach (var reminderLeaseRenewal in leaseRenewal.ReminderLeaseRenewals)
                {
                    var reminder = @default.ReminderLeaseRenewals.FirstOrDefault(x => x.Id == reminderLeaseRenewal.Id);
                    if (reminder == null)
                    {
                        @default.ReminderLeaseRenewals.Add(reminderLeaseRenewal);
                    }
                    else
                    {
                        _context.Entry(reminder).CurrentValues.SetValues(reminderLeaseRenewal);
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
            try
            {
                var reminder = _context.ReminderLeaseRenewals.FirstOrDefault(x => x.Id == key);
                _context.ReminderLeaseRenewals.Remove(reminder);
                isSuccess = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                isSuccess = false;
                throw new Exception("Record does not exist in the database");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw;
            }

            return isSuccess;
        }

        public DocumentLeaseRenewal FindDocument(int key)
        {
            var document = _context.DocumentLeaseRenewals.FirstOrDefault(x => x.Id == key);
            return document;
        }

        public DocumentLeaseRenewal DeleteDocument(int key)
        {
            bool isSuccess = false;
            var document = _context.DocumentLeaseRenewals.FirstOrDefault(x => x.Id == key);
            try
            {
                _context.DocumentLeaseRenewals.Remove(document);
                isSuccess = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                isSuccess = false;
                throw new Exception("Record does not exist in the database");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw;
            }

            return document;
        }
    }
}