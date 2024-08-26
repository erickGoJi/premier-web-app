using biz.premier.Models.Call;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace biz.premier.Repository.Call
{
    public interface ICallRepository : IGenericRepository<biz.premier.Entities.Call>
    {
        ActionResult GetworkOrderBySR(int service_record_id, int service_line_id);
        ActionResult GetServicesBySO(int service_record_id, int service_line_id);
        List<CallByServiceRecord> GetCallByServiceRecord(int service_record_id, int service_line_id);
        Entities.Call UpdateCustom(Entities.Call call);
    }
}
