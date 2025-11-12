using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Ind2
{
    public partial class Form1 : Form
    {
        private List<PointF> polygon1 = new List<PointF>();
        private List<PointF> polygon2 = new List<PointF>();
        private List<PointF> finalPoints = new List<PointF>();
        private List<PointF> currentPoints = new List<PointF>();

        private bool polygon1Complete = false;
        private bool polygon2Complete = false;
        private bool drawingPolygon1 = true;

        private Pen polygon1Pen = new Pen(Color.Blue, 2);
        private Pen polygon2Pen = new Pen(Color.Red, 2);
        private Pen resultPen = new Pen(Color.Green, 3);

        private Brush polygon1Brush = new SolidBrush(Color.Blue);
        private Brush polygon2Brush = new SolidBrush(Color.Red);
        private Brush resultBrush = new SolidBrush(Color.Orange);
        private Brush intersectionBrush = new SolidBrush(Color.Green);

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            mainPanel.Paint += MainPanel_Paint;
            mainPanel.MouseClick += MainPanel_MouseClick;
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Рисуем полигон 1
            if (polygon1.Count > 1)
            {
                if (polygon1Complete)
                {
                    g.DrawPolygon(polygon1Pen, polygon1.ToArray());
                }
                else
                {
                    g.DrawLines(polygon1Pen, polygon1.ToArray());
                }

                // Рисуем точки полигона 1
                foreach (var point in polygon1)
                {
                    g.FillEllipse(polygon1Brush, point.X - 2, point.Y - 2, 4, 4);
                }
            }

            // Рисуем полигон 2
            if (polygon2.Count > 1)
            {
                if (polygon2Complete)
                {
                    g.DrawPolygon(polygon2Pen, polygon2.ToArray());
                }
                else
                {
                    g.DrawLines(polygon2Pen, polygon2.ToArray());
                }

                // Рисуем точки полигона 2
                foreach (var point in polygon2)
                {
                    g.FillEllipse(polygon2Brush, point.X - 2, point.Y - 2, 4, 4);
                }
            }

            // Рисуем результат
            if (finalPoints.Count > 1)
            {
                // Рисуем линии
                g.DrawPolygon(resultPen, finalPoints.ToArray());

                // Рисуем точки
                for (int i = 0; i < finalPoints.Count; i++)
                {
                    var point = finalPoints[i];
                    g.FillEllipse(resultBrush, point.X - 4, point.Y - 4, 8, 8);

                    // Подписываем точки
                    string label = i.ToString();
                    var labelSize = g.MeasureString(label, this.Font);
                    g.DrawString(label, this.Font, Brushes.Black,
                        point.X - labelSize.Width / 2, point.Y - 20);
                }
            }
        }

        private void MainPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!polygon1Complete)
                {
                    polygon1.Add(new PointF(e.X, e.Y));
                    drawingPolygon1 = true;
                    currentPoints = polygon1;
                }
                else if (!polygon2Complete)
                {
                    polygon2.Add(new PointF(e.X, e.Y));
                    drawingPolygon1 = false;
                    currentPoints = polygon2;
                }

                mainPanel.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (!polygon1Complete && polygon1.Count > 2)
                {
                    polygon1Complete = true;
                    MessageBox.Show("Полигон 1 завершен");
                }
                else if (!polygon2Complete && polygon2.Count > 2)
                {
                    polygon2Complete = true;
                    MessageBox.Show("Полигон 2 завершен");
                }

                mainPanel.Invalidate();
            }
        }

        private double Rotate(PointF a, PointF b, PointF c)
        {
            return (b.X - a.X) * (c.Y - b.Y) - (b.Y - a.Y) * (c.X - b.X);
        }

        private PointF? FindLineIntersection(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            float denominator = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);

            if (Math.Abs(denominator) < 1e-10)
                return null;

            float x = ((p1.X * p2.Y - p1.Y * p2.X) * (p3.X - p4.X) -
                      (p1.X - p2.X) * (p3.X * p4.Y - p3.Y * p4.X)) / denominator;
            float y = ((p1.X * p2.Y - p1.Y * p2.X) * (p3.Y - p4.Y) -
                      (p1.Y - p2.Y) * (p3.X * p4.Y - p3.Y * p4.X)) / denominator;

            // Проверяем, что точка находится на отрезках
            if (x < Math.Min(p1.X, p2.X) - 1e-5 || x > Math.Max(p1.X, p2.X) + 1e-5 ||
                x < Math.Min(p3.X, p4.X) - 1e-5 || x > Math.Max(p3.X, p4.X) + 1e-5 ||
                y < Math.Min(p1.Y, p2.Y) - 1e-5 || y > Math.Max(p1.Y, p2.Y) + 1e-5 ||
                y < Math.Min(p3.Y, p4.Y) - 1e-5 || y > Math.Max(p3.Y, p4.Y) + 1e-5)
                return null;

            return new PointF(x, y);
        }

        private List<PointF> FindAllIntersections()
        {
            List<PointF> intersections = new List<PointF>();

            for (int i = 0; i < polygon1.Count; i++)
            {
                PointF p1 = polygon1[i];
                PointF p2 = polygon1[(i + 1) % polygon1.Count];

                for (int j = 0; j < polygon2.Count; j++)
                {
                    PointF p3 = polygon2[j];
                    PointF p4 = polygon2[(j + 1) % polygon2.Count];

                    PointF? intersection = FindLineIntersection(p1, p2, p3, p4);
                    if (intersection.HasValue)
                    {
                        intersections.Add(intersection.Value);
                    }
                }
            }

            return intersections;
        }

        private bool PointInPolygon(PointF point, List<PointF> polygon)
        {
            int n = polygon.Count;
            bool inside = false;

            PointF p1 = polygon[0];
            for (int i = 1; i <= n; i++)
            {
                PointF p2 = polygon[i % n];
                if (point.Y > Math.Min(p1.Y, p2.Y))
                {
                    if (point.Y <= Math.Max(p1.Y, p2.Y))
                    {
                        if (point.X <= Math.Max(p1.X, p2.X))
                        {
                            if (p1.Y != p2.Y)
                            {
                                float xIntersection = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                                if (p1.X == p2.X || point.X <= xIntersection)
                                {
                                    inside = !inside;
                                }
                            }
                        }
                    }
                }
                p1 = p2;
            }

            return inside;
        }

        private bool PolygonContainsPolygon(List<PointF> container, List<PointF> contained)
        {
            foreach (var point in contained)
            {
                if (!PointInPolygon(point, container))
                    return false;
            }
            return true;
        }

        private List<PointF> GrahamScan(List<PointF> points)
        {
            if (points.Count < 3)
                return points;

            // Находим самую нижнюю левую точку
            PointF start = points.OrderBy(p => p.Y).ThenBy(p => p.X).First();

            // Сортируем точки по полярному углу
            var sortedPoints = points.Where(p => !p.Equals(start))
                                   .OrderBy(p => Math.Atan2(p.Y - start.Y, p.X - start.X))
                                   .ToList();

            List<PointF> hull = new List<PointF> { start };
            if (sortedPoints.Count > 0)
                hull.Add(sortedPoints[0]);

            for (int i = 1; i < sortedPoints.Count; i++)
            {
                while (hull.Count > 1 &&
                       Rotate(hull[hull.Count - 2], hull[hull.Count - 1], sortedPoints[i]) <= 0)
                {
                    hull.RemoveAt(hull.Count - 1);
                }
                hull.Add(sortedPoints[i]);
            }

            return hull;
        }

        private List<PointF> MergeConvexHull(List<PointF> points1, List<PointF> points2)
        {
            List<PointF> allPoints = new List<PointF>();
            allPoints.AddRange(points1);
            allPoints.AddRange(points2);

            // Убираем дубликаты
            var uniquePoints = allPoints.Distinct(new PointFComparer()).ToList();

            return GrahamScan(uniquePoints);
        }

        private void UnionPolygons()
        {
            finalPoints.Clear();

            if (polygon1.Count == 0 || polygon2.Count == 0)
                return;

            List<PointF> intersections = FindAllIntersections();

            bool poly1ContainsPoly2 = PolygonContainsPolygon(polygon1, polygon2);
            bool poly2ContainsPoly1 = PolygonContainsPolygon(polygon2, polygon1);

            if (poly1ContainsPoly2)
            {
                finalPoints = polygon1;
            }
            else if (poly2ContainsPoly1)
            {
                finalPoints = polygon2;
            }
            else if (intersections.Count == 0)
            {
                finalPoints = MergeConvexHull(polygon1, polygon2);
            }
            else
            {
                List<PointF> allPoints = new List<PointF>();
                allPoints.AddRange(intersections);

                foreach (var point in polygon1)
                {
                    if (!PointInPolygon(point, polygon2))
                    {
                        allPoints.Add(point);
                    }
                }

                foreach (var point in polygon2)
                {
                    if (!PointInPolygon(point, polygon1))
                    {
                        allPoints.Add(point);
                    }
                }

                if (allPoints.Count > 0)
                {
                    float centerX = allPoints.Average(p => p.X);
                    float centerY = allPoints.Average(p => p.Y);

                    allPoints = allPoints.OrderBy(p => Math.Atan2(p.Y - centerY, p.X - centerX)).ToList();
                    finalPoints = allPoints;
                }
            }

            mainPanel.Invalidate();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            polygon1.Clear();
            polygon2.Clear();
            finalPoints.Clear();
            currentPoints.Clear();
            polygon1Complete = false;
            polygon2Complete = false;
            drawingPolygon1 = true;
            mainPanel.Invalidate();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (!polygon1Complete || !polygon2Complete)
            {
                MessageBox.Show("Завершите оба полигона перед вычислением объединения");
                return;
            }

            UnionPolygons();
        }
    }

    public class PointFComparer : IEqualityComparer<PointF>
    {
        public bool Equals(PointF p1, PointF p2)
        {
            return Math.Abs(p1.X - p2.X) < 1e-5 && Math.Abs(p1.Y - p2.Y) < 1e-5;
        }

        public int GetHashCode(PointF p)
        {
            return (Math.Round(p.X, 5).GetHashCode() ^ Math.Round(p.Y, 5).GetHashCode());
        }
    }
}