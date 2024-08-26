using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace biz.premier.Repository.ClientPartner
{
    public interface IClientPartnerProfileClientRepository : IGenericRepository<biz.premier.Entities.ClientPartnerProfileClient>
    {
        Task<bool> AssociatePartnerInformation(int idClientFrom, int idClientTo);
        Task<bool> DetachPartnerInformation(int idClientFrom, int idClientTo);
    }
}
