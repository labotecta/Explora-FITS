using ExploraFits;
using ExploraFits.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ExploraFITS
{
    public partial class FITS : Form
    {
        private const int BLOQUE = 2880;
        public const int X1M = 1000000;
        public const int X10M = 10000000;
        private const int MAX_IMAGENES = 128;
        private const int MAX_TABLAS = 128;
        private const int MAX_SUBCOL = 32;

        public char s_decimal;
        public char s_millar;
        public class HDU
        {
            public string nombre;
            public int puntero_ini;
            public int n_imagenes;
            public int n_tablas;
            public int byPorDato;
            public int clase_dato;
            public int conjunto;
            public bool normalizar;
            public int ind_color_nan;
            public string cota_inf;
            public int ind_color_cota_inf;
            public string cota_sup;
            public int ind_color_cota_sup;
            public bool raiz;
            public bool invertir_y;
            public bool invertir_x;
            public bool alta_resolucion;
            public HDU(string nombre, int puntero_ini, int n_imagenes, int n_tablas)
            {
                this.nombre = nombre;
                this.puntero_ini = puntero_ini;
                this.n_imagenes = n_imagenes;
                this.n_tablas = n_tablas;
                normalizar = true;
                ind_color_nan = 0;
                cota_inf = string.Empty;
                ind_color_cota_inf = 0;
                cota_sup = string.Empty;
                ind_color_cota_sup = 0;
                raiz = false;
                invertir_y = false;
                invertir_x = false;
                alta_resolucion = false;
            }
        }
        public List<HDU> hdu;
        public int hdu_actual;

        public string FICHERO_FITS;
        public class ParUtiles
        {
            public int NAXIS;
            public int[] NAXISn;
            public string FILENAME;
            public int punto_referencia;
            public int[] proyeccion = new int[3];
            public double[] CRPIX;
            public double[] CRVAL;
            public string[] CUNIT;
            public string[] CTYPE;
            public double[] CDELT;
            public double[] CROTA;
            public double[] CD1 = new double[2];
            public double[] CD2 = new double[2];
            public double[] PC1 = new double[2];
            public double[] PC2 = new double[2];
            public string XTENSION;
            public int TFIELDS;
            public string[] TTYPE;
            public int[] TBCOL;
            public string[] TFORM;
            public string[] TUNIT;
        }

        public bool SIMPLE; // T o F conformidad con el estandar
        /*
          8	Character or unsigned binary integer
         16	16-bit twos-complement binary integer
         32	32-bit twos-complement binary integer
        -32	IEEE single precision floating point
        -64	IEEE double precision floating point
        */
        public int BITPIX;
        public int NAXIS;
        public int[] NAXISn;
        public string FILENAME;
        public int[] proyeccion = new int[3];
        public int punto_referencia;
        public string[] CTYPE; // rootulo de la coordenada asociada a cada eje 
        public double[] CRPIX; // Localización (en unidades del eje) de un punto de referencia sobre el eje n, es un indice de 1 a NAXISn (número de pixel empezando en 1)
        public double[] CRVAL; // Valor de la coordenada del correspondiente punto CRPIX
        public string[] CUNIT;
        public double[] CDELT;
        public double[] CROTA;
        /*
        Many astronomical instruments provide image files in which the 'x' and 'y' coordinate axes are not orientated with equatorial north corresponding to 'up'
        (and east == 'left'). According to WCS, there are three options for the scale and rotation:
        Historically, CDELT1 and CDELT2 have been used to indicate the plate scale in degrees per pixel and CROTA2 has been used to indicate the rotation of
        the horizontal and vertical axes in degrees. Usually the axes rotate together and CROTA2 is used to indicate that angle in degrees.
        The FITS WCS standard uses a rotation matrix, CD1_1, CD1_2, CD2_1, and CD2_2 to indicate both rotation and scale, allowing a more intuitive computation if
        the axes are skewed. This model has been used by HST and IRAF for several years.        
        
        CD1_1 = CDELT1 * cos (CROTA2)
        CD1_2 = -CDELT2 * sin (CROTA2)
        CD2_1 = CDELT1 * sin (CROTA2)
        CD2_2 = CDELT2 * cos (CROTA2)
        */
        public readonly double[] CD1 = new double[2];
        public readonly double[] CD2 = new double[2];
        public readonly double[] PC1 = new double[2];
        public readonly double[] PC2 = new double[2];
        /*
        XTENSION
        IMAGE
        BINTABLE (binary table)
        TABLE (ASCII table)
         */

        // Random Access Groups. NAXIS1 es siempre 0
        // Esta aplicación no acepta este formato, hoy se prefieren las tablas binarias.

        public int PCOUNT;  // Número de parámetros
        public int GCOUNT;  // Número de grupos

        public string XTENSION;
        public int TFIELDS;    // Número de campos
        public string[] TTYPE; // Nombre de columna
        public int[] TBCOL;    // Posición de inicio de la columna
        /*
        TFORMn
        (r = vector length, default = 1)
            rA  - character string
            rAw - array of strings, each of length w
            rL  - logical
            rX  - bit
            rB  - unsigned byte
            rS  - signed byte **
            rI  - signed 16-bit integer
            rU  - unsigned 16-bit integer **
            rJ  - signed 32-bit integer
            rV  - unsigned 32-bit integer **
            rK  - signed 64-bit integer
            rE  - 32-bit floating point
            rD  - 64-bit floating point
            rC  - 32-bit complex pair
            rM  - 64-bit complex pair
        */
        public string[] TFORM; // Formato (Fortran)
        public string[] TUNIT; // Rotulo de unidades
        private const int MAX_COL = 512;
        private readonly int[] des_columna = new int[MAX_COL];

        private int axis_pendientes;
        private readonly SortedList<string, string> parametros = new SortedList<string, string>();

        private long num_octetos;
        private byte[] octetos;

        /*
         float:  A quiet NaN is represented by any bit pattern between X'7FC0 0000' and X'7FFF FFFF' or
         between X'FFC0 0000' and X'FFFF FFFF'
         double: A quiet NaN is represented by any bit pattern between X'7FF80000 00000000' and X'7FFFFFFF FFFFFFFF' or
         between X'FFF80000 00000000' and X'FFFFFFFF FFFFFFFF'
        */

        // Habría que sustituirlos por hdu[ihdu].

        public int tipoDato;
        public int byPorDato;

        private object[,] datos;
        public byte[,] datosb;
        public short[,] datoss;
        public int[,] datosi;
        public float[,] datosf;
        public double[,] datosd;
        public long[,] datosl;

        private object[,,] datos3;
        public byte[,,] datosb3;
        public short[,,] datoss3;
        public int[,,] datosi3;
        public float[,,] datosf3;
        public double[,,] datosd3;
        public long[,,] datosl3;

        private int ind_color_nan;
        public Color color_nan;
        private int ind_color_cota_inf;
        public Color color_cota_inf;
        private int ind_color_cota_sup;
        public Color color_cota_sup;
        private readonly Pen lapiz_circulo = new Pen(Brushes.Black, 2);
        private readonly Pen lapiz_ejes = new Pen(Brushes.Black, 1);
        private readonly Pen lapiz_arco = new Pen(Brushes.Red, 10);
        private Rectangle rectangulo_reloj;

        private Bitmap img_histogramas;
        private const int his_margen_izq = 0;
        private const int his_margen_dch = 0;
        private const int his_margen_sup = 8;
        private const int his_margen_inf = 0;
        public int num_histogramas;
        public int[] histograma;
        public Bitmap img;

        private const double MIN_DEFECTO = 0.25;
        private const double RANGO_X = 0.02;
        private const int MIN_RANGO_X = 3;
        private const int DIF_MIN_ENTORNO = 3;
        private readonly RegresionLineal regLin = new RegresionLineal();

        public Form2 panel_img;

        public string fichero_hyperleda;
        private const string hyperleda_obj_tipo = "objtype";
        private const string hyperleda_nombre_ar = "al2000";
        private const string hyperleda_nombre_de = "de2000";
        private int hyperleda_col_tipo;
        private int hyperleda_col_ar;
        private int hyperleda_col_de;
        public class IndiceCatalogo
        {
            public int indice;
            public byte tipo;
            public double al2000;
            public double de2000;
            public IndiceCatalogo(int indice, byte tipo, double al2000, double de2000)
            {
                // al2000 y de2000 deben estár en grados

                this.indice = indice;
                this.tipo = tipo;
                this.al2000 = al2000;
                this.de2000 = de2000;
            }
        }
        public readonly string[,] hyperleda_tipos =
        {
            {"G","Galaxy or quasar"},
            {"Q","QSO"},
            {"g","Extended source"},
            {"c","Star cluster"},
            {"r","Supernova remnant"},
            {"h","HII region"},
            {"n","Gaseous nebula"},
            {"P","Planetary nebula"},
            {"S","Star"},
            {"?","Extended source of unknown/uncertain nature"},
            {"!","Plate defect or transcient/moving object"},
            {"sn","Supernova"},
            {"M","Multiple galaxy"},
            {"R","Radiosource"},
            {"M2","Pair of galaxies"},
            {"M3","Triple galaxy"},
            {"MG","Group of galaxies"},
            {"MC","Cluster of galaxies"},
            {"u","Non-existent object"},
            {"rn","Reflexion Nebulae"},
            {"S2","Double star"},
            {"S3","Triple star"},
            {"a","Stellar association"},
            {"PG","Part of galaxy"},
            {"HI","HI source"},
            {"S+","Group of stars Asterism."},
            {"gc","Globular Cluster"},
            {"oc","Open cluster"},
            {"kn","kilonova"}
        };
        public List<IndiceCatalogo> indices_hyperleda = null;

        public string fichero_sao;
        public List<IndiceCatalogo> indices_sao = null;

        public Form3 f_referencia;
        public bool f_referencia_cancelado;
        public double f_referencia_crpix1;
        public double f_referencia_crpix2;
        public double f_referencia_crval1;
        public double f_referencia_crval2;
        public double f_referencia_cdelt1;
        public double f_referencia_cdelt2;
        public Form4 f_ficha_hyperleda;
        public Form5 f_ficha_sao;

        private bool primeravez;
        public string sendaApp = string.Empty;
        private const string FICHERO_IDIOMAS = "idiomas.txt";
        private string VERSIONAPP;
        private Bitmap imguk = null;
        private Bitmap imgspain = null;

        public class Superficie
        {
            public double xizq_s;
            public double xdch_s;
            public double ysup_s;
            public double yinf_s;
            public double deltax_s;
            public double deltay_s;
            public double[] xizq;
            public double[] xdch;
            public double[] ysup;
            public double[] yinf;
            public double[] deltax;
            public double[] deltay;
            public Superficie(int n, double xizq_s, double xdch_s, double ysup_s, double yinf_s, double deltax_s, double deltay_s)
            {
                this.xizq_s = xizq_s;
                this.xdch_s = xdch_s;
                this.ysup_s = ysup_s;
                this.yinf_s = yinf_s;
                this.deltax_s = deltax_s;
                this.deltay_s = deltay_s;
                xizq = new double[n];
                xdch = new double[n];
                ysup = new double[n];
                yinf = new double[n];
                deltax = new double[n];
                deltay = new double[n];
            }
        }
        public class Espectro
        {
            public double[] x;
            public double[] y;
            public double minx;
            public double maxx;
            public double miny;
            public double maxy;
            public int dim_x;
            public int dim_y;
            public int util_x;
            public int util_y;
            public int mizq;
            public int mdch;
            public int msup;
            public int minf;
            public double escala_x;
            public double escala_y;
            public Espectro(double[] x, double[] y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public Espectro es_actual;
        public class EspectrosAtomicos
        {
            public int intensidad;
            public double longitud_onda;
            public string elemento;
            public string isotopo;
            public EspectrosAtomicos(int intensidad, double longitud_onda, string elemento, string isotopo)
            {
                this.intensidad = intensidad;
                this.longitud_onda = longitud_onda;
                this.elemento = elemento;
                this.isotopo = isotopo;
            }
        }
        public readonly List<EspectrosAtomicos> lineas_atomicas = new List<EspectrosAtomicos>();
        public readonly List<string> lista_elegibles_principal = new List<string>();

        public FITS()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Assembly thisExe = Assembly.GetExecutingAssembly();
            imguk = new Bitmap(thisExe.GetManifestResourceStream("ExploraFits.Imagenes.ingles.png"));
            imgspain = new Bitmap(thisExe.GetManifestResourceStream("ExploraFits.Imagenes.spain.png"));
            s_decimal = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            s_millar = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator);
            sendaApp = Application.StartupPath;
            primeravez = Settings.Default.primeravez;
            Idioma.lengua = Settings.Default.idioma;
            if (MensajesPorIdioma() == -1) Environment.Exit(-1);
            if (Idioma.lengua == -1) Idioma.lengua = 0;
            if (primeravez == true)
            {
                if (CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.Equals("spa", StringComparison.OrdinalIgnoreCase) == true)
                {
                    // Español

                    Idioma.lengua = 0;
                }
                else
                {
                    // Inglés

                    Idioma.lengua = 1;
                }
                MessageBox.Show(Idioma.msg[Idioma.lengua, 1], Idioma.msg[Idioma.lengua, 2], MessageBoxButtons.OK, MessageBoxIcon.Information);
                Settings.Default.primeravez = false;
                Settings.Default.idioma = Idioma.lengua;
                Settings.Default.Save();
            }
            VERSIONAPP = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Text = string.Format(Idioma.msg[Idioma.lengua, 0], VERSIONAPP, string.Empty);
            Location = new Point(Screen.PrimaryScreen.Bounds.Width - Width, Screen.PrimaryScreen.Bounds.Height - Height);
            Bandera();
            CambiaIdioma();
            FICHERO_FITS = string.Empty;
            num_histogramas = (panel_histograma.Width - his_margen_izq - his_margen_dch - 1) / 2;
            histograma = new int[num_histogramas];
            rectangulo_reloj = new Rectangle(5, 5, reloj_ar.Width - 12, reloj_ar.Height - 12);
            ind_color_nan = 0;
            Actualiza_b_nan();
            ind_color_cota_inf = 0;
            Actualiza_b_color_inf();
            ind_color_cota_sup = 0;
            Actualiza_b_color_sup();
            LeeEspectrosAtomicos();
            panel_img = new Form2
            {
                principal = this
            };
            panel_img.Show();
            IniciaParametros();
        }
        private void FITS_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrEmpty(FICHERO_FITS))
            {
                // Actualiza el previo

                CreaCFG(FICHERO_FITS);
            }
        }
        public static void SalvaCFG()
        {
            Settings.Default.primeravez = false;
            Settings.Default.idioma = Idioma.lengua;
            Settings.Default.Save();
        }
        private int MensajesPorIdioma()
        {
            try
            {
                // Mensajes en los distintos idiomas

                int ri;
                if ((ri = Idioma.LeeMensajes(Path.Combine(sendaApp, FICHERO_IDIOMAS))) != -1)
                {
                    MessageBox.Show(string.Format("Error ({0}) en el Fichero de Idionas", ri), "ExploraFITS");
                    return -1;
                }
            }
            catch { return -1; }
            return 0;
        }
        private void CambiaIdioma()
        {
            normalizar.Text = Idioma.msg[Idioma.lengua, 9];
            r_acotar.Text = Idioma.msg[Idioma.lengua, 10];
            r_leidos.Text = Idioma.msg[Idioma.lengua, 11];
            r_de.Text = Idioma.msg[Idioma.lengua, 12];
            r_ceros.Text = Idioma.msg[Idioma.lengua, 13];
            r_HDU.Text = Idioma.msg[Idioma.lengua, 14];
            r_imagen.Text = Idioma.msg[Idioma.lengua, 15];
            r_tabla.Text = Idioma.msg[Idioma.lengua, 16];
            r_indexar.Text = Idioma.msg[Idioma.lengua, 17];
            if (panel_img != null) panel_img.CambiaIdioma();
        }
        public void Bandera()
        {
            b_idioma.Image = Idioma.lengua switch
            {
                0 => imguk,
                _ => imgspain,
            };
        }
        public void Disponible(bool que)
        {
            normalizar.Enabled = que;
            v_acotar_min.Enabled = que;
            v_acotar_max.Enabled = que;
            b_nan.Enabled = que;
            b_cota_inf.Enabled = que;
            b_cota_sup.Enabled = que;
            raiz.Enabled = que;
            alta_resolucion.Enabled = que;
            invertir_y.Enabled = que;
            invertir_x.Enabled = que;
            b_leer_fits.Enabled = que;
            b_redibuja.Enabled = que;
            b_indexar_hyperleda.Enabled = que;
            b_indexar_sao.Enabled = que;
            b_exporta_fits.Enabled = que;
            b_exporta_imagen.Enabled = que;
            b_conv_espectro.Enabled = que;
            b_exporta_tabla.Enabled = que;
            b_exporta_cabeceras.Enabled = que;
            b_apilar.Enabled = que;
            b_restar.Enabled = que;
            lista_cabeceras.Enabled = que;
            lista_parametros.Enabled = que;
            tabla.Enabled = que;
            sel_HDU.Enabled = que;
            if (sel_imagen.Items.Count > 1) sel_imagen.Enabled = que;
            if (sel_tabla.Items.Count > 1) sel_tabla.Enabled = que;
            Cursor = que ? Cursors.Default : Cursors.WaitCursor;
            panel_img.b_ver_crpix.Enabled = que;
            panel_img.v_reduccion.Enabled = que;
            panel_img.v_margen_brillo.Enabled = que;
            panel_img.v_brillo.Enabled = que;
            panel_img.b_cimas.Enabled = que;
            panel_img.b_salva_cimas.Enabled = que;
            panel_img.sel_catalogo.Enabled = que;
            panel_img.b_volver.Enabled = que;
            panel_img.b_salvar.Enabled = que;
            panel_img.b_redibuja.Enabled = que;
            panel_img.b_ficha_hyperleda.Enabled = que;
            panel_img.b_ficha_sao.Enabled = que;
            if (!panel_img.escalando) panel_img.b_escalar.Enabled = que;
            panel_img.b_picos.Enabled = que;
            panel_img.S_simplifica.Enabled = que;
            panel_img.b_limpiar.Enabled = que;
            panel_img.v_hueco.Enabled = que;
            panel_img.v_z.Enabled = que;
            panel_img.lista_elegibles.Enabled = que; ;
            panel_img.lista_elegidas.Enabled = que; ;
            panel_img.Cursor = que ? Cursors.Default : Cursors.WaitCursor;
            Application.DoEvents();
        }
        public void LeeEspectrosAtomicos()
        {
            FileStream fe = new FileStream(Path.Combine(sendaApp, "lineas_atomicas.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader r = new StreamReader(fe);
            lineas_atomicas.Clear();
            lista_elegibles_principal.Clear();
            string linea;
            string[] sd;
            int intensidad;
            double longitud_onda;
            string elemento;
            string isotopo;
            while (!r.EndOfStream)
            {
                linea = r.ReadLine();
                sd = linea.Split('\t');
                intensidad = sd[2].Trim().Length == 0 ? 0 : Convert.ToInt32(sd[2]);
                longitud_onda = Convert.ToDouble(sd[3].Trim().Replace('.', ','));
                elemento = sd[4].Trim();
                isotopo = sd[4].Trim() + " " + sd[1].Trim();
                lista_elegibles_principal.Add(string.Format("{0,10:f3} {1} {2}", longitud_onda, isotopo, intensidad));
                lineas_atomicas.Add(new EspectrosAtomicos(intensidad, longitud_onda, elemento, isotopo));
            }
            r.Close();
        }
        private void B_idioma_Click(object sender, EventArgs e)
        {
            Idioma.lengua = 1 - Idioma.lengua;
            Settings.Default.idioma = Idioma.lengua;
            Settings.Default.Save();
            Bandera();
            CambiaIdioma();
        }
        private void B_exporta_imagen_Click(object sender, EventArgs e)
        {
            Disponible(false);
            int cHDU = hdu_actual = sel_HDU.SelectedIndex;
            if (hdu[cHDU].n_imagenes == 0) return;
            SaveFileDialog ficheroescritura = new SaveFileDialog()
            {
                Filter = "BIN (*.bin)|*.bin|TODO (*.*)|*.*",
                Title = Idioma.msg[Idioma.lengua, 136],
                FilterIndex = 1
            };
            if (ficheroescritura.ShowDialog() == DialogResult.OK)
            {
                FileStream fe = new FileStream(ficheroescritura.FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                BinaryWriter sw = new BinaryWriter(fe);

                // El tipo de datos (int32)

                sw.Write(hdu[cHDU].clase_dato);

                // Dos valores int32 con NAXISn[0] y NAXISn[1]

                sw.Write(NAXISn[0]);
                sw.Write(NAXISn[1]);

                // Seis valores double con CRPIX[0] = 1, CRPIX[1] = 1, CRVAL[0], CRVAL[1], CDELT[0], CDELT[1]

                double v = 1;
                sw.Write(v);
                sw.Write(v);
                sw.Write(panel_img.x_izq);
                sw.Write(panel_img.y_sup);
                v = (panel_img.x_dch - panel_img.x_izq) / NAXISn[0];
                sw.Write(v);
                v = (panel_img.y_inf - panel_img.y_sup) / NAXISn[1];
                sw.Write(v);
                int i2;
                int i1;
                if (NAXIS == 2)
                {
                    for (i2 = 0; i2 < NAXISn[1]; i2++)
                    {
                        for (i1 = 0; i1 < NAXISn[0]; i1++)
                        {
                            switch (hdu[cHDU].clase_dato)
                            {
                                case 1:
                                    sw.Write(datosb[i1, i2]);
                                    break;
                                case 2:
                                    sw.Write(datoss[i1, i2]);
                                    break;
                                case 3:
                                    sw.Write(datosi[i1, i2]);
                                    break;
                                case 4:
                                    sw.Write(datosf[i1, i2]);
                                    break;
                                case 6:
                                    sw.Write(datosl[i1, i2]);
                                    break;
                                case 5:
                                    sw.Write(datosd[i1, i2]);
                                    break;
                            }
                        }
                    }
                }
                else if (NAXIS == 3)
                {
                    int i3 = sel_imagen.SelectedIndex;
                    for (i2 = 0; i2 < NAXISn[1]; i2++)
                    {
                        for (i1 = 0; i1 < NAXISn[0]; i1++)
                        {
                            switch (hdu[cHDU].clase_dato)
                            {
                                case 1:
                                    sw.Write(datosb3[i1, i2, i3]);
                                    break;
                                case 2:
                                    sw.Write(datoss3[i1, i2, i3]);
                                    break;
                                case 3:
                                    sw.Write(datosi3[i1, i2, i3]);
                                    break;
                                case 4:
                                    sw.Write(datosf3[i1, i2, i3]);
                                    break;
                                case 6:
                                    sw.Write(datosl3[i1, i2, i3]);
                                    break;
                                case 5:
                                    sw.Write(datosd3[i1, i2, i3]);
                                    break;
                            }
                        }
                    }
                }
                sw.Close();
                Console.Beep();
            }
            if (es_actual != null)
            {
                ficheroescritura = new SaveFileDialog()
                {
                    Filter = "CSV (*.csv)|*.csv|TODO (*.*)|*.*",
                    Title = Idioma.msg[Idioma.lengua, 137],
                    FilterIndex = 1
                };
                if (ficheroescritura.ShowDialog() == DialogResult.OK)
                {
                    FileStream fet = new FileStream(ficheroescritura.FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                    StreamWriter swt = new StreamWriter(fet);
                    swt.WriteLine("W;F");
                    for (int i = 0; i < es_actual.x.Length; i++)
                    {
                        swt.WriteLine("{0};{1}", es_actual.x[i], es_actual.y[i]);
                    }
                    swt.Close();
                    Console.Beep();
                }
            }
            Disponible(true);
        }
        private void B_exporta_cabeceras_Click(object sender, EventArgs e)
        {
            if (lista_cabeceras.Items.Count == 0) return;
            SaveFileDialog ficheroescritura = new SaveFileDialog()
            {
                Filter = "TXT (*.txt)|*.txt|TODO (*.*)|*.*",
                FilterIndex = 1
            };
            if (ficheroescritura.ShowDialog() == DialogResult.OK)
            {
                FileStream fe = new FileStream(ficheroescritura.FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                StreamWriter sw = new StreamWriter(fe, Encoding.UTF8);
                for (int j = 0; j < lista_cabeceras.Items.Count; j++)
                {
                    sw.WriteLine(lista_cabeceras.Items[j].ToString());
                }
                sw.Close();
                Console.Beep();
            }
        }
        private void B_leer_fits_Click(object sender, EventArgs e)
        {
            OpenFileDialog ficherolectura = new OpenFileDialog()
            {
                Filter = "FITS |*.fits;*.fit|IMG |*.bmp;*.png;*.jpg;*.jpeg|BIN |*.bin|TODO |*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };
            if (ficherolectura.ShowDialog() == DialogResult.OK)
            {
                Disponible(false);
                LeeFichero(ficherolectura.FileName);
                Disponible(true);
            }
        }
        public string CreaFITS(string fichero_fte, string f_fits, bool extraer, bool coordenadas, string rotulo)
        {
            if (f_fits.Length == 0)
            {
                f_fits = Path.Combine(Path.GetDirectoryName(fichero_fte), Path.GetFileNameWithoutExtension(fichero_fte) + "_EF.fits");
            }
            FileStream fsfc = new FileStream(f_fits, FileMode.Create, FileAccess.Write, FileShare.None);
            if (fsfc != null)
            {
                // Escribe una cabecera mínima

                BinaryWriter sw = new BinaryWriter(fsfc);
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "SIMPLE", "T")));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "BITPIX", "-32")));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "NAXIS", "2")));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "NAXIS1", img.Width)));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "NAXIS2", img.Height)));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "COMMENT", rotulo)));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= '{1,-68}'", "FILENAME", Path.GetFileName(fichero_fte))));

                // 'nc' con una más por el END

                int nc = 8;
                if (coordenadas)
                {
                    nc += 6;
                    sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRPIX1", CRPIX[0])));
                    sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRPIX2", CRPIX[1])));
                    sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRVAL1", CRVAL[0])));
                    sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRVAL2", CRVAL[1])));
                    sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CDELT1", CDELT[0])));
                    sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CDELT2", CDELT[1])));
                }
                for (int i = 0; i < BLOQUE - nc * 80; i++)
                {
                    sw.Write((byte)32);
                }
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}  {1,-70}", "END", "")));

                // Extrae los datos de la imagen

                if (extraer) panel_img.ExtraeFlujo(img);

                // Escribe los datos

                r_leidos.Text = Idioma.msg[Idioma.lengua, 109];
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 106];
                r_octetos.Text = string.Format("{0:N0}", img.Height * img.Width);
                Application.DoEvents();

                bool le = BitConverter.IsLittleEndian;
                byte[] bytes_valor;
                int contador = 0;
                for (int i2 = 0; i2 < img.Height; i2++)
                {
                    for (int i1 = 0; i1 < img.Width; i1++)
                    {
                        if (contador % X1M == 0)
                        {
                            v_leidos.Text = string.Format("{0:N0}", contador);
                            Application.DoEvents();
                        }
                        if (le)
                        {
                            // Invertir

                            bytes_valor = BitConverter.GetBytes(datosf[i1, i2]);
                            Array.Reverse(bytes_valor);
                            sw.Write(bytes_valor);
                        }
                        else
                        {
                            sw.Write(datosf[i1, i2]);
                        }
                        contador++;
                    }
                }
                sw.Close();
            }
            return f_fits;
        }
        public string CreaFITS(string fichero_fte, string f_fits, string rotulo, string[] ficheros, float[,] dd)
        {
            if (f_fits.Length == 0)
            {
                f_fits = Path.Combine(Path.GetDirectoryName(fichero_fte), Path.GetFileNameWithoutExtension(fichero_fte) + "_EF.fits");
            }
            FileStream fsfc = new FileStream(f_fits, FileMode.Create, FileAccess.Write, FileShare.None);
            if (fsfc != null)
            {
                // Escribe una cabecera mínima

                BinaryWriter sw = new BinaryWriter(fsfc);
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "SIMPLE", "T")));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "BITPIX", "-32")));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "NAXIS", "2")));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "NAXIS1", dd.GetLength(0))));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "NAXIS2", dd.GetLength(1))));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "COMMENT", rotulo)));
                int nc = 6;
                if (ficheros != null && ficheros.Length > 0)
                {
                    for (int i = 0; i < ficheros.Length; i++)
                    {
                        sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "COMMENT", ficheros[i])));
                        nc++;
                    }
                }
                else
                {
                    sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= '{1,-68}'", "FILENAME", Path.GetFileName(fichero_fte))));
                    nc++;
                }
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRPIX1", CRPIX[0])));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRPIX2", CRPIX[1])));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRVAL1", CRVAL[0])));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CRVAL2", CRVAL[1])));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CDELT1", CDELT[0])));
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}= {1,-70}", "CDELT2", CDELT[1])));
                nc += 6;

                // 'nc' con una más por el END

                nc++;
                for (int i = 0; i < BLOQUE - nc * 80; i++)
                {
                    sw.Write((byte)32);
                }
                sw.Write(Encoding.ASCII.GetBytes(string.Format("{0,-8}  {1,-70}", "END", "")));

                // Escribe los datos

                r_leidos.Text = Idioma.msg[Idioma.lengua, 109];
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 106];
                r_octetos.Text = string.Format("{0:N0}", dd.GetLength(0) * dd.GetLength(1));
                Application.DoEvents();

                bool le = BitConverter.IsLittleEndian;
                byte[] bytes_valor;
                int contador = 0;
                for (int i2 = 0; i2 < dd.GetLength(1); i2++)
                {
                    for (int i1 = 0; i1 < dd.GetLength(0); i1++)
                    {
                        if (contador % X1M == 0)
                        {
                            v_leidos.Text = string.Format("{0:N0}", contador);
                            Application.DoEvents();
                        }
                        if (le)
                        {
                            // Invertir

                            bytes_valor = BitConverter.GetBytes(dd[i1, i2]);
                            Array.Reverse(bytes_valor);
                            sw.Write(bytes_valor);
                        }
                        else
                        {
                            sw.Write(dd[i1, i2]);
                        }
                        contador++;
                    }
                }
                sw.Close();
            }
            return f_fits;
        }
        private bool LeeFichero(string fichero)
        {
            if (!string.IsNullOrEmpty(FICHERO_FITS))
            {
                // Actualiza el previo

                CreaCFG(FICHERO_FITS);
            }
            if (panel_img == null || !panel_img.Visible)
            {
                panel_img = new Form2
                {
                    principal = this
                };
                panel_img.Show();
            }
            FileStream fs;
            string ext = Path.GetExtension(fichero);
            if (ext.Equals(".bin", StringComparison.OrdinalIgnoreCase))
            {
                fs = new FileStream(fichero, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return false;
                byte[] bytes_valor = new byte[8];
                NAXIS = 2;
                NAXISn = new int[2];
                fs.Read(bytes_valor, 0, 4);
                int clase_dato = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                NAXISn[0] = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                NAXISn[1] = BitConverter.ToInt32(bytes_valor);
                double[] val = new double[6];
                for (int k = 0; k < 6; k++)
                {
                    fs.Read(bytes_valor, 0, 8);
                    val[k] = BitConverter.ToDouble(bytes_valor);
                }
                CRPIX = new double[2];
                CRVAL = new double[2];
                CDELT = new double[2];
                CRPIX[0] = val[0];
                CRPIX[1] = val[1];
                CRVAL[0] = val[2];
                CRVAL[1] = val[3];
                CDELT[0] = val[4];
                CDELT[1] = val[5];
                datosf = new float[NAXISn[0], NAXISn[1]];
                for (int i2 = 0; i2 < NAXISn[1]; i2++)
                {
                    for (int i1 = 0; i1 < NAXISn[0]; i1++)
                    {
                        switch (clase_dato)
                        {
                            case 1:
                                fs.Read(bytes_valor, 0, 1);
                                datosf[i1, i2] = bytes_valor[0];
                                break;
                            case 2:
                                fs.Read(bytes_valor, 0, 2);
                                datosf[i1, i2] = BitConverter.ToInt16(bytes_valor);
                                break;
                            case 3:
                                fs.Read(bytes_valor, 0, 4);
                                datosf[i1, i2] = BitConverter.ToInt32(bytes_valor);
                                break;
                            case 4:
                                fs.Read(bytes_valor, 0, 4);
                                datosf[i1, i2] = BitConverter.ToSingle(bytes_valor);
                                break;
                            case 6:
                                fs.Read(bytes_valor, 0, 8);
                                datosl[i1, i2] = BitConverter.ToInt64(bytes_valor);
                                break;
                            default:
                                fs.Read(bytes_valor, 0, 8);
                                datosf[i1, i2] = (float)BitConverter.ToDouble(bytes_valor);
                                break;
                        }
                    }
                }
                fs.Close();
                FICHERO_FITS = fichero = CreaFITS(fichero, "", Idioma.msg[Idioma.lengua, 105], null, datosf);
                Text = string.Format(Idioma.msg[Idioma.lengua, 0], VERSIONAPP, FICHERO_FITS);
            }
            else if (
                ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
                )
            {
                // Crea un fichero FITS en la misma capeta y luego lo lee

                f_referencia = new Form3
                {
                    fits = this
                };
                f_referencia.ShowDialog(this);
                if (!f_referencia_cancelado)
                {
                    CRPIX = new double[2];
                    CRVAL = new double[2];
                    CDELT = new double[2];
                    CRPIX[0] = f_referencia_crpix1;
                    CRPIX[1] = f_referencia_crpix2;
                    CRVAL[0] = f_referencia_crval1;
                    CRVAL[1] = f_referencia_crval2;
                    CDELT[0] = f_referencia_cdelt1;
                    CDELT[1] = f_referencia_cdelt2;
                }
                img = new Bitmap(fichero);
                FICHERO_FITS = fichero = CreaFITS(fichero, "", true, !f_referencia_cancelado, Idioma.msg[Idioma.lengua, 94]);
                Text = string.Format(Idioma.msg[Idioma.lengua, 0], VERSIONAPP, FICHERO_FITS);
            }
            fs = new FileStream(fichero, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs != null)
            {
                // Lee a memoria todos los bytes del fichero

                num_octetos = fs.Length;
                if (num_octetos > 2147483591)
                {
                    fs.Close();
                    Form7 frac = new Form7();
                    frac.Text = Idioma.msg[Idioma.lengua, 4];
                    frac.label1.Text = Idioma.msg[Idioma.lengua, 131];
                    frac.fracciones.Text = "1000";
                    DialogResult respuesta = frac.ShowDialog(this);
                    if (respuesta == DialogResult.OK)
                    {
                        int ff = Convert.ToInt32(frac.nf);
                        frac.Dispose();
                        ExtraeFITS(fichero, ff);
                    }
                    else
                    {
                        frac.Dispose();
                    }
                    return false;
                }
                FICHERO_FITS = fichero;
                Text = string.Format(Idioma.msg[Idioma.lengua, 0], VERSIONAPP, FICHERO_FITS);
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 107];
                r_octetos.Text = string.Format("{0:N0}", num_octetos);
                octetos = new byte[num_octetos];
                fs.Read(octetos, 0, (int)num_octetos);
                fs.Close();

                IniciaParametros();
                sel_HDU.Items.Clear();
                sel_imagen.Items.Clear();
                sel_tabla.Items.Clear();

                // Explora los bytes leidos

                int res;
                int puntero = 0;
                r_leidos.Text = Idioma.msg[Idioma.lengua, 11];
                v_leidos.Text = string.Format("{0:N0}", puntero);
                Application.DoEvents();
                int cHDU = 0;
                hdu = new List<HDU>();
                hdu_actual = -1;
                while (puntero < num_octetos)
                {
                    HDU nueva_hdu = new HDU((cHDU + 1).ToString(), puntero, 0, 0);
                    res = ExploraFichero(cHDU, ref puntero, nueva_hdu);
                    if (res == -1)
                    {
                        MessageBox.Show(Idioma.msg[Idioma.lengua, 3], Idioma.msg[Idioma.lengua, 4], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        FICHERO_FITS = string.Empty;
                        return false;
                    }
                    else if (res == 1)
                    {
                        break;
                    }
                    if (NAXIS == 3)
                    {
                        nueva_hdu.n_imagenes *= NAXISn[2];
                        nueva_hdu.n_tablas *= NAXISn[2];
                    }
                    hdu.Add(nueva_hdu);
                    cHDU++;
                    v_leidos.Text = string.Format("{0:N0}", puntero);
                    Application.DoEvents();
                }
                foreach (HDU h in hdu)
                {
                    sel_HDU.Items.Add(h.nombre);
                }
                if (hdu.Count > 0)
                {
                    // Ver si existe fichero de configuración

                    string fc = Path.Combine(Path.GetDirectoryName(FICHERO_FITS), Path.GetFileNameWithoutExtension(FICHERO_FITS) + ".cfg");
                    if (File.Exists(fc))
                    {
                        // Se lee

                        FileStream fsfc = new FileStream(fc, FileMode.Open, FileAccess.Read, FileShare.Read);
                        if (fsfc != null)
                        {
                            StreamReader sr = new StreamReader(fsfc);
                            bool correcto = true;
                            string valor = string.Empty;
                            for (int i = 0; i < hdu.Count; i++)
                            {
                                correcto = ProcesaLineaCFG(sr, "NORMALIZAR", ref valor);
                                if (!correcto) break;
                                hdu[i].normalizar = valor.Equals("TRUE");
                                correcto = ProcesaLineaCFG(sr, "NAN", ref valor);
                                if (!correcto) break;
                                hdu[i].ind_color_nan = Convert.ToInt32(valor);
                                correcto = ProcesaLineaCFG(sr, "COTA_INF", ref valor);
                                if (!correcto) break;
                                hdu[i].cota_inf = valor;
                                correcto = ProcesaLineaCFG(sr, "NEGATIVOS", ref valor);
                                if (!correcto) break;
                                hdu[i].ind_color_cota_inf = Convert.ToInt32(valor);
                                correcto = ProcesaLineaCFG(sr, "COTA_SUP", ref valor);
                                if (!correcto) break;
                                hdu[i].cota_sup = valor;
                                correcto = ProcesaLineaCFG(sr, "SATURADOS", ref valor);
                                if (!correcto) break;
                                hdu[i].ind_color_cota_sup = Convert.ToInt32(valor);
                                correcto = ProcesaLineaCFG(sr, "INVERTIR_Y", ref valor);
                                if (!correcto) break;
                                hdu[i].invertir_y = valor.Equals("TRUE");
                                correcto = ProcesaLineaCFG(sr, "INVERTIR_X", ref valor);
                                if (!correcto) break;
                                hdu[i].invertir_x = valor.Equals("TRUE");
                                correcto = ProcesaLineaCFG(sr, "ALTA_RESOLUCION", ref valor);
                                if (!correcto) break;
                                hdu[i].alta_resolucion = valor.Equals("TRUE");
                                correcto = ProcesaLineaCFG(sr, "1/2", ref valor);
                                if (!correcto) break;
                                hdu[i].raiz = valor.Equals("TRUE");
                            }
                            sr.Close();
                            if (!correcto)
                            {
                                for (int i = 0; i < hdu.Count; i++)
                                {
                                    hdu[i].normalizar = true;
                                    hdu[i].cota_inf = string.Empty;
                                    hdu[i].cota_sup = string.Empty;
                                    hdu[i].ind_color_nan = ind_color_nan = 0;
                                    hdu[i].ind_color_cota_inf = ind_color_cota_inf = 0;
                                    hdu[i].ind_color_cota_sup = ind_color_cota_sup = 0;
                                    hdu[i].invertir_y = false;
                                    hdu[i].invertir_x = false;
                                    hdu[i].alta_resolucion = false;
                                    hdu[i].raiz = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Crea el fichero de configuración

                        CreaCFG(FICHERO_FITS);
                    }
                    sel_HDU.SelectedIndex = 0;
                }
                if (panel_img == null || !panel_img.Visible)
                {
                    panel_img = new Form2
                    {
                        principal = this
                    };
                }
                GC.Collect();
                panel_img.Show();
                MessageBox.Show(Idioma.msg[Idioma.lengua, 5], Idioma.msg[Idioma.lengua, 4], MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
        }
        private int ExploraFichero(int cHDU, ref int puntero, HDU nueva)
        {
            // Cabeceras

            string s;
            string[] sd;
            NAXIS = 0;
            BITPIX = 0;
            axis_pendientes = -1;
            bool hay_cabecera = false;
            do
            {
                s = LeeCabecera(octetos, ref puntero);
                if (s.Length > 0)
                {
                    hay_cabecera = true;

                    // Elimina comentarios al final de la línea

                    sd = s.Split('/');
                    s = sd[0];
                    if (s.StartsWith("END")) break;
                    if (s.StartsWith("XTENSION"))
                    {
                        sd = s.Split('=');
                        XTENSION = sd[1].Trim().Trim('\'').Trim();
                    }
                    else if (s.StartsWith("BITPIX "))
                    {
                        sd = s.Split('=');
                        BITPIX = Convert.ToInt32(sd[1]);
                    }
                    else if (s.StartsWith("NAXIS "))
                    {
                        sd = s.Split('=');
                        NAXIS = Convert.ToInt32(sd[1]);
                        int ejes_min;
                        if (NAXIS == 1)
                        {
                            // Tara tratarlo como [n, 1]

                            ejes_min = 2;
                        }
                        else
                        {
                            ejes_min = NAXIS;
                        }
                        NAXISn = new int[ejes_min];
                        axis_pendientes = NAXIS;
                    }
                    if (axis_pendientes > 0)
                    {
                        for (int i = 0; i < NAXIS; i++)
                        {
                            if (s.StartsWith(string.Format("NAXIS{0} ", i + 1)))
                            {
                                sd = s.Split('=');
                                NAXISn[i] = Convert.ToInt32(sd[1]);
                                axis_pendientes--;
                            }
                        }
                    }
                }
                if (puntero >= octetos.Length) break;
            } while (true);
            if (!hay_cabecera)
            {
                return 1;
            }
            // Ajustar el puntero al final del último bloque de cabeceras

            int bloques_leidos = puntero / BLOQUE;
            if (bloques_leidos * BLOQUE < puntero) bloques_leidos++;
            puntero = bloques_leidos * BLOQUE;

            // Datos

            if (NAXIS > 0 && BITPIX != 0)
            {
                if (XTENSION.Length == 0)
                {
                    // Imagen

                    nueva.conjunto = 0;
                }
                else
                {
                    switch (XTENSION.ToUpper())
                    {
                        case "IMAGE":
                            // Imagen

                            nueva.conjunto = 0;
                            break;
                        case "BINTABLE":
                            // Tabla binaria

                            nueva.conjunto = 1;
                            break;
                        case "TABLE":
                            // Tabla ASCII

                            nueva.conjunto = 2;
                            break;
                        default:
                            return -1;
                    }
                }
                int ndatos = 1;
                for (int i = 0; i < NAXIS; i++) ndatos *= NAXISn[i];
                if (ndatos == 0) return 0;
                if (BITPIX > 0)
                {
                    // Enteros

                    tipoDato = 0;
                }
                else
                {
                    // Punto flotante

                    tipoDato = 1;
                }
                byPorDato = Math.Abs(BITPIX / 8);
                nueva.byPorDato = byPorDato;
                if (tipoDato == 0 && byPorDato == 1)
                {
                    nueva.clase_dato = 1;
                }
                else if (tipoDato == 0 && byPorDato == 2)
                {
                    nueva.clase_dato = 2;
                }
                else if (tipoDato == 0 && byPorDato == 4)
                {
                    nueva.clase_dato = 3;
                }
                else if (tipoDato == 1 && byPorDato == 4)
                {
                    nueva.clase_dato = 4;
                }
                else if (tipoDato == 1 && byPorDato == 8)
                {
                    nueva.clase_dato = 5;
                }
                else if (tipoDato == 0 && byPorDato == 8)
                {
                    nueva.clase_dato = 6;
                }
                else
                {
                    MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 7], cHDU, tipoDato, byPorDato), Idioma.msg[Idioma.lengua, 6], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return -1;
                }
                if (NAXIS == 1)
                {
                    NAXIS = 2;
                    NAXISn[1] = 1;
                }
                if (NAXIS == 2 || NAXIS == 3)
                {
                    if (nueva.conjunto == 0)
                    {
                        // Imagen

                        nueva.n_imagenes++;
                        puntero += ndatos * byPorDato;
                    }
                    else
                    {
                        // Tabla

                        nueva.n_tablas++;
                        puntero += ndatos * byPorDato;
                    }
                    AjustaPuntero(ref puntero);
                }
                else
                {
                    MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 8], cHDU, NAXIS), Idioma.msg[Idioma.lengua, 6], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    puntero += ndatos * byPorDato;
                }
            }
            return 0;
        }
        private bool LeeHDU(int cHDU, int puntero)
        {
            hdu_actual = cHDU;
            IniciaParametros();

            // Cabeceras

            string linea;
            axis_pendientes = -1;
            do
            {
                linea = LeeCabecera(octetos, ref puntero).Trim();
                if (linea.Length > 0)
                {
                    lista_cabeceras.Items.Add(string.Format("{0}", linea));
                    if (linea.StartsWith("END"))
                    {
                        break;
                    }
                    else
                    {
                        if (!InterpretaCabecera(cHDU, linea))
                        {
                            MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 128], cHDU, linea), Idioma.msg[Idioma.lengua, 4], MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                }
                if (puntero >= octetos.Length) break;
            } while (true);
            foreach (KeyValuePair<string, string> p in parametros)
            {
                lista_parametros.Items.Add(p.Key);
            }
            Application.DoEvents();
            if (GCOUNT > 1 || PCOUNT > 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 128], Idioma.msg[Idioma.lengua, 4], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Ajustar el puntero al final del último bloque de cabeceras

            int bloques_leidos = puntero / BLOQUE;
            if (bloques_leidos * BLOQUE < puntero) bloques_leidos++;
            puntero = bloques_leidos * BLOQUE;

            if (NAXIS == 1)
            {
                if (Math.Abs(CDELT[0]) < 1e-20 && Math.Abs(CD1[0]) > 1e-20)
                {
                    CDELT[0] = CD1[0];
                }
            }
            else
            {
                // Matriz CD

                if (proyeccion[0] == 2 + 2 + 2 + 2)
                {
                    // CPRIX, CVAL, CD_1, CD_2

                    if (Math.Abs(CD1[1]) < 1e-20 && Math.Abs(CD2[0]) < 1e-20)
                    {
                        CDELT[0] = CD1[0];
                        CDELT[1] = CD2[1];
                    }
                }
                else if (proyeccion[1] == 2 + 2 + 2)
                {
                    // CPRIX, CVAL, CDELT

                    CD1[0] = CDELT[0];
                    CD1[1] = 0;
                    CD2[0] = 0;
                    CD2[1] = CDELT[1];
                    proyeccion[0] = 2 + 2 + 2 + 2;
                }
            }

            // Datos

            if (NAXIS > 0 && BITPIX != 0)
            {
                int ndatos = 1;
                for (int i = 0; i < NAXIS; i++) ndatos *= NAXISn[i];
                if (ndatos == 0) return true;
                if (BITPIX > 0)
                {
                    // Enteros

                    tipoDato = 0;
                }
                else
                {
                    // Punto flotante

                    tipoDato = 1;
                }
                byPorDato = Math.Abs(BITPIX / 8);
                byte[] bytesDato = new byte[byPorDato];
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 107];
                r_octetos.Text = string.Format("{0:N0}", ndatos * byPorDato);
                Application.DoEvents();
                panel_img.i3 = 0;
                panel_img.clase_dato = hdu[cHDU].clase_dato;

                // Los datos están anidados de fuera a dentro en formato big-endian: el byte más significativo está el primero

                bool le = BitConverter.IsLittleEndian;

                int contador = 0;
                if (NAXIS == 1)
                {
                    NAXIS = 2;
                    NAXISn[1] = 1;
                }
                if (NAXIS == 2)
                {
                    if (hdu[cHDU].conjunto == 0)
                    {
                        // Imagen

                        int i2;
                        int i1;
                        switch (hdu[cHDU].clase_dato)
                        {
                            case 1:
                                datosb = new byte[NAXISn[0], NAXISn[1]];
                                for (i2 = 0; i2 < NAXISn[1]; i2++)
                                {
                                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                                    {
                                        contador++;
                                        if (contador % X10M == 0)
                                        {
                                            v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                            Application.DoEvents();
                                        }
                                        datosb[i1, i2] = octetos[puntero++];
                                    }
                                }
                                break;
                            case 2:
                                datoss = new short[NAXISn[0], NAXISn[1]];
                                for (i2 = 0; i2 < NAXISn[1]; i2++)
                                {
                                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                                    {
                                        contador++;
                                        if (contador % X10M == 0)
                                        {
                                            v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                            Application.DoEvents();
                                        }

                                        // Los datos están en formato big-endian: el byte más significativo está el primero

                                        if (le)
                                        {
                                            // El sistema operativo trabaja en little-endian
                                            // Pasar los datos a little-endian

                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[0] = octetos[puntero++];
                                        }
                                        else
                                        {
                                            bytesDato[0] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                        }
                                        datoss[i1, i2] = BitConverter.ToInt16(bytesDato);
                                    }
                                }
                                break;
                            case 3:
                                datosi = new int[NAXISn[0], NAXISn[1]];
                                for (i2 = 0; i2 < NAXISn[1]; i2++)
                                {
                                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                                    {
                                        contador++;
                                        if (contador % X10M == 0)
                                        {
                                            v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                            Application.DoEvents();
                                        }

                                        // Los datos están en formato big-endian: el byte más significativo está el primero

                                        if (le)
                                        {
                                            // El sistema operativo trabaja en little-endian
                                            // Pasar los datos a little-endian

                                            bytesDato[3] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[0] = octetos[puntero++];
                                        }
                                        else
                                        {
                                            bytesDato[0] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[3] = octetos[puntero++];
                                        }
                                        datosi[i1, i2] = BitConverter.ToInt32(bytesDato);
                                    }
                                }
                                break;
                            case 4:
                                datosf = new float[NAXISn[0], NAXISn[1]];
                                for (i2 = 0; i2 < NAXISn[1]; i2++)
                                {
                                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                                    {
                                        contador++;
                                        if (contador % X10M == 0)
                                        {
                                            v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                            Application.DoEvents();
                                        }

                                        // Los datos están en formato big-endian: el byte más significativo está el primero

                                        if (le)
                                        {
                                            // El sistema operativo trabaja en little-endian
                                            // Pasar los datos a little-endian

                                            bytesDato[3] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[0] = octetos[puntero++];
                                        }
                                        else
                                        {
                                            bytesDato[0] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[3] = octetos[puntero++];
                                        }
                                        datosf[i1, i2] = BitConverter.ToSingle(bytesDato);
                                    }
                                }
                                break;
                            case 6:
                                datosl = new long[NAXISn[0], NAXISn[1]];
                                for (i2 = 0; i2 < NAXISn[1]; i2++)
                                {
                                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                                    {
                                        contador++;
                                        if (contador % X10M == 0)
                                        {
                                            v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                            Application.DoEvents();
                                        }

                                        // Los datos están en formato big-endian: el byte más significativo está el primero

                                        if (le)
                                        {
                                            // El sistema operativo trabaja en little-endian
                                            // Pasar los datos a little-endian

                                            bytesDato[7] = octetos[puntero++];
                                            bytesDato[6] = octetos[puntero++];
                                            bytesDato[5] = octetos[puntero++];
                                            bytesDato[4] = octetos[puntero++];
                                            bytesDato[3] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[0] = octetos[puntero++];
                                        }
                                        else
                                        {
                                            bytesDato[0] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[3] = octetos[puntero++];
                                            bytesDato[4] = octetos[puntero++];
                                            bytesDato[5] = octetos[puntero++];
                                            bytesDato[6] = octetos[puntero++];
                                            bytesDato[7] = octetos[puntero++];
                                        }
                                        datosl[i1, i2] = BitConverter.ToInt64(bytesDato);
                                    }
                                }
                                break;
                            default:
                                datosd = new double[NAXISn[0], NAXISn[1]];
                                for (i2 = 0; i2 < NAXISn[1]; i2++)
                                {
                                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                                    {
                                        contador++;
                                        if (contador % X10M == 0)
                                        {
                                            v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                            Application.DoEvents();
                                        }

                                        // Los datos están en formato big-endian: el byte más significativo está el primero

                                        if (le)
                                        {
                                            // El sistema operativo trabaja en little-endian
                                            // Pasar los datos a little-endian

                                            bytesDato[7] = octetos[puntero++];
                                            bytesDato[6] = octetos[puntero++];
                                            bytesDato[5] = octetos[puntero++];
                                            bytesDato[4] = octetos[puntero++];
                                            bytesDato[3] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[0] = octetos[puntero++];
                                        }
                                        else
                                        {
                                            bytesDato[0] = octetos[puntero++];
                                            bytesDato[1] = octetos[puntero++];
                                            bytesDato[2] = octetos[puntero++];
                                            bytesDato[3] = octetos[puntero++];
                                            bytesDato[4] = octetos[puntero++];
                                            bytesDato[5] = octetos[puntero++];
                                            bytesDato[6] = octetos[puntero++];
                                            bytesDato[7] = octetos[puntero++];
                                        }
                                        datosd[i1, i2] = BitConverter.ToDouble(bytesDato);
                                    }
                                }
                                break;
                        }
                        v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                        Application.DoEvents();
                        switch (hdu[cHDU].clase_dato)
                        {
                            case 1:
                                panel_img.Dibuja(datosb, true);
                                break;
                            case 2:
                                panel_img.Dibuja(datoss, true);
                                break;
                            case 3:
                                panel_img.Dibuja(datosi, true);
                                break;
                            case 4:
                                panel_img.Dibuja(datosf, true);
                                break;
                            case 6:
                                panel_img.Dibuja(datosl, true);
                                break;
                            default:
                                panel_img.Dibuja(datosd, true);
                                break;
                        }
                        b_redibuja.Enabled = true;
                    }
                    else
                    {
                        // Tabla

                        int i2;
                        int i1;
                        contador = 0;
                        if (byPorDato == 1)
                        {
                            datosb = new byte[NAXISn[0], NAXISn[1]];
                            for (i2 = 0; i2 < NAXISn[1]; i2++)
                            {
                                for (i1 = 0; i1 < NAXISn[0]; i1++)
                                {
                                    contador++;
                                    if (contador % X1M == 0)
                                    {
                                        v_leidos.Text = string.Format("{0:N0}", contador);
                                        Application.DoEvents();
                                    }
                                    datosb[i1, i2] = octetos[puntero++];
                                }
                            }
                            if (hdu[cHDU].conjunto == 1)
                            {
                                TablaBinaria(datosb);
                            }
                            else if (hdu[cHDU].conjunto == 2)
                            {
                                TablaASCII(datosb);
                            }
                        }
                        else
                        {
                            datos = new object[NAXISn[0], NAXISn[1]];
                            for (i2 = 0; i2 < NAXISn[1]; i2++)
                            {
                                for (i1 = 0; i1 < NAXISn[0]; i1++)
                                {
                                    contador++;
                                    if (contador % X1M == 0)
                                    {
                                        v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                        Application.DoEvents();
                                    }

                                    // Los datos están en formato big-endian: el byte más significativo está el primero

                                    if (le)
                                    {
                                        // El sistema operativo trabaja en little-endian
                                        // Pasar los datos a little-endian

                                        for (int k = byPorDato - 1; k >= 0; k--)
                                        {
                                            bytesDato[k] = octetos[puntero++];
                                        }
                                    }
                                    else
                                    {
                                        for (int k = 0; k < byPorDato; k++)
                                        {
                                            bytesDato[k] = octetos[puntero++];
                                        }
                                    }
                                    if (tipoDato == 0)
                                    {
                                        datos[i1, i2] = byPorDato switch
                                        {
                                            2 => BitConverter.ToInt16(bytesDato),
                                            _ => BitConverter.ToInt32(bytesDato),
                                        };
                                    }
                                    else
                                    {
                                        datos[i1, i2] = byPorDato switch
                                        {
                                            4 => BitConverter.ToSingle(bytesDato),
                                            _ => BitConverter.ToDouble(bytesDato),
                                        };
                                    }
                                }
                            }
                            v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                            Application.DoEvents();
                            if (hdu[cHDU].conjunto == 1)
                            {
                                TablaBinaria(datos);
                            }
                            else if (hdu[cHDU].conjunto == 2)
                            {
                                TablaASCII(datos);
                            }
                        }
                    }
                }
                else if (NAXIS == 3)
                {
                    if (hdu[cHDU].conjunto == 0)
                    {
                        // Imagen

                        switch (hdu[cHDU].clase_dato)
                        {
                            case 1:
                                datosb3 = new byte[NAXISn[0], NAXISn[1], NAXISn[2]];
                                break;
                            case 2:
                                datoss3 = new short[NAXISn[0], NAXISn[1], NAXISn[2]];
                                break;
                            case 3:
                                datosi3 = new int[NAXISn[0], NAXISn[1], NAXISn[2]];
                                break;
                            case 4:
                                datosf3 = new float[NAXISn[0], NAXISn[1], NAXISn[2]];
                                break;
                            case 6:
                                datosl3 = new long[NAXISn[0], NAXISn[1], NAXISn[2]];
                                break;
                            default:
                                datosd3 = new double[NAXISn[0], NAXISn[1], NAXISn[2]];
                                break;
                        }
                        int i3;
                        int i2;
                        int i1;
                        for (i3 = 0; i3 < NAXISn[2]; i3++)
                        {
                            for (i2 = 0; i2 < NAXISn[1]; i2++)
                            {
                                for (i1 = 0; i1 < NAXISn[0]; i1++)
                                {
                                    contador++;
                                    if (contador % X10M == 0)
                                    {
                                        v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                        Application.DoEvents();
                                    }
                                    if (byPorDato > 1)
                                    {
                                        if (le)
                                        {
                                            for (int k = byPorDato - 1; k >= 0; k--)
                                            {
                                                bytesDato[k] = octetos[puntero++];
                                            }
                                        }
                                        else
                                        {
                                            for (int k = 0; k < byPorDato; k++)
                                            {
                                                bytesDato[k] = octetos[puntero++];
                                            }
                                        }
                                    }
                                    switch (hdu[cHDU].clase_dato)
                                    {
                                        case 1:
                                            datosb3[i1, i2, i3] = octetos[puntero++];
                                            break;
                                        case 2:
                                            datoss3[i1, i2, i3] = BitConverter.ToInt16(bytesDato);
                                            break;
                                        case 3:
                                            datosi3[i1, i2, i3] = BitConverter.ToInt32(bytesDato);
                                            break;
                                        case 4:
                                            datosf3[i1, i2, i3] = BitConverter.ToSingle(bytesDato);
                                            break;
                                        case 6:
                                            datosl3[i1, i2, i3] = BitConverter.ToInt64(bytesDato);
                                            break;
                                        default:
                                            datosd3[i1, i2, i3] = BitConverter.ToDouble(bytesDato);
                                            break;
                                    }
                                }
                            }
                        }
                        v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                        Application.DoEvents();
                        switch (hdu[cHDU].clase_dato)
                        {
                            case 1:
                                panel_img.Dibuja(datosb3, true, 0);
                                break;
                            case 2:
                                panel_img.Dibuja(datoss3, true, 0);
                                break;
                            case 3:
                                panel_img.Dibuja(datosi3, true, 0);
                                break;
                            case 4:
                                panel_img.Dibuja(datosf3, true, 0);
                                break;
                            case 6:
                                panel_img.Dibuja(datosl3, true, 0);
                                break;
                            default:
                                panel_img.Dibuja(datosd3, true, 0);
                                break;
                        }
                        b_redibuja.Enabled = true;
                    }
                    else
                    {
                        // Tabla

                        datos3 = new object[NAXISn[0], NAXISn[1], NAXISn[2]];
                        int i3;
                        int i2;
                        int i1;
                        for (i3 = 0; i3 < NAXISn[2]; i3++)
                        {
                            for (i2 = 0; i2 < NAXISn[1]; i2++)
                            {
                                for (i1 = 0; i1 < NAXISn[0]; i1++)
                                {
                                    contador++;
                                    if (contador % 100000 == 0)
                                    {
                                        v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                                        Application.DoEvents();
                                    }
                                    if (byPorDato == 1)
                                    {
                                        datos3[i1, i2, i3] = octetos[puntero++];
                                        continue;
                                    }

                                    // Los datos están en formato big-endian: el byte más significativo está el primero

                                    if (le)
                                    {
                                        // El sistema operativo trabaja en little-endian
                                        // Pasar los datos a little-endian

                                        for (int k = byPorDato - 1; k >= 0; k--)
                                        {
                                            bytesDato[k] = octetos[puntero++];
                                        }
                                    }
                                    else
                                    {
                                        for (int k = 0; k < byPorDato; k++)
                                        {
                                            bytesDato[k] = octetos[puntero++];
                                        }
                                    }
                                    if (tipoDato == 0)
                                    {
                                        datos3[i1, i2, i3] = byPorDato switch
                                        {
                                            2 => BitConverter.ToInt16(bytesDato),
                                            _ => BitConverter.ToInt32(bytesDato),
                                        };
                                    }
                                    else
                                    {
                                        datos3[i1, i2, i3] = byPorDato switch
                                        {
                                            4 => BitConverter.ToSingle(bytesDato),
                                            _ => BitConverter.ToDouble(bytesDato),
                                        };
                                    }
                                }
                            }
                        }
                        v_leidos.Text = string.Format("{0:N0}", contador * byPorDato);
                        Application.DoEvents();
                        if (hdu[cHDU].conjunto == 1)
                        {
                            TablaBinaria(datos3, 0);
                        }
                        else if (hdu[cHDU].conjunto == 2)
                        {
                            TablaASCII(datos3, 0);
                        }
                    }
                    v_leidos.Text = string.Format("{0:N0}", ndatos * byPorDato);
                }
                else
                {
                    MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 8], cHDU, NAXIS), Idioma.msg[Idioma.lengua, 54], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return true;
        }
        private void CreaCFG(string fichero)
        {
            if (hdu != null && hdu.Count > 0)
            {
                string fc = Path.Combine(Path.GetDirectoryName(fichero), Path.GetFileNameWithoutExtension(fichero) + ".cfg");
                FileStream fsfc = new FileStream(fc, FileMode.Create, FileAccess.Write, FileShare.None);
                if (fsfc != null)
                {
                    StreamWriter sw = new StreamWriter(fsfc);
                    for (int i = 0; i < hdu.Count; i++)
                    {
                        sw.WriteLine(string.Format("NORMALIZAR;{0}", hdu[i].normalizar ? "TRUE" : "FALSE"));
                        sw.WriteLine(string.Format("NAN;{0}", hdu[i].ind_color_nan));
                        sw.WriteLine(string.Format("COTA_INF;{0}", hdu[i].cota_inf));
                        sw.WriteLine(string.Format("NEGATIVOS;{0}", hdu[i].ind_color_cota_inf));
                        sw.WriteLine(string.Format("COTA_SUP;{0}", hdu[i].cota_sup));
                        sw.WriteLine(string.Format("SATURADOS;{0}", hdu[i].ind_color_cota_sup));
                        sw.WriteLine(string.Format("INVERTIR_Y;{0}", hdu[i].invertir_y ? "TRUE" : "FALSE"));
                        sw.WriteLine(string.Format("INVERTIR_X;{0}", hdu[i].invertir_x ? "TRUE" : "FALSE"));
                        sw.WriteLine(string.Format("ALTA_RESOLUCION;{0}", hdu[i].alta_resolucion ? "TRUE" : "FALSE"));
                        sw.WriteLine(string.Format("1/2;{0}", hdu[i].raiz ? "TRUE" : "FALSE"));
                    }
                    sw.Close();
                }
            }
        }
        private static bool ProcesaLineaCFG(StreamReader sr, string cadena, ref string valor)
        {
            if (sr.EndOfStream) return false;
            string linea = sr.ReadLine();
            if (linea.StartsWith(cadena))
            {
                string[] sd = linea.Split(';');
                if (sd.Length == 2)
                {
                    valor = sd[1];
                    return true;
                }
                else return false;
            }
            else return false;
        }
        private void AjustaPuntero(ref int puntero)
        {
            if (puntero < num_octetos)
            {
                // Ajustar el puntero al final del último bloque de datos

                int bloques_leidos = puntero / BLOQUE;
                if (bloques_leidos * BLOQUE < puntero) bloques_leidos++;
                puntero = bloques_leidos * BLOQUE;
            }
            v_leidos.Text = string.Format("{0:N0}", puntero);
            Application.DoEvents();
        }
        private void IniciaParametros()
        {
            NAXIS = 0;
            NAXISn = null;
            FILENAME = string.Empty;
            punto_referencia = 0;
            proyeccion[0] = proyeccion[1] = proyeccion[2] = 0;
            CRPIX = null;
            CRVAL = null;
            CUNIT = null;
            CTYPE = new string[2];
            CTYPE[0] = CTYPE[1] = string.Empty;
            CDELT = null;
            CROTA = null;
            CD1[0] = 0;
            CD1[1] = 0;
            CD2[0] = 0;
            CD2[1] = 0;
            PC1[0] = 0;
            PC1[1] = 0;
            PC2[0] = 0;
            PC2[1] = 0;
            XTENSION = string.Empty;
            TFIELDS = 0;
            TTYPE = null;
            TBCOL = null;
            TFORM = null;
            TUNIT = null;
            AnulaDatos();
            es_actual = null;

            ctype_1.Text = string.Empty;
            ctype_2.Text = string.Empty;
            b_redibuja.Enabled = false;
            v_leidos.Text = string.Empty;
            r_tpc_min.Text = string.Empty;
            r_tpc_max.Text = string.Empty;
            r_cantidad.Text = string.Empty;
            r_octetos.Text = string.Empty;
            histo_ceros.Text = string.Empty;
            histo_ceros_tpc.Text = string.Empty;
            r_h.Text = string.Empty;
            r_min.Text = string.Empty;
            r_max.Text = string.Empty;
            r_med.Text = string.Empty;
            r_destandar.Text = string.Empty;
            reloj_ar.Refresh();
            reloj_de.Refresh();
            lista_cabeceras.Items.Clear();
            b_exporta_cabeceras.Visible = false;
            lista_parametros.Items.Clear();
            parametros.Clear();
            val_parametro.Text = string.Empty;
            tabla.ColumnCount = 0;
            b_exporta_fits.Visible = false;
            b_exporta_imagen.Visible = false;
            b_conv_espectro.Visible = false;
            b_exporta_tabla.Visible = false;
            panel_img.IniciaControles();
            Application.DoEvents();
        }
        private void AnulaDatos()
        {
            Array.Clear(histograma, 0, histograma.Length);
            datosb = null;
            datoss = null;
            datosi = null;
            datosf = null;
            datosd = null;
            img = null;
            panel_img.lienzo.Image = img;
            panel_histograma.Image = null;
            GC.Collect();
        }
        private static string LeeCabecera(byte[] octetos, ref int puntero)
        {
            if (puntero + 80 >= octetos.Length)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 56], Idioma.msg[Idioma.lengua, 55], MessageBoxButtons.OK, MessageBoxIcon.Error);
                puntero = octetos.Length;
                return string.Empty;
            }
            byte[] cabecera = new byte[80];
            Array.Copy(octetos, puntero, cabecera, 0, 80);
            string s = Encoding.ASCII.GetString(cabecera);
            if (s.StartsWith('\0'))
            {
                puntero = octetos.Length;
                return string.Empty;
            }
            puntero += 80;
            return s.Trim();
        }
        private static string LeeCabecera(FileStream fs, ref long puntero)
        {
            if (puntero + 80 >= fs.Length)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 56], Idioma.msg[Idioma.lengua, 55], MessageBoxButtons.OK, MessageBoxIcon.Error);
                puntero = fs.Length;
                return string.Empty;
            }
            byte[] cabecera = new byte[80];
            int lee = fs.Read(cabecera, 0, 80);
            if (lee != 80)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 56], Idioma.msg[Idioma.lengua, 55], MessageBoxButtons.OK, MessageBoxIcon.Error);
                puntero = fs.Length;
                return string.Empty;
            }
            puntero += 80;
            string s = Encoding.ASCII.GetString(cabecera);
            if (s.StartsWith('\0'))
            {
                puntero = fs.Length;
                return string.Empty;
            }
            return s.Trim();
        }
        private bool InterpretaCabecera(int cHDU, string cabecera)
        {
            if (cabecera.Trim().Length == 0) return true;
            if (cabecera.StartsWith("HISTORY ")) return true;
            if (cabecera.StartsWith("COMMENT ")) return true;

            // Sin '=' es un comentario

            if (cabecera.IndexOf('=') == -1) return true;

            // Quitar comentario al final de la línea

            string[] sd = cabecera.Split('/');
            string s = sd[0];

            if (sd[0].Trim().Length == 0) return true;
            if (sd[0].IndexOf('=') == -1) return true;

            sd = s.Split('=');
            string clave = sd[0].Trim();
            string valor = sd[1].Trim().Trim('\'').Trim();
            int nr = 0;
            string clave_r = clave;
            while (parametros.ContainsKey(clave_r))
            {
                nr++;
                clave_r = string.Format("{0} {1,3:N0}", clave, nr);
            }
            parametros.Add(clave_r, valor);

            if (s.StartsWith("SIMPLE "))
            {
                SIMPLE = true;
                return true;
            }
            if (s.StartsWith("BITPIX "))
            {
                BITPIX = Convert.ToInt32(valor);
                return true;
            }
            if (s.StartsWith("NAXIS "))
            {
                NAXIS = Convert.ToInt32(valor);
                int ejes_min;
                if (NAXIS == 0 || NAXIS == 1)
                {
                    // Tara tratarlo como [n, 1]

                    ejes_min = 2;
                }
                else
                {
                    ejes_min = NAXIS;
                }
                NAXISn = new int[ejes_min];
                CRPIX = new double[ejes_min];
                CRVAL = new double[ejes_min];
                CUNIT = new string[ejes_min];
                CTYPE = new string[ejes_min];
                CDELT = new double[ejes_min];
                CROTA = new double[ejes_min];
                for (int i = 0; i < ejes_min; i++)
                {
                    CRPIX[i] = -1;
                    CRVAL[i] = double.MinValue;
                    CUNIT[i] = null;
                    CTYPE[i] = string.Empty;
                    CDELT[i] = 0;
                    CROTA[i] = 0;
                }
                axis_pendientes = NAXIS;
                return true;
            }
            if (axis_pendientes > 0)
            {
                for (int i = 0; i < NAXIS; i++)
                {
                    if (s.StartsWith(string.Format("NAXIS{0} ", i + 1)))
                    {
                        NAXISn[i] = Convert.ToInt32(valor);
                        axis_pendientes--;
                        return true;
                    }
                }
            }
            if (s.StartsWith("FILENAME"))
            {
                FILENAME = valor;
                return true;
            }
            if (s.StartsWith("CRPIX"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["CRPIX".Length..]);
                    CRPIX[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                    proyeccion[0]++;
                    proyeccion[1]++;
                    proyeccion[2]++;
                    punto_referencia++;
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("CRVAL"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["CRVAL".Length..]);
                    CRVAL[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                    proyeccion[0]++;
                    proyeccion[1]++;
                    proyeccion[2]++;
                    punto_referencia++;
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("CUNIT"))
            {
                int n = Convert.ToInt32(clave["CUNIT".Length..]);
                CUNIT[n - 1] = valor.Trim('\'').Trim();
                return true;
            }
            if (s.StartsWith("CD1_"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["CD1_".Length..]);
                    CD1[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                    proyeccion[0]++;
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("CD2_"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["CD2_".Length..]);
                    CD2[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                    proyeccion[0]++;
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("PC1_"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["PC1_".Length..]);
                    PC1[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                    proyeccion[2]++;
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("PC2_"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["PC2_".Length..]);
                    PC2[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                    proyeccion[2]++;
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("CTYPE"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["CTYPE".Length..]);
                    CTYPE[n - 1] = valor.Trim('\'').Trim();
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("CDELT"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["CDELT".Length..]);
                    CDELT[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                    proyeccion[1]++;
                    proyeccion[2]++;
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("CROTA"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["CROTA".Length..]);
                    CROTA[n - 1] = Convert.ToDouble(valor.Replace(s_millar, s_decimal));
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("XTENSION"))
            {
                XTENSION = valor.Trim('\'').Trim();
                return true;
            }
            if (s.StartsWith("TFIELDS "))
            {
                TFIELDS = Convert.ToInt32(valor.Replace(s_millar, s_decimal));
                TTYPE = new string[TFIELDS];
                TBCOL = new int[TFIELDS + 1];
                TFORM = new string[TFIELDS];
                TUNIT = new string[TFIELDS];
                for (int i = 0; i < TFIELDS; i++)
                {
                    TUNIT[i] = string.Empty;
                }
                return true;
            }
            if (s.StartsWith("TTYPE"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["TTYPE".Length..]);
                    TTYPE[n - 1] = valor.Trim('\'').Trim();
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("TBCOL"))
            {
                int n = Convert.ToInt32(clave["TBCOL".Length..]);
                TBCOL[n - 1] = Convert.ToInt32(valor.Replace(s_millar, s_decimal));
                return true;
            }
            if (s.StartsWith("TFORM"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["TFORM".Length..]);
                    TFORM[n - 1] = valor.Trim('\'').Trim();
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("TUNIT"))
            {
                try
                {
                    int n = Convert.ToInt32(clave["TUNIT".Length..]);
                    TUNIT[n - 1] = valor.Trim('\'').Trim();
                }
                catch
                {
                    lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
                }
                return true;
            }
            if (s.StartsWith("PCOUNT "))
            {
                PCOUNT = Convert.ToInt32(valor.Replace(s_millar, s_decimal));
                return true;
            }
            if (s.StartsWith("GCOUNT "))
            {
                GCOUNT = Convert.ToInt32(valor.Replace(s_millar, s_decimal));
                return true;
            }
            return true;
        }
        private DateTime Fecha(string ss, string s)
        {
            try
            {
                int aa;
                int mm;
                int dd;
                aa = Convert.ToInt32(ss.Substring(0, 4));
                if (ss.Length > 6)
                {
                    mm = Convert.ToInt32(ss.Substring(5, 2));
                    if (ss.Length > 9)
                    {
                        dd = Convert.ToInt32(ss.Substring(8, 2));
                    }
                    else
                    {
                        dd = 1;
                    }
                }
                else
                {
                    mm = 1;
                    dd = 1;
                }
                return new DateTime(aa, mm, dd);
            }
            catch
            {
                lista_cabeceras.Items.Add(string.Format("CABECERA DESCONOCIDA {0}", s));
            }
            return new DateTime(3000, 1, 1);
        }
        private void B_exporta_tabla_Click(object sender, EventArgs e)
        {
            if (tabla == null || tabla.RowCount == 0) return;
            SaveFileDialog ficheroescritura = new SaveFileDialog()
            {
                Filter = "CSV (*.csv)|*.csv|TODO (*.*)|*.*",
                FilterIndex = 1
            };
            if (ficheroescritura.ShowDialog() == DialogResult.OK)
            {
                Disponible(false);
                FileStream fe = new FileStream(ficheroescritura.FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                StreamWriter sw = new StreamWriter(fe, Encoding.UTF8);
                int ihdu = hdu_actual = sel_HDU.SelectedIndex;
                if (hdu[ihdu].conjunto == 1)
                {
                    if (hdu[ihdu].byPorDato == 1)
                    {
                        ExportaTablaBin(datosb, sw);
                    }
                    else
                    {
                        ExportaTablaBin(datos, sw);
                    }
                }
                else
                {
                    for (int j = 0; j < tabla.ColumnCount; j++)
                    {
                        sw.Write(string.Format("{0};", tabla.Columns[j].Name));
                    }
                    sw.WriteLine("");
                    for (int i = 0; i < tabla.RowCount; i++)
                    {
                        for (int j = 0; j < tabla.ColumnCount; j++)
                        {
                            sw.Write(string.Format("{0};", tabla.Rows[i].Cells[j].Value.ToString()));
                        }
                        sw.WriteLine("");
                    }
                    sw.WriteLine("");
                }
                sw.Close();
                Disponible(true);
                Console.Beep();
            }
        }
        private void Tabla_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int i2 = e.RowIndex;
            if (i2 == -1)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 135], Idioma.msg[Idioma.lengua, 26], MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int col = e.ColumnIndex;
            int i = hdu_actual = sel_HDU.SelectedIndex;
            int r = (hdu[i].conjunto == 2) ? 1 : SubColumnas(col);
            if (hdu[i].conjunto == 2 || r <= MAX_SUBCOL)
            {
                // Clipboard

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(string.Format("{0}", tabla.Rows[i2].Cells[col].Value.ToString()));
                Clipboard.SetText(sb.ToString());
                Console.Beep();
            }
            else
            {
                // A fichero

                SaveFileDialog ficheroescritura = new SaveFileDialog()
                {
                    Filter = "CSV |*.csv;*.txt|TODO |*.*",
                    FilterIndex = 1
                };
                if (ficheroescritura.ShowDialog() != DialogResult.OK) return;
                FileStream fe = new FileStream(ficheroescritura.FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                StreamWriter sw = new StreamWriter(fe, Encoding.UTF8);
                int i1 = i2 * NAXISn[1] + des_columna[col];
                bool le = BitConverter.IsLittleEndian;
                string[] elementos_celda;
                if (byPorDato == 1)
                {
                    elementos_celda = ElementosCelda(le, r, datosb, col, ref i1, i2);
                }
                else
                {
                    elementos_celda = ElementosCelda(le, r, datos, col, ref i1, i2);
                }
                for (int ie = 0; ie < r; ie++)
                {
                    sw.WriteLine(elementos_celda[ie]);
                }
                sw.Close();
                Console.Beep();
            }
        }
        private bool[] ExportaCabeceraBin(StreamWriter sw)
        {
            bool[] quecols = new bool[TFIELDS];
            int cols = tabla.SelectedColumns.Count;
            if (cols == 0)
            {
                // Todas

                for (int i = 0; i < TFIELDS; i++) quecols[i] = true;
            }
            else
            {
                for (int i = 0; i < TFIELDS; i++) quecols[i] = false;
                for (int i = 0; i < cols; i++) quecols[tabla.SelectedColumns[i].Index] = true;
            }
            int r;
            int col = 0;
            while (col < TFIELDS)
            {
                if (quecols[col])
                {
                    r = SubColumnas(col);
                    if (r == -1) return null;
                    if (TFORM[col].IndexOf("A") != -1) r = 1;
                    if (TTYPE != null)
                    {
                        if (r == 1) sw.Write(string.Format("{0};", TTYPE[col]));
                        else
                        {
                            for (int ir = 0; ir < r; ir++) sw.Write(string.Format("{0}_{1};", TTYPE[col], ir + 1));
                        }
                    }
                    else
                    {
                        if (r == 1) sw.Write(string.Format("Col{0};", col + 1));
                        else
                        {
                            for (int ir = 0; ir < r; ir++) sw.Write(string.Format("Col{0}_{1};", col + 1, ir + 1));
                        }
                    }
                }
                col++;
            }
            sw.WriteLine();
            return quecols;
        }
        private void ExportaTablaBin(object[,] datos, StreamWriter sw)
        {
            // Cabeceras

            bool[] quecols = ExportaCabeceraBin(sw);
            if (quecols == null) return;

            // Filas

            int r;
            int w;
            int ind;
            int i1;
            int col;
            string cadena;
            StringBuilder sb;
            bool le = BitConverter.IsLittleEndian;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                i1 = 0;
                col = 0;
                while (col < TFIELDS)
                {
                    if ((ind = TFORM[col].IndexOf("L")) != -1)
                    {
                        // 1 byte (lógico)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 0, 1, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("X")) != -1)
                    {
                        // 1 bit

                        r = Rep(TFORM[col], ind);

                        if (quecols[col]) sw.Write(";");
                        i1++;
                    }
                    else if ((ind = TFORM[col].IndexOf("B")) != -1)
                    {
                        // 1 byte (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(string.Format("{0};", datos[i1, i2].ToString()));
                        i1++;

                    }
                    else if ((ind = TFORM[col].IndexOf("S")) != -1)
                    {
                        // 1 byte con signo (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(string.Format("{0};", datos[i1, i2].ToString()));
                        i1++;
                    }
                    else if ((ind = TFORM[col].IndexOf("I")) != -1)
                    {
                        // 2 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 1, 2, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("U")) != -1)
                    {
                        // 2 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 2, 2, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("J")) != -1)
                    {
                        // 4 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 3, 4, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("V")) != -1)
                    {
                        // 4 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 4, 4, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("K")) != -1)
                    {
                        // 8 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 5, 8, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("E")) != -1)
                    {
                        // 4 bytes (simple precisión)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 6, 4, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("D")) != -1)
                    {
                        // 8 bytes (doble precisión)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 7, 8, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("C")) != -1)
                    {
                        // 8 bytes (simple precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(";");
                        i1 += 8;
                    }
                    else if ((ind = TFORM[col].IndexOf("M")) != -1)
                    {
                        // 16 bytes (doble precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(";");
                        i1 += 16;
                    }
                    else if ((ind = TFORM[col].IndexOf("P")) != -1)
                    {
                        // 8 bytes (descriptor de array)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(";");
                        i1 += 8;
                    }
                    else if ((ind = TFORM[col].IndexOf("A")) != -1)
                    {
                        // 1 bytes (caracter)

                        r = Rep(TFORM[col], ind);
                        if (ind == TFORM[col].Length - 1) w = 1;
                        else w = Convert.ToInt32(TFORM[col][(ind + 1)..]);
                        sb = new StringBuilder();
                        for (int k1 = 0; k1 < r; k1++)
                        {
                            for (int k2 = 0; k2 < w; k2++)
                            {
                                if ((int)datos[i1, i2] != 0) sb.AppendFormat("{0}", Convert.ToChar(datos[i1, i2]));
                                i1++;
                            }
                        }
                        sb.Append(';');
                        if (quecols[col]) sw.Write(sb.ToString());
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 58], TFORM[col]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    col++;
                }
                if (i1 != NAXISn[0])
                {
                    MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 110], i1, NAXISn[0]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                sw.WriteLine();
            }
        }
        private void ExportaTablaBin(byte[,] datos, StreamWriter sw)
        {
            // Cabeceras

            bool[] quecols = ExportaCabeceraBin(sw);
            if (quecols == null) return;

            // Filas

            int r;
            int w;
            int ind;
            int i1;
            int col;
            string cadena;
            StringBuilder sb;
            bool le = BitConverter.IsLittleEndian;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                i1 = 0;
                col = 0;
                while (col < TFIELDS)
                {
                    if ((ind = TFORM[col].IndexOf("L")) != -1)
                    {
                        // 1 byte (lógico)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 0, 1, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("X")) != -1)
                    {
                        // 1 bit

                        r = Rep(TFORM[col], ind);

                        if (quecols[col]) sw.Write(";");
                        i1++;
                    }
                    else if ((ind = TFORM[col].IndexOf("B")) != -1)
                    {
                        // 1 byte (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(string.Format("{0};", datos[i1, i2]));
                        i1++;
                    }
                    else if ((ind = TFORM[col].IndexOf("S")) != -1)
                    {
                        // 1 byte con signo (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(string.Format("{0};", (sbyte)datos[i1, i2]));
                        i1++;
                    }
                    else if ((ind = TFORM[col].IndexOf("I")) != -1)
                    {
                        // 2 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 1, 2, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("U")) != -1)
                    {
                        // 2 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 2, 2, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("J")) != -1)
                    {
                        // 4 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 3, 4, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("V")) != -1)
                    {
                        // 4 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 4, 4, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("K")) != -1)
                    {
                        // 8 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 5, 8, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("E")) != -1)
                    {
                        // 4 bytes (simple precisión)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 6, 4, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("D")) != -1)
                    {
                        // 8 bytes (doble precisión)

                        r = Rep(TFORM[col], ind);
                        cadena = SalidaElementoTabla(le, r, datos, 7, 8, ref i1, i2);
                        if (quecols[col]) sw.Write(cadena);
                    }
                    else if ((ind = TFORM[col].IndexOf("C")) != -1)
                    {
                        // 8 bytes (simple precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(";");
                        i1 += 8;
                    }
                    else if ((ind = TFORM[col].IndexOf("M")) != -1)
                    {
                        // 16 bytes (doble precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(";");
                        i1 += 16;
                    }
                    else if ((ind = TFORM[col].IndexOf("P")) != -1)
                    {
                        // 8 bytes (descriptor de array)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col]) sw.Write(";");
                        i1 += 8;
                    }
                    else if ((ind = TFORM[col].IndexOf("A")) != -1)
                    {
                        // 1 bytes (caracter)

                        r = Rep(TFORM[col], ind);
                        if (ind == TFORM[col].Length - 1) w = 1;
                        else w = Convert.ToInt32(TFORM[col][(ind + 1)..]);
                        sb = new StringBuilder();
                        for (int k1 = 0; k1 < r; k1++)
                        {
                            for (int k2 = 0; k2 < w; k2++)
                            {
                                if (datos[i1, i2] != 0) sb.AppendFormat("{0}", Convert.ToChar(datos[i1, i2]));
                                i1++;
                            }
                        }
                        sb.Append(';');
                        if (quecols[col]) sw.Write(sb.ToString());
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 58], TFORM[col]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    col++;
                }
                if (i1 != NAXISn[0])
                {
                    MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 110], i1, NAXISn[0]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                sw.WriteLine();
            }
        }
        private int SubColumnas(int col)
        {
            int ind;
            if ((ind = TFORM[col].IndexOf("L")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("X")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("B")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("S")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("I")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("U")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("J")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("V")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("K")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("E")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("D")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("C")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("M")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("P")) != -1) return Rep(TFORM[col], ind);
            if ((ind = TFORM[col].IndexOf("A")) != -1) return Rep(TFORM[col], ind);
            MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 58], TFORM[col]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
            return -1;
        }
        private static int Rep(string s, int ind)
        {
            if (ind == 0)
            {
                return 1;
            }
            else
            {
                return Convert.ToInt32(s.Substring(0, ind));
            }
        }
        private static void AddElementoCadena(StringBuilder sbr, int tipo, byte[] sec_bytes, string sep)
        {
            switch (tipo)
            {
                case 0:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToBoolean(sec_bytes), sep);
                    break;
                case 1:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToInt16(sec_bytes), sep);
                    break;
                case 2:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToUInt16(sec_bytes), sep);
                    break;
                case 3:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToInt32(sec_bytes), sep);
                    break;
                case 4:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToUInt32(sec_bytes), sep);
                    break;
                case 5:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToInt64(sec_bytes), sep);
                    break;
                case 6:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToSingle(sec_bytes), sep);
                    break;
                case 7:
                    sbr.AppendFormat("{0}{1}", BitConverter.ToDouble(sec_bytes), sep);
                    break;
                default:
                    sbr.Append(sep);
                    break;
            }
        }
        private static string CadenaElementoTabla(bool le, int r, object[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            byte[] sec_bytes = new byte[lon];
            StringBuilder sbr = new StringBuilder();
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) sec_bytes[k] = (byte)datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) sec_bytes[k] = (byte)datos[i1++, i2];
                }
                AddElementoCadena(sbr, tipo, sec_bytes, " ");
                ir++;
                if (ir == r) break;
                if (r > MAX_SUBCOL)
                {
                    sbr.AppendFormat("... [{0}] ... ", r - 2);
                    ir = r - 1;
                    i1 += (r - 2) * lon;
                }
            }

            // Eliminar el último " "

            return sbr.ToString().Substring(0, sbr.Length - 1);
        }
        private static string CadenaElementoTabla(bool le, int r, byte[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            byte[] sec_bytes = new byte[lon];
            StringBuilder sbr = new StringBuilder();
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) sec_bytes[k] = datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) sec_bytes[k] = datos[i1++, i2];
                }
                AddElementoCadena(sbr, tipo, sec_bytes, " ");
                ir++;
                if (ir == r) break;
                if (r > MAX_SUBCOL)
                {
                    sbr.AppendFormat("... [{0}] ... ", r - 2);
                    ir = r - 1;
                    i1 += (r - 2) * lon;
                }
            }

            // Eliminar el último " "

            return sbr.ToString().Substring(0, sbr.Length - 1);
        }
        private static string[] ElementosCeldaTipo(bool le, int r, object[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            int ne = 0;
            string[] elementos = new string[r];
            byte[] doble = new byte[lon];
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) doble[k] = (byte)datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) doble[k] = (byte)datos[i1++, i2];
                }
                elementos[ne++] = tipo switch
                {
                    0 => BitConverter.ToBoolean(doble).ToString(),
                    1 => BitConverter.ToInt16(doble).ToString(),
                    2 => BitConverter.ToUInt16(doble).ToString(),
                    3 => BitConverter.ToInt32(doble).ToString(),
                    4 => BitConverter.ToUInt32(doble).ToString(),
                    5 => BitConverter.ToInt64(doble).ToString(),
                    6 => BitConverter.ToSingle(doble).ToString(),
                    7 => BitConverter.ToDouble(doble).ToString(),
                    _ => "0",
                };
                ir++;
                if (ir == r) break;
            }
            return elementos;
        }
        private static string[] ElementosCeldaTipo(bool le, int r, byte[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            int ne = 0;
            string[] elementos = new string[r];
            byte[] doble = new byte[lon];
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) doble[k] = datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) doble[k] = datos[i1++, i2];
                }
                elementos[ne++] = tipo switch
                {
                    0 => BitConverter.ToBoolean(doble).ToString(),
                    1 => BitConverter.ToInt16(doble).ToString(),
                    2 => BitConverter.ToUInt16(doble).ToString(),
                    3 => BitConverter.ToInt32(doble).ToString(),
                    4 => BitConverter.ToUInt32(doble).ToString(),
                    5 => BitConverter.ToInt64(doble).ToString(),
                    6 => BitConverter.ToSingle(doble).ToString(),
                    7 => BitConverter.ToDouble(doble).ToString(),
                    _ => "0",
                };
                ir++;
                if (ir == r) break;
            }
            return elementos;
        }
        private string[] ElementosCelda(bool le, int r, object[,] datos, int col, ref int i1, int i2)
        {
            int ind;
            string[] elementos_fila_columna = new string[1];
            if ((ind = TFORM[col].IndexOf("L")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 0, 1, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("I")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 1, 2, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("U")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 2, 2, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("J")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 3, 4, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("V")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 4, 4, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("K")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 5, 8, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("E")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 6, 4, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("D")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 7, 8, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("A")) != -1)
            {
                int w = ind == (TFORM[col].Length - 1) ? 1 : Convert.ToInt32(TFORM[col][(ind + 1)..]);
                elementos_fila_columna = new string[r];
                StringBuilder sb;
                for (int k1 = 0; k1 < r; k1++)
                {
                    sb = new StringBuilder();
                    for (int k2 = 0; k2 < w; k2++) sb.AppendFormat("{0}", Convert.ToChar(this.datos[i1++, i2]));
                    elementos_fila_columna[k1] = sb.ToString();
                }
            }
            return elementos_fila_columna;
        }
        private string[] ElementosCelda(bool le, int r, byte[,] datos, int col, ref int i1, int i2)
        {
            int ind;
            string[] elementos_fila_columna = new string[1];
            if ((ind = TFORM[col].IndexOf("L")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 0, 1, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("I")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 1, 2, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("U")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 2, 2, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("J")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 3, 4, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("V")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 4, 4, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("K")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 5, 8, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("E")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 6, 4, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("D")) != -1) elementos_fila_columna = ElementosCeldaTipo(le, r, datos, 7, 8, ref i1, i2);
            else if ((ind = TFORM[col].IndexOf("A")) != -1)
            {
                int w = ind == (TFORM[col].Length - 1) ? 1 : Convert.ToInt32(TFORM[col][(ind + 1)..]);
                elementos_fila_columna = new string[r];
                StringBuilder sb;
                for (int k1 = 0; k1 < r; k1++)
                {
                    sb = new StringBuilder();
                    for (int k2 = 0; k2 < w; k2++) sb.AppendFormat("{0}", Convert.ToChar(this.datos[i1++, i2]));
                    elementos_fila_columna[k1] = sb.ToString();
                }
            }
            return elementos_fila_columna;
        }
        private static double[] ValorElementosCeldaTipo(bool le, int r, object[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            int ne = 0;
            double[] elementos = new double[r];
            byte[] sec_bytes = new byte[lon];
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) sec_bytes[k] = (byte)datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) sec_bytes[k] = (byte)datos[i1++, i2];
                }
                elementos[ne++] = tipo switch
                {
                    0 => BitConverter.ToBoolean(sec_bytes) ? 1 : 0,
                    1 => BitConverter.ToInt16(sec_bytes),
                    2 => BitConverter.ToUInt16(sec_bytes),
                    3 => BitConverter.ToInt32(sec_bytes),
                    4 => BitConverter.ToUInt32(sec_bytes),
                    5 => BitConverter.ToInt64(sec_bytes),
                    6 => BitConverter.ToSingle(sec_bytes),
                    7 => BitConverter.ToDouble(sec_bytes),
                    _ => 0,
                };
                ir++;
                if (ir == r) break;
            }
            return elementos;
        }
        private static double[] ValorElementosCeldaTipo(bool le, int r, byte[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            int ne = 0;
            double[] elementos = new double[r];
            byte[] sec_bytes = new byte[lon];
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) sec_bytes[k] = datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) sec_bytes[k] = datos[i1++, i2];
                }
                elementos[ne++] = tipo switch
                {
                    0 => BitConverter.ToBoolean(sec_bytes) ? 1 : 0,
                    1 => BitConverter.ToInt16(sec_bytes),
                    2 => BitConverter.ToUInt16(sec_bytes),
                    3 => BitConverter.ToInt32(sec_bytes),
                    4 => BitConverter.ToUInt32(sec_bytes),
                    5 => BitConverter.ToInt64(sec_bytes),
                    6 => BitConverter.ToSingle(sec_bytes),
                    7 => BitConverter.ToDouble(sec_bytes),
                    _ => 0,
                };
                ir++;
                if (ir == r) break;
            }
            return elementos;
        }
        private double[] ValorElementosCelda(bool le, int r, byte[,] datos, int col, ref int i1, int i2)
        {
            double[] elementos_fila_columna = new double[1];
            if (TFORM[col].IndexOf("L") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 0, 1, ref i1, i2);
            else if (TFORM[col].IndexOf("I") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 1, 2, ref i1, i2);
            else if (TFORM[col].IndexOf("U") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 2, 2, ref i1, i2);
            else if (TFORM[col].IndexOf("J") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 3, 4, ref i1, i2);
            else if (TFORM[col].IndexOf("V") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 4, 4, ref i1, i2);
            else if (TFORM[col].IndexOf("K") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 5, 8, ref i1, i2);
            else if (TFORM[col].IndexOf("E") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 6, 4, ref i1, i2);
            else if (TFORM[col].IndexOf("D") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 7, 8, ref i1, i2);
            else if (TFORM[col].IndexOf("A") != -1)
            {
                elementos_fila_columna = new double[r];
                Array.Clear(elementos_fila_columna, 0, r);
            }
            return elementos_fila_columna;
        }
        private double[] ValorElementosCelda(bool le, int r, object[,] datos, int col, ref int i1, int i2)
        {
            double[] elementos_fila_columna = new double[1];
            if (TFORM[col].IndexOf("L") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 0, 1, ref i1, i2);
            else if (TFORM[col].IndexOf("I") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 1, 2, ref i1, i2);
            else if (TFORM[col].IndexOf("U") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 2, 2, ref i1, i2);
            else if (TFORM[col].IndexOf("J") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 3, 4, ref i1, i2);
            else if (TFORM[col].IndexOf("V") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 4, 4, ref i1, i2);
            else if (TFORM[col].IndexOf("K") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 5, 8, ref i1, i2);
            else if (TFORM[col].IndexOf("E") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 6, 4, ref i1, i2);
            else if (TFORM[col].IndexOf("D") != -1) elementos_fila_columna = ValorElementosCeldaTipo(le, r, datos, 7, 8, ref i1, i2);
            else if (TFORM[col].IndexOf("A") != -1)
            {
                elementos_fila_columna = new double[r];
                Array.Clear(elementos_fila_columna, 0, r);
            }
            return elementos_fila_columna;
        }
        private static string SalidaElementoTabla(bool le, int r, byte[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            byte[] sec_bytes = new byte[lon];
            StringBuilder sbr = new StringBuilder();
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) sec_bytes[k] = datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) sec_bytes[k] = datos[i1++, i2];
                }
                AddElementoCadena(sbr, tipo, sec_bytes, ";");
                ir++;
                if (ir == r) break;
            }
            return sbr.ToString();
        }
        private static string SalidaElementoTabla(bool le, int r, object[,] datos, int tipo, int lon, ref int i1, int i2)
        {
            byte[] sec_bytes = new byte[lon];
            StringBuilder sbr = new StringBuilder();
            int ir = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) sec_bytes[k] = (byte)datos[i1++, i2];
                }
                else
                {
                    for (int k = 0; k < lon; k++) sec_bytes[k] = (byte)datos[i1++, i2];
                }
                AddElementoCadena(sbr, tipo, sec_bytes, ";");
                ir++;
                if (ir == r) break;
            }
            return sbr.ToString();
        }
        private void TablaBinaria(object[,,] datos, int i3)
        {
            object[,] datos2 = new object[NAXISn[0], NAXISn[1]];
            for (int i1 = 0; i1 < NAXISn[0]; i1++)
            {
                for (int i2 = 0; i1 < NAXISn[1]; i2++)
                {
                    datos2[i1, i2] = datos[i1, i2, i3];
                }
            }
            TablaBinaria(datos2);
        }
        private void TablaBinaria(object[,] datos)
        {
            IniciaTablaBinaria();
            int r;
            int w;
            int ind;
            int i1;
            int col;
            StringBuilder sb;
            string[] fila = new string[TFIELDS];
            bool le = BitConverter.IsLittleEndian;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                i1 = 0;
                col = 0;
                while (col < TFIELDS)
                {
                    if ((ind = TFORM[col].IndexOf("L")) != -1)
                    {
                        // 1 byte (lógico)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 0, 1, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("X")) != -1)
                    {
                        // 1 bit

                        r = Rep(TFORM[col], ind);

                        i1++;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("B")) != -1)
                    {
                        // 1 byte (byte)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = ((byte)datos[i1++, i2]).ToString();
                    }
                    else if ((ind = TFORM[col].IndexOf("S")) != -1)
                    {
                        // 1 byte con signo (byte)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = ((sbyte)datos[i1++, i2]).ToString();
                    }
                    else if ((ind = TFORM[col].IndexOf("I")) != -1)
                    {
                        // 2 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 1, 2, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("U")) != -1)
                    {
                        // 2 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 2, 2, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("J")) != -1)
                    {
                        // 4 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 3, 4, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("V")) != -1)
                    {
                        // 4 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 4, 4, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("K")) != -1)
                    {
                        // 8 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 5, 8, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("E")) != -1)
                    {
                        // 4 bytes (simple precisión)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 6, 4, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("D")) != -1)
                    {
                        // 8 bytes (doble precisión)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 7, 8, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("C")) != -1)
                    {
                        // 8 bytes (simple precisión complejo)

                        r = Rep(TFORM[col], ind);
                        i1 += 8;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("M")) != -1)
                    {
                        // 16 bytes (doble precisión complejo)

                        r = Rep(TFORM[col], ind);
                        i1 += 16;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("P")) != -1)
                    {
                        // 8 bytes (descriptor de array)

                        r = Rep(TFORM[col], ind);
                        i1 += 8;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("A")) != -1)
                    {
                        // 1 bytes (caracter)

                        r = Rep(TFORM[col], ind);
                        if (ind == TFORM[col].Length - 1) w = 1;
                        else w = Convert.ToInt32(TFORM[col][(ind + 1)..]);
                        sb = new StringBuilder();
                        for (int k1 = 0; k1 < r; k1++)
                        {
                            for (int k2 = 0; k2 < w; k2++) sb.AppendFormat("{0}", Convert.ToChar((byte)datos[i1++, i2]));
                        }
                        fila[col++] = sb.ToString();
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 58], TFORM[col]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i1 != NAXISn[0])
                {
                    MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 110], i1, NAXISn[0]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tabla.Rows.Add(fila);
            }
        }
        private void TablaBinaria(byte[,] datos)
        {
            IniciaTablaBinaria();
            int r;
            int w;
            int ind;
            int i1;
            int col;
            StringBuilder sb;
            string[] fila = new string[TFIELDS];
            bool le = BitConverter.IsLittleEndian;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                i1 = 0;
                col = 0;
                while (col < TFIELDS)
                {
                    des_columna[col] = i1;
                    if ((ind = TFORM[col].IndexOf("L")) != -1)
                    {
                        // 1 byte (lógico)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 0, 1, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("X")) != -1)
                    {
                        // 1 bit

                        r = Rep(TFORM[col], ind);

                        i1++;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("B")) != -1)
                    {
                        // 1 byte (byte)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = datos[i1++, i2].ToString();
                    }
                    else if ((ind = TFORM[col].IndexOf("S")) != -1)
                    {
                        // 1 byte con signo (byte)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = ((sbyte)datos[i1++, i2]).ToString();
                    }
                    else if ((ind = TFORM[col].IndexOf("I")) != -1)
                    {
                        // 2 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 1, 2, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("U")) != -1)
                    {
                        // 2 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 2, 2, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("J")) != -1)
                    {
                        // 4 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 3, 4, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("V")) != -1)
                    {
                        // 4 bytes (entero sin signo)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 4, 4, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("K")) != -1)
                    {
                        // 8 bytes (entero)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 5, 8, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("E")) != -1)
                    {
                        // 4 bytes (simple precisión)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 6, 4, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("D")) != -1)
                    {
                        // 8 bytes (doble precisión)

                        r = Rep(TFORM[col], ind);
                        fila[col++] = CadenaElementoTabla(le, r, datos, 7, 8, ref i1, i2);
                    }
                    else if ((ind = TFORM[col].IndexOf("C")) != -1)
                    {
                        // 8 bytes (simple precisión complejo)

                        r = Rep(TFORM[col], ind);
                        i1 += 8;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("M")) != -1)
                    {
                        // 16 bytes (doble precisión complejo)

                        r = Rep(TFORM[col], ind);
                        i1 += 16;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("P")) != -1)
                    {
                        // 8 bytes (descriptor de array)

                        r = Rep(TFORM[col], ind);
                        i1 += 8;
                        fila[col++] = string.Empty;
                    }
                    else if ((ind = TFORM[col].IndexOf("A")) != -1)
                    {
                        // 1 bytes (caracter)

                        r = Rep(TFORM[col], ind);
                        if (ind == TFORM[col].Length - 1) w = 1;
                        else w = Convert.ToInt32(TFORM[col][(ind + 1)..]);
                        sb = new StringBuilder();
                        for (int k1 = 0; k1 < r; k1++)
                        {
                            for (int k2 = 0; k2 < w; k2++) sb.AppendFormat("{0}", Convert.ToChar(datos[i1++, i2]));
                        }
                        fila[col++] = sb.ToString();
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 58], TFORM[col]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (i1 != NAXISn[0])
                {
                    MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 110], i1, NAXISn[0]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tabla.Rows.Add(fila);
            }
        }
        private void TablaASCII(object[,,] datos, int i3)
        {
            object[,] datos2 = new object[NAXISn[0], NAXISn[1]];
            for (int i1 = 0; i1 < NAXISn[0]; i1++)
            {
                for (int i2 = 0; i1 < NAXISn[1]; i2++)
                {
                    datos2[i1, i2] = datos[i1, i2, i3];
                }
            }
            TablaASCII(datos2);
        }
        private void TablaASCII(object[,] datos)
        {
            IniciaTablaASCII();
            string[] fila = new string[TFIELDS];
            StringBuilder sb;
            string s;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                sb = new StringBuilder();
                for (int i1 = 0; i1 < NAXISn[0]; i1++)
                {
                    sb.AppendFormat("{0}", Convert.ToChar((byte)datos[i1, i2]));
                }
                s = sb.ToString();
                for (int i = 0; i < TFIELDS; i++)
                {
                    fila[i] = s.Substring(TBCOL[i] - 1, TBCOL[i + 1] - TBCOL[i]).Trim();
                }
                tabla.Rows.Add(fila);
            }
        }
        private void TablaASCII(byte[,] datos)
        {
            IniciaTablaASCII();
            string[] fila = new string[TFIELDS];
            StringBuilder sb;
            string s;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                sb = new StringBuilder();
                for (int i1 = 0; i1 < NAXISn[0]; i1++)
                {
                    sb.AppendFormat("{0}", Convert.ToChar(datos[i1, i2]));
                }
                s = sb.ToString();
                for (int i = 0; i < TFIELDS; i++)
                {
                    fila[i] = s.Substring(TBCOL[i] - 1, TBCOL[i + 1] - TBCOL[i]).Trim();
                }
                tabla.Rows.Add(fila);
            }
        }
        private void IniciaTablaASCII()
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
            tabla.ColumnCount = TFIELDS;
            TBCOL[TFIELDS] = NAXISn[0];
            for (int j = 0; j < TFIELDS; j++)
            {
                tabla.Columns[j].FillWeight = TBCOL[j + 1] - TBCOL[j];
                if (TTYPE != null)
                {
                    tabla.Columns[j].Name = TTYPE[j];
                }
                else
                {
                    tabla.Columns[j].Name = string.Format("Col{0}", j + 1);
                }
            }
            tabla.RowCount = 0;
        }
        private void IniciaTablaBinaria()
        {
            tabla.SelectionMode = DataGridViewSelectionMode.CellSelect;
            tabla.BackgroundColor = Color.LightGray;
            tabla.BorderStyle = BorderStyle.Fixed3D;
            tabla.ReadOnly = true;
            tabla.MultiSelect = true;
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.AllowUserToOrderColumns = false;
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
            tabla.ColumnCount = TFIELDS;
            for (int j = 0; j < TFIELDS; j++)
            {
                tabla.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;

                if (TTYPE != null)
                {
                    tabla.Columns[j].Name = TTYPE[j];
                }
                else
                {
                    tabla.Columns[j].Name = string.Format("Col{0}", j + 1);
                }
            }
            tabla.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
            tabla.RowCount = 0;
        }
        private void B_indexar_hiperleda_Click(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(sendaApp, "indicesHL.bin")))
            {
                DialogResult res = MessageBox.Show(Idioma.msg[Idioma.lengua, 60], Idioma.msg[Idioma.lengua, 59], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != DialogResult.Yes) return;
            }
            OpenFileDialog ficherolectura = new OpenFileDialog()
            {
                Filter = "TXT (*.txt)|*.txt|CSV (*.csv)|*.csv|TODO (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };
            if (ficherolectura.ShowDialog() == DialogResult.OK)
            {
                Disponible(false);
                IndexaHiperleda(ficherolectura.FileName);
                Disponible(true);
            }
        }
        private bool IndexaHiperleda(string fichero)
        {
            fichero_hyperleda = string.Empty;
            FileStream fs = new FileStream(fichero, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs == null)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 62], Idioma.msg[Idioma.lengua, 61], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            int len = (int)fs.Length;
            if (len == 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 63], Idioma.msg[Idioma.lengua, 61], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            octetos = new byte[len];
            fs.Read(octetos, 0, len);
            fs.Close();

            // Busca cabecera

            bool esta = false;
            int puntero_ant = 0;
            int puntero;
            int contador = 0;
            for (puntero = 0; puntero < len; puntero++)
            {
                if (octetos[puntero] == '\n')
                {
                    if (puntero > 0)
                    {
                        if ((esta = BuscaCabeceraHyperleda(octetos, puntero_ant, (int)(puntero - puntero_ant))))
                        {
                            break;
                        }
                        puntero_ant = puntero + 1;
                    }
                }
            }
            if (!esta)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 64], Idioma.msg[Idioma.lengua, 61], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Convertir horas a grados

            double chg = 360.0 / 24.0;

            // Extrae coordenadas AR y DE

            indices_hyperleda = new List<IndiceCatalogo>();
            string cad_tipo = string.Empty;
            byte byte_tipo = 255;
            string cad_ar = string.Empty;
            string cad_de = string.Empty;
            byte[] sb = new byte[64]; ;
            int nb;
            double ar;
            double de;
            int encontrados = 0;
            int sepa = 0;
            contador = 0;
            r_operacion.Text = Idioma.msg[Idioma.lengua, 71];
            r_reg_leidos.Text = string.Format("{0:N0}", contador);
            Application.DoEvents();
            puntero++;
            while (puntero < len)
            {
                if (octetos[puntero] == '#')
                {
                    // Saltar la línea

                    puntero++;
                    while (octetos[puntero++] != '\n' && puntero < len) ;
                    puntero_ant = puntero;
                }
                else if (octetos[puntero++] == ';')
                {
                    sepa++;
                    if (sepa == hyperleda_col_tipo)
                    {
                        nb = 0;
                        while (octetos[puntero] != ';')
                        {
                            if (octetos[puntero] == '.')
                            {
                                sb[nb] = (byte)',';
                            }
                            else
                            {
                                sb[nb] = octetos[puntero];
                            }
                            nb++;
                            puntero++;
                        }
                        cad_tipo = Encoding.Default.GetString(sb, 0, nb).Trim();
                        byte_tipo = 255;
                        for (byte i = 0; i < hyperleda_tipos.GetLength(0); i++)
                        {
                            if (cad_tipo.Equals(hyperleda_tipos[i, 0]))
                            {
                                byte_tipo = i;
                                break;
                            }
                        }
                        encontrados++;
                    }
                    else if (sepa == hyperleda_col_ar)
                    {
                        nb = 0;
                        while (octetos[puntero] != ';')
                        {
                            if (octetos[puntero] == '.')
                            {
                                sb[nb] = (byte)',';
                            }
                            else
                            {
                                sb[nb] = octetos[puntero];
                            }
                            nb++;
                            puntero++;
                        }
                        cad_ar = Encoding.Default.GetString(sb, 0, nb).Trim();
                        encontrados++;
                    }
                    else if (sepa == hyperleda_col_de)
                    {
                        nb = 0;
                        while (octetos[puntero] != ';')
                        {
                            if (octetos[puntero] == '.')
                            {
                                sb[nb] = (byte)',';
                            }
                            else
                            {
                                sb[nb] = octetos[puntero];
                            }
                            nb++;
                            puntero++;
                        }
                        cad_de = Encoding.Default.GetString(sb, 0, nb).Trim();
                        encontrados++;
                    }
                    if (encontrados == 3)
                    {
                        contador++;
                        if (cad_ar.Length > 0 && cad_de.Length > 0)
                        {
                            // Hyperleda contiene al2000 en horas y de2000 en grados
                            // FITS tiene ambas en grados

                            ar = Convert.ToDouble(cad_ar) * chg;
                            de = Convert.ToDouble(cad_de);
                            indices_hyperleda.Add(new IndiceCatalogo(puntero_ant, byte_tipo, ar, de));
                        }

                        // Terminar la línea

                        while (octetos[puntero++] != '\n' && puntero < len) ;
                        puntero_ant = puntero;
                        encontrados = 0;
                        sepa = 0;
                        if (contador % X1M == 0)
                        {
                            r_reg_leidos.Text = string.Format("{0:N0}", contador);
                            Application.DoEvents();
                        }
                    }
                }
            }
            r_operacion.Text = Idioma.msg[Idioma.lengua, 72];
            r_reg_leidos.Text = string.Format("{0:N0}", indices_hyperleda.Count);

            Application.DoEvents();

            // Ordena

            indices_hyperleda.Sort(
                delegate (IndiceCatalogo p1, IndiceCatalogo p2)
                {
                    int comparador = p1.al2000.CompareTo(p2.al2000);
                    if (comparador == 0)
                    {
                        return p1.de2000.CompareTo(p2.de2000);
                    }
                    return comparador;
                }
            );

            // Salva a ficheros

            FileStream fe = new FileStream(Path.Combine(sendaApp, "indicesHL.bin"), FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter w = new BinaryWriter(fe);

            // Nombre del fichero hyperleda

            fichero_hyperleda = Path.GetFileName(fichero);
            w.Write(fichero_hyperleda);

            // Copiar el fichero a la senda de la App si no está ya

            string syn = Path.Combine(sendaApp, fichero_hyperleda);
            if (!fichero.Equals(syn))
            {
                File.Copy(fichero, syn);
            }

            // Indices

            foreach (IndiceCatalogo i in indices_hyperleda)
            {
                w.Write(i.indice);
                w.Write(i.tipo);
                w.Write(i.al2000);
                w.Write(i.de2000);
            }
            w.Close();
            r_operacion.Text = Idioma.msg[Idioma.lengua, 73];
            return true;
        }
        private bool BuscaCabeceraHyperleda(byte[] octetos, int p, int n)
        {
            if (
                octetos[p] == '$' &&
                octetos[p + 1] == 'o' &&
                octetos[p + 2] == 'b' &&
                octetos[p + 3] == 'j' &&
                octetos[p + 4] == 'n' &&
                octetos[p + 5] == 'a' &&
                octetos[p + 6] == 'm' &&
                octetos[p + 7] == 'e' &&
                octetos[p + 8] == ';')
            {
                // Busca columnas tipo, ar y de

                int contador = 0;
                bool coincide;
                int encontrados = 0;
                for (int k = 0; k < n; k++)
                {
                    if (octetos[p] == ';') contador++;
                    coincide = true;
                    for (int i = 0; i < hyperleda_obj_tipo.Length; i++)
                    {

                        if (octetos[p + i] != hyperleda_obj_tipo[i])
                        {
                            coincide = false;
                            break;
                        }
                    }
                    if (coincide)
                    {
                        hyperleda_col_tipo = contador;
                        encontrados++;
                        if (encontrados == 3) break;
                    }
                    coincide = true;
                    for (int i = 0; i < hyperleda_nombre_ar.Length; i++)
                    {

                        if (octetos[p + i] != hyperleda_nombre_ar[i])
                        {
                            coincide = false;
                            break;
                        }
                    }
                    if (coincide)
                    {
                        hyperleda_col_ar = contador;
                        encontrados++;
                        if (encontrados == 3) break;
                    }
                    coincide = true;
                    for (int i = 0; i < hyperleda_nombre_de.Length; i++)
                    {

                        if (octetos[p + i] != hyperleda_nombre_de[i])
                        {
                            coincide = false;
                            break;
                        }
                    }
                    if (coincide)
                    {
                        hyperleda_col_de = contador;
                        encontrados++;
                        if (encontrados == 3) break;
                    }
                    p++;
                }
                return true;
            }
            return false;
        }
        public bool LeeIndicesHyperleda()
        {
            string fic = Path.Combine(sendaApp, "indicesHL.bin");
            if (!File.Exists(fic))
            {
                MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 66], fic), Idioma.msg[Idioma.lengua, 65], MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            string s = panel_img.r_x.Text;
            panel_img.r_x.ForeColor = Color.Red;
            panel_img.r_x.Text = Idioma.msg[Idioma.lengua, 132];
            Application.DoEvents();
            FileStream fe = new FileStream(Path.Combine(sendaApp, "indicesHL.bin"), FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fe != null)
            {
                BinaryReader r = new BinaryReader(fe);
                indices_hyperleda = new List<IndiceCatalogo>();
                fichero_hyperleda = r.ReadString();
                int puntero_ant;
                byte tipo;
                double ar;
                double de;
                int contador = 0;
                while (r.BaseStream.Position != r.BaseStream.Length)
                {
                    puntero_ant = r.ReadInt32();
                    tipo = r.ReadByte();
                    ar = r.ReadDouble();
                    de = r.ReadDouble();
                    indices_hyperleda.Add(new IndiceCatalogo(puntero_ant, tipo, ar, de));
                    contador++;
                    if (contador % X1M == 0)
                    {
                        panel_img.r_x.Text = string.Format(Idioma.msg[Idioma.lengua, 132] + " {0:N0}", contador);
                        Application.DoEvents();
                    }
                }
                fe.Close();
                panel_img.r_x.Text = string.Empty;
                Application.DoEvents();
                return true;
            }
            fichero_hyperleda = string.Empty;
            panel_img.r_x.ForeColor = Color.Blue;
            panel_img.r_x.Text = s;
            MessageBox.Show(Idioma.msg[Idioma.lengua, 67], Idioma.msg[Idioma.lengua, 65], MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        private void B_indexar_sao_Click(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(sendaApp, "indicesSAO.bin")))
            {
                DialogResult res = MessageBox.Show(Idioma.msg[Idioma.lengua, 69], Idioma.msg[Idioma.lengua, 68], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != DialogResult.Yes) return;
            }
            OpenFileDialog ficherolectura = new OpenFileDialog()
            {
                Filter = "TXT (*.txt)|*.txt|CSV (*.csv)|*.csv|TODO (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };
            if (ficherolectura.ShowDialog() == DialogResult.OK)
            {
                Disponible(false);
                IndexaSao(ficherolectura.FileName);
                Disponible(true);
            }
        }
        private bool IndexaSao(string fichero)
        {
            fichero_sao = string.Empty;
            FileStream fs = new FileStream(fichero, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs == null)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 62], Idioma.msg[Idioma.lengua, 70], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            int len = (int)fs.Length;
            if (len == 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 63], Idioma.msg[Idioma.lengua, 70], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            octetos = new byte[len];
            fs.Read(octetos, 0, len);
            fs.Close();

            // Convertor radianes a grados

            double crg = 180.0 / Math.PI;

            // Extrae coordenadas AR y DE

            indices_sao = new List<IndiceCatalogo>();
            byte byte_tipo;
            string cad_ar = string.Empty;
            string cad_de = string.Empty;
            byte[] sb = new byte[11];
            int nb;
            double ar;
            double de;
            int contador = 0;
            r_operacion.Text = Idioma.msg[Idioma.lengua, 71];
            r_reg_leidos.Text = string.Format("{0:N0}", contador);
            Application.DoEvents();
            int puntero_ant;
            int puntero = 0;
            while (puntero < len)
            {
                puntero_ant = puntero;
                byte_tipo = (byte)(200 + octetos[puntero + 92]);
                puntero += 183;
                for (nb = 0; nb < 10; nb++)
                {
                    if (octetos[puntero] == '.')
                    {
                        sb[nb] = (byte)',';
                    }
                    else
                    {
                        sb[nb] = octetos[puntero];
                    }
                    puntero++;
                }
                cad_ar = Encoding.Default.GetString(sb, 0, nb).Trim();
                for (nb = 0; nb < 11; nb++)
                {
                    if (octetos[puntero] == '.')
                    {
                        sb[nb] = (byte)',';
                    }
                    else
                    {
                        sb[nb] = octetos[puntero];
                    }
                    puntero++;
                }
                cad_de = Encoding.Default.GetString(sb, 0, nb).Trim();
                contador++;
                if (cad_ar.Length > 0 && cad_de.Length > 0)
                {
                    // SAO contiene al2000 y de2000 en radianes
                    // FITS tiene ambas en grados

                    ar = Convert.ToDouble(cad_ar) * crg;
                    de = Convert.ToDouble(cad_de) * crg;
                    indices_sao.Add(new IndiceCatalogo(puntero_ant, byte_tipo, ar, de));
                }
                puntero++;
                if (contador % 100000 == 0)
                {
                    r_reg_leidos.Text = string.Format("{0:N0}", contador);
                    Application.DoEvents();
                }
            }
            r_operacion.Text = Idioma.msg[Idioma.lengua, 72];
            r_reg_leidos.Text = string.Format("{0:N0}", indices_sao.Count);
            Application.DoEvents();

            // Ordena

            indices_sao.Sort(
                delegate (IndiceCatalogo p1, IndiceCatalogo p2)
                {
                    int comparador = p1.al2000.CompareTo(p2.al2000);
                    if (comparador == 0)
                    {
                        return p1.de2000.CompareTo(p2.de2000);
                    }
                    return comparador;
                }
            );

            // Salva a ficheros

            FileStream fe = new FileStream(Path.Combine(sendaApp, "indicesSAO.bin"), FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter w = new BinaryWriter(fe);

            // Nombre del fichero sao

            fichero_sao = Path.GetFileName(fichero);
            w.Write(fichero);

            // Copiar el fichero a la senda de la App si no está ya

            string syn = Path.Combine(sendaApp, fichero_sao);
            if (!fichero.Equals(syn))
            {
                File.Copy(fichero, syn);
            }

            // Indices

            foreach (IndiceCatalogo i in indices_sao)
            {
                w.Write(i.indice);
                w.Write(i.tipo);
                w.Write(i.al2000);
                w.Write(i.de2000);
            }
            w.Close();
            r_operacion.Text = Idioma.msg[Idioma.lengua, 73];
            return true;
        }
        public bool LeeIndicesSao()
        {
            string fic = Path.Combine(sendaApp, "indicesSAO.bin");
            if (!File.Exists(fic))
            {
                MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 75], fic), Idioma.msg[Idioma.lengua, 74], MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            string s = panel_img.r_x.Text;
            panel_img.r_x.ForeColor = Color.Red;
            panel_img.r_x.Text = Idioma.msg[Idioma.lengua, 133];
            Application.DoEvents();
            FileStream fe = new FileStream(Path.Combine(sendaApp, "indicesSAO.bin"), FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fe != null)
            {
                BinaryReader r = new BinaryReader(fe);
                indices_sao = new List<IndiceCatalogo>();
                fichero_sao = r.ReadString();
                int puntero_ant;
                byte tipo;
                double ar;
                double de;
                int contador = 0;
                while (r.BaseStream.Position != r.BaseStream.Length)
                {
                    puntero_ant = r.ReadInt32();
                    tipo = r.ReadByte();
                    ar = r.ReadDouble();
                    de = r.ReadDouble();
                    indices_sao.Add(new IndiceCatalogo(puntero_ant, tipo, ar, de));
                    contador++;
                    if (contador % 100000 == 0)
                    {
                        panel_img.r_x.Text = string.Format(Idioma.msg[Idioma.lengua, 133] + " {0:N0}", contador);
                        Application.DoEvents();
                    }
                }
                fe.Close();
                panel_img.r_x.Text = string.Empty;
                Application.DoEvents();
                return true;
            }
            fichero_sao = string.Empty;
            panel_img.r_x.ForeColor = Color.Blue;
            panel_img.r_x.Text = s;
            MessageBox.Show(Idioma.msg[Idioma.lengua, 76], Idioma.msg[Idioma.lengua, 74], MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        private void B_buscar_Click(object sender, EventArgs e)
        {
            Form6 f_buscar = new Form6
            {
                fits = this
            };
            double ar = 0;
            double de = 0;
            if (hdu != null && hdu_actual != -1 && hdu[hdu_actual].n_tablas > 0 && hdu[hdu_actual].conjunto == 1)
            {
                if (tabla.SelectedCells.Count == 2)
                {
                    int fila1 = tabla.SelectedCells[0].RowIndex;
                    int col1 = tabla.SelectedCells[0].ColumnIndex;
                    int fila2 = tabla.SelectedCells[1].RowIndex;
                    int col2 = tabla.SelectedCells[1].ColumnIndex;
                    if (fila1 == fila2 && col1 != col2)
                    {
                        int r1 = SubColumnas(col1);
                        int r2 = SubColumnas(col2);
                        if (r1 == 1 || r2 == 1)
                        {
                            ar = Convert.ToDouble(tabla.Rows[fila1].Cells[col2].Value.ToString());
                            de = Convert.ToDouble(tabla.Rows[fila1].Cells[col1].Value.ToString());
                        }
                    }
                }
            }
            f_buscar.Datos(ar, de, 10);
            f_buscar.ShowDialog(this);
        }
        private void B_redibuja_Click(object sender, EventArgs e)
        {
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i < 0) return;
            if (hdu[i].n_imagenes > 0)
            {
                // Imagen

                if (img == null) return;
                hdu[i].normalizar = normalizar.Checked;
                hdu[i].cota_inf = v_acotar_min.Text;
                hdu[i].cota_sup = v_acotar_max.Text;
                hdu[i].ind_color_cota_inf = ind_color_cota_inf;
                hdu[i].ind_color_cota_sup = ind_color_cota_sup;
                hdu[i].raiz = raiz.Checked;
                hdu[i].invertir_y = invertir_y.Checked;
                hdu[i].invertir_x = invertir_x.Checked;
                hdu[i].alta_resolucion = alta_resolucion.Checked;
                if (NAXIS == 2)
                {
                    panel_img.Redibuja();
                }
                else if (NAXIS == 3)
                {
                    int i3 = sel_imagen.SelectedIndex;
                    panel_img.Redibuja(i3);
                }
            }
            else if (hdu[i].n_tablas > 0 && hdu[i].conjunto == 1)
            {
                // Espectro

                if (tabla.SelectedCells.Count == 2)
                {
                    int fila1 = tabla.SelectedCells[1].RowIndex;
                    int col1 = tabla.SelectedCells[1].ColumnIndex;
                    int fila2 = tabla.SelectedCells[0].RowIndex;
                    int col2 = tabla.SelectedCells[0].ColumnIndex;
                    if (fila1 != fila2) return;
                    if (col1 == col2) return;
                    int r1 = SubColumnas(col1);
                    int r2 = SubColumnas(col2);
                    if (r1 != r2) return;
                    if (r1 < 2) return;
                    double[] elementos1;
                    double[] elementos2;
                    bool le = BitConverter.IsLittleEndian;
                    int i1 = des_columna[col1];
                    if (byPorDato == 1)
                    {
                        elementos1 = ValorElementosCelda(le, r1, datosb, col1, ref i1, fila1);
                        i1 = des_columna[col2];
                        elementos2 = ValorElementosCelda(le, r2, datosb, col2, ref i1, fila2);
                    }
                    else
                    {
                        elementos1 = ValorElementosCelda(le, r1, datos, col1, ref i1, fila1);
                        i1 = des_columna[col2];
                        elementos2 = ValorElementosCelda(le, r2, datos, col2, ref i1, fila2);
                    }
                    DialogResult res = MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 112], tabla.Columns[col1].Name), Idioma.msg[Idioma.lengua, 111], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        es_actual = new Espectro(elementos1, elementos2);
                    }
                    else
                    {
                        es_actual = new Espectro(elementos2, elementos1);
                    }
                    DibujaEspectro();
                }
            }
        }
        private void B_conv_espectro_Click(object sender, EventArgs e)
        {
            b_conv_espectro.Visible = false;
            if (CRPIX == null || CRVAL == null || CDELT == null || CDELT[0] == 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 130], Idioma.msg[Idioma.lengua, 129], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                es_actual = null;
                return;
            }
            Disponible(false);
            int cada;
            if (NAXISn[0] > 50000)
            {
                cada = NAXISn[0] / 50000;
            }
            else
            {
                cada = 1;
            }
            double incx = cada * CDELT[0];
            int nd = NAXISn[0] / cada;
            double[] elementos1 = new double[nd];
            double[] elementos2 = new double[nd];
            double x = CRVAL[0] - CRPIX[0] * CDELT[0];
            int i = 0;
            for (int i1 = 0; i1 < nd * cada; i1 += cada)
            {
                elementos1[i] = x;
                switch (hdu[hdu_actual].clase_dato)
                {
                    case 1:
                        elementos2[i] = datosb[i1, 0];
                        break;
                    case 2:
                        elementos2[i] = datoss[i1, 0];
                        break;
                    case 3:
                        elementos2[i] = datosi[i1, 0];
                        break;
                    case 4:
                        elementos2[i] = datosf[i1, 0];
                        break;
                    case 6:
                        elementos2[i] = datosl[i1, 0];
                        break;
                    case 5:
                        elementos2[i] = datosd[i1, 0];
                        break;
                }
                x += incx;
                i++;
            }
            es_actual = new Espectro(elementos1, elementos2);
            DibujaEspectro();
            Disponible(true);
        }
        public void DibujaEspectro()
        {
            Disponible(false);
            double ANCHO_MARCA = 100;

            // z = (observada - emitida) / emitida
            // emitida = observada / (1 + z)

            double z = (panel_img.v_z.Text.Trim().Length == 0) ? 0 : Convert.ToDouble(panel_img.v_z.Text.Trim().Replace(s_millar, s_decimal));
            double inv_1mz = 1 / (1 + z);

            panel_img.SelInterfaz(1);
            es_actual.util_x = es_actual.x.Length;

            double factor = (double)es_actual.util_x / panel_img.ancho_lienzo;
            if (factor < 0.6)
            {
                es_actual.mizq = (int)(200 * factor);
                es_actual.mdch = (int)(60 * factor);
                es_actual.msup = (int)(60 * factor);
                es_actual.minf = (int)(120 * factor);
            }
            else
            {
                es_actual.mizq = (int)(140 * factor);
                es_actual.mdch = (int)(50 * factor);
                es_actual.msup = (int)(50 * factor);
                es_actual.minf = (int)(70 * factor);
            }

            es_actual.dim_x = es_actual.util_x + es_actual.mizq + es_actual.mdch;
            es_actual.dim_y = (int)(es_actual.dim_x * panel_img.alto_lienzo / (double)panel_img.ancho_lienzo);

            // El tamaño máximo de un objeto es 2GB:2.147.483.648

            factor = 4;
            if ((double)es_actual.dim_x * es_actual.dim_y * factor > 2147000000.0)
            {
                es_actual.dim_y = (int)(2147000000.0 / factor / es_actual.dim_x);
                if (es_actual.dim_y < 500)
                {
                    MessageBox.Show(Idioma.msg[Idioma.lengua, 115], Idioma.msg[Idioma.lengua, 111], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Disponible(true);
                    return;
                }
            }
            es_actual.util_y = es_actual.dim_y - es_actual.msup - es_actual.minf;
            try
            {
                img = new Bitmap(es_actual.dim_x, es_actual.dim_y);
            }
            catch (Exception e)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 115] + ". " + e.Message, Idioma.msg[Idioma.lengua, 111], MessageBoxButtons.OK, MessageBoxIcon.Information);
                Disponible(true);
                return;
            }
            es_actual.minx = double.MaxValue;
            es_actual.maxx = double.MinValue;
            es_actual.miny = double.MaxValue;
            es_actual.maxy = double.MinValue;
            for (int i = 0; i < es_actual.util_x; i++)
            {
                if (es_actual.minx > es_actual.x[i] * inv_1mz) es_actual.minx = es_actual.x[i] * inv_1mz;
                if (es_actual.maxx < es_actual.x[i] * inv_1mz) es_actual.maxx = es_actual.x[i] * inv_1mz;
                if (es_actual.miny > es_actual.y[i]) es_actual.miny = es_actual.y[i];
                if (es_actual.maxy < es_actual.y[i]) es_actual.maxy = es_actual.y[i];
            }
            if (es_actual.maxx == es_actual.minx)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 126], Idioma.msg[Idioma.lengua, 111], MessageBoxButtons.OK, MessageBoxIcon.Information);
                Disponible(true);
                return;
            }
            if (es_actual.maxy == es_actual.miny)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 127], Idioma.msg[Idioma.lengua, 111], MessageBoxButtons.OK, MessageBoxIcon.Information);
                Disponible(true);
                return;
            }

            // Redondear el X mínimo a cientos por defecto

            int rx = (int)(es_actual.minx / ANCHO_MARCA);
            es_actual.minx = rx * ANCHO_MARCA;

            // Redondear el X máximo a cientos por exceso

            int ry = (int)(es_actual.maxx / ANCHO_MARCA) + 1;
            es_actual.maxx = ry * ANCHO_MARCA;

            double pvx = es_actual.util_x / (es_actual.maxx - es_actual.minx);
            double pvy = es_actual.util_y / (es_actual.maxy - es_actual.miny);
            int px1;
            int py1;
            int px2;
            int py2;
            AjustaLienzo();
            es_actual.escala_x = (double)es_actual.dim_x / panel_img.lienzo.Width;
            es_actual.escala_y = (double)es_actual.dim_y / panel_img.lienzo.Height;
            int ancho_lapiz = (int)(2 * es_actual.escala_x);
            if (ancho_lapiz < 2) ancho_lapiz = 2;
            int ancho_lapiz_fino = ancho_lapiz / 2;
            if (ancho_lapiz_fino < 1) ancho_lapiz_fino = 1;
            Brush brocha_fondo;
            Brush brocha_texto;
            Brush brocha_picos_a;
            Brush brocha_picos_e;
            Brush brocha_atomos;
            Pen lapiz;
            Pen lapiz_verde;
            Pen lapiz_fino_rojo;
            Pen lapiz_fino_azul;
            if (panel_img.fondo_blanco)
            {
                brocha_fondo = Brushes.White;
                brocha_texto = Brushes.Black;
                brocha_picos_a = Brushes.Blue;
                brocha_picos_e = Brushes.Red;
                brocha_atomos = Brushes.Green;
                lapiz = new Pen(Color.Black, ancho_lapiz);
                lapiz_verde = new Pen(Color.Green, ancho_lapiz);
                lapiz_fino_rojo = new Pen(Color.Red, ancho_lapiz_fino);
                lapiz_fino_azul = new Pen(Color.Blue, ancho_lapiz_fino);
            }
            else
            {
                brocha_fondo = Brushes.Black;
                brocha_texto = Brushes.White;
                brocha_picos_a = Brushes.LightBlue;
                brocha_picos_e = Brushes.Red;
                brocha_atomos = Brushes.LightGreen;
                lapiz = new Pen(Color.White, ancho_lapiz);
                lapiz_verde = new Pen(Color.LightGreen, ancho_lapiz);
                lapiz_fino_rojo = new Pen(Color.Red, ancho_lapiz_fino);
                lapiz_fino_azul = new Pen(Color.LightBlue, ancho_lapiz_fino);
            }
            Graphics g = Graphics.FromImage(img);
            g.FillRectangle(brocha_fondo, 0, 0, es_actual.dim_x, es_actual.dim_y);

            // Se dibuja en img antes de su adaptación al lienzo

            // Eje X

            Font fuente = new Font("Verdana", (float)(10 * es_actual.escala_x));
            int n_marcasx = (int)((es_actual.maxx - es_actual.minx) / ANCHO_MARCA) + 1;
            if (n_marcasx > 50) n_marcasx /= 10;
            if (n_marcasx < 5) n_marcasx *= 5;
            ANCHO_MARCA = (es_actual.maxx - es_actual.minx) / n_marcasx;

            double xm = es_actual.minx;
            py1 = es_actual.msup + es_actual.util_y + es_actual.minf / 2;
            for (int i = 0; i < n_marcasx; i++)
            {
                px1 = (int)((xm - es_actual.minx) * pvx);
                g.DrawLine(lapiz, es_actual.mizq + px1, es_actual.msup + es_actual.util_y, es_actual.mizq + px1, py1);
                g.DrawString(string.Format("{0:N0}", xm), fuente, brocha_texto, es_actual.mizq + px1, py1);
                xm += ANCHO_MARCA;
            }

            // Cotas en el eje Y

            // Mayor

            g.DrawLine(lapiz_fino_rojo, es_actual.mizq, es_actual.msup, es_actual.mizq + es_actual.util_x, es_actual.msup);
            py1 = (int)(es_actual.msup - (fuente.Height + 4 * es_actual.escala_y));
            if (py1 < 0) py1 = 0;
            g.DrawString(string.Format("{0:e3}", es_actual.maxy), fuente, brocha_texto, 0, py1);

            // Menor

            g.DrawLine(lapiz_fino_rojo, es_actual.mizq, es_actual.msup + es_actual.util_y, es_actual.mizq + es_actual.util_x, es_actual.msup + es_actual.util_y);
            g.DrawString(string.Format("{0:e3}", es_actual.miny), fuente, brocha_texto, 0, (float)(es_actual.msup + es_actual.util_y + 4 * es_actual.escala_y));

            // Líneas atómicas

            string linea;
            py1 = es_actual.util_y;
            for (int i = 0; i < panel_img.lista_elegidas.Items.Count; i++)
            {
                linea = panel_img.lista_elegidas.Items[i].ToString();
                xm = Convert.ToDouble(linea.Substring(0, 10).Trim());
                px1 = (int)((xm - es_actual.minx) * pvx);
                g.DrawLine(lapiz_verde, es_actual.mizq + px1, es_actual.msup + py1, es_actual.mizq + px1, es_actual.msup);
            }

            // Picos

            if (panel_img.picos != null && panel_img.picos.Count > 0)
            {
                int k;
                py1 = es_actual.util_y;
                for (int i = 0; i < panel_img.picos.Count; i++)
                {
                    k = panel_img.picos[i].indice;
                    xm = es_actual.x[k] * inv_1mz;
                    px1 = (int)((xm - es_actual.minx) * pvx);
                    if (panel_img.picos[i].valor < 0)
                    {
                        g.DrawLine(lapiz_fino_rojo, es_actual.mizq + px1, es_actual.msup + py1, es_actual.mizq + px1, es_actual.msup);
                    }
                    else
                    {
                        g.DrawLine(lapiz_fino_azul, es_actual.mizq + px1, es_actual.msup + py1, es_actual.mizq + px1, es_actual.msup);
                    }
                }
            }

            float ux;
            int cy;
            string nombre;

            // Rótulos de las líneas atómicas

            ux = 0;
            cy = 0;
            for (int i = 0; i < panel_img.lista_elegidas.Items.Count; i++)
            {
                linea = panel_img.lista_elegidas.Items[i].ToString();
                xm = Convert.ToDouble(linea.Substring(0, 10).Trim());
                nombre = linea[10..].Trim();
                px1 = (int)((xm - es_actual.minx) * pvx);
                if (es_actual.mizq + px1 < ux)
                {
                    cy++;
                    if (cy == 5) cy = 0;
                    py2 = (int)(4 * es_actual.escala_y + cy * fuente.Height);
                }
                else
                {
                    cy = 0;
                    py2 = (int)(4 * es_actual.escala_y);
                }
                g.DrawString(nombre, fuente, brocha_atomos, es_actual.mizq + px1, py2);
                ux = es_actual.mizq + px1 + g.MeasureString(nombre, fuente).Width;
            }

            // Rótulos de los picos

            if (panel_img.picos != null && panel_img.picos.Count > 0)
            {
                int k;
                ux = 0;
                cy = 0;
                for (int i = 0; i < panel_img.picos.Count; i++)
                {
                    k = panel_img.picos[i].indice;
                    xm = es_actual.x[k] * inv_1mz;
                    px1 = (int)((xm - es_actual.minx) * pvx);
                    nombre = string.Format("{0:f3}", xm);
                    if (es_actual.mizq + px1 + 2 < ux)
                    {
                        cy++;
                        if (cy == 5) cy = 0;
                        py2 = (int)(es_actual.util_y - cy * fuente.Height - 4 * es_actual.escala_y);
                    }
                    else
                    {
                        cy = 0;
                        py2 = es_actual.util_y;
                    }
                    if (panel_img.picos[i].valor < 0)
                    {
                        g.DrawString(nombre, fuente, brocha_picos_e, es_actual.mizq + px1 + 2, (float)(es_actual.msup + py2 - fuente.Height - 4 * es_actual.escala_y)); ;
                    }
                    else
                    {
                        g.DrawString(nombre, fuente, brocha_picos_a, es_actual.mizq + px1 + 2, (float)(es_actual.msup + py2 - fuente.Height - 4 * es_actual.escala_y)); ;
                    }
                    ux = es_actual.mizq + px1 + 2 + g.MeasureString(nombre, fuente).Width;
                }
            }

            // Valores

            if (panel_img.S_simplifica.Checked)
            {
                // Ajuste por mínimos cuadrados

                // Una banda de ancho fijo 'corte', centrado en el polinomio

                double corte;
                if (panel_img.v_hueco.Text.Trim().Length == 0)
                {
                    // Un 'MIN_DEFECTO' % del rango de variación : 'es_actual.maxy - es_actual.miny'

                    panel_img.v_hueco.Text = string.Format("{0}", MIN_DEFECTO * 100);
                    corte = MIN_DEFECTO * (es_actual.maxy - es_actual.miny);
                }
                else
                {
                    corte = Convert.ToDouble(panel_img.v_hueco.Text.Trim().Replace(s_millar, s_decimal)) / 100 * (es_actual.maxy - es_actual.miny);
                    if (corte < 0)
                    {
                        corte = 0;
                        panel_img.v_hueco.Text = "0";
                    }
                }
                if (AjustaPolinomio(corte))
                {
                    double[] y = new double[es_actual.util_x];
                    for (int k = 0; k < es_actual.util_x; k++)
                    {
                        if (Math.Abs(es_actual.y[k] - regLin.Ycalc[k]) < corte)
                        {
                            // Dentro del corte, el valor ajustado

                            y[k] = regLin.Ycalc[k];
                        }
                        else
                        {
                            // Fuera del corte, el dato

                            y[k] = es_actual.y[k];
                        }
                    }
                    for (int i = 0; i < es_actual.util_x - 1; i++)
                    {
                        px1 = (int)((es_actual.x[i] * inv_1mz - es_actual.minx) * pvx);
                        py1 = es_actual.util_y - (int)((y[i] - es_actual.miny) * pvy);
                        px2 = (int)((es_actual.x[i + 1] * inv_1mz - es_actual.minx) * pvx);
                        py2 = es_actual.util_y - (int)((y[i + 1] - es_actual.miny) * pvy);
                        g.DrawLine(lapiz, es_actual.mizq + px1, es_actual.msup + py1, es_actual.mizq + px2, es_actual.msup + py2);
                    }
                }
            }
            else
            {
                for (int i = 0; i < es_actual.util_x - 1; i++)
                {
                    px1 = (int)((es_actual.x[i] * inv_1mz - es_actual.minx) * pvx);
                    py1 = es_actual.util_y - (int)((es_actual.y[i] - es_actual.miny) * pvy);
                    px2 = (int)((es_actual.x[i + 1] * inv_1mz - es_actual.minx) * pvx);
                    py2 = es_actual.util_y - (int)((es_actual.y[i + 1] - es_actual.miny) * pvy);
                    g.DrawLine(lapiz, es_actual.mizq + px1, es_actual.msup + py1, es_actual.mizq + px2, es_actual.msup + py2);
                }
            }
            panel_img.lienzo.Image = img;
            Disponible(true);
        }
        private void DatosXY(double[] yfuente, ref double[] ydestino, double[] xfuente, ref double[,] xdestino, int grado, int desde, int hasta)
        {
            ydestino = new double[hasta - desde];
            int k = 0;
            for (int i = desde; i < hasta; i++)
            {
                ydestino[k] = yfuente[i];
                k++;
            }
            xdestino = DatosX(xfuente, grado, desde, hasta);
        }
        private double[,] DatosX(double[] xfuente, int grado, int desde, int hasta)
        {
            int terminos = grado + 1;
            double[,] xdestino = new double[terminos, hasta - desde];
            double xi;
            double xp;
            int k = 0;
            for (int i = desde; i < hasta; i++)
            {
                xdestino[0, k] = 1;
                xi = xp = xfuente[i];
                for (int j = 1; j < terminos; j++)
                {
                    xdestino[j, k] = xp;
                    xp *= xi;
                }
                k++;
            }
            return xdestino;
        }
        private double[] FraccionaW(double[] w, int desde, int hasta)
        {
            double[] wf = new double[hasta - desde];
            int k = 0;
            for (int i = desde; i < hasta; i++)
            {
                wf[k] = w[i];
                k++;
            }
            return wf;
        }
        private bool Regresion(RegresionLineal rl, double[] w, int grado)
        {
            int n = es_actual.x.Length;
            double[] y = null;
            double[,] x = null;

            // Realizar dos ajustes, ambos con 2/3 de los datos:
            //  1. desde el dato 0 hasta el dato 2/3 del total
            //  2. desde el dato 1/3 hasta el último dato

            int desde_a = 0;
            int hasta_a = (2 * n) / 3;
            DatosXY(es_actual.y, ref y, es_actual.x, ref x, grado, desde_a, hasta_a);
            RegresionLineal rlA = new RegresionLineal();
            if (!rlA.Regress(y, x, FraccionaW(w, desde_a, hasta_a), 0))
            {
                return false;
            }
            int desde_b = n / 3;
            int hasta_b = n;
            DatosXY(es_actual.y, ref y, es_actual.x, ref x, grado, desde_b, hasta_b);
            RegresionLineal rlB = new RegresionLineal();
            if (!rlB.Regress(y, x, FraccionaW(w, desde_b, hasta_b), 0))
            {
                return false;
            }

            // Fusionar los dos ajustes.

            DatosXY(es_actual.y, ref y, es_actual.x, ref x, grado, 0, n);
            Soldar(w, x, y, rlA, rlB, rl, desde_a, hasta_a, desde_b, hasta_b);
            return true;
        }
        private void Soldar(double[] w, double[,] x, double[] y, RegresionLineal linRegFteA, RegresionLineal linRegFteB, RegresionLineal linRegDestino, int desdeA, int hastaA, int desdeB, int hastaB)
        {
            // Unir las dos regresiones A y B

            int primer_compartido = desdeB;
            int ultimo_compartido = hastaA;
            double numero_compartidos = ultimo_compartido - primer_compartido;
            int terminos_polinomio = x.GetLength(0);
            linRegDestino.CA = new double[terminos_polinomio];
            linRegDestino.CB = new double[terminos_polinomio];
            linRegDestino.Ycalc = new double[hastaB];
            linRegDestino.SDVMovil = new double[hastaB];

            // Datos de A

            int k = 0;
            for (int i = desdeA; i < primer_compartido; i++)
            {
                linRegDestino.Ycalc[k] = linRegFteA.Ycalc[k];
                linRegDestino.SDVMovil[k] = linRegFteA.SDVMovil[k];
                k++;
            }
            linRegDestino.RYSQa = linRegFteA.RYSQa;
            linRegDestino.FRega = linRegFteA.FRega;
            linRegDestino.SDVErra = linRegFteA.SDVErra;
            for (int i = 0; i < terminos_polinomio; i++)
            {
                linRegDestino.CA[i] = linRegFteA.CA[i];
            }

            // Para los datos compartidos se pondera con peso variable de 0 (primer dato compartido por B) a 1 (último dato compartido por B), de forma lineal

            double pondera_b;
            int kb = 0;
            for (int i = primer_compartido; i < ultimo_compartido; i++)
            {
                pondera_b = kb / numero_compartidos;
                linRegDestino.Ycalc[k] = (1 - pondera_b) * linRegFteA.Ycalc[k] + pondera_b * linRegFteB.Ycalc[kb];
                linRegDestino.SDVMovil[k] = (linRegFteA.SDVMovil[k] + linRegFteB.SDVMovil[kb]) / 2;
                k++;
                kb++;
            }

            // Datos de B

            for (int i = ultimo_compartido; i < hastaB; i++)
            {
                linRegDestino.Ycalc[k] = linRegFteB.Ycalc[kb];
                linRegDestino.SDVMovil[k] = linRegFteB.SDVMovil[kb];
                k++;
                kb++;
            }
            linRegDestino.RYSQb = linRegFteB.RYSQa;
            linRegDestino.FRegb = linRegFteB.FRega;
            linRegDestino.SDVErrb = linRegFteB.SDVErra;
            for (int i = 0; i < terminos_polinomio; i++)
            {
                linRegDestino.CB[i] = linRegFteB.CA[i];
            }

            // Estadísticas del ajuste fusionado

            linRegDestino.Estadisticas(y, x, w, 0);
        }
        private bool AjustaPolinomio(double corte)
        {
            const int MAX_GRADO = 10;
            int n = es_actual.x.Length;

            double[] w = new double[n];
            for (int i = 0; i < n; i++)
            {
                w[i] = 1;
            }
            int grado = 4;
            if (!Regresion(regLin, w, grado))
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 139], Idioma.msg[Idioma.lengua, 140], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            AnulaPicos(w, corte);
            if (!Regresion(regLin, w, grado))
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 139], Idioma.msg[Idioma.lengua, 140], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Realiza nuevas regresiones, aumentando el grado, mientras R2 mejore
            // A mayor grado debería producirse un mejor ajuste, pero hay un problema de precisión numérica
            // un número 'double' tiene un máximo de 16 cifras significativas, los valores de la longitud de onda
            // son del orden de 10^3 por lo que el termino x^10 del polinomio es del orden de 10^30 de forma que
            // sumarle el termino x^1 (10^3) no cambia su valor.

            double r2;
            do
            {
                r2 = Math.Min(regLin.RYSQa, regLin.RYSQb);
                grado++;
                if (!Regresion(regLin, w, grado))
                {
                    MessageBox.Show(Idioma.msg[Idioma.lengua, 139], Idioma.msg[Idioma.lengua, 140], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            } while (Math.Min(regLin.RYSQa, regLin.RYSQb) > r2 && grado < MAX_GRADO);
            if (Math.Min(regLin.RYSQa, regLin.RYSQb) < r2)
            {
                // Retroceder un grado

                grado--;
                if (!Regresion(regLin, w, grado))
                {
                    MessageBox.Show(Idioma.msg[Idioma.lengua, 139], Idioma.msg[Idioma.lengua, 140], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // Realiza una última regresión anulando los picos conforme a la regresión anterior

            AnulaPicos(w, corte);
            if (!Regresion(regLin, w, grado))
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 139], Idioma.msg[Idioma.lengua, 140], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void AnulaPicos(double[] w, double corte)
        {
            for (int i = 0; i < es_actual.x.Length; i++)
            {
                if (Math.Abs(es_actual.y[i] - regLin.Ycalc[i]) < corte)
                {
                    w[i] = 1;
                }
                else
                {
                    w[i] = 0;
                }
            }
        }
        private void AjustaLienzo()
        {
            int ancho_img_datos = img.Width;
            int alto_img_datos = img.Height;
            panel_img.lienzo.Size = new Size(panel_img.ancho_lienzo, panel_img.alto_lienzo);
            if (panel_img.lienzo.Width >= ancho_img_datos && panel_img.lienzo.Height >= alto_img_datos)
            {
                panel_img.lienzo.Size = new Size(ancho_img_datos, alto_img_datos);
            }
            else if (panel_img.lienzo.Width < ancho_img_datos && panel_img.lienzo.Height < alto_img_datos)
            {
                float fx = (float)panel_img.lienzo.Width / ancho_img_datos;
                float fy = (float)panel_img.lienzo.Height / alto_img_datos;
                if (fx < fy)
                {
                    float f = (float)alto_img_datos / ancho_img_datos;
                    int x = panel_img.lienzo.Width;
                    int y = (int)(x * f);
                    panel_img.lienzo.Size = new Size(x, y);
                }
                else
                {
                    float f = (float)ancho_img_datos / alto_img_datos;
                    int y = panel_img.lienzo.Height;
                    int x = (int)(y * f);
                    panel_img.lienzo.Size = new Size(x, y);
                }
            }
            else if (panel_img.lienzo.Width < ancho_img_datos)
            {
                float f = (float)alto_img_datos / ancho_img_datos;
                panel_img.lienzo.Size = new Size(panel_img.lienzo.Width, (int)(panel_img.lienzo.Width * f));
            }
            else if (panel_img.lienzo.Height < alto_img_datos)
            {
                float f = (float)ancho_img_datos / alto_img_datos;
                panel_img.lienzo.Size = new Size((int)(panel_img.lienzo.Height * f), panel_img.lienzo.Height);
            }
        }
        public Form2.Coordenadas PuntoEspectro(int px, int py)
        {
            if (es_actual == null) return null;

            // Pasar pixeles del lienzo a pixeles de img

            px = (int)(px * es_actual.escala_x);
            py = (int)(py * es_actual.escala_y);

            Form2.Coordenadas xy = new Form2.Coordenadas(0, 0);
            double pvx = es_actual.util_x / (es_actual.maxx - es_actual.minx);
            double pvy = es_actual.util_y / (es_actual.maxy - es_actual.miny);
            xy.x = es_actual.minx + (px - es_actual.mizq) / pvx;
            xy.y = es_actual.miny + (es_actual.util_y - (py - es_actual.msup)) / pvy;
            return xy;
        }
        public bool BuscaPicos()
        {
            Disponible(false);
            if (es_actual == null) return false;
            int n_media_movil;
            if (panel_img.v_movil.Text.Trim().Length == 0)
            {
                n_media_movil = 1;
                panel_img.v_movil.Text = string.Format("{0}", n_media_movil);
            }
            else
            {
                n_media_movil = Convert.ToInt32(panel_img.v_movil.Text.Trim());
                if (n_media_movil < 1)
                {
                    n_media_movil = 1;
                    panel_img.v_movil.Text = string.Format("{0}", n_media_movil);
                }
            }
            double min;
            if (panel_img.v_hueco.Text.Trim().Length == 0)
            {
                // Un 'MIN_DEFECTO' % del rango de variación : 'es_actual.maxy - es_actual.miny'

                panel_img.v_hueco.Text = string.Format("{0}", MIN_DEFECTO * 100);
                min = MIN_DEFECTO * (es_actual.maxy - es_actual.miny);
            }
            else
            {
                min = Convert.ToDouble(panel_img.v_hueco.Text.Trim().Replace(s_millar, s_decimal)) / 100 * (es_actual.maxy - es_actual.miny);
                if (min < 0)
                {
                    min = 0;
                    panel_img.v_hueco.Text = "0";
                }
            }
            double dif_significativa;
            if (panel_img.v_significativa.Text.Trim().Length == 0)
            {
                dif_significativa = 0;
                panel_img.v_significativa.Text = "0";
            }
            else
            {
                dif_significativa = Convert.ToDouble(panel_img.v_significativa.Text.Trim().Replace(s_millar, s_decimal)) / 100;
                if (dif_significativa < 0)
                {
                    dif_significativa = 0;
                    panel_img.v_significativa.Text = "0";
                }
            }
            double dif_significativax2 = 2 * dif_significativa;

            int candidato;
            int tendencia = (es_actual.y[1] - es_actual.y[0] > 0) ? 1 : 0;    // 1 = crece
            double var_acu_uno = (tendencia == 0) ? es_actual.y[0] - es_actual.y[1] : es_actual.y[1] - es_actual.y[0];
            double var_acu_dos;
            if (panel_img.picos == null)
            {
                panel_img.picos = new List<Form2.Pico>();
            }
            else
            {
                panel_img.picos.Clear();
            }

            // Media movil

            double[] mmy = new double[es_actual.x.Length];
            double suma;
            for (int k = 0; k < n_media_movil - 1; k++) mmy[k] = 0;
            for (int k = n_media_movil - 1; k < es_actual.x.Length; k++)
            {
                suma = 0;
                for (int j = 0; j < n_media_movil; j++) suma += es_actual.y[k - j];
                mmy[k] = suma / n_media_movil;
            }

            // Sólo es un pico si se separa de la media de su entorno (en un rango de X) en más de 'min / DIF_MIN_BASE'

            double dif_min_base = min / DIF_MIN_ENTORNO;

            // Un RANGO_X (2%) de los datos para calcular el valor medio en su entorno

            int rango_x = (int)(RANGO_X * es_actual.x.Length) / 2;
            if (rango_x < MIN_RANGO_X) rango_x = MIN_RANGO_X;
            double media;
            int desde;
            int hasta;
            double var;
            double varpu;
            int iant;
            int i = n_media_movil;
            while (i < es_actual.x.Length)
            {
                iant = i - 1;
                var = mmy[i] - mmy[iant];
                var_acu_uno += (tendencia == 0) ? -var : var;
                varpu = (mmy[iant] == 0) ? dif_significativa + 1 : Math.Abs(var) / mmy[iant];
                if (varpu > dif_significativa)
                {
                    if (var > 0 && tendencia == 0 || var < 0 && tendencia == 1)
                    {
                        // Cambio de tendencia

                        if (var_acu_uno >= min)
                        {
                            // 'iant = i - 1' sólo es un pico si se separa de la media de su entorno (en un rango de X) en más de 'min / DIF_MIN_BASE'

                            media = 0;
                            desde = iant - rango_x;
                            if (desde < 0) desde = 0;
                            hasta = iant + rango_x;
                            if (hasta > es_actual.x.Length) hasta = es_actual.x.Length;
                            for (int k = desde; k < hasta; k++)
                            {
                                // Sin contarlo a él

                                if (k != iant) media += mmy[k];
                            }
                            media /= (hasta - desde - 1);
                            if (Math.Abs(mmy[iant] - media) > dif_min_base)
                            {
                                // Candidato a nuevo pico
                                // Un pico tiene que tener una rama ascendente y otra descendente

                                candidato = iant;
                            }
                            else
                            {
                                candidato = -1;
                            }

                            // Continuar con la nueva tendencia hasta el siguiente cambio de tendencia

                            if (i < es_actual.x.Length - 1)
                            {
                                var_acu_dos = (tendencia == 1) ? -var : var;
                                do
                                {
                                    iant = i;
                                    i++;
                                    var = mmy[i] - mmy[iant];
                                    var_acu_dos += (tendencia == 1) ? -var : var;
                                    varpu = (mmy[iant] == 0) ? dif_significativax2 + 1 : Math.Abs(var) / mmy[iant];

                                    // En la rama complementaria se es más exigente para anotar un cambio de tendencia

                                    if (varpu > dif_significativax2)
                                    {
                                        // 'tendencia' aún tiene el valor anterior al cambio

                                        if (var < 0 && tendencia == 0 || var > 0 && tendencia == 1) break;
                                    }
                                } while (i < es_actual.x.Length - 1);
                                if (candidato != -1 && var_acu_dos >= min)
                                {
                                    // Un nuevo pico

                                    if (tendencia == 0) var_acu_uno = -var_acu_uno;
                                    panel_img.picos.Add(new Form2.Pico(candidato, var_acu_uno));
                                }
                            }
                        }
                        tendencia = (var > 0) ? 1 : 0;
                        var_acu_uno = (tendencia == 0) ? -var : var;
                    }
                }
                i++;
            }
            MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 113], panel_img.picos.Count), Idioma.msg[Idioma.lengua, 114], MessageBoxButtons.OK, MessageBoxIcon.Information);
            Disponible(true);
            Console.Beep();
            return true;
        }
        public bool BuscaPicosSimplificado()
        {
            int n = es_actual.x.Length;

            // Una banda de ancho fijo 'corte', centrada en el polinomio

            double corte;
            if (panel_img.v_hueco.Text.Trim().Length == 0)
            {
                // Un MIN_DEFECTO % del rango de variación

                panel_img.v_hueco.Text = string.Format("{0}", MIN_DEFECTO * 100);
                corte = MIN_DEFECTO * (es_actual.maxy - es_actual.miny);
            }
            else
            {
                corte = Convert.ToDouble(panel_img.v_hueco.Text.Trim().Replace(s_millar, s_decimal)) / 100 * (es_actual.maxy - es_actual.miny);
                if (corte < 0)
                {
                    corte = 0;
                    panel_img.v_hueco.Text = "0";
                }
            }
            if (!AjustaPolinomio(corte)) return false;
            if (panel_img.picos == null)
            {
                panel_img.picos = new List<Form2.Pico>();
            }
            else
            {
                panel_img.picos.Clear();
            }
            int candidato;
            double valor;
            bool tendencia;
            int k1;
            int k = 0;
            while (k < n)
            {
                if (Math.Abs(es_actual.y[k] - regLin.Ycalc[k]) >= corte)
                {
                    k1 = k + 1;
                    candidato = k;
                    valor = es_actual.y[k];
                    tendencia = es_actual.y[k] > regLin.Ycalc[k];

                    // Mientras se diferencie del ajuste en mas de corte/2 se considera el mismo pico

                    while (k1 < n && Math.Abs(es_actual.y[k1] - regLin.Ycalc[k1]) > corte / 2)
                    {
                        if (tendencia)
                        {
                            if (es_actual.y[k1] > valor)
                            {
                                candidato = k1;
                                valor = es_actual.y[k1];
                            }
                        }
                        else
                        {
                            if (es_actual.y[k1] < valor)
                            {
                                candidato = k1;
                                valor = es_actual.y[k1];
                            }
                        }
                        k1++;
                    }
                    k = k1;
                    panel_img.picos.Add(new Form2.Pico(candidato, valor));
                }
                k++;
            }
            return true;
        }
        public void DibujaHistograma()
        {
            int ancho = panel_histograma.Width;
            int alto = panel_histograma.Height;
            img_histogramas = new Bitmap(ancho, alto, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(img_histogramas);
            g.FillRectangle(Brushes.White, 0, 0, ancho, alto);
            if (panel_img == null || panel_img.estadistica == null || panel_img.estadistica.max_abs == 0) return;
            if (NAXIS == 0) return;
            histo_ceros.Text = string.Format("{0:N0}", histograma[0]);
            histo_ceros_tpc.Text = string.Format("{0:f2}", 100.0 * histograma[0] / (NAXISn[0] * NAXISn[1]));
            r_h.Text = string.Empty;
            r_min.Text = string.Format("{0:e5}", panel_img.ancho_valor_histograma);
            r_max.Text = string.Format("{0:e5}", panel_img.estadistica.max_abs);
            r_med.Text = string.Format("Med: {0:e4}", panel_img.estadistica.media);
            r_destandar.Text = string.Format("DE: {0:e4}", panel_img.estadistica.dest);
            int hx;
            int suma;
            int suma_t = 0;

            // Sin contar el cero

            int max = int.MinValue;
            for (int i = 1; i < num_histogramas; i++)
            {
                if (max < histograma[i]) max = histograma[i];
                suma_t += histograma[i];
            }
            if (max > 0)
            {
                double fy = (double)(alto - his_margen_sup - his_margen_inf) / max;
                int ancho_pixeles_histograma = (ancho - his_margen_izq - his_margen_dch) / (num_histogramas - 1);
                int x = his_margen_izq;
                int y;
                for (int i = 1; i < num_histogramas; i++)
                {
                    y = alto - his_margen_inf - (int)(fy * histograma[i]);
                    g.FillRectangle(Brushes.Blue, x, y, ancho_pixeles_histograma, (alto - his_margen_inf - y));
                    x += ancho_pixeles_histograma;
                }
                g.DrawLine(Pens.Red, 0, 0, 0, alto);
                g.DrawLine(Pens.Red, x, 0, x, alto);

                // Marcar cotas

                double acotado;
                if (v_acotar_min.Text.Trim().Length > 0)
                {
                    acotado = Convert.ToDouble(v_acotar_min.Text.Trim().Replace(s_millar, s_decimal));
                    x = (int)(ancho_pixeles_histograma * acotado / panel_img.ancho_valor_histograma);
                    g.DrawLine(Pens.Green, x, 0, x, alto);
                    hx = (int)(acotado / panel_img.ancho_valor_histograma);
                    if (hx >= num_histogramas)
                    {
                        suma = suma_t;
                    }
                    else
                    {
                        suma = 0;
                        for (int i = 1; i < hx; i++)
                        {
                            suma += histograma[i];
                        }
                    }
                    r_tpc_min.Text = string.Format("{0:f2}%", 100.0 * suma / suma_t);
                }
                else
                {
                    r_tpc_min.Text = string.Empty;
                }
                if (v_acotar_max.Text.Trim().Length > 0)
                {
                    acotado = Convert.ToDouble(v_acotar_max.Text.Trim().Replace(s_millar, s_decimal));
                    x = (int)(ancho_pixeles_histograma * acotado / panel_img.ancho_valor_histograma);
                    g.DrawLine(Pens.LightGreen, x, 0, x, alto);
                    hx = (int)(acotado / panel_img.ancho_valor_histograma);
                    if (hx >= num_histogramas)
                    {
                        suma = suma_t;
                    }
                    else
                    {
                        suma = 0;
                        for (int i = 1; i < hx; i++)
                        {
                            suma += histograma[i];
                        }
                    }
                    r_tpc_max.Text = string.Format("{0:f2}%", 100.0 * (suma_t - suma) / suma_t);
                }
                else
                {
                    r_tpc_max.Text = string.Empty;
                }
                panel_histograma.Image = img_histogramas;
            }
            else
            {
                panel_histograma.Image = null;
            }
            Application.DoEvents();
        }
        private void Panel_histograma_MouseDown(object sender, MouseEventArgs e)
        {
            int ancho = panel_histograma.Width;
            int alto = panel_histograma.Height;
            int inc_pixeles_x = (ancho - his_margen_izq - his_margen_dch) / (num_histogramas - 1);
            int hx = ((e.X - his_margen_izq) / inc_pixeles_x) + 1;
            if (hx >= num_histogramas) return;
            int suma_t = 0;
            for (int i = 1; i < num_histogramas; i++)
            {
                suma_t += histograma[i];
            }
            int suma = 0;
            for (int i = 1; i < hx; i++)
            {
                suma += histograma[i];
            }
            DibujaHistograma();
            double x = hx * panel_img.ancho_valor_histograma;
            r_h.Text = string.Format("{0:e5} | {1:N0} | {2:f2}", x, histograma[hx], 100.0 * suma / suma_t);
            Graphics g = Graphics.FromImage(img_histogramas);
            g.DrawLine(Pens.Fuchsia, e.X, 0, e.X, alto);
            panel_histograma.Refresh();
        }
        private void ColorCotas()
        {
            double acotado_min;
            double acotado_max;
            if (v_acotar_min.Text.Trim().Length > 0)
            {
                acotado_min = Convert.ToDouble(v_acotar_min.Text.Trim().Replace(s_millar, s_decimal));
            }
            else
            {
                acotado_min = 0;
            }
            if (v_acotar_max.Text.Trim().Length > 0)
            {
                acotado_max = Convert.ToDouble(v_acotar_max.Text.Trim().Replace(s_millar, s_decimal));
            }
            else
            {
                acotado_max = double.MaxValue;
            }
            if (acotado_max <= acotado_min)
            {
                v_acotar_min.ForeColor = Color.Red;
                v_acotar_max.ForeColor = Color.Red;
            }
            else
            {
                v_acotar_min.ForeColor = SystemColors.WindowText;
                v_acotar_max.ForeColor = SystemColors.WindowText;
            }
        }
        private void V_acotar_min_TextChanged(object sender, EventArgs e)
        {
            ColorCotas();
            DibujaHistograma();
        }
        private void V_acotar_max_TextChanged(object sender, EventArgs e)
        {
            ColorCotas();
            DibujaHistograma();
        }

        private void Sel_HDU_SelectedIndexChanged(object sender, EventArgs e)
        {
            lista_cabeceras.Items.Clear();
            b_exporta_cabeceras.Visible = false;
            lista_parametros.Items.Clear();
            parametros.Clear();
            sel_imagen.Items.Clear();
            sel_tabla.Items.Clear();
            b_exporta_fits.Visible = false;
            b_exporta_imagen.Visible = false;
            b_conv_espectro.Visible = false;
            b_exporta_tabla.Visible = false;
            reloj_ar.Refresh();
            reloj_de.Refresh();
            int i = hdu_actual = sel_HDU.SelectedIndex;
            normalizar.Checked = hdu[i].normalizar;
            v_acotar_min.Text = hdu[i].cota_inf;
            v_acotar_max.Text = hdu[i].cota_sup;
            ind_color_nan = hdu[i].ind_color_nan;
            Actualiza_b_nan();
            ind_color_cota_inf = hdu[i].ind_color_cota_inf;
            Actualiza_b_color_inf();
            ind_color_cota_sup = hdu[i].ind_color_cota_sup;
            Actualiza_b_color_sup();
            raiz.Checked = hdu[i].raiz;
            invertir_y.Checked = hdu[i].invertir_y;
            invertir_x.Checked = hdu[i].invertir_x;
            alta_resolucion.Checked = hdu[i].alta_resolucion;
            Disponible(false);
            es_actual = null;
            bool res = LeeHDU(i, hdu[i].puntero_ini);
            Disponible(true);
            if (res)
            {
                b_exporta_fits.Visible = true;
                if (hdu[i].n_imagenes > 0)
                {
                    for (int k = 0; k < hdu[i].n_imagenes; k++) sel_imagen.Items.Add((k + 1).ToString());
                    sel_imagen.Enabled = hdu[i].n_imagenes > 1;
                    b_exporta_imagen.Visible = true;
                    sel_imagen.SelectedIndex = 0;
                }
                else
                {
                    sel_imagen.Enabled = false;
                    sel_imagen.Text = string.Empty;
                    b_exporta_imagen.Visible = false;
                }
                if (hdu[i].n_tablas > 0)
                {
                    for (int k = 0; k < hdu[i].n_tablas; k++) sel_tabla.Items.Add((k + 1).ToString());
                    sel_tabla.Enabled = hdu[i].n_tablas > 1;
                    b_exporta_tabla.Visible = true;
                    sel_tabla.SelectedIndex = 0;
                }
                else
                {
                    sel_tabla.Enabled = false;
                    sel_tabla.Text = string.Empty;
                    b_exporta_tabla.Visible = false;
                }
                b_exporta_cabeceras.Visible = true;
                b_conv_espectro.Visible = NAXISn.Length == 2 && NAXISn[1] == 1 && hdu[i].n_imagenes > 0 && hdu[i].n_tablas == 0;
            }
        }
        private void Sel_imagen_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloj_ar.Refresh();
            reloj_de.Refresh();
            if (NAXIS == 3)
            {
                int i = hdu_actual = sel_HDU.SelectedIndex;
                int j = sel_imagen.SelectedIndex;
                panel_img.i3 = j;
                switch (hdu[i].clase_dato)
                {
                    case 1:
                        panel_img.Dibuja(datosb3, true, j);
                        break;
                    case 2:
                        panel_img.Dibuja(datoss3, true, j);
                        break;
                    case 3:
                        panel_img.Dibuja(datosi3, true, j);
                        break;
                    case 4:
                        panel_img.Dibuja(datosf3, true, j);
                        break;
                    case 6:
                        panel_img.Dibuja(datosl3, true, j);
                        break;
                    default:
                        panel_img.Dibuja(datosd3, true, j);
                        break;
                }
            }
        }
        private void Sel_tabla_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloj_ar.Refresh();
            reloj_de.Refresh();
            if (NAXIS == 3)
            {
                int i = hdu_actual = sel_HDU.SelectedIndex;
                int j = sel_tabla.SelectedIndex;
                if (hdu[i].conjunto == 1)
                {
                    TablaBinaria(datos3, j);
                }
                else if (hdu[i].conjunto == 2)
                {
                    TablaASCII(datos3, j);
                }
            }
        }

        private void B_nan_Click(object sender, EventArgs e)
        {
            ind_color_nan++;
            if (ind_color_nan > 2) ind_color_nan = 0;
            Actualiza_b_nan();
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].ind_color_nan = ind_color_nan;
        }
        private void Actualiza_b_nan()
        {
            color_nan = ind_color_nan switch
            {
                0 => Color.Black,
                1 => Color.White,
                _ => Color.Brown,
            };
            b_nan.BackColor = color_nan;
            b_nan.ForeColor = (ind_color_nan == 1) ? Color.Black : Color.White;

        }
        private void B_cota_inf_Click(object sender, EventArgs e)
        {
            ind_color_cota_inf = 1 - ind_color_cota_inf;
            Actualiza_b_color_inf();
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].ind_color_cota_inf = ind_color_cota_inf;
        }
        private void Actualiza_b_color_inf()
        {
            color_cota_inf = ind_color_cota_inf switch
            {
                0 => Color.Black,
                _ => Color.Red,
            };
            if (ind_color_cota_inf == 0)
            {
                b_cota_inf.ForeColor = Color.Black;
                b_cota_inf.BackColor = Color.White;
            }
            else
            {
                b_cota_inf.ForeColor = Color.White;
                b_cota_inf.BackColor = color_cota_inf;
            }
        }
        private void B_cota_sup_Click(object sender, EventArgs e)
        {
            ind_color_cota_sup = 1 - ind_color_cota_sup;
            Actualiza_b_color_sup();
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].ind_color_cota_sup = ind_color_cota_sup;
        }
        private void Actualiza_b_color_sup()
        {
            color_cota_sup = ind_color_cota_sup switch
            {
                0 => Color.White,
                _ => Color.Blue,
            };
            if (ind_color_cota_sup == 0)
            {
                b_cota_sup.ForeColor = Color.Black;
                b_cota_sup.BackColor = Color.White;
            }
            else
            {
                b_cota_sup.ForeColor = Color.White;
                b_cota_sup.BackColor = color_cota_sup;
            }
        }
        private void Normalizar_CheckedChanged(object sender, EventArgs e)
        {
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].normalizar = normalizar.Checked;
        }
        private void Invertir_y_CheckedChanged(object sender, EventArgs e)
        {
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].invertir_y = invertir_y.Checked;
        }
        private void Invertir_x_CheckedChanged(object sender, EventArgs e)
        {
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].invertir_x = invertir_x.Checked;
        }
        private void Alta_resolucion_CheckedChanged(object sender, EventArgs e)
        {
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].alta_resolucion = alta_resolucion.Checked;
        }
        private void Raiz_CheckedChanged(object sender, EventArgs e)
        {
            int i = hdu_actual = sel_HDU.SelectedIndex;
            if (i != -1) hdu[i].raiz = raiz.Checked;
        }
        private void Reloj_ar_Paint(object sender, PaintEventArgs e)
        {
            Reloj(reloj_ar, e.Graphics, panel_img.x_izq, panel_img.x_dch);
        }
        private void Reloj_de_Paint(object sender, PaintEventArgs e)
        {
            Reloj(reloj_de, e.Graphics, panel_img.y_sup, panel_img.y_inf);
        }
        private void Reloj(PictureBox pr, Graphics g, double min, double max)
        {
            if (es_actual != null && NAXIS == 2 && NAXISn[1] != 1 && CRVAL != null && CRVAL[0] != double.MinValue)
            {
                if (CTYPE == null)
                {
                    ctype_1.Text = string.Empty;
                    ctype_2.Text = string.Empty;
                }
                else
                {
                    ctype_1.Text = CTYPE[0];
                    ctype_2.Text = CTYPE[1];
                }
                g.DrawEllipse(lapiz_circulo, rectangulo_reloj);
                g.DrawLine(lapiz_ejes, pr.Width / 2 - 1, 0, pr.Width / 2 - 1, pr.Height);
                g.DrawLine(lapiz_ejes, 0, pr.Height / 2 - 1, pr.Width, pr.Height / 2 - 1);
                float v_min = (float)((min < max) ? min : max);
                float v_max = (float)((min > max) ? min : max);
                float arco = v_max - v_min;
                if (arco < 4) arco = 4;
                g.DrawArc(lapiz_arco, rectangulo_reloj, -v_max, arco);
            }
        }
        private void Lista_parametros_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = lista_parametros.SelectedItem.ToString();
            val_parametro.Text = parametros.GetValueOrDefault(s);
        }

        private void B_apilar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ficherolectura = new OpenFileDialog()
            {
                Filter = "BIN |*.bin|TODO |*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true
            };
            if (ficherolectura.ShowDialog() == DialogResult.OK)
            {
                if (ficherolectura.FileNames.Length < 2)
                {
                    MessageBox.Show(Idioma.msg[Idioma.lengua, 97], Idioma.msg[Idioma.lengua, 99], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Fichero de salida

                SaveFileDialog ficheroescritura = new SaveFileDialog()
                {
                    Filter = "FITS |*.fits;*.fit|TODO |*.*",
                    FilterIndex = 1
                };
                if (ficheroescritura.ShowDialog() != DialogResult.OK) return;
                string salida = ficheroescritura.FileName;
                Disponible(false);
                bool res;
                if (ComparaBin(ficherolectura.FileNames))
                {
                    res = ApilaFicherosBinIgua(ficherolectura.FileNames);
                }
                else
                {
                    res = ApilaFicherosBin(ficherolectura.FileNames);
                }
                if (res)
                {
                    // Crea un fichero FITS en la misma carpeta y luego lo lee

                    CreaFITS(ficherolectura.FileNames[0], salida, Idioma.msg[Idioma.lengua, 95], ficherolectura.FileNames, datosf);
                    LeeFichero(salida);
                }
                Disponible(true);
            }
        }
        private bool ApilaFicherosBinIgua(string[] ficheros)
        {
            FileStream fs;
            double[] val = new double[6];
            byte[] bytes_valor = new byte[8];
            int clase_dato;
            NAXISn = new int[2];
            CRPIX = new double[2];
            CRVAL = new double[2];
            CDELT = new double[2];

            // Del primer fichero se toman los datos básicos

            fs = new FileStream(ficheros[0], FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs == null) return false;
            fs.Read(bytes_valor, 0, 4);
            clase_dato = BitConverter.ToInt32(bytes_valor);
            fs.Read(bytes_valor, 0, 4);
            NAXISn[0] = BitConverter.ToInt32(bytes_valor);
            fs.Read(bytes_valor, 0, 4);
            NAXISn[1] = BitConverter.ToInt32(bytes_valor);
            for (int k = 0; k < 6; k++)
            {
                fs.Read(bytes_valor, 0, 8);
                val[k] = BitConverter.ToDouble(bytes_valor);
            }
            fs.Close();
            CRPIX[0] = val[0];
            CRPIX[1] = val[1];
            CRVAL[0] = val[2];
            CRVAL[1] = val[3];
            CDELT[0] = val[4];
            CDELT[1] = val[5];

            // Se trabaja con 'float'

            datosf = new float[NAXISn[0], NAXISn[1]];
            Array.Clear(datosf, 0, NAXISn[0] * NAXISn[1]);
            int contador;
            for (int i = 0; i < ficheros.Length; i++)
            {
                fs = new FileStream(ficheros[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return false;
                fs.Read(bytes_valor, 0, 4);
                fs.Read(bytes_valor, 0, 4);
                fs.Read(bytes_valor, 0, 4);
                for (int k = 0; k < 6; k++)
                {
                    fs.Read(bytes_valor, 0, 8);
                }

                // Lee y acumula los datos

                contador = 0;
                r_leidos.Text = Idioma.msg[Idioma.lengua, 11];
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 106];
                r_octetos.Text = string.Format("{0:N0}", NAXISn[0] * NAXISn[1]);
                v_leidos.Text = string.Format("{0:N0}", contador);
                for (int i2 = 0; i2 < NAXISn[1]; i2++)
                {
                    for (int i1 = 0; i1 < NAXISn[0]; i1++)
                    {
                        contador++;
                        if (contador % X10M == 0)
                        {
                            v_leidos.Text = string.Format("{0:N0}", contador);
                            Application.DoEvents();
                        }
                        switch (clase_dato)
                        {
                            case 1:
                                fs.Read(bytes_valor, 0, 1);
                                datosf[i1, i2] += bytes_valor[0];
                                break;
                            case 2:
                                fs.Read(bytes_valor, 0, 2);
                                datosf[i1, i2] += BitConverter.ToInt16(bytes_valor);
                                break;
                            case 3:
                                fs.Read(bytes_valor, 0, 4);
                                datosf[i1, i2] += BitConverter.ToInt32(bytes_valor);
                                break;
                            case 4:
                                fs.Read(bytes_valor, 0, 4);
                                datosf[i1, i2] += BitConverter.ToSingle(bytes_valor);
                                break;
                            case 6:
                                fs.Read(bytes_valor, 0, 8);
                                datosl[i1, i2] += BitConverter.ToInt64(bytes_valor);
                                break;
                            case 5:
                                fs.Read(bytes_valor, 0, 8);
                                datosf[i1, i2] += (float)BitConverter.ToDouble(bytes_valor);
                                break;
                        }
                    }
                }
                v_leidos.Text = string.Format("{0:N0}", contador);
                Application.DoEvents();
                fs.Close();
            }
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                for (int i1 = 0; i1 < NAXISn[0]; i1++)
                {
                    datosf[i1, i2] /= ficheros.Length;
                }
            }
            return true;
        }
        private bool ApilaFicherosBin(string[] ficheros)
        {
            FileStream fs;
            int[] eje = new int[2];
            byte[] bytes_valor = new byte[8];
            int clase_dato;
            float v_dato;
            NAXISn = new int[2];
            CRPIX = new double[2];
            CRVAL = new double[2];
            CDELT = new double[2];

            Superficie su = RegionMaxima(ficheros);
            CRPIX[0] = 1;
            CRPIX[1] = 1;
            CRVAL[0] = su.xizq_s;
            CRVAL[1] = su.ysup_s;
            CDELT[0] = su.deltax_s;
            CDELT[1] = su.deltay_s;

            // Se trabaja con 'float'

            NAXISn[0] = (int)((su.xdch_s - su.xizq_s) / su.deltax_s);
            NAXISn[1] = (int)((su.yinf_s - su.ysup_s) / su.deltay_s);
            datosf = new float[NAXISn[0], NAXISn[1]];
            Array.Clear(datosf, 0, NAXISn[0] * NAXISn[1]);
            int[,] cuenta = new int[NAXISn[0], NAXISn[1]];
            Array.Clear(cuenta, 0, NAXISn[0] * NAXISn[1]);
            double x;
            double y;
            int i1d;
            int i2d;
            int contador;
            for (int i = 0; i < ficheros.Length; i++)
            {
                fs = new FileStream(ficheros[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return false;
                fs.Read(bytes_valor, 0, 4);
                clase_dato = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                eje[0] = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                eje[1] = BitConverter.ToInt32(bytes_valor);
                for (int k = 0; k < 6; k++)
                {
                    fs.Read(bytes_valor, 0, 8);
                }

                // Lee y suma los datos

                contador = 0;
                r_leidos.Text = Idioma.msg[Idioma.lengua, 11];
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 106];
                r_octetos.Text = string.Format("{0:N0}", eje[0] * eje[1]);
                v_leidos.Text = string.Format("{0:N0}", contador);
                for (int i2 = 0; i2 < eje[1]; i2++)
                {
                    y = su.ysup[i] + i2 * su.deltay[i];
                    for (int i1 = 0; i1 < eje[0]; i1++)
                    {
                        contador++;
                        if (contador % X10M == 0)
                        {
                            v_leidos.Text = string.Format("{0:N0}", contador);
                            Application.DoEvents();
                        }
                        x = su.xizq[i] + i1 * su.deltax[i];
                        switch (clase_dato)
                        {
                            case 1:
                                fs.Read(bytes_valor, 0, 1);
                                v_dato = bytes_valor[0];
                                break;
                            case 2:
                                fs.Read(bytes_valor, 0, 2);
                                v_dato = BitConverter.ToInt16(bytes_valor);
                                break;
                            case 3:
                                fs.Read(bytes_valor, 0, 4);
                                v_dato = BitConverter.ToInt32(bytes_valor);
                                break;
                            case 4:
                                fs.Read(bytes_valor, 0, 4);
                                v_dato = BitConverter.ToSingle(bytes_valor);
                                break;
                            case 6:
                                fs.Read(bytes_valor, 0, 8);
                                v_dato = BitConverter.ToInt64(bytes_valor);
                                break;
                            default:
                                fs.Read(bytes_valor, 0, 8);
                                v_dato = (float)BitConverter.ToDouble(bytes_valor);
                                break;
                        }

                        // Hay que leer todos los datos del fichero
                        // por eso no se pueden hacer las comprobaciones antes

                        i2d = (int)((y - su.ysup_s - 1e-8) / su.deltay_s);
                        if (i2d >= NAXISn[1]) i2d = NAXISn[1] - 1;
                        i1d = (int)((x - su.xizq_s - 1e-8) / su.deltax_s);
                        if (i1d >= NAXISn[0]) i1d = NAXISn[0] - 1;
                        if (i == 0)
                        {
                            datosf[i1d, i2d] = v_dato;
                        }
                        else
                        {
                            datosf[i1d, i2d] += v_dato;
                        }
                        cuenta[i1d, i2d]++;
                    }
                }
                v_leidos.Text = string.Format("{0:N0}", contador);
                Application.DoEvents();
                fs.Close();
            }
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                for (int i1 = 0; i1 < NAXISn[0]; i1++)
                {
                    if (cuenta[i1, i2] > 0) datosf[i1, i2] /= cuenta[i1, i2];
                }
            }
            return true;
        }
        private void B_restar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ficherolectura = new OpenFileDialog()
            {
                Filter = "BIN |*.bin|TODO |*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true
            };
            if (ficherolectura.ShowDialog() == DialogResult.OK)
            {
                if (ficherolectura.FileNames.Length != 2)
                {
                    MessageBox.Show(Idioma.msg[Idioma.lengua, 98], Idioma.msg[Idioma.lengua, 100], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Fichero de salida

                SaveFileDialog ficheroescritura = new SaveFileDialog()
                {
                    Filter = "FITS |*.fits;*.fit|TODO |*.*",
                    FilterIndex = 1
                };
                if (ficheroescritura.ShowDialog() != DialogResult.OK) return;
                string salida = ficheroescritura.FileName;
                Disponible(false);
                bool res;
                if (ComparaBin(ficherolectura.FileNames))
                {
                    res = RestaFicherosBinIgua(ficherolectura.FileNames);
                }
                else
                {
                    res = RestaFicherosBin(ficherolectura.FileNames);
                }
                if (res)
                {
                    // Crea un fichero FITS en la misma carpeta y luego lo lee

                    CreaFITS(ficherolectura.FileNames[0], salida, Idioma.msg[Idioma.lengua, 96], ficherolectura.FileNames, datosf);
                    LeeFichero(salida);
                }
                Disponible(true);
            }
        }
        private bool RestaFicherosBinIgua(string[] ficheros)
        {
            FileStream fs;
            double[] val = new double[6];
            byte[] bytes_valor = new byte[8];
            int clase_dato;
            float v_dato;
            NAXISn = new int[2];
            CRPIX = new double[2];
            CRVAL = new double[2];
            CDELT = new double[2];

            // Del primer fichero se toman los datos básicos

            fs = new FileStream(ficheros[0], FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs == null) return false;
            fs.Read(bytes_valor, 0, 4);
            clase_dato = BitConverter.ToInt32(bytes_valor);
            fs.Read(bytes_valor, 0, 4);
            NAXISn[0] = BitConverter.ToInt32(bytes_valor);
            fs.Read(bytes_valor, 0, 4);
            NAXISn[1] = BitConverter.ToInt32(bytes_valor);
            for (int k = 0; k < 6; k++)
            {
                fs.Read(bytes_valor, 0, 8);
                val[k] = BitConverter.ToDouble(bytes_valor);
            }
            fs.Close();
            CRPIX[0] = val[0];
            CRPIX[1] = val[1];
            CRVAL[0] = val[2];
            CRVAL[1] = val[3];
            CDELT[0] = val[4];
            CDELT[1] = val[5];

            // Se trabaja con 'float'

            datosf = new float[NAXISn[0], NAXISn[1]];
            Array.Clear(datosf, 0, NAXISn[0] * NAXISn[1]);
            int contador;
            for (int i = 0; i < ficheros.Length; i++)
            {
                fs = new FileStream(ficheros[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return false;
                fs.Read(bytes_valor, 0, 4);
                fs.Read(bytes_valor, 0, 4);
                fs.Read(bytes_valor, 0, 4);
                for (int k = 0; k < 6; k++)
                {
                    fs.Read(bytes_valor, 0, 8);
                }

                // Lee y resta los datos

                contador = 0;
                r_leidos.Text = Idioma.msg[Idioma.lengua, 11];
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 106];
                r_octetos.Text = string.Format("{0:N0}", NAXISn[0] * NAXISn[1]);
                v_leidos.Text = string.Format("{0:N0}", contador);
                for (int i2 = 0; i2 < NAXISn[1]; i2++)
                {
                    for (int i1 = 0; i1 < NAXISn[0]; i1++)
                    {
                        contador++;
                        if (contador % X10M == 0)
                        {
                            v_leidos.Text = string.Format("{0:N0}", contador);
                            Application.DoEvents();
                        }
                        switch (clase_dato)
                        {
                            case 1:
                                fs.Read(bytes_valor, 0, 1);
                                v_dato = bytes_valor[0];
                                break;
                            case 2:
                                fs.Read(bytes_valor, 0, 2);
                                v_dato = BitConverter.ToInt16(bytes_valor);
                                break;
                            case 3:
                                fs.Read(bytes_valor, 0, 4);
                                v_dato = BitConverter.ToInt32(bytes_valor);
                                break;
                            case 4:
                                fs.Read(bytes_valor, 0, 4);
                                v_dato = BitConverter.ToSingle(bytes_valor);
                                break;
                            case 6:
                                fs.Read(bytes_valor, 0, 8);
                                v_dato = BitConverter.ToInt64(bytes_valor);
                                break;
                            default:
                                fs.Read(bytes_valor, 0, 8);
                                v_dato = (float)BitConverter.ToDouble(bytes_valor);
                                break;
                        }
                        if (i == 0)
                        {
                            datosf[i1, i2] = v_dato;
                        }
                        else
                        {
                            datosf[i1, i2] -= v_dato;
                        }
                    }
                }
                v_leidos.Text = string.Format("{0:N0}", contador);
                Application.DoEvents();
                fs.Close();
            }
            return true;
        }
        private bool RestaFicherosBin(string[] ficheros)
        {
            FileStream fs;
            int[] eje = new int[2];
            byte[] bytes_valor = new byte[8];
            int clase_dato;
            float v_dato;
            NAXISn = new int[2];
            CRPIX = new double[2];
            CRVAL = new double[2];
            CDELT = new double[2];

            Superficie su = RegionComun(ficheros);
            CRPIX[0] = 1;
            CRPIX[1] = 1;
            CRVAL[0] = su.xizq_s;
            CRVAL[1] = su.ysup_s;
            CDELT[0] = su.deltax_s;
            CDELT[1] = su.deltay_s;

            // Se trabaja con 'float'

            NAXISn[0] = (int)((su.xdch_s - su.xizq_s) / su.deltax_s);
            NAXISn[1] = (int)((su.yinf_s - su.ysup_s) / su.deltay_s);
            if (NAXISn[0] <= 0 || NAXISn[1] <= 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 108], Idioma.msg[Idioma.lengua, 100], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            datosf = new float[NAXISn[0], NAXISn[1]];
            Array.Clear(datosf, 0, NAXISn[0] * NAXISn[1]);
            double x;
            double y;
            int i1d;
            int i2d;
            int contador;
            for (int i = 0; i < ficheros.Length; i++)
            {
                fs = new FileStream(ficheros[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return false;
                fs.Read(bytes_valor, 0, 4);
                clase_dato = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                eje[0] = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                eje[1] = BitConverter.ToInt32(bytes_valor);
                for (int k = 0; k < 6; k++)
                {
                    fs.Read(bytes_valor, 0, 8);
                }

                // Lee y resta los datos

                contador = 0;
                r_leidos.Text = Idioma.msg[Idioma.lengua, 11];
                r_cantidad.Text = Idioma.msg[Idioma.lengua, 106];
                r_octetos.Text = string.Format("{0:N0}", eje[0] * eje[1]);
                v_leidos.Text = string.Format("{0:N0}", contador);
                for (int i2 = 0; i2 < eje[1]; i2++)
                {
                    y = su.ysup[i] + i2 * su.deltay[i];
                    for (int i1 = 0; i1 < eje[0]; i1++)
                    {
                        contador++;
                        if (contador % X10M == 0)
                        {
                            v_leidos.Text = string.Format("{0:N0}", contador);
                            Application.DoEvents();
                        }
                        x = su.xizq[i] + i1 * su.deltax[i];
                        switch (clase_dato)
                        {
                            case 1:
                                fs.Read(bytes_valor, 0, 1);
                                v_dato = bytes_valor[0];
                                break;
                            case 2:
                                fs.Read(bytes_valor, 0, 2);
                                v_dato = BitConverter.ToInt16(bytes_valor);
                                break;
                            case 3:
                                fs.Read(bytes_valor, 0, 4);
                                v_dato = BitConverter.ToInt32(bytes_valor);
                                break;
                            case 4:
                                fs.Read(bytes_valor, 0, 4);
                                v_dato = BitConverter.ToSingle(bytes_valor);
                                break;
                            case 6:
                                fs.Read(bytes_valor, 0, 8);
                                v_dato = BitConverter.ToInt64(bytes_valor);
                                break;
                            default:
                                fs.Read(bytes_valor, 0, 8);
                                v_dato = (float)BitConverter.ToDouble(bytes_valor);
                                break;
                        }

                        // Hay que leer todos los datos del fichero
                        // por eso no se pueden hacer las comprobaciones antes

                        i2d = (int)((y - su.ysup_s - 1e-8) / su.deltay_s);
                        if (i2d < 0 || i2d >= NAXISn[1]) continue;
                        i1d = (int)((x - su.xizq_s - 1e-8) / su.deltax_s);
                        if (i1d < 0 || i1d >= NAXISn[0]) continue;
                        if (i == 0)
                        {
                            datosf[i1d, i2d] = v_dato;
                        }
                        else
                        {
                            datosf[i1d, i2d] -= v_dato;
                        }
                    }
                }
                v_leidos.Text = string.Format("{0:N0}", contador);
                Application.DoEvents();
                fs.Close();
            }
            return true;
        }
        private static bool ComparaBin(string[] ficheros)
        {
            FileStream fs;
            fs = new FileStream(ficheros[0], FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs == null) return false;
            int n = 3 * 4 + 6 * 8;
            byte[] bytes1 = new byte[n];
            fs.Read(bytes1, 0, n);
            fs.Close();
            byte[] bytes2 = new byte[n];
            for (int i = 1; i < ficheros.Length; i++)
            {
                fs = new FileStream(ficheros[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return false;
                fs.Read(bytes2, 0, n);
                fs.Close();
                for (int k = 0; k < n; k++)
                {
                    if (bytes1[k] != bytes2[k]) return false;
                }
            }
            return true;
        }
        private Superficie RegionComun(string[] ficheros)
        {
            int[] eje = new int[2];
            double[] val = new double[6];
            byte[] bytes_valor = new byte[8];
            double va;
            double vb;
            double xmin;
            double xmax;
            double ymin;
            double ymax;
            double deltax;
            double deltay;
            NAXISn = new int[2];
            CRPIX = new double[2];
            CRVAL = new double[2];
            CDELT = new double[2];
            Superficie su = new Superficie(2, double.MaxValue, double.MinValue, double.MaxValue, double.MinValue, 0, 0);
            FileStream fs;
            for (int i = 0; i < ficheros.Length; i++)
            {
                fs = new FileStream(ficheros[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return su;
                fs.Read(bytes_valor, 0, 4);
                fs.Read(bytes_valor, 0, 4);
                eje[0] = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                eje[1] = BitConverter.ToInt32(bytes_valor);
                for (int k = 0; k < 6; k++)
                {
                    fs.Read(bytes_valor, 0, 8);
                    val[k] = BitConverter.ToDouble(bytes_valor);
                }
                va = val[2] - val[0] * val[4];
                vb = val[2] + (eje[0] - val[0]) * val[4];
                su.xizq[i] = va;
                su.xdch[i] = vb;
                su.deltax[i] = val[4];
                if (va < vb)
                {
                    xmin = va;
                    xmax = vb;
                }
                else
                {
                    xmin = vb;
                    xmax = va;
                }
                va = val[3] - val[1] * val[5];
                vb = val[3] + (eje[1] - val[1]) * val[5];
                su.ysup[i] = va;
                su.yinf[i] = vb;
                su.deltay[i] = val[5];
                if (va < vb)
                {
                    ymin = va;
                    ymax = vb;
                }
                else
                {
                    ymin = vb;
                    ymax = va;
                }
                deltax = Math.Abs(val[4]);
                deltay = Math.Abs(val[5]);
                if (i == 0)
                {
                    su.xizq_s = xmin;
                    su.xdch_s = xmax;
                    su.ysup_s = ymin;
                    su.yinf_s = ymax;
                    su.deltax_s = deltax;
                    su.deltay_s = deltay;
                }
                else
                {
                    if (xmin > su.xizq_s) su.xizq_s = xmin;
                    if (xmax < su.xdch_s) su.xdch_s = xmax;
                    if (ymin > su.ysup_s) su.ysup_s = ymin;
                    if (ymax < su.yinf_s) su.yinf_s = ymax;
                    if (deltax > su.deltax_s) su.deltax_s = deltax;
                    if (deltay > su.deltay_s) su.deltay_s = deltay;
                }
                fs.Close();
            }
            return su;
        }
        private Superficie RegionMaxima(string[] ficheros)
        {
            double[] val = new double[6];
            byte[] bytes_valor = new byte[8];
            double va;
            double vb;
            double xmin;
            double xmax;
            double ymin;
            double ymax;
            double deltax;
            double deltay;
            int[] eje = new int[2];
            NAXISn = new int[2];
            CRPIX = new double[2];
            CRVAL = new double[2];
            CDELT = new double[2];
            Superficie su = new Superficie(ficheros.Length, double.MaxValue, double.MinValue, double.MaxValue, double.MinValue, 0, 0);
            FileStream fs;
            for (int i = 0; i < ficheros.Length; i++)
            {
                fs = new FileStream(ficheros[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs == null) return su;
                fs.Read(bytes_valor, 0, 4);
                fs.Read(bytes_valor, 0, 4);
                eje[0] = BitConverter.ToInt32(bytes_valor);
                fs.Read(bytes_valor, 0, 4);
                eje[1] = BitConverter.ToInt32(bytes_valor);
                for (int k = 0; k < 6; k++)
                {
                    fs.Read(bytes_valor, 0, 8);
                    val[k] = BitConverter.ToDouble(bytes_valor);
                }
                va = val[2] - val[0] * val[4];
                vb = val[2] + (eje[0] - val[0]) * val[4];
                su.xizq[i] = va;
                su.xdch[i] = vb;
                su.deltax[i] = val[4];
                if (va < vb)
                {
                    xmin = va;
                    xmax = vb;
                }
                else
                {
                    xmin = vb;
                    xmax = va;
                }
                va = val[3] - val[1] * val[5];
                vb = val[3] + (eje[1] - val[1]) * val[5];
                su.ysup[i] = va;
                su.yinf[i] = vb;
                su.deltay[i] = val[5];
                if (va < vb)
                {
                    ymin = va;
                    ymax = vb;
                }
                else
                {
                    ymin = vb;
                    ymax = va;
                }
                deltax = Math.Abs(val[4]);
                deltay = Math.Abs(val[5]);
                if (i == 0)
                {
                    su.xizq_s = xmin;
                    su.xdch_s = xmax;
                    su.ysup_s = ymin;
                    su.yinf_s = ymax;
                    su.deltax_s = deltax;
                    su.deltay_s = deltay;
                }
                else
                {
                    if (xmin < su.xizq_s) su.xizq_s = xmin;
                    if (xmax > su.xdch_s) su.xdch_s = xmax;
                    if (ymin < su.ysup_s) su.ysup_s = ymin;
                    if (ymax > su.yinf_s) su.yinf_s = ymax;
                    if (deltax > su.deltax_s) su.deltax_s = deltax;
                    if (deltay > su.deltay_s) su.deltay_s = deltay;
                }
                fs.Close();
            }
            return su;
        }

        private void B_exporta_fits_Click(object sender, EventArgs e)
        {
            ExtraeFITS(FICHERO_FITS, 0);
        }
        private bool ExtraeFITS(string fichero, int fracciones)
        {
            FileStream fs = new FileStream(fichero, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs == null) return false;

            SaveFileDialog ficheroescritura = new SaveFileDialog()
            {
                Filter = "CSV (*.csv)|*.csv|TODO (*.*)|*.*",
                FilterIndex = 1
            };
            if (ficheroescritura.ShowDialog() != DialogResult.OK) return false;

            string carpeta = Path.GetDirectoryName(ficheroescritura.FileName);
            string fichero_sal = Path.GetFileNameWithoutExtension(ficheroescritura.FileName);
            FileStream fe;
            StreamWriter sw;
            Disponible(false);
            long bloques_leidos;
            long puntero = 0;
            fs.Seek(puntero, SeekOrigin.Begin);

            // Cabeceras

            ParUtiles p = new ParUtiles();
            CopiaParametros(p);
            bool le = BitConverter.IsLittleEndian;
            string fidat;
            string linea;
            int cHDU = 0;
            while (puntero < fs.Length)
            {
                fe = new FileStream(Path.Combine(carpeta, string.Format("{0}_{1}_cab.txt", fichero_sal, cHDU + 1)), FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fe, Encoding.UTF8);
                axis_pendientes = -1;
                do
                {
                    linea = LeeCabecera(fs, ref puntero).Trim();
                    if (linea.Length > 0)
                    {
                        sw.WriteLine(linea);
                        if (linea.StartsWith("END"))
                        {
                            break;
                        }
                        else
                        {
                            if (!InterpretaCabecera(cHDU, linea))
                            {
                                MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 128], cHDU, linea), Idioma.msg[Idioma.lengua, 4], MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                    }
                    if (puntero >= fs.Length) break;
                } while (true);
                sw.Close();

                // Ajustar el puntero al final del último bloque de cabeceras

                bloques_leidos = puntero / BLOQUE;
                if (bloques_leidos * BLOQUE < puntero) bloques_leidos++;
                puntero = bloques_leidos * BLOQUE;
                if (puntero >= fs.Length) break;
                fs.Seek(puntero, SeekOrigin.Begin);
                Application.DoEvents();

                if (NAXIS != 0)
                {
                    HDU hdu_act = new HDU((cHDU + 1).ToString(), 0, 0, 0);
                    if (XTENSION.Length == 0)
                    {
                        hdu_act.conjunto = 0;
                    }
                    else
                    {
                        switch (XTENSION.ToUpper())
                        {
                            case "IMAGE":
                                hdu_act.conjunto = 0;
                                break;
                            case "BINTABLE":
                                hdu_act.conjunto = 1;
                                break;
                            case "TABLE":
                                hdu_act.conjunto = 2;
                                break;
                            default:
                                RecuperaParametros(p);
                                Disponible(true);
                                return false;
                        }
                    }
                    hdu_act.byPorDato = Math.Abs(BITPIX / 8);
                    if (BITPIX > 0 && hdu_act.byPorDato == 1)
                    {
                        hdu_act.clase_dato = 1;
                    }
                    else if (BITPIX > 0 && hdu_act.byPorDato == 2)
                    {
                        hdu_act.clase_dato = 2;
                    }
                    else if (BITPIX > 0 && hdu_act.byPorDato == 4)
                    {
                        hdu_act.clase_dato = 3;
                    }
                    else if (BITPIX < 0 && hdu_act.byPorDato == 4)
                    {
                        hdu_act.clase_dato = 4;
                    }
                    else if (BITPIX < 0 && hdu_act.byPorDato == 8)
                    {
                        hdu_act.clase_dato = 5;
                    }
                    else if (BITPIX > 0 && hdu_act.byPorDato == 8)
                    {
                        hdu_act.clase_dato = 6;
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 7], cHDU, tipoDato, byPorDato), Idioma.msg[Idioma.lengua, 6], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        RecuperaParametros(p);
                        Disponible(true);
                        return false;
                    }
                    r_cantidad.Text = Idioma.msg[Idioma.lengua, 134];
                    r_octetos.Text = string.Format("{0:N0}", NAXISn[1]);
                    v_leidos.Text = string.Format("{0:N0}", 0);
                    Application.DoEvents();
                    if (hdu_act.conjunto == 0)
                    {
                        // Datos

                        fidat = Path.Combine(carpeta, string.Format("{0}_{1}_dat.csv", fichero_sal, cHDU + 1));
                        fe = new FileStream(fidat, FileMode.Create, FileAccess.Write, FileShare.Read);
                        sw = new StreamWriter(fe, Encoding.UTF8);
                        if (NAXIS == 3)
                        {
                            for (int i3 = 0; i3 < NAXISn[2]; i3++)
                            {
                                if (!ExtraeDatos(fs, sw, le, hdu_act))
                                {
                                    RecuperaParametros(p);
                                    Disponible(true);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (!ExtraeDatos(fs, sw, le, hdu_act))
                            {
                                RecuperaParametros(p);
                                Disponible(true);
                                return false;
                            }
                        }
                    }
                    else if (hdu_act.conjunto == 1)
                    {
                        // Tabla binaria

                        if (fracciones == 0)
                        {
                            fidat = Path.Combine(carpeta, string.Format("{0}_{1}_tab_b.csv", fichero_sal, cHDU + 1));
                            fe = new FileStream(fidat, FileMode.Create, FileAccess.Write, FileShare.Read);
                            sw = new StreamWriter(fe, Encoding.UTF8);
                            if (NAXIS == 3)
                            {
                                for (int i3 = 0; i3 < NAXISn[2]; i3++)
                                {
                                    if (!ExtraeTB(fs, sw, le))
                                    {
                                        RecuperaParametros(p);
                                        Disponible(true);
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!ExtraeTB(fs, sw, le))
                                {
                                    RecuperaParametros(p);
                                    Disponible(true);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (NAXIS == 3)
                            {
                                for (int i3 = 0; i3 < NAXISn[2]; i3++)
                                {
                                    if (!ExtraeTB(fs, carpeta, fichero_sal, cHDU, fracciones, le, i3))
                                    {
                                        RecuperaParametros(p);
                                        Disponible(true);
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!ExtraeTB(fs, carpeta, fichero_sal, cHDU, fracciones, le, -1))
                                {
                                    RecuperaParametros(p);
                                    Disponible(true);
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Tabla ASCII

                        fidat = Path.Combine(carpeta, string.Format("{0}_{1}_tab_a.csv", fichero_sal, cHDU + 1));
                        fe = new FileStream(fidat, FileMode.Create, FileAccess.Write, FileShare.Read);
                        sw = new StreamWriter(fe, Encoding.UTF8);
                        if (NAXIS == 3)
                        {
                            for (int i3 = 0; i3 < NAXISn[2]; i3++)
                            {
                                if (!ExtraeTA(fs, sw, le, hdu_act))
                                {
                                    RecuperaParametros(p);
                                    Disponible(true);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (!ExtraeTA(fs, sw, le, hdu_act))
                            {
                                RecuperaParametros(p);
                                Disponible(true);
                                return false;
                            }
                        }
                    }
                    sw.Close();
                    bloques_leidos = fs.Position / BLOQUE;
                    if (bloques_leidos * BLOQUE < fs.Position) bloques_leidos++;
                    puntero = bloques_leidos * BLOQUE;
                    if (puntero >= fs.Length) break;
                    fs.Seek(puntero, SeekOrigin.Begin);
                }
                cHDU++;
            }
            fs.Close();
            RecuperaParametros(p);
            Console.Beep();
            Disponible(true);
            return true;
        }
        private void CopiaParametros(ParUtiles p)
        {
            p.NAXIS = NAXIS;
            if (NAXISn == null) p.NAXISn = null;
            else
            {
                p.NAXISn = new int[NAXISn.Length];
                for (int i = 0; i < NAXISn.Length; i++) p.NAXISn[i] = NAXISn[i];
            }
            p.FILENAME = FILENAME;
            p.punto_referencia = punto_referencia;
            p.proyeccion[0] = proyeccion[0];
            p.proyeccion[1] = proyeccion[1];
            p.proyeccion[2] = proyeccion[2];
            if (CRPIX == null) p.CRPIX = null;
            else
            {
                p.CRPIX = new double[CRPIX.Length];
                for (int i = 0; i < CRPIX.Length; i++) p.CRPIX[i] = CRPIX[i];
            }
            if (CRVAL == null) p.CRVAL = null;
            else
            {
                p.CRVAL = new double[CRVAL.Length];
                for (int i = 0; i < CRVAL.Length; i++) p.CRVAL[i] = CRVAL[i];
            }
            if (CUNIT == null) p.CUNIT = null;
            else
            {
                p.CUNIT = new string[CUNIT.Length];
                for (int i = 0; i < CUNIT.Length; i++) p.CUNIT[i] = CUNIT[i];
            }
            if (CTYPE == null) p.CTYPE = null;
            else
            {
                p.CTYPE = new string[CTYPE.Length];
                for (int i = 0; i < CTYPE.Length; i++) p.CTYPE[i] = CTYPE[i];
            }
            if (CDELT == null) p.CDELT = null;
            else
            {
                p.CDELT = new double[CDELT.Length];
                for (int i = 0; i < CDELT.Length; i++) p.CDELT[i] = CDELT[i];
            }
            if (CROTA == null) p.CROTA = null;
            else
            {
                p.CROTA = new double[CROTA.Length];
                for (int i = 0; i < CROTA.Length; i++) p.CROTA[i] = CROTA[i];
            }
            p.CD1[0] = CD1[0];
            p.CD1[1] = CD1[1];
            p.CD2[0] = CD2[0];
            p.CD2[1] = CD2[1];
            p.PC1[0] = PC1[0];
            p.PC1[1] = PC1[1];
            p.PC2[0] = PC2[0];
            p.PC2[1] = PC2[1];
            p.XTENSION = XTENSION;
            p.TFIELDS = TFIELDS;
            if (TTYPE == null) p.TTYPE = null;
            else
            {
                p.TTYPE = new string[TTYPE.Length];
                for (int i = 0; i < TTYPE.Length; i++) p.TTYPE[i] = TTYPE[i];
            }
            if (TBCOL == null) p.TBCOL = null;
            else
            {
                p.TBCOL = new int[TBCOL.Length];
                for (int i = 0; i < TBCOL.Length; i++) p.TBCOL[i] = TBCOL[i];
            }
            if (TFORM == null) p.TFORM = null;
            else
            {
                p.TFORM = new string[TFORM.Length];
                for (int i = 0; i < TFORM.Length; i++) p.TFORM[i] = TFORM[i];
            }
            if (TUNIT == null) p.TUNIT = null;
            else
            {
                p.TUNIT = new string[TUNIT.Length];
                for (int i = 0; i < TUNIT.Length; i++) p.TUNIT[i] = TUNIT[i];
            }
        }
        private void RecuperaParametros(ParUtiles p)
        {
            NAXIS = p.NAXIS;
            if (p.NAXISn == null) NAXISn = null;
            else
            {
                NAXISn = new int[p.NAXISn.Length];
                for (int i = 0; i < p.NAXISn.Length; i++) NAXISn[i] = p.NAXISn[i];
            }
            FILENAME = p.FILENAME;
            punto_referencia = p.punto_referencia;
            proyeccion[0] = p.proyeccion[0];
            proyeccion[1] = p.proyeccion[1];
            proyeccion[2] = p.proyeccion[2];
            if (p.CRPIX == null) CRPIX = null;
            else
            {
                CRPIX = new double[p.CRPIX.Length];
                for (int i = 0; i < p.CRPIX.Length; i++) CRPIX[i] = p.CRPIX[i];
            }
            if (p.CRVAL == null) CRVAL = null;
            else
            {
                CRVAL = new double[p.CRVAL.Length];
                for (int i = 0; i < p.CRVAL.Length; i++) CRVAL[i] = p.CRVAL[i];
            }
            if (p.CUNIT == null) CUNIT = null;
            else
            {
                CUNIT = new string[p.CUNIT.Length];
                for (int i = 0; i < p.CUNIT.Length; i++) CUNIT[i] = p.CUNIT[i];
            }
            if (p.CTYPE == null) CTYPE = null;
            else
            {
                CTYPE = new string[p.CTYPE.Length];
                for (int i = 0; i < p.CTYPE.Length; i++) CTYPE[i] = p.CTYPE[i];
            }
            if (p.CDELT == null) CDELT = null;
            else
            {
                CDELT = new double[p.CDELT.Length];
                for (int i = 0; i < p.CDELT.Length; i++) CDELT[i] = p.CDELT[i];
            }
            if (p.CROTA == null) CROTA = null;
            else
            {
                CROTA = new double[p.CROTA.Length];
                for (int i = 0; i < p.CROTA.Length; i++) CROTA[i] = p.CROTA[i];
            }
            CD1[0] = p.CD1[0];
            CD1[1] = p.CD1[1];
            CD2[0] = p.CD2[0];
            CD2[1] = p.CD2[1];
            PC1[0] = p.PC1[0];
            PC1[1] = p.PC1[1];
            PC2[0] = p.PC2[0];
            PC2[1] = p.PC2[1];
            XTENSION = p.XTENSION;
            TFIELDS = p.TFIELDS;
            if (p.TTYPE == null) TTYPE = null;
            else
            {
                TTYPE = new string[p.TTYPE.Length];
                for (int i = 0; i < p.TTYPE.Length; i++) TTYPE[i] = p.TTYPE[i];
            }
            if (p.TBCOL == null) TBCOL = null;
            else
            {
                TBCOL = new int[p.TBCOL.Length];
                for (int i = 0; i < p.TBCOL.Length; i++) TBCOL[i] = p.TBCOL[i];
            }
            if (p.TFORM == null) TFORM = null;
            else
            {
                TFORM = new string[p.TFORM.Length];
                for (int i = 0; i < p.TFORM.Length; i++) TFORM[i] = p.TFORM[i];
            }
            if (p.TUNIT == null) TUNIT = null;
            else
            {
                TUNIT = new string[p.TUNIT.Length];
                for (int i = 0; i < p.TUNIT.Length; i++) TUNIT[i] = p.TUNIT[i];
            }
        }
        private bool ElementosCeldaFichero(FileStream fs, StreamWriter sw, bool le, int tipo, int lon, int inde, int col)
        {
            int r = Rep(TFORM[col], inde);
            string[] elementos_fila_columna;
            byte[] dat = new byte[r * lon];
            fs.Read(dat, 0, r * lon);
            elementos_fila_columna = ElementosCeldaTipoFichero(le, r, dat, tipo, lon);
            for (int k = 0; k < elementos_fila_columna.Length; k++) sw.Write(string.Format("{0};", elementos_fila_columna[k]));
            return true;
        }
        private static string[] ElementosCeldaTipoFichero(bool le, int r, byte[] dat, int tipo, int lon)
        {
            int ne = 0;
            string[] elementos = new string[r];
            byte[] doble = new byte[lon];
            int ir = 0;
            int i1 = 0;
            while (true)
            {
                if (le)
                {
                    for (int k = lon - 1; k >= 0; k--) doble[k] = dat[i1++];
                }
                else
                {
                    for (int k = 0; k < lon; k++) doble[k] = dat[i1++];
                }
                elementos[ne++] = tipo switch
                {
                    0 => BitConverter.ToBoolean(doble).ToString(),
                    1 => BitConverter.ToInt16(doble).ToString(),
                    2 => BitConverter.ToUInt16(doble).ToString(),
                    3 => BitConverter.ToInt32(doble).ToString(),
                    4 => BitConverter.ToUInt32(doble).ToString(),
                    5 => BitConverter.ToInt64(doble).ToString(),
                    6 => BitConverter.ToSingle(doble).ToString(),
                    7 => BitConverter.ToDouble(doble).ToString(),
                    _ => "0",
                };
                ir++;
                if (ir == r) break;
            }
            return elementos;
        }
        private bool ExtraeDatos(FileStream fs, StreamWriter sw, bool le, HDU hdu_act)
        {
            int i2;
            int i1;
            byte[] bytesDato = new byte[hdu_act.byPorDato];
            int contador = 0;
            for (i2 = 0; i2 < NAXISn[1]; i2++)
            {
                contador++;
                if (contador % 1000 == 0)
                {
                    v_leidos.Text = string.Format("{0:N0}", contador);
                    Application.DoEvents();
                }
                for (i1 = 0; i1 < NAXISn[0]; i1++)
                {
                    if (le)
                    {
                        for (int k = hdu_act.byPorDato - 1; k >= 0; k--) bytesDato[k] = (byte)fs.ReadByte();
                    }
                    else
                    {
                        for (int k = 0; k < hdu_act.byPorDato; k++) bytesDato[k] = (byte)fs.ReadByte();
                    }
                    switch (hdu_act.clase_dato)
                    {
                        case 1:
                            sw.Write(string.Format("{0};", bytesDato[0]));
                            break;
                        case 2:
                            sw.Write(string.Format("{0};", BitConverter.ToInt16(bytesDato)));
                            break;
                        case 3:
                            sw.Write(string.Format("{0};", BitConverter.ToInt32(bytesDato)));
                            break;
                        case 4:
                            sw.Write(string.Format("{0};", BitConverter.ToSingle(bytesDato)));
                            break;
                        case 6:
                            sw.Write(string.Format("{0};", BitConverter.ToInt64(bytesDato)));
                            break;
                        default:
                            sw.Write(string.Format("{0};", BitConverter.ToDouble(bytesDato)));
                            break;
                    }
                }
                sw.WriteLine();
                sw.Flush();
            }
            v_leidos.Text = string.Format("{0:N0}", contador);
            return true;
        }
        private bool ExtraeTA(FileStream fs, StreamWriter sw, bool le, HDU hdu_act)
        {
            for (int col = 0; col < TFIELDS; col++)
            {
                if (TTYPE != null)
                {
                    sw.Write(string.Format("{0};", TTYPE[col]));
                }
                else
                {
                    sw.Write(string.Format("Col{0};", col + 1));
                }
            }
            sw.WriteLine("");
            int i2;
            int i1;
            byte b;
            char c;
            int contador = 0;
            string s;
            StringBuilder sb;
            TBCOL[TFIELDS] = NAXISn[0];
            if (hdu_act.byPorDato == 1)
            {
                for (i2 = 0; i2 < NAXISn[1]; i2++)
                {
                    contador++;
                    if (contador % 1000 == 0)
                    {
                        v_leidos.Text = string.Format("{0:N0}", contador);
                        Application.DoEvents();
                    }
                    sb = new StringBuilder();
                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                    {
                        b = (byte)fs.ReadByte();
                        c = Convert.ToChar(b);
                        if (c == s_millar) c = s_decimal;
                        sb.AppendFormat("{0}", c);
                    }
                    s = sb.ToString();
                    for (int i = 0; i < TFIELDS; i++)
                    {
                        sw.Write(string.Format("{0};", s.Substring(TBCOL[i] - 1, TBCOL[i + 1] - TBCOL[i]).Trim()));
                    }
                    sw.WriteLine();
                    sw.Flush();
                }
            }
            else
            {
                double v;
                byte[] bytesDato = new byte[hdu_act.byPorDato];
                for (i2 = 0; i2 < NAXISn[1]; i2++)
                {
                    contador++;
                    if (contador % 1000 == 0)
                    {
                        v_leidos.Text = string.Format("{0:N0}", contador);
                        Application.DoEvents();
                    }
                    sb = new StringBuilder();
                    for (i1 = 0; i1 < NAXISn[0]; i1++)
                    {
                        if (le)
                        {
                            for (int k = hdu_act.byPorDato - 1; k >= 0; k--) bytesDato[k] = (byte)fs.ReadByte();
                        }
                        else
                        {
                            for (int k = 0; k < hdu_act.byPorDato; k++) bytesDato[k] = (byte)fs.ReadByte();
                        }
                        if (hdu_act.clase_dato == 0)
                        {
                            v = hdu_act.byPorDato switch
                            {
                                2 => BitConverter.ToInt16(bytesDato),
                                8 => BitConverter.ToInt64(bytesDato),
                                _ => BitConverter.ToInt32(bytesDato),
                            };
                        }
                        else
                        {
                            v = hdu_act.byPorDato switch
                            {
                                4 => BitConverter.ToSingle(bytesDato),
                                _ => BitConverter.ToDouble(bytesDato),
                            };
                        }
                        c = Convert.ToChar(v);
                        if (c == s_millar) c = s_decimal;
                        sb.AppendFormat("{0}", c);
                    }
                    s = sb.ToString();
                    for (int i = 0; i < TFIELDS; i++)
                    {
                        sw.Write(string.Format("{0};", s.Substring(TBCOL[i] - 1, TBCOL[i + 1] - TBCOL[i]).Trim()));
                    }
                    sw.WriteLine();
                    sw.Flush();
                }
            }
            v_leidos.Text = string.Format("{0:N0}", contador);
            return true;
        }
        private bool ExtraeTB(FileStream fs, StreamWriter sw, bool le)
        {
            int r;
            int col;
            int ind;
            int contador = 0;
            bool[] quecols = ExportaCabeceraBin(sw);
            if (quecols == null) return false;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                contador++;
                if (contador % 1000 == 0)
                {
                    v_leidos.Text = string.Format("{0:N0}", contador);
                    Application.DoEvents();
                }
                col = 0;
                while (col < TFIELDS)
                {
                    if ((ind = TFORM[col].IndexOf("L")) != -1)
                    {
                        // 1 byte (lógico)

                        r = Rep(TFORM[col], ind);
                        for (int k = 0; k < r; k++)
                        {
                            if (quecols[col]) sw.Write(";");
                            fs.Seek(1, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("X")) != -1)
                    {
                        // 1 bit

                        r = Rep(TFORM[col], ind);
                        for (int k = 0; k < r; k++)
                        {
                            if (quecols[col]) sw.Write(";");
                            fs.Seek(1, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("B")) != -1)
                    {
                        // 1 byte (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                sw.Write(string.Format("{0};", (byte)fs.ReadByte()));
                            }
                        }
                        else
                        {
                            fs.Seek(r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("S")) != -1)
                    {
                        // 1 byte con signo (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                sw.Write(string.Format("{0};", (sbyte)fs.ReadByte()));
                            }
                        }
                        else
                        {
                            fs.Seek(r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("I")) != -1)
                    {
                        // 2 bytes (entero)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 1, 2, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(2, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("U")) != -1)
                    {
                        // 2 bytes (entero sin signo)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 2, 2, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(2, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("J")) != -1)
                    {
                        // 4 bytes (entero)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 3, 4, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(4, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("V")) != -1)
                    {
                        // 4 bytes (entero sin signo)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 4, 4, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(4, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("K")) != -1)
                    {
                        // 8 bytes (entero)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 5, 8, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(8, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("E")) != -1)
                    {
                        // 4 bytes (simple precisión)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 6, 4, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(4, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("D")) != -1)
                    {
                        // 8 bytes (doble precisión)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 7, 8, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(8, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("C")) != -1)
                    {
                        // 8 bytes (simple precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                fs.Seek(8, SeekOrigin.Current);
                                sw.Write(";");
                            }
                        }
                        else
                        {
                            fs.Seek(8 * r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("M")) != -1)
                    {
                        // 16 bytes (doble precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                fs.Seek(16, SeekOrigin.Current);
                                sw.Write(";");
                            }
                        }
                        else
                        {
                            fs.Seek(16 * r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("P")) != -1)
                    {
                        // 8 bytes (descriptor de array)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                fs.Seek(8, SeekOrigin.Current);
                                sw.Write(";");
                            }
                        }
                        else
                        {
                            fs.Seek(8 * r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("A")) != -1)
                    {
                        // 1 bytes (caracter)

                        int b;
                        r = Rep(TFORM[col], ind);
                        int w = ind == (TFORM[col].Length - 1) ? 1 : Convert.ToInt32(TFORM[col][(ind + 1)..]);
                        if (quecols[col])
                        {
                            StringBuilder sb = new StringBuilder(r * w);
                            for (int k1 = 0; k1 < r; k1++)
                            {
                                for (int k2 = 0; k2 < w; k2++)
                                {
                                    b = fs.ReadByte();
                                    if (b != 0) sb.AppendFormat("{0}", Convert.ToChar((byte)b));
                                }
                            }
                            sw.Write(string.Format("{0};", sb.ToString()));
                        }
                        else
                        {
                            fs.Seek(w * r, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 58], TFORM[col]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        sw.Close();
                        fs.Close();
                        Disponible(true);
                        return false;
                    }
                    col++;
                }
                sw.WriteLine("");
                sw.Flush();
            }
            v_leidos.Text = string.Format("{0:N0}", contador);
            return true;
        }
        private bool ExtraeTB(FileStream fs, string carpeta, string fichero_sal, int cHDU, int fracciones, bool le, int eje)
        {
            string fidat;
            FileStream fe;
            StreamWriter sw;
            int n_fraccion = 0;
            if (eje == -1)
            {
                fidat = Path.Combine(carpeta, string.Format("{0}_{1}_{2:D4}_tab_b.csv", fichero_sal, cHDU + 1, n_fraccion));
            }
            else
            {
                fidat = Path.Combine(carpeta, string.Format("{0}_{1}_{2}_{3:D4}_tab_b.csv", fichero_sal, cHDU + 1, eje, n_fraccion));
            }
            fe = new FileStream(fidat, FileMode.Create, FileAccess.Write, FileShare.Read);
            sw = new StreamWriter(fe, Encoding.UTF8);
            bool[] quecols = ExportaCabeceraBin(sw);
            if (quecols == null) return false;
            int r;
            int col;
            int ind;
            int c_fraccion = 0;
            int contador = 0;
            for (int i2 = 0; i2 < NAXISn[1]; i2++)
            {
                contador++;
                if (contador % 100 == 0)
                {
                    v_leidos.Text = string.Format("{0:N0}", contador);
                    Application.DoEvents();
                }
                col = 0;
                while (col < TFIELDS)
                {
                    if ((ind = TFORM[col].IndexOf("L")) != -1)
                    {
                        // 1 byte (lógico)

                        r = Rep(TFORM[col], ind);
                        for (int k = 0; k < r; k++)
                        {
                            if (quecols[col]) sw.Write(";");
                            fs.Seek(1, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("X")) != -1)
                    {
                        // 1 bit

                        r = Rep(TFORM[col], ind);
                        for (int k = 0; k < r; k++)
                        {
                            if (quecols[col]) sw.Write(";");
                            fs.Seek(1, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("B")) != -1)
                    {
                        // 1 byte (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                sw.Write(string.Format("{0};", (byte)fs.ReadByte()));
                            }
                        }
                        else
                        {
                            fs.Seek(r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("S")) != -1)
                    {
                        // 1 byte con signo (byte)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                sw.Write(string.Format("{0};", (sbyte)fs.ReadByte()));
                            }
                        }
                        else
                        {
                            fs.Seek(r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("I")) != -1)
                    {
                        // 2 bytes (entero)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 1, 2, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(2, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("U")) != -1)
                    {
                        // 2 bytes (entero sin signo)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 2, 2, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(2, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("J")) != -1)
                    {
                        // 4 bytes (entero)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 3, 4, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(4, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("V")) != -1)
                    {
                        // 4 bytes (entero sin signo)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 4, 4, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(4, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("K")) != -1)
                    {
                        // 8 bytes (entero)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 5, 8, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(8, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("E")) != -1)
                    {
                        // 4 bytes (simple precisión)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 6, 4, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(4, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("D")) != -1)
                    {
                        // 8 bytes (doble precisión)

                        if (quecols[col])
                        {
                            if (!ElementosCeldaFichero(fs, sw, le, 7, 8, ind, col))
                            {
                                sw.Close();
                                fs.Close();
                                return false;
                            }
                        }
                        else
                        {
                            fs.Seek(8, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("C")) != -1)
                    {
                        // 8 bytes (simple precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                fs.Seek(8, SeekOrigin.Current);
                                sw.Write(";");
                            }
                        }
                        else
                        {
                            fs.Seek(8 * r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("M")) != -1)
                    {
                        // 16 bytes (doble precisión complejo)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                fs.Seek(16, SeekOrigin.Current);
                                sw.Write(";");
                            }
                        }
                        else
                        {
                            fs.Seek(16 * r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("P")) != -1)
                    {
                        // 8 bytes (descriptor de array)

                        r = Rep(TFORM[col], ind);
                        if (quecols[col])
                        {
                            for (int k = 0; k < r; k++)
                            {
                                fs.Seek(8, SeekOrigin.Current);
                                sw.Write(";");
                            }
                        }
                        else
                        {
                            fs.Seek(8 * r, SeekOrigin.Current);
                        }
                    }
                    else if ((ind = TFORM[col].IndexOf("A")) != -1)
                    {
                        // 1 bytes (caracter)

                        int b;
                        r = Rep(TFORM[col], ind);
                        int w = ind == (TFORM[col].Length - 1) ? 1 : Convert.ToInt32(TFORM[col][(ind + 1)..]);
                        if (quecols[col])
                        {
                            StringBuilder sb = new StringBuilder(r * w);
                            for (int k1 = 0; k1 < r; k1++)
                            {
                                for (int k2 = 0; k2 < w; k2++)
                                {
                                    b = fs.ReadByte();
                                    if (b != 0) sb.AppendFormat("{0}", Convert.ToChar((byte)b));
                                }
                            }
                            sw.Write(string.Format("{0};", sb.ToString()));
                        }
                        else
                        {
                            fs.Seek(w * r, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 58], TFORM[col]), Idioma.msg[Idioma.lengua, 57], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        sw.Close();
                        fs.Close();
                        Disponible(true);
                        return false;
                    }
                    col++;
                }
                sw.WriteLine("");
                sw.Flush();
                c_fraccion++;
                if (c_fraccion == fracciones)
                {
                    sw.Close();
                    c_fraccion = 0;
                    n_fraccion++;
                    if (eje == -1)
                    {
                        fidat = Path.Combine(carpeta, string.Format("{0}_{1}_{2:D4}_tab_b.csv", fichero_sal, cHDU + 1, n_fraccion));
                    }
                    else
                    {
                        fidat = Path.Combine(carpeta, string.Format("{0}_{1}_{2}_{3:D4}_tab_b.csv", fichero_sal, cHDU + 1, eje, n_fraccion));
                    }
                    fe = new FileStream(fidat, FileMode.Create, FileAccess.Write, FileShare.Read);
                    sw = new StreamWriter(fe, Encoding.UTF8);
                    quecols = ExportaCabeceraBin(sw);
                    if (quecols == null) return false;
                    v_leidos.Text = string.Format("{0:N0}", contador);
                    Application.DoEvents();
                }
            }
            v_leidos.Text = string.Format("{0:N0}", contador);
            return true;
        }
    }
}
