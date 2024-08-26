using biz.premier.Entities;
using biz.premier.Repository.SchoolingSearch;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.SchoolingSearch
{
    public class DocumentSchoolingSearchRepository : GenericRepository<DocumentSchoolingSearch>, IDocumentSchoolingSearchRepository
    {
        public DocumentSchoolingSearchRepository(Db_PremierContext context) : base(context) { }
    }
}
