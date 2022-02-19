namespace ExploraFits
{
    partial class Form7
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.b_esc = new System.Windows.Forms.Button();
            this.fracciones = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.b_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // b_esc
            // 
            this.b_esc.Location = new System.Drawing.Point(132, 90);
            this.b_esc.Name = "b_esc";
            this.b_esc.Size = new System.Drawing.Size(74, 30);
            this.b_esc.TabIndex = 1;
            this.b_esc.Text = "Esc";
            this.b_esc.UseVisualStyleBackColor = true;
            this.b_esc.Click += new System.EventHandler(this.b_esc_Click);
            // 
            // fracciones
            // 
            this.fracciones.Location = new System.Drawing.Point(313, 46);
            this.fracciones.Name = "fracciones";
            this.fracciones.Size = new System.Drawing.Size(91, 27);
            this.fracciones.TabIndex = 2;
            this.fracciones.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(268, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Fraccionar en bloques (0 sin fraccionar)";
            // 
            // b_ok
            // 
            this.b_ok.Location = new System.Drawing.Point(249, 90);
            this.b_ok.Name = "b_ok";
            this.b_ok.Size = new System.Drawing.Size(74, 30);
            this.b_ok.TabIndex = 4;
            this.b_ok.Text = "OK";
            this.b_ok.UseVisualStyleBackColor = true;
            this.b_ok.Click += new System.EventHandler(this.b_ok_Click);
            // 
            // Form7
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 135);
            this.Controls.Add(this.b_ok);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fracciones);
            this.Controls.Add(this.b_esc);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form7";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Fraccionar en bloques (0 sin fraccionar)";
            this.Load += new System.EventHandler(this.Form7_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button b_esc;
        public System.Windows.Forms.TextBox fracciones;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button b_ok;
    }
}