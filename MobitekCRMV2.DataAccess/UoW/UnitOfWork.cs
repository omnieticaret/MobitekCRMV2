using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.DataAccess.Context;

namespace MobitekCRMV2.DataAccess.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public UnitOfWork(CRMDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
