using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.ResidencyPermit
{
    public interface IResidencyPermitRepository : IGenericRepository<Entities.ResidencyPermit>
    {
        Entities.ResidencyPermit UpdateCustom(Entities.ResidencyPermit residencyPermit, int key);
        Entities.ResidencyPermit GetResidencyPermitCustom(int key);
    }
}
