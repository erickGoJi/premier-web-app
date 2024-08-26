using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.LegalReview
{
    public interface ILegalReviewRepository : IGenericRepository<Entities.LegalReview>
    {
        Entities.LegalReview UpdateCustom(Entities.LegalReview legalReview, int key);
        Entities.LegalReview GetLegalReviewById(int id);
    }
}
