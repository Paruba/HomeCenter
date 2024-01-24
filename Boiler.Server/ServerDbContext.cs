using Boiler.Server.Models;
using Boiler.Server.Models.Camera;
using Boiler.Server.Models.Identity;
using Boiler.Server.Models.Thermometer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Boiler.Server;

public class ServerDbContext : IdentityDbContext<IdentityUser>
{
    public ServerDbContext(DbContextOptions<ServerDbContext> opts) : base(opts)
    {

    }
    public DbSet<ApplicationUserModel> ApplicationUsers { get; set; }
    public DbSet<ApplicationRoleModel> ApplicationRoles { get; set; }
    public DbSet<ApplicationUserRoleModel> ApplicationUserRoles { get; set; }
    public DbSet<TemperatureModel> Temperatures { get; set; }
    public DbSet<ThermometerModel> Thermometer { get; set; }
    public DbSet<CameraModel> Camera { get; set; }
    public DbSet<VideoModel> Video { get; set; }
    public DbSet<ImageModel> Image { get; set; }
    public DbSet<FaceDetectionModel> Faces { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TemperatureModel>(b =>
        {
            b.ToTable("Temperature", Constants.Database.PublicDbSchema).HasKey(x => x.Id);
        });

        modelBuilder.Entity<ApplicationUserModel>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Id).HasColumnName("Id");

            b.Property(p => p.UserName).HasColumnName("UserName").HasMaxLength(256);

            b.HasMany(b => b.Roles).WithMany(x => x.Users).UsingEntity<ApplicationUserRoleModel>();

            b.ToTable("AspNetUsers", Constants.Database.PublicDbSchema);
        });

        modelBuilder.Entity<ApplicationRoleModel>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Id).HasColumnName("Id");

            b.HasMany(x => x.Users).WithMany(y => y.Roles).UsingEntity<ApplicationUserRoleModel>();

            b.ToTable("AspNetRoles", Constants.Database.PublicDbSchema);
        });

        modelBuilder.Entity<ApplicationUserRoleModel>(b =>
        {
            b.Property(p => p.UserId).HasColumnName("UserId");
            b.HasOne(p => p.User).WithMany().HasForeignKey(x => x.UserId);
            b.Property(p => p.RoleId).HasColumnName("RoleId");
            b.HasOne(p => p.Role).WithMany().HasForeignKey(x => x.RoleId);
            b.ToTable("AspNetUserRoles", Constants.Database.PublicDbSchema).HasKey(x => new { x.UserId, x.RoleId });
        });

        modelBuilder.Entity<ThermometerModel>(b =>
        {
            b.ToTable("Thermometer", Constants.Database.PublicDbSchema).HasKey(x => x.Id);
            b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<CameraModel>(b =>
        {
            b.ToTable("Camera", Constants.Database.PublicDbSchema).HasKey(x => x.Id);
            b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<VideoModel>(b =>
        {
            b.ToTable("Video", Constants.Database.PublicDbSchema).HasKey(x => x.Id);
            b.HasOne(x => x.Camera).WithMany().HasForeignKey(x => x.CameraId);
        });

        modelBuilder.Entity<ImageModel>(b =>
        {
            b.ToTable("Image", Constants.Database.PublicDbSchema).HasKey(x => x.Id);
            b.HasOne(x => x.Camera).WithMany().HasForeignKey(x => x.CameraId);
        });

        modelBuilder.Entity<FaceDetectionModel>(b =>
        {
            b.ToTable("Faces", Constants.Database.PublicDbSchema).HasKey(x => x.Id);
            b.HasOne(x => x.Video).WithMany().HasForeignKey(x => x.VideoId);
        });

        base.OnModelCreating(modelBuilder);
    }
}