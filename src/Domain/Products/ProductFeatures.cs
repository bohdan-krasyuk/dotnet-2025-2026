namespace Domain.Products;

public class ProductFeatures
{
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public double? Width { get; set; }

    public double? Ram { get; set; }
    public double? Storage { get; set; }
    public double? ScreenSize { get; set; }

    private ProductFeatures(
        double? weight,
        double? height,
        double? width,
        double? ram,
        double? storage,
        double? screenSize)
    {
        Weight = weight;
        Height = height;
        Width = width;
        Ram = ram;
        Storage = storage;
        ScreenSize = screenSize;
    }

    public static ProductFeatures New(
        double? weight,
        double? height,
        double? width,
        double? ram,
        double? storage,
        double? screenSize)
        => new(weight: weight, height: height, width: width, ram: ram, storage: storage, screenSize: screenSize);

    public static ProductFeatures NewAsRefrigerator(double weight, double height, double width)
        => new(width: width, height: height, weight: weight, ram: null, storage: null, screenSize: null);

    public static ProductFeatures NewAsPhone(double? ram, double? storage, double? screenSize)
        => new(width: null, height: null, weight: null, ram: ram, storage: storage, screenSize: screenSize);

    public static ProductFeatures Empty() => new(null, null, null, null, null, null);
}