using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.PropertyReport;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.PropertyReport
{
    public class PhotoPropertyReportRepository : GenericRepository<PhotosPropertyReportSection>, IPhotoPropertyReportRepository
    {
        public PhotoPropertyReportRepository(Db_PremierContext context) : base(context) { } 
    }
}
