using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.City;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.City
{
    public class CityRepository : GenericRepository<CatCity>, ICityRepository
    {
        private IConfiguration _config;
        private IEmailService _emailservice;

        public CityRepository(Db_PremierContext context, IConfiguration config, IEmailService emailService) : base(context)
        {
            _config = config;
            _emailservice = emailService;
        }

        public void SendMail(string emailTo, string body, string subject)
        {
            throw new NotImplementedException();
        }
    }
}
