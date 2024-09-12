using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Repositories.Entity;

namespace MilkStore.Repositories.Context
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaims, ApplicationUserRoles, ApplicationUserLogins, ApplicationRoleClaims, ApplicationUserTokens>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        #region Entity
        // user
        public virtual DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public virtual DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();
        public virtual DbSet<ApplicationUserClaims> ApplicationUserClaims => Set<ApplicationUserClaims>();
        public virtual DbSet<ApplicationUserRoles> ApplicationUserRoles => Set<ApplicationUserRoles>();
        public virtual DbSet<ApplicationUserLogins> ApplicationUserLogins => Set<ApplicationUserLogins>();
        public virtual DbSet<ApplicationRoleClaims> ApplicationRoleClaims => Set<ApplicationRoleClaims>();
        public virtual DbSet<ApplicationUserTokens> ApplicationUserTokens => Set<ApplicationUserTokens>();

        public virtual DbSet<UserInfo> UserInfos => Set<UserInfo>();
        public virtual DbSet<User> Users => Set<User>();
        public virtual DbSet<Post> Posts => Set<Post>();
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? throw new Exception("DATABASE_CONNECTION_STRING is not set"), b => b.MigrationsAssembly("MilkStore.Repositories"));
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Post>()
        //        .HasMany(p => p.Products)
        //        .WithMany(p => p.Posts)
        //        .UsingEntity(j => j.ToTable("PostProducts"));  // Custom join table
        //}

    }
}
