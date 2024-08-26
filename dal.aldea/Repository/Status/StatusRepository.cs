using biz.premier.Entities;
using biz.premier.Repository.Status;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Status
{
    public class StatusRepository : GenericRepository<CatStatus>, IStatusRepository
    {
        private IConfiguration _config;
        private IEmailService _emailservice;

        public StatusRepository(Db_PremierContext context, IConfiguration config, IEmailService emailService) : base(context)
        {
            _config = config;
            _emailservice = emailService;
        }
    }
}
