using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Scalate
{
    public interface IScalateServiceRepository : IGenericRepository<ScalateService>
    {
        ScalateService selectCustom(int key, int sr);
        ActionResult GetEscalationCommunication(int service_line, int sr);
        ActionResult GetEscalationCommunicationById(int id);
    }
}
