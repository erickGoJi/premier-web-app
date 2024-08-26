using biz.premier.Entities;
using biz.premier.Repository.Applicant;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Applicant
{
    public class ApplicantRepository : GenericRepository<CatApplicant>, IApplicantRepository
    {
        public ApplicantRepository(Db_PremierContext context) : base(context) { }
    }
}
