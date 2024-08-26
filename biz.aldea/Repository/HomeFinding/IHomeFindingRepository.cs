using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.HomeFinding
{
    public interface IHomeFindingRepository : IGenericRepository<Entities.HomeFinding>
    {
        Entities.HomeFinding UpdateCustom(Entities.HomeFinding homeFinding, int key, int userId, string number_server);
        Entities.HomeFinding GetCustom(int key);

        Entities.BundledServicesWorkOrder GetBundleServiceWoId(int id_wo_service);
        Entities.StandaloneServiceWorkOrder GetAloneServiceWoId(int id_wo_service);

        Boolean DeleteReminder(int key);
        Boolean DeleteDocument(int key);

        Boolean DeleteDocumentHousing(int key);

        DocumentHomeFinding FindDocument(int key);
    }
}
