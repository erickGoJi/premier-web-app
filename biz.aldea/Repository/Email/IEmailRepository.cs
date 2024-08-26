using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository.Email
{
    public interface IEmailRepository : IGenericRepository<EmailSend>
    {
        ActionResult GetEmailSendByServiceRecord(int service_record_id, int service_line_id);
    }
}
