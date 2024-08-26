using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.MapIt
{
    public interface IMapItRepository : IGenericRepository<Entities.MapIt>
    {
        Entities.MapIt UpdateCustom(Entities.MapIt mapIt, int key);
        ActionResult SelectCustom(int key);
        ActionResult LocationType();
        ActionResult GetMapIt(int ServiceLineId, int service_record_id);
    }
}
