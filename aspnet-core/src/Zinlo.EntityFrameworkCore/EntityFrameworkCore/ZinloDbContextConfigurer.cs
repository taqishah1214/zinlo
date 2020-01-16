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
        // Bellow is mysql configuration

        //public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, string connectionString)
        //{
        //    builder.UseMySql(connectionString);
        //}

        //public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, DbConnection connection)
        //{
        //    builder.UseMySql(connection);
        //}
        //New PosgreSQL Configuration
        public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, string connectionString)
        {
            builder.UseNpgsql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ZinloDbContext> builder, DbConnection connection)
        {
            builder.UseNpgsql(connection);
        }
    }
}