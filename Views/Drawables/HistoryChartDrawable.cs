using Microsoft.Maui.Graphics;

namespace QuitSmoke.Views.Drawables;

public class HistoryChartDrawable : IDrawable
{
    public IList<int> Counts { get; set; } = new List<int>();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.Antialias = true;

        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // Margins
        float left = 20;
        float right = 10;
        float top = 10;
        float bottom = 20;

        var plotWidth = width - left - right;
        var plotHeight = height - top - bottom;
        if (plotWidth <= 0 || plotHeight <= 0)
        {
            canvas.RestoreState();
            return;
        }

        // Background
        canvas.FillColor = Color.FromArgb("#F5F5F5");
        canvas.FillRectangle(dirtyRect);

        // Axes lines
        var axisColor = Color.FromArgb("#DDDDDD");
        canvas.StrokeColor = axisColor;
        canvas.StrokeSize = 1;
        // X axis
        canvas.DrawLine(left, height - bottom, width - right, height - bottom);
        // Y axis
        canvas.DrawLine(left, top, left, height - bottom);

        if (Counts == null || Counts.Count == 0)
        {
            canvas.RestoreState();
            return;
        }

        var max = Counts.Max();
        if (max == 0) max = 1;

        // Points
        int n = Counts.Count;
        float stepX = n > 1 ? plotWidth / (n - 1) : plotWidth;

        var points = new List<PointF>(n);
        for (int i = 0; i < n; i++)
        {
            float x = left + stepX * i;
            float y = top + plotHeight * (1 - (Counts[i] / (float)max));
            points.Add(new PointF(x, y));
        }

        // Draw line
        canvas.StrokeColor = Color.FromArgb("#4CAF50");
        canvas.StrokeSize = 3;
        for (int i = 1; i < points.Count; i++)
        {
            canvas.DrawLine(points[i - 1], points[i]);
        }

        // Draw points
        foreach (var p in points)
        {
            canvas.FillColor = Colors.White;
            canvas.FillCircle(p, 4);
            canvas.StrokeColor = Color.FromArgb("#4CAF50");
            canvas.StrokeSize = 2;
            canvas.DrawCircle(p, 4);
        }

        canvas.RestoreState();
    }
}
