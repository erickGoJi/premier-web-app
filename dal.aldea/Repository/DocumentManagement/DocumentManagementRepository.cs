using biz.premier.Repository.DocumentManagement;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.DocumentManagement
{
    public class DocumentManagementRepository : GenericRepository<biz.premier.Entities.DocumentManagement>, IDocumentManagementRepository
    {
        public DocumentManagementRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.DocumentManagement UpdateCustom(biz.premier.Entities.DocumentManagement documentManagement, int key)
        {
            if (documentManagement == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.DocumentManagement>()
                .Include(i => i.CommentDocumentManagements)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(documentManagement);
                foreach (var package in documentManagement.CommentDocumentManagements)
                {
                    var comments = exist.CommentDocumentManagements.Where(p => p.Id == package.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        package.User = null;
                        exist.CommentDocumentManagements.Add(package);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(package);
                    }
                }
                
                _context.SaveChanges();
            }
            UpdateStatusServiceRecord(exist.WorkOrderServicesId.Value,exist.StatusId);
            return exist;
        }
    }
}
