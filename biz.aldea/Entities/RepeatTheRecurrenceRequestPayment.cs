﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class RepeatTheRecurrenceRequestPayment
    {
        public int Recurrence { get; set; }
        public int Day { get; set; }

        public virtual CatDay DayNavigation { get; set; }
        public virtual RecurrenceRequestPayment RecurrenceNavigation { get; set; }
    }
}