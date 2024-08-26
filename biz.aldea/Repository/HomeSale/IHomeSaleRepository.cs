namespace biz.premier.Repository.HomeSale
{
    public interface IHomeSaleRepository : IGenericRepository<Entities.HomeSale>
    {
        Entities.HomeSale GetCustom(int key);
        bool DeleteReminder(int key);
        bool DeleteDocument(int key);
        Entities.HomeSale UpdateCustom(Entities.HomeSale homeSale, int key);
    }
}