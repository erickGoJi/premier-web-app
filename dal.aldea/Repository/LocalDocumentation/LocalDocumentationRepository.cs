using biz.premier.Repository.LocalDocumentation;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.LocalDocumentation
{
    public class LocalDocumentationRepository: GenericRepository<biz.premier.Entities.LocalDocumentation>, ILocalDocumentationRepository
    {
        public LocalDocumentationRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.LocalDocumentation UpdateCustom(biz.premier.Entities.LocalDocumentation localDocumentation, int key)
        {
            if (localDocumentation == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.LocalDocumentation>()
                .Include(i => i.CommentLocalDocumentations)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(localDocumentation);
                foreach (var i in localDocumentation.CommentLocalDocumentations)
                {
                    var comments = exist.CommentLocalDocumentations.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        i.User = null;
                        exist.CommentLocalDocumentations.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId.Value,exist.StatusId);
            return exist;
        }
    }
}
