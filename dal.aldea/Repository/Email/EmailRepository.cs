using dal.premier.DBContext;
using biz.premier.Entities;
using biz.premier.Repository.Email;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace dal.premier.Repository.Email
{
    public class EmailRepository : GenericRepository<EmailSend>, IEmailRepository
    {
        public EmailRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetEmailSendByServiceRecord(int service_record_id, int service_line_id)
        {
            var service = _context.CatEmailSends
                .GroupJoin(_context.EmailSends, e => e.Id, a => a.EmailId, (e, a) => new
                {
                    e,
                    a
                }).SelectMany(
      temp => temp.a.DefaultIfEmpty(),
      (temp, p) =>
         new
         {
             p.Id,
             p.ServiceLineId,
             p.ServiceRecordId,
             p.ServiceLine.ServiceLine,
             Email = p.Email.Email == null ? temp.e.Email : p.Email.Email,
             p.Date,
             p.Completed
         }).Where(x => x.ServiceLineId == service_line_id && x.ServiceRecordId == service_record_id);

            return new ObjectResult(service);
        }
    }
}
