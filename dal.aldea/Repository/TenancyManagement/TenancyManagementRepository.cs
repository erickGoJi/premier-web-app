using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using biz.premier.Entities;
using biz.premier.Models;
using biz.premier.Repository.TenancyManagement;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.TenancyManagement
{
    public class TenancyManagementRepository : GenericRepository<biz.premier.Entities.TenancyManagement>, ITenancyManagementRepository
    {
        public TenancyManagementRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public biz.premier.Entities.TenancyManagement GetCustom(int key)
        {
            if (key == 0)
                return null;
            var managements = _context.TenancyManagements
                .Include(i => i.DocumentTenancyManagements)
                .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.DocumentTenancyManagements)
                .ThenInclude(i => i.CountryOriginNavigation)
                .Include(i => i.CommentTenancyManagements)
                .ThenInclude(i => i.CreatedByNavigation)
                .ThenInclude(i => i.Role)
                .Include(i => i.ReminderTenancyManagements)
                .Include(i => i.ReportAnEvents)
                .ThenInclude(i => i.AssignedPhotos)
                .Include(i => i.ReportAnEvents)
                .ThenInclude(i => i.SupplierConsultantPhotos)
                .Include(i => i.ReportAnEvents)
                .ThenInclude(i => i.CommentReportAnEvents)
                    .ThenInclude(i => i.CreatedByNavigation)
                .SingleOrDefault(s => s.Id == key);
            return managements;
        }

        public List<ReportAnEventTable> GetReportAnEventTable(int key)
        {
            var table = _context.ReportAnEvents.Where(x => x.TenancyManagementId == key).Select(s =>
                new ReportAnEventTable()
                {
                    Id = s.Id,
                    Description = s.Description,
                    Photos = s.AssignedPhotos.Count + s.SupplierConsultantPhotos.Count,
                    Severity = s.Severity.Severity,
                    Status = s.Status.Status,
                    IssueCloseDate = s.EventCloseDate.Value,
                    IssueReportDate = s.EventReportDate.Value,
                    QuoteApproval = s.QuoteApproval
                }).ToList();
            return table;
        }

        public biz.premier.Entities.TenancyManagement UpdateCustom(biz.premier.Entities.TenancyManagement tenancyManagement, int key)
        {
            var exist = _context.TenancyManagements
                .Include(i => i.DocumentTenancyManagements)
                .Include(i => i.DocumentTenancyManagements)
                .Include(i => i.CommentTenancyManagements)
                .Include(i => i.ReminderTenancyManagements)
                .Include(i => i.ReportAnEvents)
                .ThenInclude(i => i.AssignedPhotos)
                .Include(i => i.ReportAnEvents)
                .ThenInclude(i => i.SupplierConsultantPhotos)
                .Include(i => i.ReportAnEvents)
                .ThenInclude(i => i.CommentReportAnEvents)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(tenancyManagement);
                // DOCUMENT
                foreach (var i in tenancyManagement.DocumentTenancyManagements)
                {
                    var document = exist.DocumentTenancyManagements.FirstOrDefault(p => p.Id == i.Id);
                    if (document == null)
                    {
                        exist.DocumentTenancyManagements.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(i);
                    }
                }
                // REMINDER
                foreach (var i in tenancyManagement.ReminderTenancyManagements)
                {
                    var reminder = exist.ReminderTenancyManagements.FirstOrDefault(p => p.Id == i.Id);
                    if (reminder == null)
                    {
                        exist.ReminderTenancyManagements.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(reminder).CurrentValues.SetValues(i);
                    }
                }
                // COMMENT
                foreach (var i in tenancyManagement.CommentTenancyManagements)
                {
                    var comment = exist.CommentTenancyManagements.FirstOrDefault(p => p.Id == i.Id);
                    if (comment == null)
                    {
                        exist.CommentTenancyManagements.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                // REPORT AN EVENTS
                foreach (var i in tenancyManagement.ReportAnEvents)
                {
                    var reportAnEvent = exist.ReportAnEvents.FirstOrDefault(p => p.Id == i.Id);
                    if (reportAnEvent == null)
                    {
                        exist.ReportAnEvents.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(reportAnEvent).CurrentValues.SetValues(i);
                        // ASSIGNED PHOTO
                        foreach (var assignedPhoto in i.AssignedPhotos)
                        {
                            var photo = reportAnEvent.AssignedPhotos.FirstOrDefault(p => p.Id == assignedPhoto.Id);
                            if (photo == null)
                            {
                                reportAnEvent.AssignedPhotos.Add(assignedPhoto);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(photo).CurrentValues.SetValues(assignedPhoto);
                            }
                        }
                        // SUPPLIER CONSULTANT PHOTO
                        foreach (var supplierConsultantPhoto in i.SupplierConsultantPhotos)
                        {
                            var photo = reportAnEvent.SupplierConsultantPhotos.FirstOrDefault(p => p.Id == supplierConsultantPhoto.Id);
                            if (photo == null)
                            {
                                reportAnEvent.SupplierConsultantPhotos.Add(supplierConsultantPhoto);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(photo).CurrentValues.SetValues(supplierConsultantPhoto);
                            }
                        }
                    }
                }
                
                tenancyManagement.UpdatedDate = DateTime.Now;
                _context.SaveChanges();
            }

            UpdateStatusServiceRecord(exist.WorkOrderServices.Value,exist.StatusId.Value);
            return exist;
        }

        public bool DeleteReminder(int key)
        {
            bool isSuccess = false;
            var reminder = _context.ReminderTenancyManagements.Find(key);
            if (reminder != null)
            {
                _context.ReminderTenancyManagements.Remove(reminder);
                _context.SaveChanges();
                isSuccess = true;
            }

            return isSuccess;
        }

        public bool DeleteDocument(DocumentTenancyManagement key)
        {
            bool isSuccess = false;
            try
            {
                _context.DocumentTenancyManagements.Remove(key);
                _context.SaveChanges();
                isSuccess = true;
            }
            catch (DbException e)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public DocumentTenancyManagement FindDocument(int key)
        {
            var find = _context.DocumentTenancyManagements.Find(key);
            return find;
        }
    }
}