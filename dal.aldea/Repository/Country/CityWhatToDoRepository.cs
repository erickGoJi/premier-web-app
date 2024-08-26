using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class CityWhatToDoRepository : GenericRepository<biz.premier.Entities.CityWhatToDo>, ICityWhatToDoRepository
    {
        public CityWhatToDoRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
