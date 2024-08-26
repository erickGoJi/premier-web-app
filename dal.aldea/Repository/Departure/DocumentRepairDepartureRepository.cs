using biz.premier.Entities;
using biz.premier.Repository.Departure;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Departure
{
    public class DocumentRepairDepartureRepository : GenericRepository<DocumentRepair>, IDocumentRepairDepartureRepository
    {
        public DocumentRepairDepartureRepository(Db_PremierContext context) : base(context) { }
    }
}
