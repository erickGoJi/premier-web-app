using biz.premier.Entities;
using biz.premier.Repository.RentalFurnitureCoordination;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.RentalFurnitureCoordination
{
    public class ExtensionRentalFurnitureCoordinationRepository : GenericRepository<StayExtensionRentalFurnitureCoordination>, IExtensionRentalFurnitureCoordinationRepository
    {
        public ExtensionRentalFurnitureCoordinationRepository(Db_PremierContext context) : base(context) { }
    }
}
