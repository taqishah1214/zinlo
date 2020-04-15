﻿using Zinlo.TimeManagements;
using Zinlo.Tests;
using Zinlo.Categories;
using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Zinlo.Authorization.Roles;
using Zinlo.Authorization.Users;
using Zinlo.Chat;
using Zinlo.Editions;
using Zinlo.Friendships;
using Zinlo.MultiTenancy;
using Zinlo.MultiTenancy.Accounting;
using Zinlo.MultiTenancy.Payments;
using Zinlo.Storage;
using Abp.Localization;
using Zinlo.Reconciliation;
using Zinlo.ImportsPaths;
using Zinlo.InstructionVersions;

namespace Zinlo.EntityFrameworkCore
{
    public class ZinloDbContext : AbpZeroDbContext<Tenant, Role, User, ZinloDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<TimeManagement> TimeManagements { get; set; }

        public virtual DbSet<Attachment.Attachment> Attachments { get; set; }
        public virtual DbSet<ClosingChecklist.ClosingChecklist> ClosingChecklists { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment.Comment> Comments { get; set; }
        public virtual DbSet<AccountSubType.AccountSubType> AccountSubTypes { get; set; }
        public virtual DbSet<ChartofAccounts.ChartofAccounts> ChartsofAccounts { get; set; }
        public virtual DbSet<Itemization> Itemizations { get; set; }
        public virtual DbSet<Amortization> Amortizations { get; set; }
        public virtual DbSet<ImportsPath> ImportsPaths { get; set; }

        public virtual DbSet<UserPaymentDetails>  UserPaymentDetails { get; set; }
        public virtual DbSet<UserBusinessInfo>  UserBusinessInfos { get; set; }

        /* Define an IDbSet for each entity of the application */


        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }
        public virtual DbSet<InstructionVersion> Versions { get; set; }

        public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

        public ZinloDbContext(DbContextOptions<ZinloDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Specific for postgresql bellow 3 lines
           
            modelBuilder.Entity<TimeManagement>(t =>
            {
                t.HasIndex(e => new { e.TenantId });
            });
 modelBuilder.Entity<ApplicationLanguageText>()
            .Property(p => p.Value)
            .HasMaxLength(100); // any integer that is smaller than 10485760


            //           modelBuilder.Entity<Category>(c =>
            //           {
            //               c.HasIndex(e => new { e.TenantId });
            //           });
            //modelBuilder.Entity<Test>(t =>
            //           {
            //               t.HasIndex(e => new { e.TenantId });
            //           });
            //modelBuilder.Entity<Category.Category>(c =>
            //           {
            //               c.HasIndex(e => new { e.TenantId });
            //           });
            //modelBuilder.Entity<BinaryObject>(b =>
            //           {
            //               b.HasIndex(e => new { e.TenantId });
            //           });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.Entity<SubscriptionPaymentExtensionData>(b =>
            {
                b.HasQueryFilter(m => !m.IsDeleted)
                    .HasIndex(e => new { e.SubscriptionPaymentId, e.Key, e.IsDeleted })
                    .IsUnique();
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}
