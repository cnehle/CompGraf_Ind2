namespace Ind2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnUnion;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelControls;

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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnUnion = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelControls.SuspendLayout();
            this.SuspendLayout();

            // pictureBox
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 40);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(800, 510);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);

            // panelControls
            this.panelControls.Controls.Add(this.btnUnion);
            this.panelControls.Controls.Add(this.btnClear);
            this.panelControls.Controls.Add(this.lblStatus);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControls.Location = new System.Drawing.Point(0, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(800, 40);
            this.panelControls.TabIndex = 1;

            // btnClear
            this.btnClear.Location = new System.Drawing.Point(12, 8);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 25);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            // btnUnion
            this.btnUnion.Location = new System.Drawing.Point(118, 8);
            this.btnUnion.Name = "btnUnion";
            this.btnUnion.Size = new System.Drawing.Size(100, 25);
            this.btnUnion.TabIndex = 1;
            this.btnUnion.Text = "Объединить";
            this.btnUnion.UseVisualStyleBackColor = true;
            this.btnUnion.Click += new System.EventHandler(this.btnUnion_Click);

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(240, 13);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(180, 16);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Нарисуйте первый полигон";

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.panelControls);
            this.Name = "Form1";
            this.Text = "Объединение выпуклых полигонов";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}