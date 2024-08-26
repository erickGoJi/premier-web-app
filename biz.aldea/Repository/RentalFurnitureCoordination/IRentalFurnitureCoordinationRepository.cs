using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.RentalFurnitureCoordination
{
    public interface IRentalFurnitureCoordinationRepository : IGenericRepository<Entities.RentalFurnitureCoordination>
    {
        Entities.RentalFurnitureCoordination GetCustom(int key);
        Entities.RentalFurnitureCoordination UpdateCustom(Entities.RentalFurnitureCoordination rentalFurnitureCoordination, int key);
    }
}
