﻿namespace ScottPlot.Plottables;

public class Text : LabelStyleProperties, IPlottable
{
    public override Label LabelStyle { get; } = new();
    public Coordinates Location { get; set; }
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }

    #region obsolete

    [Obsolete("Interact properties in this class (e.g., LabelFontColor) or properties of LabelStyle", true)]
    public readonly Label Label = null!;

    [Obsolete("Use LabelFontColor", true)]
    public Color Color { get => LabelFontColor; set => LabelFontColor = value; }

    [Obsolete("Use LabelFontColor", true)]
    public Color FontColor { get => LabelBackgroundColor; set => LabelFontColor = value; }

    [Obsolete("Use LabelBackgroundColor", true)]
    public Color BackgroundColor { get => LabelBackgroundColor; set => LabelBackgroundColor = value; }

    [Obsolete("use LabelBackgroundColor", true)]
    public Color BackColor { get; set; }

    [Obsolete("use LabelBackgroundColor", true)]
    public Color BorderColor { get => LabelBorderColor; set => LabelBorderColor = value; }

    [Obsolete("use LabelBorderWidth", true)]
    public float BorderWidth { get => LabelBorderWidth; set => LabelBorderWidth = value; }

    [Obsolete("use LabelPadding or LabelPixelPadding", true)]
    public float Padding { set => LabelPadding = value; }

    [Obsolete("use LabelFontSize", true)]
    public float Size { get => LabelFontSize; set => LabelFontSize = value; }

    [Obsolete("use LabelFontSize", true)]
    public float FontSize { get => LabelFontSize; set => LabelFontSize = value; }

    [Obsolete("use LabelBold", true)]
    public bool Bold { get => LabelBold; set => LabelBold = value; }

    [Obsolete("use LabelRotation", true)]
    public float Rotation { get => LabelRotation; set => LabelRotation = value; }

    [Obsolete("use LabelFontName", true)]
    public string FontName { get => LabelFontName; set => LabelFontName = value; }

    [Obsolete("use LineSpacing", true)]
    public float? LineSpacing { get => LabelLineSpacing; set => LabelLineSpacing = value; }

    #endregion

    public AxisLimits GetAxisLimits()
    {
        return new AxisLimits(Location);
    }

    public void Render(RenderPack rp)
    {
        Pixel pixelLocation = Axes.GetPixel(Location);
        pixelLocation.X += OffsetX;
        pixelLocation.Y += OffsetY;
        LabelStyle.Render(rp.Canvas, pixelLocation);
    }
}
