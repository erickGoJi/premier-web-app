using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.AreaOrientation
{
    public interface IAreaOrientationRepository : IGenericRepository<Entities.AreaOrientation>
    {
        Entities.AreaOrientation UpdateCustom(Entities.AreaOrientation areaOrientation, int key);
        Entities.AreaOrientation GetCustom(int key);
    }
}
