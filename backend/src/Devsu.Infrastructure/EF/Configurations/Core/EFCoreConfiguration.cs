using Domain.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Devsu.Infrastructure.EF.Configurations.Core;

public abstract class EFCoreConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
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
