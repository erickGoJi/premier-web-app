using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository.Training;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Training
{
    public class ParticipantRepository : GenericRepository<Participant>, IParticipantRepository
    {
        public ParticipantRepository(Db_PremierContext context):base(context){}
        public ActionResult GetMyTrainings(int user)
        {
            var trainings = _context.Participants.Where(x => x.UserParticipant == user).Select(s => new
            {
                s.Training,
                participantId = s.Id,
                s.Score,
                status = s.StatusNavigation.Name,
                s.TrainingNavigation.Description,
                title = s.TrainingNavigation.Name,
                s.CompletedDate,
                s.Percentage,
                s.TrainingNavigation.Photo
            }).ToList();
            return new ObjectResult(trainings);
        }

        public ActionResult GetTraining(int participant)
        {
            var myTraining = _context.Participants
                .Include(i => i.ParticipantContents)
                .ThenInclude(i => i.ParticipantEvaluations)
                .SingleOrDefault(x => x.Id == participant);
            var trainingFind = _context.Training
                .Include(i => i.Contents)
                .ThenInclude(i => i.Evaluations)
                .ThenInclude(i => i.Answers)
                .Include(i => i.Contents)
                .ThenInclude(i => i.Themes)
                .ThenInclude(i => i.Files)
                .SingleOrDefault(x => x.Id == myTraining.Training);
            var objectResponse = new
            {
                myTrainingAdvance = myTraining,
                training = trainingFind
            };
            return new ObjectResult(objectResponse);
        }

        public ActionResult GetParticipantList()
        {
            var participants = _context.Users.Where(x => x.RoleId != 4).Select(s => new
            {
                s.Id,
                name = $"{s.Name} {s.LastName} {s.MotherLastName}",
                role = s.Role.Role
            }).ToList();
            return new ObjectResult(participants);
        }
    }
}