using System;
using System.Linq;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Zinlo.Editions;
using Zinlo.EntityFrameworkCore;

namespace Zinlo.Migrations.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly ZinloDbContext _context;
        private readonly string _tenantName;

        public DefaultTenantBuilder(ZinloDbContext context)
        {
            _context = context;
        }
        public DefaultTenantBuilder(ZinloDbContext context,string tenantName)
        {
            _context = context;
            _tenantName = tenantName;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        public void CreateSeederTenants ()
        {
            GenerateSeederTenants();
        }

        private void GenerateSeederTenants()
        {

            var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == _tenantName);
            if (defaultTenant == null)
            {
                defaultTenant = new MultiTenancy.Tenant(_tenantName, _tenantName);

                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == "Standard");
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();

            }
        }

            private void CreateDefaultTenant()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == MultiTenancy.Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                defaultTenant = new MultiTenancy.Tenant(AbpTenantBase.DefaultTenantName, AbpTenantBase.DefaultTenantName);

                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();
            }
        }
    }
}
