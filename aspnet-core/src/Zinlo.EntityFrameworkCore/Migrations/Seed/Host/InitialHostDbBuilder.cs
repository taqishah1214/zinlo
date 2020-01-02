using Zinlo.EntityFrameworkCore;

namespace Zinlo.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly ZinloDbContext _context;

        public InitialHostDbBuilder(ZinloDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
