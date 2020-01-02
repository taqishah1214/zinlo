using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Zinlo.EntityFrameworkCore
{
    public static class ZinloDbContextConfigurer
    {
        //public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, string connectionString)
        //{
        //    builder.UseSqlServer(connectionString);
        //}

        //public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, DbConnection connection)
        //{
        //    builder.UseSqlServer(connection);
        //}
        public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection);
        }
    }
}