using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.Training
{
    public class trainingDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public bool Groups { get; set; }
        public int? TrainingGroup { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // public virtual CatTrainingGroup TrainingGroupNavigation { get; set; }
        // public virtual CatTrainingType TypeNavigation { get; set; }
        public virtual ICollection<ContentDto> Contents { get; set; }
    }
}