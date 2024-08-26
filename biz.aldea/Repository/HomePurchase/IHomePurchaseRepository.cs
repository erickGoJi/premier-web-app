using biz.premier.Entities;

namespace biz.premier.Repository.HomePurchase
{
    public interface IHomePurchaseRepository : IGenericRepository<Entities.HomePurchase>
    {
        Entities.HomePurchase GetCustom(int key);
        Entities.HomePurchase UpdateCustom(Entities.HomePurchase homePurchase,int key);
        bool DeleteDocument(int key);
        bool DeleteReminder(int key);
        DocumentHomePurchase FindDocument(int key);
    }
}