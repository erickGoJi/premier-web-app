using System.Collections.Generic;
using biz.premier.Entities;

namespace biz.premier.Repository.Training
{
    public interface ITrainingRepository : IGenericRepository<Entities.Training>
    {
        Entities.Training UpdateCustom(Entities.Training training, int key);
        Entities.Training GetCustom(int key);
        bool AddParticipants(int[] participants, int? training, int? groups, int assignedBy);
    }
}