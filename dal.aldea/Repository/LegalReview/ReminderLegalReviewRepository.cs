using biz.premier.Entities;
using biz.premier.Repository.LegalReview;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.LegalReview
{
    public class ReminderLegalReviewRepository : GenericRepository<ReminderLegalReview>, IReminderLegalReviewRepository
    {
        public ReminderLegalReviewRepository(Db_PremierContext context) : base(context) { }
    }
}
