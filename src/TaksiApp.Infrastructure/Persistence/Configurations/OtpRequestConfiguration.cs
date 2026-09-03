using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaksiApp.Domain.Entities;

namespace TaksiApp.Infrastructure.Persistence.Configurations;

public class OtpRequestConfiguration : IEntityTypeConfiguration<OtpRequest>
{
    public void Configure(EntityTypeBuilder<OtpRequest> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.CountryCode)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.OtpType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Role)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(o => new { o.PhoneNumber, o.Code });
    }
}
