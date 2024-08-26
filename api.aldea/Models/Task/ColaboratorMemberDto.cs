using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Task
{
    public class ColaboratorMemberDto
    {
        public int Task { get; set; }
        public int Colaborator { get; set; }

        public virtual UserDto ColaboratorNavigation { get; set; }
        public virtual TaskDto TaskNavigation { get; set; }
    }
}
