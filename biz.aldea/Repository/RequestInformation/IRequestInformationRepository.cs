using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.RequestInformation
{
    public interface IRequestInformationRepository : IGenericRepository<Entities.RequestInformation>
    {
        Entities.RequestInformation insert(Entities.RequestInformation request);
        string SendMail(string emailTo, string body, string subject, List<RequestInformationDocument> documents);
        List<Tuple<int, string>> HousingAvailible(int sr);
        int GetWorkOrderService(int sr, int type);
    }
}
