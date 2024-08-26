using System;
using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository.PropertyManagement;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.PropertyManagement
{
    public class PropertyManagementRepository : GenericRepository<biz.premier.Entities.PropertyManagement>, IPropertyManagementRepository
    {
        public PropertyManagementRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public biz.premier.Entities.PropertyManagement GetCustom(int key)
        {
            var property = _context.PropertyManagements
                .Include(i => i.VisitReportPropertyManagements)
                .Include(i => i.DocumentPropertyManagements)
                .Include(i => i.DocumentReportIssuePropertyManagements)
                .Include(i => i.PhotoPropertyManagements)
                .Include(i => i.PhotoBillPropertyManagements)
                .Include(i => i.PhotoInspectionPropertyManagements)
                .Include(i => i.PhotoMailPropertyManagements)
                .Include(i => i.PhotoReportIssuePropertyManagements)
                .Include(i => i.ReminderPropertyManagements)
                .Include(i => i.CommentPropertyManagements)
                .ThenInclude(i => i.CreatedByNavigation)
                .ThenInclude(i => i.Role)
                .SingleOrDefault(s => s.Id == key);
            return property;
        }

        public biz.premier.Entities.PropertyManagement UpdateCustom(biz.premier.Entities.PropertyManagement propertyManagement, int key)
        {
             if (propertyManagement == null)
                return null;
            var @default = _context.PropertyManagements
                .Include(i => i.VisitReportPropertyManagements)
                .Include(i => i.DocumentPropertyManagements)
                .Include(i => i.DocumentReportIssuePropertyManagements)
                .Include(i => i.PhotoPropertyManagements)
                .Include(i => i.PhotoBillPropertyManagements)
                .Include(i => i.PhotoInspectionPropertyManagements)
                .Include(i => i.PhotoMailPropertyManagements)
                .Include(i => i.PhotoReportIssuePropertyManagements)
                .Include(i => i.ReminderPropertyManagements)
                .Include(i => i.CommentPropertyManagements)
                .SingleOrDefault(x => x.Id == key);
            if (@default != null)
            {
                _context.Entry(@default).CurrentValues.SetValues(propertyManagement);
                // VISIT
                foreach (var visitReportPropertyManagement in propertyManagement.VisitReportPropertyManagements)
                {
                    var visit = @default.VisitReportPropertyManagements.FirstOrDefault(x => x.Id == visitReportPropertyManagement.Id);
                    if (visit == null)
                    {
                        @default.VisitReportPropertyManagements.Add(visitReportPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(visit).CurrentValues.SetValues(visitReportPropertyManagement);
                    }
                }
                // Comment
                foreach (var commentPropertyManagement in propertyManagement.CommentPropertyManagements)
                {
                    var comment = @default.CommentPropertyManagements.FirstOrDefault(x => x.Id == commentPropertyManagement.Id);
                    if (comment == null)
                    {
                        @default.CommentPropertyManagements.Add(commentPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(commentPropertyManagement);
                    }
                }
                // Document
                foreach (var documentPropertyManagement in propertyManagement.DocumentPropertyManagements)
                {
                    var document = @default.DocumentPropertyManagements.FirstOrDefault(x => x.Id == documentPropertyManagement.Id);
                    if (document == null)
                    {
                        @default.DocumentPropertyManagements.Add(documentPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(documentPropertyManagement);
                    }
                }
                // ISSUE
                foreach (var issuePropertyManagement in propertyManagement.DocumentReportIssuePropertyManagements)
                {
                    var document = @default.DocumentReportIssuePropertyManagements.FirstOrDefault(x => x.Id == issuePropertyManagement.Id);
                    if (document == null)
                    {
                        @default.DocumentReportIssuePropertyManagements.Add(issuePropertyManagement);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(issuePropertyManagement);
                    }
                }
                // Reminder
                foreach (var reminderPropertyManagement in propertyManagement.ReminderPropertyManagements)
                {
                    var reminder = @default.ReminderPropertyManagements.FirstOrDefault(x => x.Id == reminderPropertyManagement.Id);
                    if (reminder == null)
                    {
                        @default.ReminderPropertyManagements.Add(reminderPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(reminder).CurrentValues.SetValues(reminderPropertyManagement);
                    }
                }
                // Photo
                foreach (var photoInspectionPropertyManagement in propertyManagement.PhotoInspectionPropertyManagements)
                {
                    var _photo = @default.PhotoInspectionPropertyManagements.FirstOrDefault(x => x.Id == photoInspectionPropertyManagement.Id);
                    if (_photo == null)
                    {
                        @default.PhotoInspectionPropertyManagements.Add(photoInspectionPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(_photo).CurrentValues.SetValues(photoInspectionPropertyManagement);
                    }
                }
                
                foreach (var photoPropertyManagement in propertyManagement.PhotoPropertyManagements)
                {
                    var _photo = @default.PhotoPropertyManagements.FirstOrDefault(x => x.Id == photoPropertyManagement.Id);
                    if (_photo == null)
                    {
                        @default.PhotoPropertyManagements.Add(photoPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(_photo).CurrentValues.SetValues(photoPropertyManagement);
                    }
                }
                
                foreach (var photoBillPropertyManagement in propertyManagement.PhotoBillPropertyManagements)
                {
                    var _photo = @default.PhotoBillPropertyManagements.FirstOrDefault(x => x.Id == photoBillPropertyManagement.Id);
                    if (_photo == null)
                    {
                        @default.PhotoBillPropertyManagements.Add(photoBillPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(_photo).CurrentValues.SetValues(photoBillPropertyManagement);
                    }
                }
                foreach (var photoMailPropertyManagement in propertyManagement.PhotoMailPropertyManagements)
                {
                    var _photo = @default.PhotoMailPropertyManagements.FirstOrDefault(x => x.Id == photoMailPropertyManagement.Id);
                    if (_photo == null)
                    {
                        @default.PhotoMailPropertyManagements.Add(photoMailPropertyManagement);
                    }
                    else
                    {
                        _context.Entry(_photo).CurrentValues.SetValues(photoMailPropertyManagement);
                    }
                }
                foreach (var photoReportIssuePropertyManagement in propertyManagement.PhotoReportIssuePropertyManagements)
                {
                    var _photo = @default.PhotoReportIssuePropertyManagements.FirstOrDefault(x => x.Id == photoReportIssuePropertyManagement.Id);
                    if (_photo == null)
                    {
                        @default.PhotoReportIssuePropertyManagements.Add(photoReportIssuePropertyManagement);
                    }
                    else
                    {
                        _context.Entry(_photo).CurrentValues.SetValues(photoReportIssuePropertyManagement);
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
            var find = _context.ReminderPropertyManagements.Find(key);
            if (find != null)
            {
                _context.ReminderPropertyManagements.Remove(find);
                _context.SaveChanges();
                isSuccess = true;
            }

            return isSuccess;
        }

        public bool DeleteDocument(DocumentPropertyManagement key)
        {
            bool isSuccess = false;
            if (key != null)
            {
                _context.DocumentPropertyManagements.Remove(key);
                _context.SaveChanges();
                isSuccess = true;
            }
            
            return isSuccess;
        }

        public DocumentPropertyManagement FindDocument(int key)
        {
            var find = _context.DocumentPropertyManagements.FirstOrDefault(x => x.Id == key);
            return find;
        }

        public Tuple<int, string> FindPhoto(int key, int type)
        {
            var data = new Tuple<int, string>(0, "");
            switch (type)
            {
                case (1):
                    data = _context.PhotoInspectionPropertyManagements.Where(x => x.Id == key).Select(s => new Tuple<int, string>(
                        s.Id, s.Photo)).FirstOrDefault();
                    break;
                case (2):
                    data = _context.PhotoPropertyManagements.Where(x => x.Id == key).Select(s => new Tuple<int, string>(
                        s.Id, s.Photo)).FirstOrDefault();
                    break;
                case (3):
                    data = _context.PhotoBillPropertyManagements.Where(x => x.Id == key).Select(s => new Tuple<int, string>(
                        s.Id, s.Photo)).FirstOrDefault();
                    break;
                case (4):
                    data = _context.PhotoMailPropertyManagements.Where(x => x.Id == key).Select(s => new Tuple<int, string>(
                        s.Id, s.Photo)).FirstOrDefault();
                    break;
                case (5):
                    data = _context.PhotoReportIssuePropertyManagements.Where(x => x.Id == key).Select(s => new Tuple<int, string>(
                        s.Id, s.Photo)).FirstOrDefault();
                    break;
                default:
                    data = data;
                    break;
            }

            return data;
        }

        public bool DeletePhoto(int key, int type)
        {
            bool isSuccess = false;
            switch (type)
            {
                case (1):
                    var photoInspectionPropertyManagement = _context.PhotoInspectionPropertyManagements.FirstOrDefault(x => x.Id == key);
                    _context.PhotoInspectionPropertyManagements.Remove(photoInspectionPropertyManagement);
                    isSuccess = true;
                    break;
                case (2):
                    var photoPropertyManagement = _context.PhotoPropertyManagements.FirstOrDefault(x => x.Id == key);
                    _context.PhotoPropertyManagements.Remove(photoPropertyManagement);
                    isSuccess = true;
                    break;
                case (3):
                    var photoBillPropertyManagement = _context.PhotoBillPropertyManagements.FirstOrDefault(x => x.Id == key);
                    _context.PhotoBillPropertyManagements.Remove(photoBillPropertyManagement);
                    isSuccess = true;
                    break;
                case (4):
                    var photoMailPropertyManagement = _context.PhotoMailPropertyManagements.FirstOrDefault(x => x.Id == key);
                    _context.PhotoMailPropertyManagements.Remove(photoMailPropertyManagement);
                    isSuccess = true;
                    break;
                case (5):
                    var photoReportIssuePropertyManagement = _context.PhotoReportIssuePropertyManagements.FirstOrDefault(x => x.Id == key);
                    _context.PhotoReportIssuePropertyManagements.Remove(photoReportIssuePropertyManagement);
                    isSuccess = true;
                    break;
                default:
                    isSuccess = false;
                    break;
            }

            _context.SaveChanges();
            return isSuccess;
        }
    }
}