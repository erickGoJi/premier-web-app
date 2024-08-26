using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Renewal;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Renewal
{
    public class DocumentRenawalRepository : GenericRepository<DocumentRenewal>, IDocumentRenawalRepository
    {
        public DocumentRenawalRepository(Db_PremierContext context) : base(context) { }
    }
}
