namespace MobitekCRMV2.DataAccess.UoW
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Veritabanına setter bir işlem yapılacağı zaman sadece repository'de ki metodu çağırmak yeterli olmaz!!!
        /// Bu metot çalıştırıldığı zaman repository'den kullanılan işlemler db'ye yansıyacaktır.
        /// Db'ye aynı process içerisinde birden fazla işlem yapılacaksa, tüm işlemler uygulandıktan sonra bu metot çağrılmalıdır ki
        /// herhangibi bir hatada tüm yapılan transaction'lar rollback yapılsın. Bu sayede db'ye eksik,yanlış,hatalı kayıt atılmasının önüne geçilir.
        /// </summary>
        /// <returns></returns>
        Task CommitAsync();
    }
}
