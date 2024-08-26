using biz.premier.Entities;
using biz.premier.Repository.TemporaryHousingCoordinaton;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.TemporaryHousingCoordinaton
{
    public class ExtensionTemporaryHousingCoordinatonRepository : GenericRepository<StayExtensionTemporaryHousing>, IExtensionTemporaryHousingCoordinatonRepository
    {
        public ExtensionTemporaryHousingCoordinatonRepository(Db_PremierContext context) : base(context) { }
    }
}
