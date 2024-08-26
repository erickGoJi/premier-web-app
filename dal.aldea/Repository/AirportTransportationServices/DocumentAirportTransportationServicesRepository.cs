using biz.premier.Entities;
using biz.premier.Repository.AirportTransportationServices;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.AirportTransportationServices
{
    public class DocumentAirportTransportationServicesRepository : GenericRepository<DocumentAirportTransportationService>, IDocumentAirportTransportationServicesRepository
    {
        public DocumentAirportTransportationServicesRepository(Db_PremierContext context) : base(context) { }
    }
}
