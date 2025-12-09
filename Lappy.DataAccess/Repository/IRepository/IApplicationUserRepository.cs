using Lappy.Models;


namespace Lappy.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser obj);
    }
}
