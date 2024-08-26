using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class CountryDocumentRepository : GenericRepository<biz.premier.Entities.CountryDocument>, ICountryDocumentRepository
    {
        public CountryDocumentRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
