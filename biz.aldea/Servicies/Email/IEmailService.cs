using biz.premier.Models.Email;

namespace biz.premier.Servicies
{
    public interface IEmailService
    {
        string SendEmail(EmailModel email);
        string SendEmailAttach(EmailModelAttach email);
    }
}
