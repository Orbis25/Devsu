namespace Devsu.Infrastructure.EF.Configurations.Models;

public class TransactionsEFCoreConfig : EFCoreConfiguration<Transaction>
{
    public override void ConfigureEF(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasIndex(x => x.Type);
        builder.Property(x => x.CurrentBalance).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        
        builder.HasOne(x => x.Account)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}