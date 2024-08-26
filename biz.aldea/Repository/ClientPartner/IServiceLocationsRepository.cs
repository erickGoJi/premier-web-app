using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.ClientPartner
{
    public interface IServiceLocationsRepository : IGenericRepository<biz.premier.Entities.ServiceLocation>
    {
        ActionResult GetServiceLocationById(int id);
        Entities.ServiceLocation UpdateCustom(Entities.ServiceLocation serviceLocation, int key);
    }
}
