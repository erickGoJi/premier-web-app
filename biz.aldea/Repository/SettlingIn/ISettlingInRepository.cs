using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.SettlingIn
{
    public interface ISettlingInRepository : IGenericRepository<Entities.SettlingIn>
    {
        Entities.SettlingIn UpdateCustom(Entities.SettlingIn settlingIn, int key);
        Entities.SettlingIn GetCustom(int key);
    }
}
