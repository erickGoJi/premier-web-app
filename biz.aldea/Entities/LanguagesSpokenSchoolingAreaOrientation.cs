﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class LanguagesSpokenSchoolingAreaOrientation
    {
        public int Schooling { get; set; }
        public int LanguagesSpoken { get; set; }

        public virtual CatLanguage LanguagesSpokenNavigation { get; set; }
        public virtual SchoolingAreaOrientation SchoolingNavigation { get; set; }
    }
}