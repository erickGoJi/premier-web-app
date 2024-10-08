﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SettlingIn
{
    public class ExtensionSettlingInDto
    {
        public int Id { get; set; }
        public int? SettlingInId { get; set; }
        public string RequestedBy { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? Time { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual SettlingInDto SettlingIn { get; set; }
    }
}
