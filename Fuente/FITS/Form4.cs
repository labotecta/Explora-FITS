using ExploraFITS;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExploraFits
{
    public partial class Form4 : Form
    {
        public FITS fits;

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            CambiaIdioma();
        }
        private void CambiaIdioma()
        {
            b_portapapeles.Text = Idioma.msg[Idioma.lengua, 26];
        }
        public void Datos(string[] rotulos, string[] datos)
        {
            string[] sd;
            string[] fila = new string[4];
            IniciaTabla();
            for (int i = 0; i < datos.Length; i++)
            {
                if (!string.IsNullOrEmpty(datos[i].Trim()))
                {
                    sd = rotulos[i].Split(';');
                    fila[0] = sd[0];
                    fila[1] = datos[i].Trim();
                    fila[2] = sd[2];
                    fila[3] = sd[3];
                    tabla.Rows.Add(fila);
                }
            }
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
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            tabla.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            tabla.AllowUserToResizeRows = false;
            tabla.AllowUserToResizeColumns = false;
            tabla.ColumnHeadersVisible = true;
            tabla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            tabla.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            tabla.DefaultCellStyle.SelectionBackColor = Color.LightSalmon;
            tabla.RowHeadersVisible = false;
            tabla.ScrollBars = ScrollBars.Both;
            int tabla_alto_fila = tabla.RowTemplate.Height;
            int tabla_alto_cabecera = tabla.ColumnHeadersHeight;
            if (tabla_alto_cabecera <= tabla_alto_fila) tabla_alto_cabecera = tabla_alto_fila + 1;
            tabla.RowTemplate.Height = tabla_alto_fila;
            tabla.ColumnHeadersHeight = tabla_alto_cabecera;
            tabla.ColumnCount = 4;
            int j = 0;
            tabla.Columns[j].FillWeight = 10;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            tabla.Columns[j].Name = Idioma.msg[Idioma.lengua, 22];
            j++;
            tabla.Columns[j].FillWeight = 10;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tabla.Columns[j].Name = Idioma.msg[Idioma.lengua, 23];
            j++;
            tabla.Columns[j].FillWeight = 10;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tabla.Columns[j].Name = Idioma.msg[Idioma.lengua, 24];
            j++;
            tabla.Columns[j].FillWeight = 80;
            tabla.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            tabla.Columns[j].Name = Idioma.msg[Idioma.lengua, 25];
            tabla.RowCount = 0;
        }
        private void B_portapapeles_Click(object sender, System.EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tabla.RowCount; i++)
            {
                sb.AppendFormat(string.Format("{0};{1};{2};{3}\n", tabla.Rows[i].Cells[0].Value.ToString(), tabla.Rows[i].Cells[1].Value.ToString(), tabla.Rows[i].Cells[2].Value.ToString(), tabla.Rows[i].Cells[3].Value.ToString()));
            }
            Clipboard.SetText(sb.ToString());
            Console.Beep();
        }
    }
}
