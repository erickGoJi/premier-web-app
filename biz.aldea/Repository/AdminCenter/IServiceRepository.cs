using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository.AdminCenter
{
    public interface IServiceRepository : IGenericRepository<Entities.Service>
    {
        Entities.Service GetCustom(int key);
        Entities.Service UpdateCustom(Entities.Service service, int key);
        ActionResult GetScopeDocuments(int service, int client);
        System.Threading.Tasks.Task<bool> DeleteDocumentAsync(int id);
        bool IsExistServiceInCountry(int country, int service);
        bool IsExistService(int service);
        int searchServiceId(int service);
    }
}
