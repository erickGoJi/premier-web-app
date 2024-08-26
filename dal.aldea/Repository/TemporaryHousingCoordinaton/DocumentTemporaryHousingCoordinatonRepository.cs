using biz.premier.Entities;
using biz.premier.Repository.TemporaryHousingCoordinaton;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.TemporaryHousingCoordinaton
{
    public class DocumentTemporaryHousingCoordinatonRepository : GenericRepository<DocumentTemporaryHousingCoordinaton>, IDocumentTemporaryHousingCoordinatonRepository
    {
        public DocumentTemporaryHousingCoordinatonRepository(Db_PremierContext context) : base(context) { }
    }
}
