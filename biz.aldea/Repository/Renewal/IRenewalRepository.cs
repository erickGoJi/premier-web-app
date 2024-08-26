using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Renewal
{
    public interface IRenewalRepository : IGenericRepository<Entities.Renewal>
    {
        Entities.Renewal UpdateCustom(Entities.Renewal renewal, int key);
        Entities.Renewal GetRenewalById(int id);
    }
}
