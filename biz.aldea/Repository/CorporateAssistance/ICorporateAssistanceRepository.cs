using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.CorporateAssistance
{
    public interface ICorporateAssistanceRepository : IGenericRepository<Entities.CorporateAssistance>
    {
        Entities.CorporateAssistance UpdateCustom(Entities.CorporateAssistance corporateAssistance, int key);
        Entities.CorporateAssistance GetCorporateAssistanceById(int id);
    }
}
