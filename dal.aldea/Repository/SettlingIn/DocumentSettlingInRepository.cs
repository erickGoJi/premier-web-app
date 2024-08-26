using biz.premier.Entities;
using biz.premier.Repository.SettlingIn;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.SettlingIn
{
    public class DocumentSettlingInRepository : GenericRepository<DocumentSettlingIn>, IDocumentSettlingInRepository
    {
        public DocumentSettlingInRepository(Db_PremierContext context) : base(context) { }
    }
}
