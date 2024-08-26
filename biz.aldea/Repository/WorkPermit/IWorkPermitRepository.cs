using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.WorkPermit
{
    public interface IWorkPermitRepository : IGenericRepository<Entities.WorkPermit>
    {
        Entities.WorkPermit UpdateCustom(Entities.WorkPermit workPermit);
        Entities.WorkPermit GetCustomWorkPermit(int key);
    }
}
