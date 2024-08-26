using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.DocumentManagement
{
    public interface IDocumentManagementRepository : IGenericRepository<Entities.DocumentManagement>
    {
        Entities.DocumentManagement UpdateCustom(Entities.DocumentManagement documentManagement, int key);
    }
}
