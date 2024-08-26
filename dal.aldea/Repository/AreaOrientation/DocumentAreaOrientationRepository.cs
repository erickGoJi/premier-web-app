using biz.premier.Entities;
using biz.premier.Repository.AreaOrientation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.AreaOrientation
{
    public class DocumentAreaOrientationRepository : GenericRepository<DocumentAreaOrientation>, IDocumentAreaOrientationRepository
    {
        public DocumentAreaOrientationRepository(Db_PremierContext context) : base(context) { }
    }
}
