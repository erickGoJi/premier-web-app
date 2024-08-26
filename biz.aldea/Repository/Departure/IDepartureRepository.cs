using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Departure
{
    public interface IDepartureRepository : IGenericRepository<Entities.Departure>
    {
        Entities.Departure GetCustom(int key);
        Entities.Departure UpdateCustom(Entities.Departure departure, int key);
    }
}
