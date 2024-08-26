using biz.premier.Repository.EmailServiceRecord;
using dal.premier.DBContext;

namespace dal.premier.Repository.EmailServiceRecord
{
    public class EmailServiceRecordRepository : GenericRepository<biz.premier.Entities.EmailServiceRecord>, IEmailServiceRecordRepository
    {
        public EmailServiceRecordRepository(Db_PremierContext context) : base(context){}
    }
}