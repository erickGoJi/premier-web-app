using biz.premier.Repository.LegalReview;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.LegalReview
{
    public class LegalReviewRepository: GenericRepository<biz.premier.Entities.LegalReview>, ILegalReviewRepository
    {
        public LegalReviewRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.LegalReview GetLegalReviewById(int id)
        {
            biz.premier.Entities.LegalReview review = _context.LegalReviews
                .Include(x => x.ReminderLegalReviews)
                .Include(x => x.DocumentLegalReviews)
                .Include(x => x.CommentLegalReviews)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .SingleOrDefault(x => x.Id == id);
            return review;
        }

        public biz.premier.Entities.LegalReview UpdateCustom(biz.premier.Entities.LegalReview legalReview, int key)
        {
            if (legalReview == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.LegalReview>()
                .Include(i => i.CommentLegalReviews)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(legalReview);
                foreach (var i in legalReview.CommentLegalReviews)
                {
                    var comments = exist.CommentLegalReviews.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        i.User = null;
                        exist.CommentLegalReviews.Add(i);
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
