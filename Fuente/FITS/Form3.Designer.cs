
namespace ExploraFITS
{
    partial class Form3
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
            this.r_pixel = new System.Windows.Forms.Label();
            this.crpix_1 = new System.Windows.Forms.TextBox();
            this.crpix_2 = new System.Windows.Forms.TextBox();
            this.r_arydec = new System.Windows.Forms.Label();
            this.crval_2 = new System.Windows.Forms.TextBox();
            this.crval_1 = new System.Windows.Forms.TextBox();
            this.cdelt_2 = new System.Windows.Forms.TextBox();
            this.cdelt_1 = new System.Windows.Forms.TextBox();
            this.r_delta = new System.Windows.Forms.Label();
            this.b_omite = new System.Windows.Forms.Button();
            this.b_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // r_pixel
            // 
            this.r_pixel.AutoSize = true;
            this.r_pixel.Location = new System.Drawing.Point(24, 17);
            this.r_pixel.Name = "r_pixel";
            this.r_pixel.Size = new System.Drawing.Size(131, 20);
            this.r_pixel.TabIndex = 0;
            this.r_pixel.Text = "Pixel de referencia";
            // 
            // crpix_1
            // 
            this.crpix_1.Location = new System.Drawing.Point(188, 17);
            this.crpix_1.Name = "crpix_1";
            this.crpix_1.Size = new System.Drawing.Size(75, 27);
            this.crpix_1.TabIndex = 1;
            // 
            // crpix_2
            // 
            this.crpix_2.Location = new System.Drawing.Point(269, 17);
            this.crpix_2.Name = "crpix_2";
            this.crpix_2.Size = new System.Drawing.Size(75, 27);
            this.crpix_2.TabIndex = 2;
            // 
            // r_arydec
            // 
            this.r_arydec.AutoSize = true;
            this.r_arydec.Location = new System.Drawing.Point(24, 51);
            this.r_arydec.Name = "r_arydec";
            this.r_arydec.Size = new System.Drawing.Size(71, 20);
            this.r_arydec.TabIndex = 3;
            this.r_arydec.Text = "AR y DEC";
            // 
            // crval_2
            // 
            this.crval_2.Location = new System.Drawing.Point(269, 51);
            this.crval_2.Name = "crval_2";
            this.crval_2.Size = new System.Drawing.Size(75, 27);
            this.crval_2.TabIndex = 5;
            // 
            // crval_1
            // 
            this.crval_1.Location = new System.Drawing.Point(188, 51);
            this.crval_1.Name = "crval_1";
            this.crval_1.Size = new System.Drawing.Size(75, 27);
            this.crval_1.TabIndex = 4;
            // 
            // cdelt_2
            // 
            this.cdelt_2.Location = new System.Drawing.Point(269, 84);
            this.cdelt_2.Name = "cdelt_2";
            this.cdelt_2.Size = new System.Drawing.Size(75, 27);
            this.cdelt_2.TabIndex = 8;
            // 
            // cdelt_1
            // 
            this.cdelt_1.Location = new System.Drawing.Point(188, 84);
            this.cdelt_1.Name = "cdelt_1";
            this.cdelt_1.Size = new System.Drawing.Size(75, 27);
            this.cdelt_1.TabIndex = 7;
            // 
            // r_delta
            // 
            this.r_delta.AutoSize = true;
            this.r_delta.Location = new System.Drawing.Point(24, 84);
            this.r_delta.Name = "r_delta";
            this.r_delta.Size = new System.Drawing.Size(106, 20);
            this.r_delta.TabIndex = 6;
            this.r_delta.Text = "delta x, delta y";
            // 
            // b_omite
            // 
            this.b_omite.Location = new System.Drawing.Point(24, 125);
            this.b_omite.Name = "b_omite";
            this.b_omite.Size = new System.Drawing.Size(131, 31);
            this.b_omite.TabIndex = 46;
            this.b_omite.Text = "Omite";
            this.b_omite.UseVisualStyleBackColor = true;
            this.b_omite.Click += new System.EventHandler(this.B_cancela_Click);
            // 
            // b_ok
            // 
            this.b_ok.Location = new System.Drawing.Point(213, 125);
            this.b_ok.Name = "b_ok";
            this.b_ok.Size = new System.Drawing.Size(131, 31);
            this.b_ok.TabIndex = 47;
            this.b_ok.Text = "Ok";
            this.b_ok.UseVisualStyleBackColor = true;
            this.b_ok.Click += new System.EventHandler(this.B_ok_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 165);
            this.ControlBox = false;
            this.Controls.Add(this.b_ok);
            this.Controls.Add(this.b_omite);
            this.Controls.Add(this.cdelt_2);
            this.Controls.Add(this.cdelt_1);
            this.Controls.Add(this.r_delta);
            this.Controls.Add(this.crval_2);
            this.Controls.Add(this.crval_1);
            this.Controls.Add(this.r_arydec);
            this.Controls.Add(this.crpix_2);
            this.Controls.Add(this.crpix_1);
            this.Controls.Add(this.r_pixel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Punto de referencia";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label r_pixel;
        private System.Windows.Forms.TextBox crpix_1;
        private System.Windows.Forms.TextBox crpix_2;
        private System.Windows.Forms.Label r_arydec;
        private System.Windows.Forms.TextBox crval_2;
        private System.Windows.Forms.TextBox crval_1;
        private System.Windows.Forms.TextBox cdelt_2;
        private System.Windows.Forms.TextBox cdelt_1;
        private System.Windows.Forms.Label r_delta;
        public System.Windows.Forms.Button b_omite;
        public System.Windows.Forms.Button b_ok;
    }
}