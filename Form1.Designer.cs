using System.Drawing;
using System.Windows.Forms;

namespace Ind2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private PictureBox canvas;
        private Button clearButton;
        private Button goButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form1
            this.Text = "Объединение выпуклых оболочек";
            this.Size = new Size(SIZE + 200, SIZE + 100);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Canvas
            canvas = new PictureBox
            {
                Width = SIZE,
                Height = SIZE,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new System.Drawing.Point(10, 10)
            };
            canvas.Paint += Canvas_Paint;
            canvas.MouseClick += Canvas_MouseClick;
            this.Controls.Add(canvas);

            // Clear button
            clearButton = new Button
            {
                Text = "Clear",
                Location = new System.Drawing.Point(SIZE + 20, 20),
                Size = new Size(80, 30)
            };
            clearButton.Click += ClearButton_Click;
            this.Controls.Add(clearButton);

            // Go button
            goButton = new Button
            {
                Text = "Go",
                Location = new System.Drawing.Point(SIZE + 20, 60),
                Size = new Size(80, 30)
            };
            goButton.Click += GoButton_Click;
            this.Controls.Add(goButton);

            // Label with instructions
            var label = new Label
            {
                Text = "ЛКМ - добавить точку, ПКМ - закрыть полигон. Вершины по часовой стрелке!",
                Location = new System.Drawing.Point(SIZE + 20, 100),
                Size = new Size(170, 40),
                AutoSize = true
            };
            this.Controls.Add(label);

            this.ResumeLayout();
        }
    }
}