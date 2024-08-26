using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models
{
    public class AddReminder
    {
        public int user { get; set; }
        public int service { get; set; }
        public DateTime reminderDate { get; set; } 
        public int category { get; set; } 
        public string comment { get; set; }
    }
}
