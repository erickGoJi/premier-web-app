using System;
using System.Linq;
using biz.premier.Repository.EmailServiceRecord;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.EmailServiceRecord
{
    public class EmailRepository : GenericRepository<biz.premier.Entities.Email>, IEmailRepository
    {
        public EmailRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public ActionResult GetEmailForServiceRecord(int user, int sr)
        {
            var country = _context.ServiceRecords.Where(x => x.Id == sr)
                .Select(s => s.AssigneeInformations.FirstOrDefault().HostCountry.Value).FirstOrDefault();
            
            var profile = _context.ProfileUsers.Where(x => x.UserId == user).Select(s => new
            {
                immigration = s.Immigration.HasValue ? s.Immigration.Value : false,
                relocation = s.Relocation.HasValue ? s.Relocation.Value : false
            }).FirstOrDefault();
            
            var emailsImmigration = profile.immigration
                ? _context.Emails.Where(x => x.Service.SericeLineId == 1 && x.CountryId == country).ToList()
                : null;
            var emailsRelocation = profile.relocation
                ? _context.Emails.Where(x => x.Service.SericeLineId == 2 && x.CountryId == country).ToList()
                : null;
            
            var emailsFromSRImmigration = _context.EmailServiceRecords
                .Include(i => i.Email)
                .Where(x => x.ServiceRecordId == sr && x.Email.Service.SericeLineId == 1).ToList();
            
            var emailsFromSRRelocation = _context.EmailServiceRecords
                .Include(i => i.Email)
                .Where(x => x.ServiceRecordId == sr && x.Email.Service.SericeLineId == 2).ToList();

            if (emailsImmigration != null)
            {
                foreach (var email in emailsFromSRImmigration)
                {
                    var find = emailsImmigration.FirstOrDefault(x => x.Id == email.Email.Id);
                    if (find != null)
                    {
                        emailsImmigration.Remove(find);
                    }
                }    
            }

            if (emailsRelocation != null)
            {
                foreach (var email in emailsFromSRRelocation)
                {
                    var find = emailsRelocation.FirstOrDefault(x => x.Id == email.Email.Id);
                    if (find != null)
                    {
                        emailsRelocation.Remove(find);
                    }
                }
            }

            var emailsNotSendRelo = emailsRelocation != null ? emailsRelocation.Select(s => new
            {
                Email = s,
                Id = 0,
                s.NickName,
                Date = "-",
                Completed = false
            }).ToList() : null;
            
            var emailsNotSendImmi = emailsImmigration != null ?emailsImmigration.Select(s => new
            {
                Email = s,
                Id = 0,
                s.NickName,
                Date = "-",
                Completed = false
            }).ToList() : null;

            var emailsSendedRelo = emailsFromSRRelocation != null ? emailsFromSRRelocation.Select(s => new
            {
                s.Email,
                s.Id,
                s.Email.NickName,
                Date = s.Date.Value.ToString(),
                Completed = s.Completed.Value
            }).ToList() : null;
            
            var emailsSendedImmi = emailsFromSRImmigration != null ? emailsFromSRImmigration.Select(s => new
            {
                s.Email,
                s.Id,
                s.Email.NickName,
                Date = s.Date.Value.ToString(),
                Completed = s.Completed.Value
            }).ToList() : null;
            
            // emailsSendedImmi.Add(new {
            //     Email = new biz.premier.Entities.Email(),
            //     Id = 0, 
            //     NickName = "Send App Access",
            //     Date = "-",
            //     Completed = false
            // });
            //
            // emailsSendedRelo.Add(new {
            //     Email = new biz.premier.Entities.Email(),
            //     Id = 0, 
            //     NickName = "Send App Access",
            //     Date = "-",
            //     Completed = false
            // });

            var services = new
            {
                relocation = emailsNotSendRelo != null ? emailsNotSendRelo.Union(emailsSendedRelo).ToList() : emailsSendedRelo,
                immigration = emailsNotSendImmi != null ? emailsNotSendImmi.Union(emailsSendedImmi).ToList() : emailsSendedImmi
            };
            return new ObjectResult(services);
        }
    }
}