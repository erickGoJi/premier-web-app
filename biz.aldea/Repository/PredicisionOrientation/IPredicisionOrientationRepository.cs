using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.PredicisionOrientation
{
    public interface IPredicisionOrientationRepository: IGenericRepository<Entities.PredecisionOrientation>
    {
        Entities.PredecisionOrientation UpdateCustom(Entities.PredecisionOrientation predecisionOrientation, int key);
        Entities.PredecisionOrientation GetCustom(int key);
    }
}
