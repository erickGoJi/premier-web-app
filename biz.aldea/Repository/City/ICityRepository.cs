using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.City
{
    public interface ICityRepository : IGenericRepository<CatCity>
    {
        void SendMail(string emailTo, string body, string subject);
    }
}
