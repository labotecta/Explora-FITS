
namespace ExploraFITS
{
    partial class FITS
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FITS));
            this.b_leer_fits = new System.Windows.Forms.Button();
            this.lista_cabeceras = new System.Windows.Forms.ListBox();
            this.b_exporta_tabla = new System.Windows.Forms.Button();
            this.tabla = new System.Windows.Forms.DataGridView();
            this.v_leidos = new System.Windows.Forms.Label();
            this.r_leidos = new System.Windows.Forms.Label();
            this.r_octetos = new System.Windows.Forms.Label();
            this.r_de = new System.Windows.Forms.Label();
            this.r_cantidad = new System.Windows.Forms.Label();
            this.b_indexar_hyperleda = new System.Windows.Forms.Button();
            this.r_operacion = new System.Windows.Forms.Label();
            this.r_reg_leidos = new System.Windows.Forms.Label();
            this.normalizar = new System.Windows.Forms.CheckBox();
            this.v_acotar_max = new System.Windows.Forms.TextBox();
            this.r_acotar = new System.Windows.Forms.Label();
            this.invertir_y = new System.Windows.Forms.CheckBox();
            this.b_redibuja = new System.Windows.Forms.Button();
            this.v_acotar_min = new System.Windows.Forms.TextBox();
            this.panel_histograma = new System.Windows.Forms.PictureBox();
            this.r_ceros = new System.Windows.Forms.Label();
            this.histo_ceros = new System.Windows.Forms.Label();
            this.histo_ceros_tpc = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.r_min = new System.Windows.Forms.Label();
            this.r_max = new System.Windows.Forms.Label();
            this.r_destandar = new System.Windows.Forms.Label();
            this.r_med = new System.Windows.Forms.Label();
            this.alta_resolucion = new System.Windows.Forms.CheckBox();
            this.r_h = new System.Windows.Forms.Label();
            this.sel_HDU = new System.Windows.Forms.ComboBox();
            this.r_HDU = new System.Windows.Forms.Label();
            this.sel_imagen = new System.Windows.Forms.ComboBox();
            this.sel_tabla = new System.Windows.Forms.ComboBox();
            this.r_imagen = new System.Windows.Forms.Label();
            this.r_tabla = new System.Windows.Forms.Label();
            this.b_exporta_cabeceras = new System.Windows.Forms.Button();
            this.sel_HDU_ok = new System.Windows.Forms.Label();
            this.b_nan = new System.Windows.Forms.Button();
            this.b_cota_inf = new System.Windows.Forms.Button();
            this.b_cota_sup = new System.Windows.Forms.Button();
            this.raiz = new System.Windows.Forms.CheckBox();
            this.invertir_x = new System.Windows.Forms.CheckBox();
            this.reloj_ar = new System.Windows.Forms.PictureBox();
            this.reloj_de = new System.Windows.Forms.PictureBox();
            this.ctype_1 = new System.Windows.Forms.Label();
            this.ctype_2 = new System.Windows.Forms.Label();
            this.lista_parametros = new System.Windows.Forms.ListBox();
            this.val_parametro = new System.Windows.Forms.Label();
            this.r_indexar = new System.Windows.Forms.Label();
            this.b_indexar_sao = new System.Windows.Forms.Button();
            this.b_idioma = new System.Windows.Forms.Button();
            this.b_exporta_imagen = new System.Windows.Forms.Button();
            this.b_apilar = new System.Windows.Forms.Button();
            this.b_restar = new System.Windows.Forms.Button();
            this.r_tpc_min = new System.Windows.Forms.Label();
            this.r_tpc_max = new System.Windows.Forms.Label();
            this.b_conv_espectro = new System.Windows.Forms.Button();
            this.b_buscar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tabla)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_histograma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reloj_ar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reloj_de)).BeginInit();
            this.SuspendLayout();
            // 
            // b_leer_fits
            // 
            this.b_leer_fits.Image = ((System.Drawing.Image)(resources.GetObject("b_leer_fits.Image")));
            this.b_leer_fits.Location = new System.Drawing.Point(796, 6);
            this.b_leer_fits.Name = "b_leer_fits";
            this.b_leer_fits.Size = new System.Drawing.Size(42, 42);
            this.b_leer_fits.TabIndex = 0;
            this.b_leer_fits.UseVisualStyleBackColor = true;
            this.b_leer_fits.Click += new System.EventHandler(this.B_leer_fits_Click);
            // 
            // lista_cabeceras
            // 
            this.lista_cabeceras.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lista_cabeceras.FormattingEnabled = true;
            this.lista_cabeceras.ItemHeight = 17;
            this.lista_cabeceras.Location = new System.Drawing.Point(12, 354);
            this.lista_cabeceras.Name = "lista_cabeceras";
            this.lista_cabeceras.Size = new System.Drawing.Size(802, 174);
            this.lista_cabeceras.TabIndex = 2;
            // 
            // b_exporta_tabla
            // 
            this.b_exporta_tabla.Image = ((System.Drawing.Image)(resources.GetObject("b_exporta_tabla.Image")));
            this.b_exporta_tabla.Location = new System.Drawing.Point(236, 154);
            this.b_exporta_tabla.Name = "b_exporta_tabla";
            this.b_exporta_tabla.Size = new System.Drawing.Size(42, 42);
            this.b_exporta_tabla.TabIndex = 5;
            this.b_exporta_tabla.UseVisualStyleBackColor = true;
            this.b_exporta_tabla.Click += new System.EventHandler(this.B_exporta_tabla_Click);
            // 
            // tabla
            // 
            this.tabla.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tabla.Location = new System.Drawing.Point(12, 534);
            this.tabla.Name = "tabla";
            this.tabla.RowHeadersWidth = 51;
            this.tabla.RowTemplate.Height = 29;
            this.tabla.Size = new System.Drawing.Size(979, 196);
            this.tabla.TabIndex = 13;
            this.tabla.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tabla_CellDoubleClick);
            // 
            // v_leidos
            // 
            this.v_leidos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_leidos.ForeColor = System.Drawing.SystemColors.ControlText;
            this.v_leidos.Location = new System.Drawing.Point(78, 50);
            this.v_leidos.Name = "v_leidos";
            this.v_leidos.Size = new System.Drawing.Size(126, 20);
            this.v_leidos.TabIndex = 29;
            this.v_leidos.Text = "0";
            this.v_leidos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // r_leidos
            // 
            this.r_leidos.AutoSize = true;
            this.r_leidos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.r_leidos.Location = new System.Drawing.Point(12, 50);
            this.r_leidos.Name = "r_leidos";
            this.r_leidos.Size = new System.Drawing.Size(54, 20);
            this.r_leidos.TabIndex = 30;
            this.r_leidos.Text = "Leidos";
            // 
            // r_octetos
            // 
            this.r_octetos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.r_octetos.ForeColor = System.Drawing.SystemColors.ControlText;
            this.r_octetos.Location = new System.Drawing.Point(257, 50);
            this.r_octetos.Name = "r_octetos";
            this.r_octetos.Size = new System.Drawing.Size(126, 20);
            this.r_octetos.TabIndex = 31;
            this.r_octetos.Text = "0";
            this.r_octetos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // r_de
            // 
            this.r_de.AutoSize = true;
            this.r_de.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.r_de.Location = new System.Drawing.Point(218, 50);
            this.r_de.Name = "r_de";
            this.r_de.Size = new System.Drawing.Size(26, 20);
            this.r_de.TabIndex = 32;
            this.r_de.Text = "de";
            // 
            // r_cantidad
            // 
            this.r_cantidad.AutoSize = true;
            this.r_cantidad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.r_cantidad.Location = new System.Drawing.Point(393, 50);
            this.r_cantidad.Name = "r_cantidad";
            this.r_cantidad.Size = new System.Drawing.Size(47, 20);
            this.r_cantidad.TabIndex = 33;
            this.r_cantidad.Text = "bytes";
            // 
            // b_indexar_hyperleda
            // 
            this.b_indexar_hyperleda.Location = new System.Drawing.Point(74, 736);
            this.b_indexar_hyperleda.Name = "b_indexar_hyperleda";
            this.b_indexar_hyperleda.Size = new System.Drawing.Size(100, 31);
            this.b_indexar_hyperleda.TabIndex = 34;
            this.b_indexar_hyperleda.Text = "HYPERLEDA";
            this.b_indexar_hyperleda.UseVisualStyleBackColor = true;
            this.b_indexar_hyperleda.Click += new System.EventHandler(this.B_indexar_hiperleda_Click);
            // 
            // r_operacion
            // 
            this.r_operacion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.r_operacion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.r_operacion.Location = new System.Drawing.Point(378, 741);
            this.r_operacion.Name = "r_operacion";
            this.r_operacion.Size = new System.Drawing.Size(110, 20);
            this.r_operacion.TabIndex = 37;
            this.r_operacion.Text = "-";
            this.r_operacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // r_reg_leidos
            // 
            this.r_reg_leidos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.r_reg_leidos.ForeColor = System.Drawing.SystemColors.ControlText;
            this.r_reg_leidos.Location = new System.Drawing.Point(249, 741);
            this.r_reg_leidos.Name = "r_reg_leidos";
            this.r_reg_leidos.Size = new System.Drawing.Size(110, 20);
            this.r_reg_leidos.TabIndex = 35;
            this.r_reg_leidos.Text = "0";
            this.r_reg_leidos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // normalizar
            // 
            this.normalizar.AutoSize = true;
            this.normalizar.Checked = true;
            this.normalizar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.normalizar.Location = new System.Drawing.Point(12, 10);
            this.normalizar.Name = "normalizar";
            this.normalizar.Size = new System.Drawing.Size(105, 24);
            this.normalizar.TabIndex = 38;
            this.normalizar.Text = "Normalizar";
            this.normalizar.UseVisualStyleBackColor = true;
            this.normalizar.CheckedChanged += new System.EventHandler(this.Normalizar_CheckedChanged);
            // 
            // v_acotar_max
            // 
            this.v_acotar_max.Location = new System.Drawing.Point(432, 9);
            this.v_acotar_max.Name = "v_acotar_max";
            this.v_acotar_max.Size = new System.Drawing.Size(73, 27);
            this.v_acotar_max.TabIndex = 39;
            this.v_acotar_max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.v_acotar_max.TextChanged += new System.EventHandler(this.V_acotar_max_TextChanged);
            // 
            // r_acotar
            // 
            this.r_acotar.AutoSize = true;
            this.r_acotar.Location = new System.Drawing.Point(181, 12);
            this.r_acotar.Name = "r_acotar";
            this.r_acotar.Size = new System.Drawing.Size(53, 20);
            this.r_acotar.TabIndex = 40;
            this.r_acotar.Text = "Acotar";
            // 
            // invertir_y
            // 
            this.invertir_y.AutoSize = true;
            this.invertir_y.Location = new System.Drawing.Point(686, 10);
            this.invertir_y.Name = "invertir_y";
            this.invertir_y.Size = new System.Drawing.Size(47, 24);
            this.invertir_y.TabIndex = 44;
            this.invertir_y.Text = "I Y";
            this.invertir_y.UseVisualStyleBackColor = true;
            this.invertir_y.CheckedChanged += new System.EventHandler(this.Invertir_y_CheckedChanged);
            // 
            // b_redibuja
            // 
            this.b_redibuja.Enabled = false;
            this.b_redibuja.Image = ((System.Drawing.Image)(resources.GetObject("b_redibuja.Image")));
            this.b_redibuja.Location = new System.Drawing.Point(846, 6);
            this.b_redibuja.Name = "b_redibuja";
            this.b_redibuja.Size = new System.Drawing.Size(42, 42);
            this.b_redibuja.TabIndex = 45;
            this.b_redibuja.UseVisualStyleBackColor = true;
            this.b_redibuja.Click += new System.EventHandler(this.B_redibuja_Click);
            // 
            // v_acotar_min
            // 
            this.v_acotar_min.Location = new System.Drawing.Point(295, 9);
            this.v_acotar_min.Name = "v_acotar_min";
            this.v_acotar_min.Size = new System.Drawing.Size(73, 27);
            this.v_acotar_min.TabIndex = 46;
            this.v_acotar_min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.v_acotar_min.TextChanged += new System.EventHandler(this.V_acotar_min_TextChanged);
            // 
            // panel_histograma
            // 
            this.panel_histograma.Location = new System.Drawing.Point(329, 79);
            this.panel_histograma.Name = "panel_histograma";
            this.panel_histograma.Size = new System.Drawing.Size(662, 215);
            this.panel_histograma.TabIndex = 47;
            this.panel_histograma.TabStop = false;
            this.panel_histograma.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_histograma_MouseDown);
            // 
            // r_ceros
            // 
            this.r_ceros.AutoSize = true;
            this.r_ceros.ForeColor = System.Drawing.Color.Blue;
            this.r_ceros.Location = new System.Drawing.Point(717, 54);
            this.r_ceros.Name = "r_ceros";
            this.r_ceros.Size = new System.Drawing.Size(44, 20);
            this.r_ceros.TabIndex = 48;
            this.r_ceros.Text = "ceros";
            // 
            // histo_ceros
            // 
            this.histo_ceros.ForeColor = System.Drawing.Color.Blue;
            this.histo_ceros.Location = new System.Drawing.Point(774, 54);
            this.histo_ceros.Name = "histo_ceros";
            this.histo_ceros.Size = new System.Drawing.Size(102, 20);
            this.histo_ceros.TabIndex = 49;
            this.histo_ceros.Text = "0";
            this.histo_ceros.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // histo_ceros_tpc
            // 
            this.histo_ceros_tpc.ForeColor = System.Drawing.Color.Blue;
            this.histo_ceros_tpc.Location = new System.Drawing.Point(902, 54);
            this.histo_ceros_tpc.Name = "histo_ceros_tpc";
            this.histo_ceros_tpc.Size = new System.Drawing.Size(53, 20);
            this.histo_ceros_tpc.TabIndex = 50;
            this.histo_ceros_tpc.Text = "0";
            this.histo_ceros_tpc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.Blue;
            this.label12.Location = new System.Drawing.Point(970, 54);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(21, 20);
            this.label12.TabIndex = 51;
            this.label12.Text = "%";
            // 
            // r_min
            // 
            this.r_min.Location = new System.Drawing.Point(329, 301);
            this.r_min.Name = "r_min";
            this.r_min.Size = new System.Drawing.Size(120, 20);
            this.r_min.TabIndex = 52;
            this.r_min.Text = "0";
            this.r_min.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // r_max
            // 
            this.r_max.Location = new System.Drawing.Point(871, 301);
            this.r_max.Name = "r_max";
            this.r_max.Size = new System.Drawing.Size(120, 20);
            this.r_max.TabIndex = 53;
            this.r_max.Text = "0";
            this.r_max.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // r_destandar
            // 
            this.r_destandar.ForeColor = System.Drawing.Color.Green;
            this.r_destandar.Location = new System.Drawing.Point(659, 301);
            this.r_destandar.Name = "r_destandar";
            this.r_destandar.Size = new System.Drawing.Size(169, 20);
            this.r_destandar.TabIndex = 54;
            this.r_destandar.Text = "0";
            this.r_destandar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // r_med
            // 
            this.r_med.ForeColor = System.Drawing.Color.Green;
            this.r_med.Location = new System.Drawing.Point(481, 301);
            this.r_med.Name = "r_med";
            this.r_med.Size = new System.Drawing.Size(169, 20);
            this.r_med.TabIndex = 55;
            this.r_med.Text = "0";
            this.r_med.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // alta_resolucion
            // 
            this.alta_resolucion.AutoSize = true;
            this.alta_resolucion.Location = new System.Drawing.Point(575, 10);
            this.alta_resolucion.Name = "alta_resolucion";
            this.alta_resolucion.Size = new System.Drawing.Size(101, 24);
            this.alta_resolucion.TabIndex = 56;
            this.alta_resolucion.Text = "48bppRgb";
            this.alta_resolucion.UseVisualStyleBackColor = true;
            this.alta_resolucion.CheckedChanged += new System.EventHandler(this.Alta_resolucion_CheckedChanged);
            // 
            // r_h
            // 
            this.r_h.ForeColor = System.Drawing.Color.Fuchsia;
            this.r_h.Location = new System.Drawing.Point(329, 329);
            this.r_h.Name = "r_h";
            this.r_h.Size = new System.Drawing.Size(310, 20);
            this.r_h.TabIndex = 57;
            this.r_h.Text = "-";
            this.r_h.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sel_HDU
            // 
            this.sel_HDU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sel_HDU.Location = new System.Drawing.Point(145, 83);
            this.sel_HDU.Name = "sel_HDU";
            this.sel_HDU.Size = new System.Drawing.Size(77, 28);
            this.sel_HDU.TabIndex = 58;
            this.sel_HDU.SelectedIndexChanged += new System.EventHandler(this.Sel_HDU_SelectedIndexChanged);
            // 
            // r_HDU
            // 
            this.r_HDU.AutoSize = true;
            this.r_HDU.Location = new System.Drawing.Point(58, 87);
            this.r_HDU.Name = "r_HDU";
            this.r_HDU.Size = new System.Drawing.Size(41, 20);
            this.r_HDU.TabIndex = 59;
            this.r_HDU.Text = "HDU";
            // 
            // sel_imagen
            // 
            this.sel_imagen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sel_imagen.FormattingEnabled = true;
            this.sel_imagen.Location = new System.Drawing.Point(145, 125);
            this.sel_imagen.Name = "sel_imagen";
            this.sel_imagen.Size = new System.Drawing.Size(77, 28);
            this.sel_imagen.TabIndex = 60;
            this.sel_imagen.SelectedIndexChanged += new System.EventHandler(this.Sel_imagen_SelectedIndexChanged);
            // 
            // sel_tabla
            // 
            this.sel_tabla.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sel_tabla.FormattingEnabled = true;
            this.sel_tabla.Location = new System.Drawing.Point(145, 168);
            this.sel_tabla.Name = "sel_tabla";
            this.sel_tabla.Size = new System.Drawing.Size(77, 28);
            this.sel_tabla.TabIndex = 61;
            this.sel_tabla.SelectedIndexChanged += new System.EventHandler(this.Sel_tabla_SelectedIndexChanged);
            // 
            // r_imagen
            // 
            this.r_imagen.AutoSize = true;
            this.r_imagen.Location = new System.Drawing.Point(58, 133);
            this.r_imagen.Name = "r_imagen";
            this.r_imagen.Size = new System.Drawing.Size(59, 20);
            this.r_imagen.TabIndex = 62;
            this.r_imagen.Text = "Imagen";
            // 
            // r_tabla
            // 
            this.r_tabla.AutoSize = true;
            this.r_tabla.Location = new System.Drawing.Point(58, 176);
            this.r_tabla.Name = "r_tabla";
            this.r_tabla.Size = new System.Drawing.Size(44, 20);
            this.r_tabla.TabIndex = 63;
            this.r_tabla.Text = "Tabla";
            // 
            // b_exporta_cabeceras
            // 
            this.b_exporta_cabeceras.Image = ((System.Drawing.Image)(resources.GetObject("b_exporta_cabeceras.Image")));
            this.b_exporta_cabeceras.Location = new System.Drawing.Point(12, 309);
            this.b_exporta_cabeceras.Name = "b_exporta_cabeceras";
            this.b_exporta_cabeceras.Size = new System.Drawing.Size(42, 42);
            this.b_exporta_cabeceras.TabIndex = 64;
            this.b_exporta_cabeceras.UseVisualStyleBackColor = true;
            this.b_exporta_cabeceras.Click += new System.EventHandler(this.B_exporta_cabeceras_Click);
            // 
            // sel_HDU_ok
            // 
            this.sel_HDU_ok.AutoSize = true;
            this.sel_HDU_ok.Location = new System.Drawing.Point(236, 87);
            this.sel_HDU_ok.Name = "sel_HDU_ok";
            this.sel_HDU_ok.Size = new System.Drawing.Size(29, 20);
            this.sel_HDU_ok.TabIndex = 65;
            this.sel_HDU_ok.Text = "OK";
            // 
            // b_nan
            // 
            this.b_nan.BackColor = System.Drawing.Color.Black;
            this.b_nan.ForeColor = System.Drawing.Color.White;
            this.b_nan.Location = new System.Drawing.Point(122, 8);
            this.b_nan.Name = "b_nan";
            this.b_nan.Size = new System.Drawing.Size(52, 31);
            this.b_nan.TabIndex = 68;
            this.b_nan.Text = "NaN";
            this.b_nan.UseVisualStyleBackColor = false;
            this.b_nan.Click += new System.EventHandler(this.B_nan_Click);
            // 
            // b_cota_inf
            // 
            this.b_cota_inf.BackColor = System.Drawing.Color.Red;
            this.b_cota_inf.ForeColor = System.Drawing.Color.White;
            this.b_cota_inf.Location = new System.Drawing.Point(239, 8);
            this.b_cota_inf.Name = "b_cota_inf";
            this.b_cota_inf.Size = new System.Drawing.Size(52, 31);
            this.b_cota_inf.TabIndex = 69;
            this.b_cota_inf.Text = "Min";
            this.b_cota_inf.UseVisualStyleBackColor = false;
            this.b_cota_inf.Click += new System.EventHandler(this.B_cota_inf_Click);
            // 
            // b_cota_sup
            // 
            this.b_cota_sup.BackColor = System.Drawing.Color.Blue;
            this.b_cota_sup.ForeColor = System.Drawing.Color.White;
            this.b_cota_sup.Location = new System.Drawing.Point(376, 8);
            this.b_cota_sup.Name = "b_cota_sup";
            this.b_cota_sup.Size = new System.Drawing.Size(52, 31);
            this.b_cota_sup.TabIndex = 70;
            this.b_cota_sup.Text = "Max";
            this.b_cota_sup.UseVisualStyleBackColor = false;
            this.b_cota_sup.Click += new System.EventHandler(this.B_cota_sup_Click);
            // 
            // raiz
            // 
            this.raiz.AutoSize = true;
            this.raiz.Location = new System.Drawing.Point(512, 10);
            this.raiz.Name = "raiz";
            this.raiz.Size = new System.Drawing.Size(53, 24);
            this.raiz.TabIndex = 71;
            this.raiz.Text = "1/2";
            this.raiz.UseVisualStyleBackColor = true;
            this.raiz.CheckedChanged += new System.EventHandler(this.Raiz_CheckedChanged);
            // 
            // invertir_x
            // 
            this.invertir_x.AutoSize = true;
            this.invertir_x.Location = new System.Drawing.Point(744, 10);
            this.invertir_x.Name = "invertir_x";
            this.invertir_x.Size = new System.Drawing.Size(48, 24);
            this.invertir_x.TabIndex = 72;
            this.invertir_x.Text = "I X";
            this.invertir_x.UseVisualStyleBackColor = true;
            this.invertir_x.CheckedChanged += new System.EventHandler(this.Invertir_x_CheckedChanged);
            // 
            // reloj_ar
            // 
            this.reloj_ar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.reloj_ar.Location = new System.Drawing.Point(78, 233);
            this.reloj_ar.Name = "reloj_ar";
            this.reloj_ar.Size = new System.Drawing.Size(100, 100);
            this.reloj_ar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.reloj_ar.TabIndex = 73;
            this.reloj_ar.TabStop = false;
            this.reloj_ar.Paint += new System.Windows.Forms.PaintEventHandler(this.Reloj_ar_Paint);
            // 
            // reloj_de
            // 
            this.reloj_de.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.reloj_de.Location = new System.Drawing.Point(209, 233);
            this.reloj_de.Name = "reloj_de";
            this.reloj_de.Size = new System.Drawing.Size(100, 100);
            this.reloj_de.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.reloj_de.TabIndex = 74;
            this.reloj_de.TabStop = false;
            this.reloj_de.Paint += new System.Windows.Forms.PaintEventHandler(this.Reloj_de_Paint);
            // 
            // ctype_1
            // 
            this.ctype_1.Location = new System.Drawing.Point(66, 209);
            this.ctype_1.Name = "ctype_1";
            this.ctype_1.Size = new System.Drawing.Size(125, 20);
            this.ctype_1.TabIndex = 75;
            this.ctype_1.Text = "-";
            this.ctype_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctype_2
            // 
            this.ctype_2.Location = new System.Drawing.Point(197, 209);
            this.ctype_2.Name = "ctype_2";
            this.ctype_2.Size = new System.Drawing.Size(125, 20);
            this.ctype_2.TabIndex = 76;
            this.ctype_2.Text = "-";
            this.ctype_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lista_parametros
            // 
            this.lista_parametros.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lista_parametros.ForeColor = System.Drawing.Color.Blue;
            this.lista_parametros.FormattingEnabled = true;
            this.lista_parametros.ItemHeight = 17;
            this.lista_parametros.Location = new System.Drawing.Point(820, 354);
            this.lista_parametros.Name = "lista_parametros";
            this.lista_parametros.Size = new System.Drawing.Size(171, 174);
            this.lista_parametros.TabIndex = 77;
            this.lista_parametros.SelectedIndexChanged += new System.EventHandler(this.Lista_parametros_SelectedIndexChanged);
            // 
            // val_parametro
            // 
            this.val_parametro.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.val_parametro.ForeColor = System.Drawing.Color.Blue;
            this.val_parametro.Location = new System.Drawing.Point(651, 329);
            this.val_parametro.Name = "val_parametro";
            this.val_parametro.Size = new System.Drawing.Size(340, 20);
            this.val_parametro.TabIndex = 78;
            this.val_parametro.Text = "-";
            this.val_parametro.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // r_indexar
            // 
            this.r_indexar.AutoSize = true;
            this.r_indexar.Location = new System.Drawing.Point(11, 741);
            this.r_indexar.Name = "r_indexar";
            this.r_indexar.Size = new System.Drawing.Size(58, 20);
            this.r_indexar.TabIndex = 79;
            this.r_indexar.Text = "Indexar";
            // 
            // b_indexar_sao
            // 
            this.b_indexar_sao.Location = new System.Drawing.Point(181, 736);
            this.b_indexar_sao.Name = "b_indexar_sao";
            this.b_indexar_sao.Size = new System.Drawing.Size(52, 31);
            this.b_indexar_sao.TabIndex = 80;
            this.b_indexar_sao.Text = "SAO";
            this.b_indexar_sao.UseVisualStyleBackColor = true;
            this.b_indexar_sao.Click += new System.EventHandler(this.B_indexar_sao_Click);
            // 
            // b_idioma
            // 
            this.b_idioma.Image = ((System.Drawing.Image)(resources.GetObject("b_idioma.Image")));
            this.b_idioma.Location = new System.Drawing.Point(949, 6);
            this.b_idioma.Name = "b_idioma";
            this.b_idioma.Size = new System.Drawing.Size(42, 42);
            this.b_idioma.TabIndex = 81;
            this.b_idioma.UseVisualStyleBackColor = true;
            this.b_idioma.Click += new System.EventHandler(this.B_idioma_Click);
            // 
            // b_exporta_imagen
            // 
            this.b_exporta_imagen.Image = ((System.Drawing.Image)(resources.GetObject("b_exporta_imagen.Image")));
            this.b_exporta_imagen.Location = new System.Drawing.Point(236, 111);
            this.b_exporta_imagen.Name = "b_exporta_imagen";
            this.b_exporta_imagen.Size = new System.Drawing.Size(42, 42);
            this.b_exporta_imagen.TabIndex = 82;
            this.b_exporta_imagen.UseVisualStyleBackColor = true;
            this.b_exporta_imagen.Click += new System.EventHandler(this.B_exporta_imagen_Click);
            // 
            // b_apilar
            // 
            this.b_apilar.Image = ((System.Drawing.Image)(resources.GetObject("b_apilar.Image")));
            this.b_apilar.Location = new System.Drawing.Point(902, 736);
            this.b_apilar.Name = "b_apilar";
            this.b_apilar.Size = new System.Drawing.Size(42, 42);
            this.b_apilar.TabIndex = 83;
            this.b_apilar.UseVisualStyleBackColor = true;
            this.b_apilar.Click += new System.EventHandler(this.B_apilar_Click);
            // 
            // b_restar
            // 
            this.b_restar.Image = ((System.Drawing.Image)(resources.GetObject("b_restar.Image")));
            this.b_restar.Location = new System.Drawing.Point(949, 736);
            this.b_restar.Name = "b_restar";
            this.b_restar.Size = new System.Drawing.Size(42, 42);
            this.b_restar.TabIndex = 84;
            this.b_restar.UseVisualStyleBackColor = true;
            this.b_restar.Click += new System.EventHandler(this.B_restar_Click);
            // 
            // r_tpc_min
            // 
            this.r_tpc_min.ForeColor = System.Drawing.Color.Green;
            this.r_tpc_min.Location = new System.Drawing.Point(480, 50);
            this.r_tpc_min.Name = "r_tpc_min";
            this.r_tpc_min.Size = new System.Drawing.Size(65, 20);
            this.r_tpc_min.TabIndex = 85;
            this.r_tpc_min.Text = "0";
            this.r_tpc_min.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // r_tpc_max
            // 
            this.r_tpc_max.ForeColor = System.Drawing.Color.Green;
            this.r_tpc_max.Location = new System.Drawing.Point(568, 50);
            this.r_tpc_max.Name = "r_tpc_max";
            this.r_tpc_max.Size = new System.Drawing.Size(65, 20);
            this.r_tpc_max.TabIndex = 86;
            this.r_tpc_max.Text = "0";
            this.r_tpc_max.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // b_conv_espectro
            // 
            this.b_conv_espectro.Image = ((System.Drawing.Image)(resources.GetObject("b_conv_espectro.Image")));
            this.b_conv_espectro.Location = new System.Drawing.Point(10, 111);
            this.b_conv_espectro.Name = "b_conv_espectro";
            this.b_conv_espectro.Size = new System.Drawing.Size(42, 42);
            this.b_conv_espectro.TabIndex = 87;
            this.b_conv_espectro.UseVisualStyleBackColor = true;
            this.b_conv_espectro.Click += new System.EventHandler(this.B_conv_espectro_Click);
            // 
            // b_buscar
            // 
            this.b_buscar.Image = ((System.Drawing.Image)(resources.GetObject("b_buscar.Image")));
            this.b_buscar.Location = new System.Drawing.Point(901, 6);
            this.b_buscar.Name = "b_buscar";
            this.b_buscar.Size = new System.Drawing.Size(42, 42);
            this.b_buscar.TabIndex = 88;
            this.b_buscar.UseVisualStyleBackColor = true;
            this.b_buscar.Click += new System.EventHandler(this.B_buscar_Click);
            // 
            // FITS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1002, 780);
            this.Controls.Add(this.b_buscar);
            this.Controls.Add(this.b_conv_espectro);
            this.Controls.Add(this.r_tpc_max);
            this.Controls.Add(this.r_tpc_min);
            this.Controls.Add(this.b_restar);
            this.Controls.Add(this.b_apilar);
            this.Controls.Add(this.b_exporta_imagen);
            this.Controls.Add(this.b_idioma);
            this.Controls.Add(this.b_indexar_sao);
            this.Controls.Add(this.r_indexar);
            this.Controls.Add(this.val_parametro);
            this.Controls.Add(this.lista_parametros);
            this.Controls.Add(this.ctype_2);
            this.Controls.Add(this.ctype_1);
            this.Controls.Add(this.reloj_de);
            this.Controls.Add(this.reloj_ar);
            this.Controls.Add(this.invertir_x);
            this.Controls.Add(this.raiz);
            this.Controls.Add(this.b_cota_sup);
            this.Controls.Add(this.b_cota_inf);
            this.Controls.Add(this.b_nan);
            this.Controls.Add(this.sel_HDU_ok);
            this.Controls.Add(this.b_exporta_cabeceras);
            this.Controls.Add(this.r_tabla);
            this.Controls.Add(this.r_imagen);
            this.Controls.Add(this.sel_tabla);
            this.Controls.Add(this.sel_imagen);
            this.Controls.Add(this.r_HDU);
            this.Controls.Add(this.sel_HDU);
            this.Controls.Add(this.r_h);
            this.Controls.Add(this.alta_resolucion);
            this.Controls.Add(this.r_med);
            this.Controls.Add(this.r_destandar);
            this.Controls.Add(this.r_max);
            this.Controls.Add(this.r_min);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.histo_ceros_tpc);
            this.Controls.Add(this.histo_ceros);
            this.Controls.Add(this.r_ceros);
            this.Controls.Add(this.panel_histograma);
            this.Controls.Add(this.v_acotar_min);
            this.Controls.Add(this.b_redibuja);
            this.Controls.Add(this.invertir_y);
            this.Controls.Add(this.r_acotar);
            this.Controls.Add(this.v_acotar_max);
            this.Controls.Add(this.normalizar);
            this.Controls.Add(this.r_operacion);
            this.Controls.Add(this.r_reg_leidos);
            this.Controls.Add(this.b_indexar_hyperleda);
            this.Controls.Add(this.r_cantidad);
            this.Controls.Add(this.r_de);
            this.Controls.Add(this.r_octetos);
            this.Controls.Add(this.r_leidos);
            this.Controls.Add(this.v_leidos);
            this.Controls.Add(this.tabla);
            this.Controls.Add(this.b_exporta_tabla);
            this.Controls.Add(this.lista_cabeceras);
            this.Controls.Add(this.b_leer_fits);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FITS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Principal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FITS_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tabla)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_histograma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reloj_ar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reloj_de)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button b_leer_fits;
        public System.Windows.Forms.ListBox lista_cabeceras;
        private System.Windows.Forms.Button b_exporta_tabla;
        private System.Windows.Forms.DataGridView tabla;
        private System.Windows.Forms.Label v_leidos;
        private System.Windows.Forms.Label r_leidos;
        private System.Windows.Forms.Label r_octetos;
        private System.Windows.Forms.Label r_de;
        private System.Windows.Forms.Label r_cantidad;
        private System.Windows.Forms.Button b_indexar_hyperleda;
        private System.Windows.Forms.Label r_operacion;
        private System.Windows.Forms.Label r_reg_leidos;
        public System.Windows.Forms.CheckBox normalizar;
        public System.Windows.Forms.TextBox v_acotar_max;
        private System.Windows.Forms.Label r_acotar;
        public System.Windows.Forms.CheckBox invertir_y;
        private System.Windows.Forms.Button b_redibuja;
        public System.Windows.Forms.TextBox v_acotar_min;
        public System.Windows.Forms.PictureBox panel_histograma;
        private System.Windows.Forms.Label r_ceros;
        public System.Windows.Forms.Label histo_ceros;
        public System.Windows.Forms.Label histo_ceros_tpc;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.Label r_min;
        public System.Windows.Forms.Label r_max;
        public System.Windows.Forms.Label r_destandar;
        public System.Windows.Forms.Label r_med;
        public System.Windows.Forms.CheckBox alta_resolucion;
        public System.Windows.Forms.Label r_h;
        private System.Windows.Forms.ComboBox sel_HDU;
        private System.Windows.Forms.Label r_HDU;
        private System.Windows.Forms.ComboBox sel_imagen;
        private System.Windows.Forms.ComboBox sel_tabla;
        private System.Windows.Forms.Label r_imagen;
        private System.Windows.Forms.Label r_tabla;
        private System.Windows.Forms.Button b_exporta_cabeceras;
        private System.Windows.Forms.Label sel_HDU_ok;
        private System.Windows.Forms.Button b_nan;
        private System.Windows.Forms.Button b_cota_inf;
        private System.Windows.Forms.Button b_cota_sup;
        public System.Windows.Forms.CheckBox raiz;
        public System.Windows.Forms.CheckBox invertir_x;
        public System.Windows.Forms.PictureBox reloj_ar;
        public System.Windows.Forms.PictureBox reloj_de;
        private System.Windows.Forms.Label ctype_1;
        private System.Windows.Forms.Label ctype_2;
        public System.Windows.Forms.ListBox lista_parametros;
        private System.Windows.Forms.Label val_parametro;
        private System.Windows.Forms.Label r_indexar;
        private System.Windows.Forms.Button b_indexar_sao;
        private System.Windows.Forms.Button b_idioma;
        private System.Windows.Forms.Button b_exporta_imagen;
        private System.Windows.Forms.Button b_apilar;
        private System.Windows.Forms.Button b_restar;
        public System.Windows.Forms.Label r_tpc_min;
        public System.Windows.Forms.Label r_tpc_max;
        private System.Windows.Forms.Button b_conv_espectro;
        private System.Windows.Forms.Button b_buscar;
    }
}

