using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository.EmailServiceRecord
{
    public interface IEmailRepository : IGenericRepository<Entities.Email>
    {
        ActionResult GetEmailForServiceRecord(int user, int sr);
    }
}