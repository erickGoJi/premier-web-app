using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Transportation
{
    public interface ITransportationRepository : IGenericRepository<Entities.Transportation>
    {
        Entities.Transportation GetCustom(int key);
        Entities.Transportation UpdateCustom(Entities.Transportation transportation, int key);
        ActionResult GetTransportations(int applicatId, int service_order_id, int type_service);

        ActionResult GetSingleTransportationById(int service_id, int country_id);
    }
}
