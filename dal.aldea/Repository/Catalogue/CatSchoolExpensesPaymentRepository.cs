using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatSchoolExpensesPaymentRepository : GenericRepository<CatSchoolExpensesPayment>, ICatSchoolExpensesPaymentRepository
    {
        public CatSchoolExpensesPaymentRepository(Db_PremierContext context) : base(context) { }
    }
}
