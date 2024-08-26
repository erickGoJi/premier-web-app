using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.VisaDeregistration
{
    public interface IVisaDeregistrationRepository : IGenericRepository<Entities.VisaDeregistration>
    {
        Entities.VisaDeregistration UpdateCustom(Entities.VisaDeregistration  visaDeregistration, int key);
        Entities.VisaDeregistration GetCustomVisaDeregistration(int key);
    }
}
