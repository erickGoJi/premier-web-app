using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Catalogue
{
    public interface IServiceRepository : IGenericRepository<CatService>
    {
        ActionResult GetServiceByServiceLine(int key);
        ActionResult GetServiceWithNickname(int country, int client, int serviceLine);
        ActionResult GetServiceWithNicknameByDasboard(int serviceId, int client, int serviceLine);
    }
}
