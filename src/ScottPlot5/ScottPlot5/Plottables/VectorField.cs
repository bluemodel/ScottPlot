﻿using ScottPlot.Interfaces;

namespace ScottPlot.Plottables;

public class VectorField(IVectorFieldSource source) : IPlottable, IHasArrow
{
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();
    public string? Label { get; set; }

    public ArrowStyle ArrowStyle { get; set; } = new();
    public ArrowAnchor ArrowAnchor { get => ArrowStyle.Anchor; set => ArrowStyle.Anchor = value; }
    public Color ArrowColor { get => ArrowStyle.LineStyle.Color; set => ArrowStyle.LineStyle.Color = value; }
    public float ArrowLineWidth { get => ArrowStyle.LineStyle.Width; set => ArrowStyle.LineStyle.Width = value; }

    public IColormap? Colormap { get; set; } = null;

    public IEnumerable<LegendItem> LegendItems => EnumerableExtensions.One(
        new LegendItem
        {
            Label = Label,
            Marker = MarkerStyle.None,
            Line = ArrowStyle.LineStyle,
            HasArrow = true
        });

    IVectorFieldSource Source { get; set; } = source;

    public AxisLimits GetAxisLimits() => Source.GetLimits();

    public void Render(RenderPack rp)
    {
        if (!IsVisible)
            return;

        float maxLength = 25;

        // TODO: Filter out those that are off-screen? This is subtle, an arrow may be fully off-screen except for its arrowhead, if the blades are long enough.
        var vectors = Source.GetRootedVectors().Select(v => new RootedPixelVector(Axes.GetPixel(v.Point), v.Vector)).ToArray();
        if (vectors.Length == 0)
            return;

        var minMagnitudeSquared = double.PositiveInfinity;
        var maxMagnitudeSquared = double.NegativeInfinity;
        foreach (var v in vectors)
        {
            var magSquared = v.MagnitudeSquared;
            minMagnitudeSquared = Math.Min(minMagnitudeSquared, magSquared);
            maxMagnitudeSquared = Math.Max(maxMagnitudeSquared, magSquared);
        }

        var range = new Range(Math.Sqrt(minMagnitudeSquared), Math.Sqrt(maxMagnitudeSquared));
        if (range.Min == range.Max)
        {
            range = new Range(0, range.Max);
        }

        for (int i = 0; i < vectors.Length; i++)
        {
            var oldMagnitude = vectors[i].Magnitude;
            var newMagnitude = range.Normalize(oldMagnitude) * maxLength;

            var direction = vectors[i].Angle;
            vectors[i].Vector = new((float)(Math.Cos(direction) * newMagnitude), (float)(Math.Sin(direction) * newMagnitude));
        }

        double minPixelMag = Math.Sqrt(vectors.Select(x => x.MagnitudeSquared).Min());
        double maxPixelMag = Math.Sqrt(vectors.Select(x => x.MagnitudeSquared).Max());
        Range pixelMagRange = new(minPixelMag, maxPixelMag);

        using SKPaint paint = new();
        ArrowStyle.LineStyle.ApplyToPaint(paint);
        paint.Style = SKPaintStyle.StrokeAndFill;

        if (Colormap is not null)
        {
            var coloredVectors = vectors.ToLookup(v => Colormap.GetColor(v.Magnitude, pixelMagRange));

            foreach (var group in coloredVectors)
            {
                paint.Color = group.Key.ToSKColor();
                RenderVectors(paint, rp.Canvas, group, ArrowStyle);
            }
        }
        else
        {
            RenderVectors(paint, rp.Canvas, vectors, ArrowStyle);
        }
    }

    private static void RenderVectors(SKPaint paint, SKCanvas canvas, IEnumerable<RootedPixelVector> vectors, ArrowStyle arrowStyle)
    {
        using SKPath path = PathStrategies.Arrows.GetPath(vectors, arrowStyle);
        canvas.DrawPath(path, paint);
    }
}
