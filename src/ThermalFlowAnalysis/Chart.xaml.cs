using System.Windows;
using System.Windows.Media;
using TomsToolbox.Essentials;
using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Controls;
using TomsToolbox.Wpf.XamlExtensions;

namespace ThermalFlowAnalysis;

public class DataLine
{
    public DataLine(ICollection<Point> points, Brush color)
    {
        Points = points;
        Color = color;
    }

    public ICollection<Point> Points { get; }

    public Brush Color { get; }
}

public record DataPoint(Point Position, Brush Color)
{
    public string? Label { get; set; }
}

/// <summary>
/// Interaction logic for ChartView.xaml
/// </summary>
public partial class Chart
{
    private const FrameworkPropertyMetadataOptions MetadataOptions = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange;

    public Chart()
    {
        InitializeComponent();
    }

    public ICollection<DataLine>? Lines
    {
        get => (ICollection<DataLine>)GetValue(LinesProperty);
        set => SetValue(LinesProperty, value);
    }
    public static readonly DependencyProperty LinesProperty = DependencyProperty.Register(
        nameof(Lines), typeof(ICollection<DataLine>), typeof(Chart),
        new FrameworkPropertyMetadata(default(ICollection<DataLine>), MetadataOptions, (sender, _) => ((Chart)sender).Lines_Changed()));

    public ICollection<DataPoint>? Points
    {
        get => (ICollection<DataPoint>)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        nameof(Points), typeof(ICollection<DataPoint>), typeof(Chart),
        new FrameworkPropertyMetadata(default(ICollection<DataPoint>), MetadataOptions, (sender, _) => ((Chart)sender).Points_Changed()));

    public Point Origin
    {
        get => (Point)GetValue(OriginProperty);
        set => SetValue(OriginProperty, value);
    }
    public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
        nameof(Origin), typeof(Point), typeof(Chart),
        new FrameworkPropertyMetadata(default(Point), MetadataOptions, (sender, _) => ((Chart)sender).Origin_Changed()));

    public Rect BoundingRect
    {
        get => (Rect)GetValue(BoundingRectProperty);
        private set => SetValue(BoundingRectPropertyKey, value);
    }
    private static readonly DependencyPropertyKey BoundingRectPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(BoundingRect), typeof(Rect), typeof(Chart),
        new FrameworkPropertyMetadata(default(Rect), MetadataOptions));
    public static readonly DependencyProperty BoundingRectProperty = BoundingRectPropertyKey.DependencyProperty;

    public Rect DataBounds
    {
        get => (Rect)GetValue(DataBoundsProperty);
        private set => SetValue(DataBoundsPropertyKey, value);
    }
    private static readonly DependencyPropertyKey DataBoundsPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(DataBounds), typeof(Rect), typeof(Chart),
        new FrameworkPropertyMetadata(default(Rect), MetadataOptions));
    public static readonly DependencyProperty DataBoundsProperty = DataBoundsPropertyKey.DependencyProperty;

    public Size Q1
    {
        get => (Size)GetValue(Q1Property);
        private set => SetValue(Q0PropertyKey, value);
    }
    private static readonly DependencyPropertyKey Q0PropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Q1), typeof(Size), typeof(Chart),
        new FrameworkPropertyMetadata(default(Size), MetadataOptions));
    public static readonly DependencyProperty Q1Property = Q0PropertyKey.DependencyProperty;

    public Size Q3
    {
        get => (Size)GetValue(Q3Property);
        private set => SetValue(Q2PropertyKey, value);
    }
    private static readonly DependencyPropertyKey Q2PropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Q3), typeof(Size), typeof(Chart),
        new FrameworkPropertyMetadata(default(Size), MetadataOptions));
    public static readonly DependencyProperty Q3Property = Q2PropertyKey.DependencyProperty;

    private void Lines_Changed()
    {
        CalculateBounds();
    }

    private void Points_Changed()
    {
        CalculateBounds();
    }

    private void Origin_Changed()
    {
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        var dataLines = Lines ?? Array.Empty<DataLine>();
        var dataPoints = Points ?? Array.Empty<DataPoint>();

        var dataBounds = dataLines.Aggregate(Enumerable.Empty<Point>(), (points, items) => points.Concat(items.Points))
            .Concat(dataPoints.Select(item => item.Position))
            .Concat(new[] { Origin, new Point(0, 55), new Point(0, -15) })
            .ToList()
            .GetBoundingRect();

        var boundingRect = new[] { Origin, dataBounds.TopLeft, dataBounds.BottomRight }
            .GetBoundingRect();

        DataBounds = dataBounds;

        BoundingRect = new(
            Math.Floor(boundingRect.Left),
            Math.Floor(boundingRect.Top),
            Math.Ceiling(boundingRect.Width),
            Math.Ceiling(boundingRect.Height));

        Q1 = new(
            Math.Max(0, BoundingRect.Right - Origin.X),
            Math.Max(0, BoundingRect.Bottom - Origin.Y));

        Q3 = new(
            - Math.Min(0, BoundingRect.Left - Origin.X),
            - Math.Min(0, BoundingRect.Top - Origin.Y));

        this.VisualDescendants().OfType<ViewportCanvas>().ForEach(item => item.Invalidate());
    }
}