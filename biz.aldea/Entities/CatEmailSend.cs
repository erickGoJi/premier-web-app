﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class CatEmailSend
    {
        public CatEmailSend()
        {
            EmailSends = new HashSet<EmailSend>();
        }

        public int Id { get; set; }
        public string Email { get; set; }

        public virtual ICollection<EmailSend> EmailSends { get; set; }
    }
}