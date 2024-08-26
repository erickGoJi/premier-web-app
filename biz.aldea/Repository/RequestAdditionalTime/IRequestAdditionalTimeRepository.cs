using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.RequestAdditionalTime
{
    public interface IRequestAdditionalTimeRepository : IGenericRepository<Entities.RequestAdditionalTime>
    {
        Tuple<bool, string> ValidateAdditionalTime(List<Entities.RequestAdditionalTime> requestAdditionalTimes);
        bool AddExtension(int workOrderServices, DateTime creation, int categoryId, int user, int time, int requested);
    }
}
