using biz.premier.Repository.SupplierPartnerProfile;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.SupplierPartnerProfile
{
    public class CalendarConsultantContactsConsultantRepository : GenericRepository<biz.premier.Entities.CalendarConsultantContactsConsultant>, ICalendarConsultantContactsConsultantRepository
    {
        public CalendarConsultantContactsConsultantRepository(Db_PremierContext context): base(context) { }
    }
}
