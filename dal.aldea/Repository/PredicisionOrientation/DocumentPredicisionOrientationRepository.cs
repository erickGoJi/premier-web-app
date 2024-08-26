using biz.premier.Entities;
using biz.premier.Repository.PredicisionOrientation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.PredicisionOrientation
{
    public class DocumentPredicisionOrientationRepository :GenericRepository<DocumentPredecisionOrientation>, IDocumentPredicisionOrientationRepository
    {
        public DocumentPredicisionOrientationRepository(Db_PremierContext context) : base(context) { }
    }
}
