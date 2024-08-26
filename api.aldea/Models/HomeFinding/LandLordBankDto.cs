using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class LandLordBankDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public int? AccountType { get; set; }
        public string GeneralComments { get; set; }
        public string Comments { get; set; }

        //public virtual CatBankAccountTypeDto AccountTypeNavigation { get; set; }
        public virtual HomeFindingDto HomeFinding { get; set; }
        public virtual ICollection<LandLordBankDetailDto> LandLordBankDetails { get; set; }
    }
}
