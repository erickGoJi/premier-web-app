using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Email
{
    public class EmailModel
    {
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public bool IsBodyHtml { get; set; }
        public int id_app { get; set; }
    }

    public class EmailSettings
    {
        public string PrimaryDomain { get; set; }
        public string MailName { get; set; }
        public int PrimaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public bool DevTest { get; set; }
        public string MailTest { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public bool EnableSsl { get; set; }

    }
    
    public class EmailDto
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Subject { get; set; }
        public int? CountryId { get; set; }
        public int? ServiceId { get; set; }
        public string Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public virtual ICollection<EmailServiceRecordDto> EmailServiceRecords { get; set; }
    }
    
    public class EmailServiceRecordDto
    {
        public int Id { get; set; }
        public int? ServiceRecordId { get; set; }
        public int? ServiceLine { get; set; }
        public int? EmailId { get; set; }
        public DateTime? Date { get; set; }
        public bool? Completed { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual EmailDto Email { get; set; }
    }
    
}
