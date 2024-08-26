using biz.premier.Entities;
using biz.premier.Repository.Client;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Client
{
    public class ClientRepository : GenericRepository<CatClient>, IClientRepository
    {
        private IConfiguration _config;
        private IEmailService _emailservice;

        public ClientRepository(Db_PremierContext context, IConfiguration config, IEmailService emailService) : base(context)
        {
            _config = config;
            _emailservice = emailService;
        }
    }
}
