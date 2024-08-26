using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class DocumentLocationCountryRepository : GenericRepository<biz.premier.Entities.DocumentLocationCountry>, IDocumentLocationCountryRepository
    {
        public DocumentLocationCountryRepository(Db_PremierContext context) : base(context) { }
    }
}
