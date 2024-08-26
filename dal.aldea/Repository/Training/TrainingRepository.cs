using System;
using System.Collections.Generic;
using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Training;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Training
{
    public class TrainingRepository : GenericRepository<biz.premier.Entities.Training>, ITrainingRepository
    {
        public TrainingRepository(Db_PremierContext context) : base(context){}
        public biz.premier.Entities.Training UpdateCustom(biz.premier.Entities.Training training, int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Training
                .Include(i => i.Contents)
                .ThenInclude(i => i.Evaluations)
                .ThenInclude(i => i.Answers)
                .Include(i => i.Contents)
                .ThenInclude(i => i.Themes)
                .ThenInclude(i => i.Files)
                .SingleOrDefault(x => x.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(training);
                foreach (var content in training.Contents)
                {
                    var _content = exist.Contents.FirstOrDefault(p => p.Id == content.Id);
                    if (_content == null)
                    {
                        exist.Contents.Add(content);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(_content).CurrentValues.SetValues(content);
                        /*
                         * Evaluation
                         */
                        foreach (var evaluation in content.Evaluations)
                        {
                            var _exist = _content.Evaluations.FirstOrDefault(x => x.Id == evaluation.Id);
                            if (_exist == null)
                            {
                                _content.Evaluations.Add(evaluation);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(_exist).CurrentValues.SetValues(evaluation);
                                foreach (var answer in _exist.Answers)
                                {
                                    var _answer = _exist.Answers.FirstOrDefault(x => x.Id == answer.Id);
                                    if (_answer == null)
                                    {
                                        _exist.Answers.Add(answer);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_answer).CurrentValues.SetValues(answer);
                                    }
                                }
                            }
                        }
                        /*
                         * Themes
                         */
                        foreach (var theme in content.Themes)
                        {
                            var _exist = _content.Themes.FirstOrDefault(x => x.Id == theme.Id);
                            if (_exist == null)
                            {
                                _content.Themes.Add(theme);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(_exist).CurrentValues.SetValues(theme);
                                foreach (var file in _exist.Files)
                                {
                                    var _file = _exist.Files.FirstOrDefault(x => x.Id == file.Id);
                                    if (_file == null)
                                    {
                                        _exist.Files.Add(file);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_file).CurrentValues.SetValues(file);
                                    }
                                }
                            }
                        }
                    }
                }
                _context.SaveChanges();
            }

            return exist;
        }

        public biz.premier.Entities.Training GetCustom(int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Training
                .Include(i => i.Contents)
                .ThenInclude(i => i.Evaluations)
                .ThenInclude(i => i.Answers)
                .Include(i => i.Contents)
                .ThenInclude(i => i.Themes)
                .ThenInclude(i => i.Files)
                .SingleOrDefault(x => x.Id == key);
            return exist;
        }

        public bool AddParticipants(int[] participants, int? training, int? groups, int assignedBy)
        {
            bool b = false;
            if (training.HasValue)
            {
                foreach (var participant in participants)
                {
                    var _exist = _context.Participants.SingleOrDefault(s =>
                        s.Training == training && s.UserParticipant == participant);
                    if (_exist == null)
                    {
                        _context.Participants.Add(new Participant()
                        {
                            Training = training.Value,
                            UserParticipant = participant,
                            CreatedDate = DateTime.Now,
                            CreatedBy = assignedBy,
                            Status = 1
                        });
                    }
                    
                }

                b = true;
            } else if (groups.HasValue)
            {
                var trainings = _context.Training.Where(x => x.Groups == true && x.TrainingGroup == groups.Value)
                    .ToList();
                foreach (var _training in trainings)
                {
                    foreach (var participant in participants)
                    {
                        var exist = _context.Participants.SingleOrDefault(s =>
                            s.Training == _training.Id && s.UserParticipant == participant);
                        if (exist == null)
                        {
                            _context.Participants.Add(new Participant()
                            {
                                Training = _training.Id,
                                UserParticipant = participant,
                                CreatedDate = DateTime.Now,
                                CreatedBy = assignedBy,
                                Status = 1
                            });    
                        }
                    }
                }
                b = true;
            }

            _context.SaveChanges();
            return b;
        }
    }
}