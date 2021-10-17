using System;
using System.Drawing;
using ExploraFITS;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ExploraFits
{
    public partial class Form6 : Form
    {
        public FITS fits;
        private List<int> indices;
        public Form6()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            IniciaTabla();
            label1.Text = Idioma.msg[Idioma.lengua, 121];
            label2.Text = Idioma.msg[Idioma.lengua, 122];
            label3.Text = Idioma.msg[Idioma.lengua, 124];
            label4.Text = Idioma.msg[Idioma.lengua, 125];
            label5.Text = Idioma.msg[Idioma.lengua, 125];
            r_arseg.Text = Idioma.msg[Idioma.lengua, 123];
        }
        private void IniciaTabla()
        {
            tabla.BackgroundColor = Color.LightGray;
            tabla.BorderStyle = BorderStyle.Fixed3D;
            tabla.ReadOnly = true;
            tabla.MultiSelect = false;
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.AllowUserToOrderColumns = false;
            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabla.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            tabla.AllowUserToResizeRows = false;
            tabla.AllowUserToResizeColumns = false;
            tabla.ColumnHeadersVisible = true;
            tabla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            tabla.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            tabla.DefaultCellStyle.SelectionBackColor = Color.LightSalmon;
            tabla.RowHeadersVisible = false;
            int tabla_alto_fila = tabla.RowTemplate.Height;
            int tabla_alto_cabecera = tabla.ColumnHeadersHeight;
            if (tabla_alto_cabecera <= tabla_alto_fila) tabla_alto_cabecera = tabla_alto_fila + 1;
            tabla.RowTemplate.Height = tabla_alto_fila;
            tabla.ColumnHeadersHeight = tabla_alto_cabecera;
            tabla.ColumnCount = 4;
            int j = 0;
            tabla.Columns[j].FillWeight = 5;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            tabla.Columns[j].Name = "n";
            j++;
            tabla.Columns[j].FillWeight = 15;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tabla.Columns[j].Name = Idioma.msg[Idioma.lengua, 120];
            j++;
            tabla.Columns[j].FillWeight = 10;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tabla.Columns[j].Name = Idioma.msg[Idioma.lengua, 121];
            j++;
            tabla.Columns[j].FillWeight = 10;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tabla.Columns[j].Name = Idioma.msg[Idioma.lengua, 122];
            tabla.RowCount = 0;
        }
        public void Datos(double ar, double de, double margen)
        {
            v_ar.Text = ar.ToString();
            v_de.Text = de.ToString();
            v_margen.Text = margen.ToString();
            sel_unidades_ar.Items.Clear();
            sel_unidades_ar.Items.Add(Idioma.msg[Idioma.lengua, 117]);
            sel_unidades_ar.Items.Add(Idioma.msg[Idioma.lengua, 118]);
            sel_unidades_ar.SelectedIndex = 0;
            sel_unidades_de.Items.Clear();
            sel_unidades_de.Items.Add(Idioma.msg[Idioma.lengua, 117]);
            sel_unidades_de.Items.Add(Idioma.msg[Idioma.lengua, 118]);
            sel_unidades_de.SelectedIndex = 0;

            sel_catalogo.Items.Clear();
            sel_catalogo.Items.Add(Idioma.msg[Idioma.lengua, 27]);
            sel_catalogo.Items.Add(Idioma.msg[Idioma.lengua, 28]);
            sel_catalogo.Items.Add(Idioma.msg[Idioma.lengua, 29]);
            sel_catalogo.SelectedIndex = 0;
        }
        private void B_buscar_Click(object sender, EventArgs e)
        {
            int j = sel_catalogo.SelectedIndex;
            if (j < 1)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 119], Idioma.msg[Idioma.lengua, 77], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            switch (j)
            {
                case 1:
                    if (fits.indices_hyperleda == null || fits.indices_hyperleda.Count == 0)
                    {
                        fits.Disponible(false);
                        if (!fits.LeeIndicesHyperleda())
                        {
                            fits.Disponible(true);
                            return;
                        }
                        fits.Disponible(true);
                    }
                    break;
                default:
                    if (fits.indices_sao == null || fits.indices_sao.Count == 0)
                    {
                        fits.Disponible(false);
                        if (!fits.LeeIndicesSao())
                        {
                            fits.Disponible(true);
                            return;
                        }
                        fits.Disponible(true);
                    }
                    break;
            }
            double ar;
            double de;
            double margen = Convert.ToDouble(v_margen.Text.Replace(fits.s_millar, fits.s_decimal)) / 60;
            ar = Convert.ToDouble(v_ar.Text.Replace(fits.s_millar, fits.s_decimal));
            double ari = ar - margen;
            double arf = ar + margen;
            int n;
            int ri;
            int rf;
            switch (j)
            {
                case 1:
                    n = fits.indices_hyperleda.Count;
                    ri = fits.panel_img.BuscarAR(fits.indices_hyperleda, ari);
                    rf = fits.panel_img.BuscarAR(fits.indices_hyperleda, arf);
                    break;
                default:
                    n = fits.indices_sao.Count;
                    ri = fits.panel_img.BuscarAR(fits.indices_sao, ari);
                    rf = fits.panel_img.BuscarAR(fits.indices_sao, arf);
                    break;
            }
            de = Convert.ToDouble(v_de.Text.Replace(fits.s_millar, fits.s_decimal));
            double dei = de - margen;
            double def = de + margen;
            int indice;
            string[] fila = new string[4];
            tabla.RowCount = 0;
            indices = new List<int>();
            if (rf >= ri && rf < n)
            {
                n = 0;
                switch (j)
                {
                    case 1:
                        for (int i = ri; i <= rf; i++)
                        {
                            ar = fits.indices_hyperleda[i].al2000;
                            de = fits.indices_hyperleda[i].de2000;
                            if (ar >= ari && ar <= arf && de >= dei && de <= def)
                            {
                                fila[0] = n.ToString();
                                indice = fits.indices_hyperleda[i].indice;
                                indices.Add(indice);
                                fila[1] = fits.panel_img.NombreMarcaHyperleda(indice);
                                fila[2] = string.Format("{0:f3}", ar);
                                fila[3] = string.Format("{0:f3}", de);
                                tabla.Rows.Add(fila);
                                n++;
                            }
                        }
                        break;
                    default:
                        for (int i = ri; i <= rf; i++)
                        {
                            ar = fits.indices_sao[i].al2000;
                            de = fits.indices_sao[i].de2000;
                            if (ar >= ari && ar <= arf && de >= dei && de <= def)
                            {
                                fila[0] = n.ToString();
                                indice = fits.indices_sao[i].indice;
                                indices.Add(indice);
                                fila[1] = fits.panel_img.NombreMarcaSAO(indice);
                                fila[2] = string.Format("{0:f3}", ar);
                                fila[3] = string.Format("{0:f3}", de);
                                tabla.Rows.Add(fila);
                                n++;
                            }
                        }
                        break;
                }
            }
            if (tabla.RowCount == 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 83], Idioma.msg[Idioma.lengua, 77], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Tabla_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int j = sel_catalogo.SelectedIndex;
            if (j < 1) return;
            int ind = indices[e.RowIndex];
            switch (j)
            {
                case 1:
                    fits.panel_img.FichaHyperleda(ind);
                    break;
                default:
                    fits.panel_img.FichaSAO(ind);
                    break;
            }
        }
    }
}
