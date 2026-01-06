using BookStore_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore_Infrastrcuture.Data.Model;

public class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
    
            builder.HasKey(e => e.AuthorId).HasName("PK__Authors__86516BCF744ECEB0");

            builder.Property(e => e.AuthorId).HasColumnName("author_id");
            builder.Property(e => e.BirthDay).HasColumnName("birth_date");
            builder.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            builder.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
   
    }
}
