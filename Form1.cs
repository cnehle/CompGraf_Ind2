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
        private List<PointF> currentPoints = new List<PointF>();
        private List<PointF> unionPoints = new List<PointF>();
        private bool isFirstPolygonComplete = false;
        private bool isSecondPolygonComplete = false;

        public Form1()
        {
            InitializeComponent();
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (!isFirstPolygonComplete)
                lblStatus.Text = "Рисуйте первый полигон (ЛКМ - добавить точку, ПКМ - завершить)";
            else if (!isSecondPolygonComplete)
                lblStatus.Text = "Рисуйте второй полигон (ЛКМ - добавить точку, ПКМ - завершить)";
            else
                lblStatus.Text = "Оба полигона нарисованы. Нажмите 'Объединить'";
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (isFirstPolygonComplete && isSecondPolygonComplete) return;

            if (e.Button == MouseButtons.Left)
            {
                currentPoints.Add(new PointF(e.X, e.Y));
                pictureBox.Invalidate();
            }
            else if (e.Button == MouseButtons.Right && currentPoints.Count >= 3)
            {
                if (!isFirstPolygonComplete)
                {
                    polygon1 = new List<PointF>(currentPoints);
                    isFirstPolygonComplete = true;
                }
                else
                {
                    polygon2 = new List<PointF>(currentPoints);
                    isSecondPolygonComplete = true;
                }
                currentPoints.Clear();
                UpdateStatus();
                pictureBox.Invalidate();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            polygon1.Clear();
            polygon2.Clear();
            currentPoints.Clear();
            unionPoints.Clear();
            isFirstPolygonComplete = false;
            isSecondPolygonComplete = false;
            pictureBox.Invalidate();
            UpdateStatus();
        }

        private void btnUnion_Click(object sender, EventArgs e)
        {
            if (polygon1.Count < 3 || polygon2.Count < 3)
            {
                MessageBox.Show("Оба полигона должны быть завершены", "Ошибка");
                return;
            }

            unionPoints = CalculateUnion();
            pictureBox.Invalidate();
        }

        private float Cross(PointF a, PointF b, PointF c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        private PointF? FindIntersection(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            float d = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
            if (d == 0) return null;

            float ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / d;
            float ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / d;

            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            {
                return new PointF(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
            }
            return null;
        }

        private List<PointF> CalculateUnion()
        {
            List<PointF> result = new List<PointF>();
            HashSet<string> used = new HashSet<string>();

            // Начинаем с самой левой точки
            PointF start = polygon1.Concat(polygon2).OrderBy(p => p.X).ThenBy(p => p.Y).First();
            result.Add(start);
            used.Add($"{start.X},{start.Y}");

            List<PointF> currentPoly = polygon1.Contains(start) ? polygon1 : polygon2;
            List<PointF> otherPoly = currentPoly == polygon1 ? polygon2 : polygon1;

            int currentIndex = currentPoly.IndexOf(start);
            int steps = 0;

            while (steps < 1000)
            {
                steps++;
                int nextIndex = (currentIndex + 1) % currentPoly.Count;
                PointF current = currentPoly[currentIndex];
                PointF next = currentPoly[nextIndex];

                // Ищем пересечения
                PointF? closestIntersection = null;
                int closestOtherIndex = -1;
                float minDist = float.MaxValue;

                for (int i = 0; i < otherPoly.Count; i++)
                {
                    PointF a = otherPoly[i];
                    PointF b = otherPoly[(i + 1) % otherPoly.Count];
                    PointF? intersection = FindIntersection(current, next, a, b);

                    if (intersection.HasValue)
                    {
                        float dist = Distance(current, intersection.Value);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            closestIntersection = intersection;
                            closestOtherIndex = i;
                        }
                    }
                }

                if (closestIntersection.HasValue)
                {
                    // Добавляем точку пересечения
                    string key = $"{closestIntersection.Value.X},{closestIntersection.Value.Y}";
                    if (!used.Contains(key))
                    {
                        result.Add(closestIntersection.Value);
                        used.Add(key);
                    }

                    // Переключаем полигоны и находим следующую точку по часовой стрелке
                    var temp = currentPoly;
                    currentPoly = otherPoly;
                    otherPoly = temp;

                    // Находим следующую точку по часовой стрелке в новом полигоне
                    // Используем индекс ребра, которое пересеклось
                    currentIndex = (closestOtherIndex + 1) % currentPoly.Count;
                }
                else
                {
                    // Добавляем следующую точку
                    string key = $"{next.X},{next.Y}";
                    if (!used.Contains(key))
                    {
                        result.Add(next);
                        used.Add(key);
                    }
                    currentIndex = nextIndex;
                }

                // Проверка завершения
                if (result.Count > 1 && result[0] == result[result.Count - 1])
                    break;
            }

            return result;
        }

        private float Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Рисуем полигоны
            if (polygon1.Count > 0)
            {
                g.FillPolygon(new SolidBrush(Color.FromArgb(50, Color.Blue)), polygon1.ToArray());
                g.DrawPolygon(Pens.Blue, polygon1.ToArray());
            }

            if (polygon2.Count > 0)
            {
                g.FillPolygon(new SolidBrush(Color.FromArgb(50, Color.Green)), polygon2.ToArray());
                g.DrawPolygon(Pens.Green, polygon2.ToArray());
            }

            // Рисуем текущие точки
            if (currentPoints.Count > 0)
            {
                for (int i = 0; i < currentPoints.Count; i++)
                {
                    g.FillEllipse(Brushes.Black, currentPoints[i].X - 3, currentPoints[i].Y - 3, 6, 6);
                    if (i > 0)
                        g.DrawLine(Pens.Black, currentPoints[i - 1], currentPoints[i]);
                }
            }

            // Рисуем объединение
            if (unionPoints.Count > 0)
            {
                g.FillPolygon(new SolidBrush(Color.FromArgb(100, Color.Yellow)), unionPoints.ToArray());
                g.DrawPolygon(new Pen(Color.Red, 3), unionPoints.ToArray());
            }
        }
    }
}