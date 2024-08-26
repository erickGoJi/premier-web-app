using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository.Training
{
    public interface IParticipantRepository : IGenericRepository<Participant>
    {
        ActionResult GetMyTrainings(int user);
        ActionResult GetTraining(int participant);
        ActionResult GetParticipantList();
    }
}