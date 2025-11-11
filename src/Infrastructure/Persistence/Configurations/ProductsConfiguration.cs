using Domain.Products;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ProductId(x));

        builder.Property(x => x.Title)
            .HasColumnType("varchar(255)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .IsRequired(false);

        builder.OwnsOne<ProductFeatures>(x => x.Features, featuresBuilder =>
        {
            featuresBuilder.ToJson("features");

            featuresBuilder.Property(x => x.Weight).HasColumnName("weight");
            featuresBuilder.Property(x => x.Height).HasColumnName("height");
            featuresBuilder.Property(x => x.Width).HasColumnName("width");
            featuresBuilder.Property(x => x.Ram).HasColumnName("ram");
            featuresBuilder.Property(x => x.Storage).HasColumnName("storage");
            featuresBuilder.Property(x => x.ScreenSize).HasColumnName("screen_size");

            // featuresBuilder.OwnsOne<NestedFeature>(nf => nf.NestedFeature, nestedFeatureBuilder =>
            // {
            //     featuresBuilder.ToJson("nested_feature"); // Will throw an error
            // });
        });
    }
}