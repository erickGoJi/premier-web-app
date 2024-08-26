using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.AssigneeName
{
    public interface IAssigneeNameRepository : IGenericRepository<Entities.AssigneeInformation>
    {
        Entities.AssigneeInformation updateCustom(Entities.AssigneeInformation assigmentInformation);

        Entities.AssigneeInformation GetAssigneeInfoByWOSId(int wos_id);


        Entities.AssigneeInformation GetAssigneeInfoBySRId(int sr_id);

        DependentInformation GetDependentInformationId(int id);

        DependentInformation updateDepCustom(DependentInformation dep);

        int DeleteDependent(int id);

        DependentInformation AddDepCustom(DependentInformation dep);

        Pet GetPetId(int id);

        Pet updatePetCustom(Pet pet);

        int DeletePet(int id);

        Pet AddPetCustom(Pet dep);
    }
}
