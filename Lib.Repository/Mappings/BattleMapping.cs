using Lib.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Lib.Repository.Mappings;

[ExcludeFromCodeCoverage]
public class BattleMapping : IEntityTypeConfiguration<Battle>
{
    public void Configure(EntityTypeBuilder<Battle> builder)
    {
        builder.ToTable("Battle");

        builder.Property(p => p.Id).HasColumnType("INTEGER").IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.MonsterA).HasColumnType("INTEGER").IsRequired();
        builder.Property(p => p.MonsterB).HasColumnType("INTEGER").IsRequired();
        builder.Property(p => p.Winner).HasColumnType("INTEGER").IsRequired();

        builder.HasKey(p => p.Id);
    }
}