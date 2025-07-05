using KazanlakRun.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Volunteer> Volunteers { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<VolunteerRole> VolunteerRoles { get; set; } = null!;
    public DbSet<AidStation> AidStations { get; set; } = null!;
    public DbSet<Distance> Distances { get; set; } = null!;
    public DbSet<AidStationDistance> AidStationDistances { get; set; } = null!;
    public DbSet<Good> Goods { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── 1. Seed на Identity роли ────────────────────────
        var adminRoleId = "a1b2c3d4-e5f6-4782-8209-d76562e0feaa";
        var userRoleId = "f1e2d3c4-b5a6-4948-9309-c56782b0faab";

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER"
            }
        );

        // ─── 2. Seed на default user (ако вече го имаш, не е нужно да повтаряш) ───
        var defaultUserId = "7699db7d-964f-4782-8209-d76562e0fece";
        modelBuilder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = defaultUserId,
                UserName = "admin@KazanlakRun.com",
                NormalizedUserName = "ADMIN@KAZANLAKRUN.COM",
                Email = "admin@KazanlakRun.com",
                NormalizedEmail = "ADMIN@KAZANLAKRUN.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<IdentityUser>()
                                          .HashPassword(null, "Admin123!"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        // ─── 3. Асоцииране на default user с ролята "Admin" ───
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = defaultUserId
            }
        );

        var normalUserId = "11111111-2222-3333-4444-555555555555";
        modelBuilder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = normalUserId,
                UserName = "user@abv.bg",
                NormalizedUserName = "USER@ABV.BG",
                Email = "user@abv.bg",
                NormalizedEmail = "USER@ABV.BG",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<IdentityUser>()
                                       .HashPassword(null, "User123!"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = normalUserId,
                RoleId = userRoleId
            }
        );


        // ─── Composite keys for join tables ─────────────────────
        modelBuilder.Entity<VolunteerRole>()
            .HasKey(vr => new { vr.VolunteerId, vr.RoleId });

        modelBuilder.Entity<AidStationDistance>()
            .HasKey(ad => new { ad.AidStationId, ad.DistanceId });

        // ─── Relationships ───────────────────────────────────────
        modelBuilder.Entity<Volunteer>()
            .HasOne(v => v.AidStation)
            .WithMany(a => a.Volunteers)
            .HasForeignKey(v => v.AidStationId);

        modelBuilder.Entity<VolunteerRole>()
            .HasOne(vr => vr.Volunteer)
            .WithMany(v => v.VolunteerRoles)
            .HasForeignKey(vr => vr.VolunteerId);

        modelBuilder.Entity<VolunteerRole>()
            .HasOne(vr => vr.Role)
            .WithMany(r => r.VolunteerRoles)
            .HasForeignKey(vr => vr.RoleId);

        modelBuilder.Entity<AidStationDistance>()
            .HasOne(ad => ad.AidStation)
            .WithMany(a => a.AidStationDistances)
            .HasForeignKey(ad => ad.AidStationId);

        modelBuilder.Entity<AidStationDistance>()
            .HasOne(ad => ad.Distance)
            .WithMany(d => d.AidStationDistances)
            .HasForeignKey(ad => ad.DistanceId);

        modelBuilder.Entity<Good>()
            .HasOne(g => g.AidStation)
            .WithMany(a => a.Goods)
            .HasForeignKey(g => g.AidStationId);

        // ─── Seed data ───────────────────────────────────────────

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "doctor" },
            new Role { Id = 2, Name = "security" },
            new Role { Id = 3, Name = "radio" },
            new Role { Id = 4, Name = "food preparation" },
            new Role { Id = 5, Name = "time recorder" }
        );

        modelBuilder.Entity<AidStation>().HasData(
            new AidStation { Id = 1, ShortName = "A1", Name = "Aid stationn 1" },
            new AidStation { Id = 2, ShortName = "A2", Name = "Aid stationn 2" },
            new AidStation { Id = 3, ShortName = "A3", Name = "Aid stationn 3" },
            new AidStation { Id = 4, ShortName = "A4", Name = "Aid stationn 4" },
            new AidStation { Id = 5, ShortName = "A5", Name = "Aid stationn 5" }
        );

         modelBuilder.Entity<Distance>().HasData(
          new Distance { Id = 1, Distans = "10 km", RegRunners = 100 },
          new Distance { Id = 2, Distans = "20 km", RegRunners = 80 },
          new Distance { Id = 3, Distans = "40 km", RegRunners = 60 }
);

        modelBuilder.Entity<Volunteer>().HasData(
            new Volunteer { Id = 1, Names = "Nikola Nikolov", Email = "nnnnnn@nnn.bg", Phone = "0888998899", AidStationId = 1 },
            new Volunteer { Id = 2, Names = "Ivan Ivanov", Email = "iiiiii@.iii.bg", Phone = "0888999999", AidStationId = 2 },
            new Volunteer { Id = 3, Names = "Georg Georgiev", Email = "gggggg@ggg.bg", Phone = "0888777777", AidStationId = 3 },
            new Volunteer { Id = 4, Names = "Petar Petrov", Email = "petarp@p.bg", Phone = "0888111111", AidStationId = 1 },
            new Volunteer { Id = 5, Names = "Maria Ivanova", Email = "maria@i.bg", Phone = "0888222222", AidStationId = 2 },
            new Volunteer { Id = 6, Names = "Stoyan Dimitrov", Email = "stoyand@d.bg", Phone = "0888333333", AidStationId = 3 },
            new Volunteer { Id = 7, Names = "Elena Petrova", Email = "elena@p.bg", Phone = "0888444444", AidStationId = 4 },
            new Volunteer { Id = 8, Names = "Vladimir Stoyanov", Email = "vlad@st.bg", Phone = "0888555555", AidStationId = 5 },
            new Volunteer { Id = 9, Names = "Tsveta Koleva", Email = "tsveta@k.bg", Phone = "0888666666", AidStationId = 1 },
            new Volunteer { Id = 10, Names = "Nikolay Marinov", Email = "nikolay@m.bg", Phone = "0888778778", AidStationId = 2 }
        );

        modelBuilder.Entity<Good>().HasData(
            new Good { Id = 1, Name = "Table", Measure = "pcs", Quantity = 10, AidStationId = 1 },
            new Good { Id = 2, Name = "Chair", Measure = "pcs", Quantity = 20, AidStationId = 1 },
            new Good { Id = 3, Name = "Tent", Measure = "pcs", Quantity = 7, AidStationId = 1 },
            new Good { Id = 4, Name = "Plates", Measure = "pcs", Quantity = 40, AidStationId = 2 },
            new Good { Id = 5, Name = "Cups", Measure = "pcs", Quantity = 200, AidStationId = 2 },
            new Good { Id = 6, Name = "Salt", Measure = "kg", Quantity = 1, AidStationId = 2 },
            new Good { Id = 7, Name = "Drinking water", Measure = "l", Quantity = 100, AidStationId = 3 },
            new Good { Id = 8, Name = "Isotonic", Measure = "l", Quantity = 50, AidStationId = 3 },
            new Good { Id = 9, Name = "Lemons", Measure = "pcs", Quantity = 100, AidStationId = 3 },
            new Good { Id = 10, Name = "Apples", Measure = "kg", Quantity = 100, AidStationId = 4 },
            new Good { Id = 11, Name = "Bananas", Measure = "pcs", Quantity = 100, AidStationId = 4 },
            new Good { Id = 12, Name = "Chocolate", Measure = "pcs", Quantity = 50, AidStationId = 4 },
            new Good { Id = 13, Name = "Soap", Measure = "bottles", Quantity = 10, AidStationId = 5 }
        );
    }
}
