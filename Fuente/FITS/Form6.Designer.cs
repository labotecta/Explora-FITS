
namespace ExploraFits
{
    partial class Form6
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
            this.v_ar = new System.Windows.Forms.TextBox();
            this.v_de = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.v_margen = new System.Windows.Forms.TextBox();
            this.sel_unidades_ar = new System.Windows.Forms.ComboBox();
            this.sel_unidades_de = new System.Windows.Forms.ComboBox();
            this.b_buscar = new System.Windows.Forms.Button();
            this.sel_catalogo = new System.Windows.Forms.ComboBox();
            this.r_arseg = new System.Windows.Forms.Label();
            this.tabla = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tabla)).BeginInit();
            this.SuspendLayout();
            // 
            // v_ar
            // 
            this.v_ar.Location = new System.Drawing.Point(99, 12);
            this.v_ar.Name = "v_ar";
            this.v_ar.Size = new System.Drawing.Size(134, 27);
            this.v_ar.TabIndex = 47;
            this.v_ar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // v_de
            // 
            this.v_de.Location = new System.Drawing.Point(99, 45);
            this.v_de.Name = "v_de";
            this.v_de.Size = new System.Drawing.Size(134, 27);
            this.v_de.TabIndex = 48;
            this.v_de.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 20);
            this.label1.TabIndex = 49;
            this.label1.Text = "AR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 20);
            this.label2.TabIndex = 50;
            this.label2.Text = "DEC";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 20);
            this.label3.TabIndex = 52;
            this.label3.Text = "Margen";
            // 
            // v_margen
            // 
            this.v_margen.Location = new System.Drawing.Point(99, 78);
            this.v_margen.Name = "v_margen";
            this.v_margen.Size = new System.Drawing.Size(134, 27);
            this.v_margen.TabIndex = 51;
            this.v_margen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // sel_unidades_ar
            // 
            this.sel_unidades_ar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sel_unidades_ar.Location = new System.Drawing.Point(376, 12);
            this.sel_unidades_ar.Name = "sel_unidades_ar";
            this.sel_unidades_ar.Size = new System.Drawing.Size(61, 28);
            this.sel_unidades_ar.TabIndex = 59;
            this.sel_unidades_ar.Visible = false;
            // 
            // sel_unidades_de
            // 
            this.sel_unidades_de.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sel_unidades_de.Location = new System.Drawing.Point(376, 45);
            this.sel_unidades_de.Name = "sel_unidades_de";
            this.sel_unidades_de.Size = new System.Drawing.Size(61, 28);
            this.sel_unidades_de.TabIndex = 60;
            this.sel_unidades_de.Visible = false;
            // 
            // b_buscar
            // 
            this.b_buscar.Location = new System.Drawing.Point(255, 160);
            this.b_buscar.Name = "b_buscar";
            this.b_buscar.Size = new System.Drawing.Size(182, 29);
            this.b_buscar.TabIndex = 61;
            this.b_buscar.Text = "Buscar";
            this.b_buscar.UseVisualStyleBackColor = true;
            this.b_buscar.Click += new System.EventHandler(this.B_buscar_Click);
            // 
            // sel_catalogo
            // 
            this.sel_catalogo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sel_catalogo.Location = new System.Drawing.Point(12, 116);
            this.sel_catalogo.Name = "sel_catalogo";
            this.sel_catalogo.Size = new System.Drawing.Size(221, 28);
            this.sel_catalogo.TabIndex = 67;
            // 
            // r_arseg
            // 
            this.r_arseg.AutoSize = true;
            this.r_arseg.Location = new System.Drawing.Point(255, 81);
            this.r_arseg.Name = "r_arseg";
            this.r_arseg.Size = new System.Drawing.Size(52, 20);
            this.r_arseg.TabIndex = 68;
            this.r_arseg.Text = "arcseg";
            // 
            // tabla
            // 
            this.tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tabla.Location = new System.Drawing.Point(443, 12);
            this.tabla.Name = "tabla";
            this.tabla.RowHeadersWidth = 51;
            this.tabla.RowTemplate.Height = 29;
            this.tabla.Size = new System.Drawing.Size(439, 177);
            this.tabla.TabIndex = 69;
            this.tabla.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Tabla_CellMouseDoubleClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(255, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 20);
            this.label4.TabIndex = 70;
            this.label4.Text = "grados";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(255, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 20);
            this.label5.TabIndex = 71;
            this.label5.Text = "grados";
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 201);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tabla);
            this.Controls.Add(this.r_arseg);
            this.Controls.Add(this.sel_catalogo);
            this.Controls.Add(this.b_buscar);
            this.Controls.Add(this.sel_unidades_de);
            this.Controls.Add(this.sel_unidades_ar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.v_margen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.v_de);
            this.Controls.Add(this.v_ar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form6";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Buscar";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form6_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tabla)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox v_ar;
        public System.Windows.Forms.TextBox v_de;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox v_margen;
        private System.Windows.Forms.ComboBox sel_unidades_ar;
        private System.Windows.Forms.ComboBox sel_unidades_de;
        private System.Windows.Forms.Button b_buscar;
        public System.Windows.Forms.ComboBox sel_catalogo;
        private System.Windows.Forms.Label r_arseg;
        private System.Windows.Forms.DataGridView tabla;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}