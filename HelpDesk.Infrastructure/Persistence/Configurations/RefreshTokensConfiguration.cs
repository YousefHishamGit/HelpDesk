using HelpDesk.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class RefreshTokensConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);
            
            builder.HasIndex(rt=>rt.Token).IsUnique();
            builder.Property(rt => rt.Token)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.HasOne(rt => rt.User)
                       .WithMany(u => u.RefreshTokens)
                       .HasForeignKey(rt => rt.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
