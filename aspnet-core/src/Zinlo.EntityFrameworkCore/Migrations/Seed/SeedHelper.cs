using System;
using System.Collections.Generic;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Zinlo.EntityFrameworkCore;
using Zinlo.Migrations.Seed.Host;
using Zinlo.Migrations.Seed.Tenants;

namespace Zinlo.Migrations.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<ZinloDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(ZinloDbContext context)
        {
            var tenantId = 0;
            List<string> tenantName = new List<string>();
            tenantName.Add("Techverx 1");
            tenantName.Add("Techverx 2");
            tenantName.Add("Techverx 3");
            tenantName.Add("Techverx 4");

            List<string> tenantPrimaryAdmin = new List<string>();
            tenantPrimaryAdmin.Add("hammad@techverx.com");
            tenantPrimaryAdmin.Add("ihsan.techverx.com");
            tenantPrimaryAdmin.Add("taqi.techverx.com");
            tenantPrimaryAdmin.Add("shaban.techverx.com");



            context.SuppressAutoSetTenantId = true;

            //Host seed
            new InitialHostDbBuilder(context).Create();

            //Default tenant seed (in host database).
            new DefaultTenantBuilder(context).Create();

            new TenantRoleAndUserBuilder(context, 1).Create();


            for (int i = 0; i<tenantName.Count; i++)
            {
                new DefaultTenantBuilder(context, tenantName[i]).CreateSeederTenants();
            }

            for (int i = 0; i < tenantPrimaryAdmin.Count; i++)
            {
                tenantId = context.Tenants.FirstAsync(p => p.Name.Equals(tenantName[i])).Result.Id;
                new TenantRoleAndUserBuilder(context, tenantId, tenantName[i], tenantPrimaryAdmin[i]).CreateSeederAdmins();
            }


            
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }
    }
}
