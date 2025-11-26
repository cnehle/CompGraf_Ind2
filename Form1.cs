using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Ind2
{
    public partial class Form1 : Form
    {
        private const int SIZE = 700;
        private bool fill = false;
        private bool fullFigure = false;
        private bool fullFigure2 = false;
        private List<Point> polygon1 = new List<Point>();
        private List<Point> polygon2 = new List<Point>();
        private List<PointF> finalPoints = new List<PointF>();
        private List<PointF> points = new List<PointF>();
        private List<PointF> points2 = new List<PointF>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonClick(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonClick();
            }
            canvas.Invalidate();
        }

        private void LeftButtonClick(int x, int y)
        {
            Point point = new Point(x, y);

            if (!fullFigure)
            {
                if (points.Count == 0 && polygon1.Count == 0)
                {
                    points.Add(new PointF((float)x, (float)y));
                    polygon1.Add(point);
                }
                else
                {
                    var lastPoint = points.Last();
                    points.Add(new PointF((float)x, (float)y));
                    point = new Point(x, y, null, polygon1.Last());
                    polygon1.Last().next = point;
                    polygon1.Add(point);
                }
            }
            else if (!fullFigure2)
            {
                if (points2.Count == 0 && polygon2.Count == 0)
                {
                    points2.Add(new PointF((float)x, (float)y));
                    polygon2.Add(point);
                }
                else
                {
                    var lastPoint = points2.Last();
                    points2.Add(new PointF((float)x, (float)y));
                    point = new Point(x, y, null, polygon2.Last());
                    polygon2.Last().next = point;
                    polygon2.Add(point);
                }
            }
        }

        private void RightButtonClick()
        {
            if (!fullFigure)
            {
                if (points.Count > 2 && polygon1.Count > 2)
                {
                    var lastPoint = points.Last();
                    var firstPoint = points.First();
                    polygon1.Last().next = polygon1.First();
                    polygon1.First().prev = polygon1.Last();
                    fullFigure = true;

                    Console.WriteLine("Polygon 1:");
                    foreach (var p in polygon1)
                        p.Print();
                }
            }
            else if (!fullFigure2)
            {
                if (points2.Count > 2 && polygon2.Count > 2)
                {
                    var lastPoint = points2.Last();
                    var firstPoint = points2.First();
                    polygon2.Last().next = polygon2.First();
                    polygon2.First().prev = polygon2.Last();
                    fullFigure2 = true;

                    Console.WriteLine("Polygon 2:");
                    foreach (var p in polygon2)
                        p.Print();
                }
            }
            canvas.Invalidate();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearWindow();
        }

        private void ClearWindow()
        {
            canvas.Invalidate();
            fullFigure = false;
            fullFigure2 = false;
            points.Clear();
            points2.Clear();
            polygon1.Clear();
            polygon2.Clear();
            finalPoints.Clear();
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            StartAlgorithm();
        }

        private void StartAlgorithm()
        {
            Union();
            if (fill)
            {
                FillPolygon();
            }
            canvas.Invalidate();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (fullFigure && polygon1.Count > 0)
            {
                DrawPolygon(g, polygon1, Pens.Black, Brushes.Transparent);
            }

            if (fullFigure2 && polygon2.Count > 0)
            {
                DrawPolygon(g, polygon2, Pens.Black, Brushes.Transparent);
            }

            if (finalPoints.Count > 0)
            {
                DrawFinalResult(g);
            }

            foreach (var p in points)
            {
                g.FillEllipse(Brushes.Red, p.X - 2, p.Y - 2, 4, 4);
            }
            foreach (var p in points2)
            {
                g.FillEllipse(Brushes.Blue, p.X - 2, p.Y - 2, 4, 4);
            }
        }

        private void DrawPolygon(Graphics g, List<Point> polygon, Pen linePen, Brush fillBrush)
        {
            if (polygon.Count < 3) return;

            PointF[] pointsArray = polygon.Select(p => new PointF((float)p.x, (float)p.y)).ToArray();
            g.DrawPolygon(linePen, pointsArray);
            if (fillBrush != Brushes.Transparent)
            {
                g.FillPolygon(fillBrush, pointsArray);
            }
        }

        private void DrawFinalResult(Graphics g)
        {
            if (finalPoints.Count < 3) return;

            Pen greenPen = new Pen(Color.Green, 2);
            Brush fillBrush = fill ? Brushes.SkyBlue : Brushes.Transparent;

            g.DrawPolygon(greenPen, finalPoints.ToArray());
            if (fill)
            {
                g.FillPolygon(fillBrush, finalPoints.ToArray());
            }

            foreach (var p in finalPoints)
            {
                g.FillEllipse(Brushes.Red, p.X - 3, p.Y - 3, 6, 6);
            }

            greenPen.Dispose();
        }

        private void FillPolygon()
        {

        }

        private PointF? Intersection(Point p1, Point p2, Point p3, Point p4)
        {
            double x1 = p1.x, y1 = p1.y, x2 = p2.x, y2 = p2.y;
            double x3 = p3.x, y3 = p3.y, x4 = p4.x, y4 = p4.y;

            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (p1.Equals(p4) || p1.Equals(p3)) return null;
            if (p2.Equals(p3) || p2.Equals(p4)) return null;

            if (Math.Abs(denom) < 1e-9) return null;

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denom;

            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                double interX = x1 + t * (x2 - x1);
                double interY = y1 + t * (y2 - y1);
                return new PointF((float)interX, (float)interY);
            }
            return null;
        }

        private bool FindSide(Point p1, Point p2, double x, double y)
        {
            double xa = p2.x - p1.x;
            double ya = p2.y - p1.y;
            x -= p1.x;
            y -= p1.y;
            return (y * xa - x * ya) > 0;
        }

        private Point FindLeftestPoint(List<Point> curPolygon)
        {
            Point leftestPoint = new Point(1000000, -1000000);
            foreach (var point in curPolygon)
            {
                if (point.x < leftestPoint.x ||
                    (Math.Abs(point.x - leftestPoint.x) < 1e-9 && point.y > leftestPoint.y))
                {
                    leftestPoint = point;
                }
            }
            return leftestPoint;
        }

        private bool IsPointInPolygon(Point point, List<Point> polygon)
        {
            foreach (var p in polygon)
            {
                if (p.Equals(point))
                    return true;
            }
            return false;
        }

        private void Union()
        {
            finalPoints.Clear();

            // Если полигоны не пересекаются и не имеют общих вершин, выбираем левый
            if (!HasIntersections() && !HasCommonVertices())
            {
                Point leftest1 = FindLeftestPoint(polygon1);
                Point leftest2 = FindLeftestPoint(polygon2);

                List<Point> resultPolygon = leftest1.x < leftest2.x ? polygon1 : polygon2;

                foreach (var point in resultPolygon)
                {
                    finalPoints.Add(new PointF((float)point.x, (float)point.y));
                }
                return;
            }

            // Начинаем с левого полигона
            Point left1 = FindLeftestPoint(polygon1);
            Point left2 = FindLeftestPoint(polygon2);

            List<Point> currentPolygon = left1.x < left2.x ? polygon1 : polygon2;
            List<Point> otherPolygon = left1.x < left2.x ? polygon2 : polygon1;

            Point current = FindLeftestPoint(currentPolygon);
            Point start = current;

            // Используем словарь для отслеживания, сколько раз посетили каждую точку
            Dictionary<Point, int> visitCount = new Dictionary<Point, int>();
            bool started = false;

            int safetyCounter = 0;
            int maxSteps = (polygon1.Count + polygon2.Count) * 3;

            while (safetyCounter++ < maxSteps)
            {
                // Добавляем текущую точку в результат (если еще не добавили слишком много раз)
                if (!visitCount.ContainsKey(current) || visitCount[current] < 2)
                {
                    finalPoints.Add(new PointF((float)current.x, (float)current.y));

                    if (!visitCount.ContainsKey(current))
                        visitCount[current] = 1;
                    else
                        visitCount[current]++;
                }

                Point next = current.next;

                // 1. Проверяем пересечения текущего ребра
                PointF? closestIntersection = null;
                Point intersectionOtherPoint = null;
                double minDistance = double.MaxValue;

                foreach (var edge in GetEdges(otherPolygon))
                {
                    PointF? intersection = Intersection(current, next, edge.Item1, edge.Item2);
                    if (intersection.HasValue)
                    {
                        double dist = Distance(current.x, current.y, intersection.Value.X, intersection.Value.Y);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            closestIntersection = intersection;
                            intersectionOtherPoint = FindSide(current, next, edge.Item1.x, edge.Item1.y) ? edge.Item2 : edge.Item1;
                        }
                    }
                }

                // 2. Если есть пересечение - переключаемся
                if (closestIntersection.HasValue)
                {
                    Point intersectionPoint = new Point(closestIntersection.Value.X, closestIntersection.Value.Y);

                    // Добавляем точку пересечения (если еще не добавили слишком много раз)
                    if (!visitCount.ContainsKey(intersectionPoint) || visitCount[intersectionPoint] < 2)
                    {
                        finalPoints.Add(closestIntersection.Value);

                        if (!visitCount.ContainsKey(intersectionPoint))
                            visitCount[intersectionPoint] = 1;
                        else
                            visitCount[intersectionPoint]++;
                    }

                    // Переключаем полигоны
                    var temp = currentPolygon;
                    currentPolygon = otherPolygon;
                    otherPolygon = temp;

                    current = intersectionOtherPoint;
                }
                // 3. Если пересечений нет, но следующая точка общая - переключаемся
                else if (IsPointInPolygon(next, otherPolygon))
                {
                    // Находим общую точку в другом полигоне
                    Point commonPoint = otherPolygon.First(p => p.Equals(next));

                    // Переключаем полигоны только если мы еще не посещали эту точку слишком много раз
                    if (!visitCount.ContainsKey(commonPoint) || visitCount[commonPoint] < 2)
                    {
                        var temp = currentPolygon;
                        currentPolygon = otherPolygon;
                        otherPolygon = temp;

                        current = commonPoint;
                    }
                    else
                    {
                        // Если уже посещали эту точку 2 раза, продолжаем по текущему полигону
                        current = next;
                    }
                }
                else
                {
                    // 4. Продолжаем по текущему полигону
                    current = next;
                }

                // Проверяем завершение (вернулись в начальную точку И уже начали обход)
                if (current.Equals(start) && started)
                {
                    // Дополнительная проверка: если мы в начальной точке и она общая,
                    // даем ей возможность быть обработанной дважды
                    if (IsPointInPolygon(start, otherPolygon) &&
                        (!visitCount.ContainsKey(start) || visitCount[start] < 2))
                    {
                        // Продолжаем, чтобы обработать начальную точку во втором полигоне
                    }
                    else
                    {
                        break;
                    }
                }

                started = true;
            }

            // Удаляем возможные дубликаты в конце (если начальная точка добавилась дважды)
            if (finalPoints.Count > 2 && finalPoints[0].Equals(finalPoints[finalPoints.Count - 1]))
            {
                finalPoints.RemoveAt(finalPoints.Count - 1);
            }

            canvas.Invalidate();
        }

        private List<(Point, Point)> GetEdges(List<Point> polygon)
        {
            var edges = new List<(Point, Point)>();
            foreach (var point in polygon)
            {
                edges.Add((point, point.next));
            }
            return edges;
        }

        private bool HasIntersections()
        {
            foreach (var edge1 in GetEdges(polygon1))
            {
                foreach (var edge2 in GetEdges(polygon2))
                {
                    if (Intersection(edge1.Item1, edge1.Item2, edge2.Item1, edge2.Item2).HasValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasCommonVertices()
        {
            foreach (var p1 in polygon1)
            {
                foreach (var p2 in polygon2)
                {
                    if (p1.Equals(p2))
                        return true;
                }
            }
            return false;
        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public class Point
        {
            public double x { get; set; }
            public double y { get; set; }
            public Point next { get; set; }
            public Point prev { get; set; }

            public Point(double x, double y, Point next = null, Point prev = null)
            {
                this.x = x;
                this.y = y;
                this.next = next;
                this.prev = prev;
            }

            public void Print()
            {
                Console.WriteLine($"({x}, {y}) next: {next?.x},{next?.y} prev: {prev?.x},{prev?.y}");
            }

            public override bool Equals(object obj)
            {
                if (obj is Point p)
                    return Math.Abs(p.x - x) < 1e-4 && Math.Abs(p.y - y) < 1e-4;
                return false;
            }

            public override int GetHashCode()
            {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }
    }
}