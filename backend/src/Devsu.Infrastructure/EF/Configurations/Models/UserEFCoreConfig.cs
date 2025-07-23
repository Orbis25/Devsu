namespace Devsu.Infrastructure.EF.Configurations.Models;

public class UserEFCoreConfig : EFCoreConfiguration<User>
{
    public override void ConfigureEF(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasIndex(x => x.ClientId);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Age).IsRequired();
        builder.Property(x => x.Phone).IsRequired();
        builder.Property(x => x.Identification).IsRequired();
        builder.Property(x => x.Gender).IsRequired();
        builder.Property(x => x.Password).IsRequired();

    }
}