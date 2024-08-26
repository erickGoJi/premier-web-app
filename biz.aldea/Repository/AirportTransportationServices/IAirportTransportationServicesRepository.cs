using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.AirportTransportationServices
{
    public interface IAirportTransportationServicesRepository : IGenericRepository<AirportTransportationService>
    {
        AirportTransportationService GetCustom(int key);
        AirportTransportationService UpdateCustom(AirportTransportationService airportTransportationService, int key);
        ActionResult GetAirportTransportation(int applicatId, int service_order_id, int type_service);

        ActionResult GetSingleAirportTransportationServicesById(int service_id);
    }
}
