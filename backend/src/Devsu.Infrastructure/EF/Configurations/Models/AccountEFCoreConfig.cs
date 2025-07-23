namespace Devsu.Infrastructure.EF.Configurations.Models;

public class AccountEFCoreConfig : EFCoreConfiguration<Account>
{
    public override void ConfigureEF(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasIndex(x => x.AccountNumber).IsUnique();
        builder.HasIndex(x => x.AccountType);
        
        builder.Property(x => x.AccountNumber).IsRequired();
        builder.Property(x => x.InitialBalance).IsRequired();
        builder.Property(x => x.CurrentBalance).IsRequired();
        builder.Property(x => x.AccountType).IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Accounts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}