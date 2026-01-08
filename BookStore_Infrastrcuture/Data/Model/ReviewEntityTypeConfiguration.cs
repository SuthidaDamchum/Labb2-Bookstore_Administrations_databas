using BookStore_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore_Infrastrcuture.Data.Model;

public class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
            builder.HasKey(e => new { e.CustomerId, e.Isbn13 }).HasName("PK_review");

            builder.ToTable("Review");

            builder.Property(e => e.CustomerId).HasColumnName("customer_id");
            builder.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            builder.Property(e => e.Comment)
                .HasMaxLength(500)
                .HasColumnName("comment");
            builder.Property(e => e.Rating).HasColumnName("rating");
            builder.Property(e => e.ReviewDatetime)
                .HasColumnType("smalldatetime")
                .HasColumnName("review_datetime");

            builder.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__customer__2EDAF651");

            builder.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Review__isbn13__2FCF1A8A");
    }
}
