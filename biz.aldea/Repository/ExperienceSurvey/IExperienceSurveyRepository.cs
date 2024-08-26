using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.ExperienceSurvey
{
    public interface IExperienceSurveyRepository : IGenericRepository<biz.premier.Entities.ExperienceSurvey>
    {
        Entities.ExperienceSurvey UpdateCustom(Entities.ExperienceSurvey experienceSurvey, int key);
        Entities.ExperienceSurvey SelectCustom(int key);
        bool CompleteSrByServiceLine(int sr, bool complete, int serviceLine);
        int ReturnCoordinator(int sr, int serviceLine);
        string ReturnService(int workOrderServices, int sr);
    }
}
