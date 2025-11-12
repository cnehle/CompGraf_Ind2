namespace Ind2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button goButton;

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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.clearButton = new System.Windows.Forms.Button();
            this.goButton = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // mainPanel
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.Location = new System.Drawing.Point(12, 12);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(700, 700);
            this.mainPanel.TabIndex = 0;

            // clearButton
            this.clearButton.Location = new System.Drawing.Point(730, 70);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 30);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);

            // goButton
            this.goButton.Location = new System.Drawing.Point(730, 30);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(75, 30);
            this.goButton.TabIndex = 2;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 730);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.mainPanel);
            this.Name = "Form1";
            this.Text = "Объединение выпуклых полигонов";
            this.ResumeLayout(false);
        }
    }
}