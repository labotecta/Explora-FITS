
namespace ExploraFits
{
    partial class Form4
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
            this.b_portapapeles = new System.Windows.Forms.Button();
            this.tabla = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.tabla)).BeginInit();
            this.SuspendLayout();
            // 
            // b_portapapeles
            // 
            this.b_portapapeles.Location = new System.Drawing.Point(865, 439);
            this.b_portapapeles.Name = "b_portapapeles";
            this.b_portapapeles.Size = new System.Drawing.Size(138, 35);
            this.b_portapapeles.TabIndex = 47;
            this.b_portapapeles.Text = "Portapapeles";
            this.b_portapapeles.UseVisualStyleBackColor = true;
            this.b_portapapeles.Click += new System.EventHandler(this.B_portapapeles_Click);
            // 
            // tabla
            // 
            this.tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tabla.Location = new System.Drawing.Point(12, 12);
            this.tabla.Name = "tabla";
            this.tabla.RowHeadersWidth = 51;
            this.tabla.RowTemplate.Height = 29;
            this.tabla.Size = new System.Drawing.Size(991, 421);
            this.tabla.TabIndex = 48;
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 479);
            this.Controls.Add(this.tabla);
            this.Controls.Add(this.b_portapapeles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ficha hyperleda";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form4_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tabla)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button b_portapapeles;
        private System.Windows.Forms.DataGridView tabla;
    }
}