using biz.premier.Entities;
using biz.premier.Repository.Transportation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Transportation
{
    public class DocumentTransportationRepository : GenericRepository<DocumentTransportation>, IDocumentTransportationRepository
    {
        public DocumentTransportationRepository(Db_PremierContext context) : base(context) { }
    }
}
