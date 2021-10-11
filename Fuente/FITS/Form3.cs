using ExploraFits;
using System;
using System.Windows.Forms;

namespace ExploraFITS
{
    public partial class Form3 : Form
    {
        public FITS fits;

        public Form3()
        {
            InitializeComponent();
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            CambiaIdioma();
        }
        private void CambiaIdioma()
        {
            r_pixel.Text = Idioma.msg[Idioma.lengua, 18];
            r_arydec.Text = Idioma.msg[Idioma.lengua, 19];
            r_delta.Text = Idioma.msg[Idioma.lengua, 20];
            b_omite.Text = Idioma.msg[Idioma.lengua, 21];
        }
        public void Inicializa(double v_crpix_1, double v_crpix_2, double v_crval_1, double v_crval_2, double v_cdelt_1, double v_cdelt_2)
        {
            crpix_1.Text = v_crpix_1.ToString();
            crpix_2.Text = v_crpix_2.ToString();
            crval_1.Text = v_crval_1.ToString();
            crval_2.Text = v_crval_2.ToString();
            cdelt_1.Text = v_cdelt_1.ToString();
            cdelt_2.Text = v_cdelt_2.ToString();
        }
        private void B_cancela_Click(object sender, EventArgs e)
        {
            fits.f_referencia_cancelado = true;
            Dispose();
        }
        private void B_ok_Click(object sender, EventArgs e)
        {
            if (crpix_1.Text.Trim().Length == 0 ||
                crpix_2.Text.Trim().Length == 0 ||
                crval_1.Text.Trim().Length == 0 ||
                crval_2.Text.Trim().Length == 0 ||
                cdelt_1.Text.Trim().Length == 0 ||
                cdelt_2.Text.Trim().Length == 0
            )
            {
                MessageBox.Show("Hay que rellenar todos los datos");
                return;
            }
            fits.f_referencia_cancelado = false;
            fits.f_referencia_crpix1 = Convert.ToDouble(crpix_1.Text.Trim());
            fits.f_referencia_crpix2 = Convert.ToDouble(crpix_2.Text.Trim());
            fits.f_referencia_crval1 = Convert.ToDouble(crval_1.Text.Trim());
            fits.f_referencia_crval2 = Convert.ToDouble(crval_2.Text.Trim());
            fits.f_referencia_cdelt1 = Convert.ToDouble(cdelt_1.Text.Trim());
            fits.f_referencia_cdelt2 = Convert.ToDouble(cdelt_2.Text.Trim());
            Dispose();
        }
    }
}
