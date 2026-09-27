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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.Property(c => c.Description)
            .HasMaxLength(500)
            .IsRequired(false);

            builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

            builder.HasMany(c => c.Tickets)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
