using Domain.Core.Models;
namespace Devsu.Infrastructure.EF.Configurations.Core;

public abstract class EfCoreConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
       where TEntity : BaseModel
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.CreatedAt).ValueGeneratedNever();
        builder.HasQueryFilter(x => !x.IsDeleted);
        ConfigureEF(builder);
    }

    public abstract void ConfigureEF(EntityTypeBuilder<TEntity> builder);
}
