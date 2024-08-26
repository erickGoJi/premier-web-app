using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.TemporaryHousingCoordinaton
{
    public interface ITemporaryHousingCoordinatonRepository: IGenericRepository<Entities.TemporaryHousingCoordinaton>
    {
        Entities.TemporaryHousingCoordinaton GetCustom(int key);
        Entities.TemporaryHousingCoordinaton UpdateCustom(Entities.TemporaryHousingCoordinaton temporaryHousingCoordinaton, int key);
    }
}
