using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PixelCircle
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Pan = false;
        Point Origin, MouseOrigin;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int size = int.Parse(tb1.Text);
            DrawCircle(size - 1, (bool)checkbox1.IsChecked);
        }

        private void DrawCircle(int size, bool axis)
        {
            var points = CalcCirclePoint(size);

            if (Canvas1.ActualHeight < 5 * size + 10)
            {
                Canvas1.Height = 5 * size + 10;
                Canvas1.UpdateLayout();
            }
            if (Canvas1.ActualWidth < 5 * size + 10)
            {
                Canvas1.Width = 5 * size + 10;
                Canvas1.UpdateLayout();
            }

            int offX = (int)Canvas1.ActualWidth / 2;
            int offY = (int)Canvas1.ActualHeight / 2;

            Canvas1.Children.Clear();

            if (axis)
                DrawAxis(offX, offY, 0, (int)Canvas1.ActualWidth, 0, (int)Canvas1.ActualHeight, 5);

            DrawPoints(points, 5, offX, offY);
            Canvas1.Offset = new Point(0, 0);
        }

        private void DrawAxis(int centerX, int centerY, int X1, int X2, int Y1, int Y2, int size)
        {
            Line temp;
            for (int i = 0; i * size * 5 + centerX < X2; i++)
            {
                temp = new Line()
                {
                    Y1 = Y1,
                    Y2 = Y2,
                    X1 = i * size * 5 + centerX,
                    X2 = i * size * 5 + centerX,
                    Name = "Yp" + i.ToString(),
                    Stroke = i % 2 == 0 ? Brushes.Black : Brushes.DarkRed,
                    StrokeThickness = 1,
                    ToolTip = i * 5
                };
                Canvas1.Children.Add(temp);
            }
            for (int i = 0; i * size * 5 + centerX > X1; i--)
            {
                temp = new Line()
                {
                    Y1 = Y1,
                    Y2 = Y2,
                    X1 = i * size * 5 + centerX,
                    X2 = i * size * 5 + centerX,
                    Name = "Yn" + i.Abs().ToString(),
                    Stroke = i % 2 == 0 ? Brushes.Black : Brushes.DarkRed,
                    StrokeThickness = 1,
                    ToolTip = i * 5
                };
                Canvas1.Children.Add(temp);
            }
            for (int i = 0; i * size * 5 + centerY > Y1; i--)
            {
                temp = new Line()
                {
                    Y1 = i * size * 5 + centerY,
                    Y2 = i * size * 5 + centerY,
                    X1 = X1,
                    X2 = X2,
                    Name = "Xn" + i.Abs().ToString(),
                    Stroke = i % 2 == 0 ? Brushes.Black : Brushes.DarkRed,
                    StrokeThickness = 1,
                    ToolTip = -i * 5
                };
                Canvas1.Children.Add(temp);
            }
            for (int i = 0; i * size * 5 + centerY < Y2; i++)
            {
                temp = new Line()
                {
                    Y1 = i * size * 5 + centerY,
                    Y2 = i * size * 5 + centerY,
                    X1 = X1,
                    X2 = X2,
                    Name = "Xp" + i.ToString(),
                    Stroke = i % 2 == 0 ? Brushes.Black : Brushes.DarkRed,
                    StrokeThickness = 1,
                    ToolTip = -i * 5
                };
                Canvas1.Children.Add(temp);
            }
        }

        private void DrawPoints(List<Vector> points, uint size, double offX, double offY)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var rect = NewRect(points[i].X, points[i].Y, size, "P" + i.ToString(), (int)offX, (int)offY);
                Canvas1.Children.Add(rect);
            }
        }

        Rectangle NewRect(double x, double y, uint size, string name, int offsetX, int offsetY)
        {
            Rectangle rec = new Rectangle()
            {
                Margin = new Thickness(x * size + offsetX - size / 2, y * size + offsetY - size / 2, 0, 0),
                Width = size,
                Height = size,
                Fill = Brushes.DarkGray,
                Name = name,
                Stroke = Brushes.DimGray,
                StrokeThickness = 1,
                ToolTip = "(" + x.Round().ToString() + ", " + (-y.Round()).ToString() + ")",
            };
            return rec;
        }


        List<Vector> CalcCirclePoint(int d)
        {
            double r = d / 2.0d;
            bool type = d % 2 == 0;

            List<Vector> points = new List<Vector>();

            double x = r, y = type ? 0 : 0.5;
            points.Add(new Vector(x, y));

            while (x > y)
            {
                y += 1;
                var d1 = getDiff(x, y, r);
                var d2 = getDiff(x - 1, y, r);
                x = d1 <= d2 ? x : x - 1;

                points.Add(new Vector(x, y));
            }
            return ExtendCirclePoint(points);
        }

        List<Vector> ExtendCirclePoint(List<Vector> orig)
        {
            List<Vector> ext = new List<Vector>();
            foreach (var item in orig)
            {
                ext.Add(item);

                Vector sp = new Vector(item.Y, item.X);
                if (!sp.Equals(item)) ext.Add(sp);
            }

            orig = ext;

            ext = new List<Vector>();
            foreach (var item in orig)
            {
                ext.Add(item);

                Vector sp = new Vector(-item.X, item.Y);
                if (!sp.Equals(item)) ext.Add(sp);
            }
            orig = ext;

            ext = new List<Vector>();
            foreach (var item in orig)
            {
                ext.Add(item);

                Vector sp = new Vector(item.X, -item.Y);
                if (!sp.Equals(item)) ext.Add(sp);
            }
            return ext;
        }

        double getDiff(double x, double y, double R)
        {
            return Math.Abs(x * x + y * y - R * R);
        }

        private void Canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var o = Canvas1.Offset;
            var m = e.GetPosition(Canvas1);
            var p = new Vector(m.X, m.Y);

            Canvas1.Scale *= e.Delta > 0 ? 1.25d : 0.8d;

            p *= e.Delta > 0 ? 0.2d : -0.25d; //画布点移动距离
            p *= Canvas1.Scale;  //实际窗口点移动距离

            Canvas1.Offset = o + p;
        }

        private void Canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pan = true;

            MouseOrigin = e.GetPosition(DockPanel1);
            Origin = Canvas1.Offset;
            var CanvasOrigin = e.GetPosition(Canvas1);
        }

        private void Canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Pan)
            {
                var b = e.GetPosition(DockPanel1);
                Canvas1.Offset = new Point(Origin.X - b.X + MouseOrigin.X, Origin.Y - b.Y + MouseOrigin.Y);
            }
        }

        private void Canvas1_MouseLeave(object sender, MouseEventArgs e)
        {
            Pan = false;
        }

        private void Canvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Pan = false;
        }
    }
}
