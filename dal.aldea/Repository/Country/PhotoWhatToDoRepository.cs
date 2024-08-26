using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class PhotoWhatToDoRepository : GenericRepository<biz.premier.Entities.PhotoWhatToDo>, IPhotoWhatToDoRepository
    {
        public PhotoWhatToDoRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
