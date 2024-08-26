﻿using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Departure;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Departure
{
    public class RepairDepartureRepository: GenericRepository<Repair>, IRepairDepartureRepository
    {
        public RepairDepartureRepository(Db_PremierContext context) : base(context) { }
    }
}
