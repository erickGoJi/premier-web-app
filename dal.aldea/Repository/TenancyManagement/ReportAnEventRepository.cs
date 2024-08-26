using System;
using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository.TenancyManagement;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.TenancyManagement
{
    public class ReportAnEventRepository : GenericRepository<ReportAnEvent>, IReportAnEventRepository
    {
        public ReportAnEventRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public ReportAnEvent UpdateCustom(ReportAnEvent @event, int key)
        {
            var exist = _context.ReportAnEvents
                .Include(i => i.AssignedPhotos)
                .Include(i => i.SupplierConsultantPhotos)
                .Include(i => i.CommentReportAnEvents)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(@event);
                // SUPPLIER CONSULTANT PHOTO
                foreach (var i in @event.SupplierConsultantPhotos)
                {
                    var photo = exist.SupplierConsultantPhotos.FirstOrDefault(p => p.Id == i.Id);
                    if (photo == null)
                    {
                        exist.SupplierConsultantPhotos.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(photo).CurrentValues.SetValues(i);
                    }
                }
                // ASSIGNED PHOTO
                foreach (var i in @event.AssignedPhotos)
                {
                    var photo = exist.AssignedPhotos.FirstOrDefault(p => p.Id == i.Id);
                    if (photo == null)
                    {
                        exist.AssignedPhotos.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(photo).CurrentValues.SetValues(i);
                    }
                }
                // COMMENT
                foreach (var i in @event.CommentReportAnEvents)
                {
                    var comment = exist.CommentReportAnEvents.FirstOrDefault(p => p.Id == i.Id);
                    if (comment == null)
                    {
                        exist.CommentReportAnEvents.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                
                @event.UpdatedDate =DateTime.Now;
                _context.SaveChanges();
            }

            return exist;
        }

        public bool DeleteAssignedPhoto(int key)
        {
            bool isSuccess = false;
            var assignedPhoto = _context.AssignedPhotos.Find(key);
            if (assignedPhoto != null)
            {
                _context.AssignedPhotos.Remove(assignedPhoto);
                _context.SaveChanges();
                isSuccess = true;
            }

            return isSuccess;
        }

        public bool DeleteSupplierConsultantPhoto(int key)
        {
            bool isSuccess = false;
            var supplierConsultantPhoto = _context.SupplierConsultantPhotos.Find(key);
            if (supplierConsultantPhoto != null)
            {
                _context.SupplierConsultantPhotos.Remove(supplierConsultantPhoto);
                _context.SaveChanges();
                isSuccess = true;
            }

            return isSuccess;
        }
        
    }
}