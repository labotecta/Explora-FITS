using ExploraFits;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ExploraFITS
{
    public partial class Form2 : Form
    {
        public FITS fits;
        public int clase_dato;
        public int i3;

        public double ancho_valor_histograma;
        private int modo_convertir_px_coor;
        public int ancho_lienzo;
        public int alto_lienzo;
        private double escala_x;
        private double escala_y;
        private bool acotado_x;
        private bool acotado_y;
        public double x_izq;
        public double x_dch;
        public double y_sup;
        public double y_inf;
        private int ancho_marco;
        private int lado_marca;
        private int medio_lado_marca;
        private Pen lapiz_marca_roja;
        private Pen lapiz_marca_verde;
        private Pen lapiz_marca_amarilla;
        private Pen lapiz_marca_azul;
        private Pen lapiz_marca_morada;
        private readonly Pen lapiz_marca_roja_1 = new Pen(Color.Red, 1);

        public class Rango
        {
            public double min_abs;
            public double max_abs;
            public double min_acotado;
            public double max_acotado;
            public double media;
            public double dest;
            public double fp;
            public Rango(double min_abs, double max_abs, double min_acotado, double max_acotado, double media, double dest, double fp)
            {
                this.min_abs = min_abs;
                this.max_abs = max_abs;
                this.min_acotado = min_acotado;
                this.max_acotado = max_acotado;
                this.media = media;
                this.dest = dest;
                this.fp = fp;
            }
        }
        public Rango estadistica = null;

        public class PixelImg
        {
            public int x;
            public int y;
            public PixelImg(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public class Coordenadas
        {
            public double x;
            public double y;
            public Coordenadas(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public class UnidadesPixel
        {
            public double CDELT1;
            public double CDELT2;
            public UnidadesPixel(double CDELT1, double CDELT2)
            {
                this.CDELT1 = CDELT1;
                this.CDELT2 = CDELT2;
            }
        }
        public class Marca
        {
            public int indice;
            public byte tipo;
            public double al2000;
            public double de2000;
            public int pixelx;
            public int pixely;
            public Marca(int indice, byte tipo, double al2000, double de2000, int pixelx, int pixely)
            {
                this.indice = indice;
                this.tipo = tipo;
                this.al2000 = al2000;
                this.de2000 = de2000;
                this.pixelx = pixelx;
                this.pixely = pixely;
            }
        }
        private const int N_CATALOGOS = 2;
        private readonly List<Marca>[] marcas = new List<Marca>[N_CATALOGOS];
        private readonly int[] marca_sel = new int[N_CATALOGOS];

        public class Cima
        {
            public int radio;
            public int pixelx;
            public int pixely;
            public Cima(int radio, int pixelx, int pixely)
            {
                this.radio = radio;
                this.pixelx = pixelx;
                this.pixely = pixely;
            }
        }
        private List<Cima> cimas = null;

        private int ancho_img_datos;
        private int alto_img_datos;
        private int multiplicador_y;
        private byte[] flujo;
        private Color color_nan;
        private Color color_cota_inf;
        private Color color_cota_sup;

        public bool escalando;
        private bool esperando_par;
        private const int MAX_ESCALAR_PUNTOS = 32;
        private int escalar_n_puntos;
        private readonly double[] escalar_pixelx = new double[MAX_ESCALAR_PUNTOS];
        private readonly double[] escalar_pixely = new double[MAX_ESCALAR_PUNTOS];
        private readonly double[] escalar_ar = new double[MAX_ESCALAR_PUNTOS];
        private readonly double[] escalar_de = new double[MAX_ESCALAR_PUNTOS];

        private readonly int[] cols_sao =
        {
              1,   7,   8,  10,  12,  18,  25,  27,  28,  34,  36,  42,  43,  45,  47,  52,  58,  60,  61,  66,
             68,  74,  77,  81,  85,  88,  90,  92,  93,  94,  95,  96,  97,  98, 100, 105, 107, 108, 110, 115,
            117, 118, 124, 125, 130, 140, 151, 153, 155, 161, 168, 169, 171, 173, 178, 184, 194, 204
        };
        public class RegLineal
        {
            public double a;
            public double m;
            public RegLineal(double a, double m)
            {
                this.a = a;
                this.m = m;
            }
        }
        public class Pico
        {
            public int indice;
            public double valor;
            public Pico(int indice, double valor)
            {
                this.indice = indice;
                this.valor = valor;
            }
        }
        public List<Pico> picos = null;
        public class Ficha
        {
            public string[] campos;
            public string[] rotulos;
        }

        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            int ancho = ClientSize.Width;
            int alto = ClientSize.Height;
            int sup = b_volver.Location.Y;

            // Interfaz imagen

            b_volver.Location = new Point(ancho - b_volver.Width - 4, sup);
            b_salvar.Location = new Point(b_volver.Location.X - b_redibuja.Width - 4, sup);
            b_redibuja.Location = new Point(b_salvar.Location.X - b_redibuja.Width - 4, sup);
            sel_catalogo.Location = new Point(b_redibuja.Location.X - sel_catalogo.Width - 4, sup + 6);
            b_salva_cimas.Location = new Point(sel_catalogo.Location.X - b_salva_cimas.Width - 4, sup);
            b_cimas.Location = new Point(b_salva_cimas.Location.X - b_cimas.Width - 4, sup);
            v_brillo.Location = new Point(b_cimas.Location.X - v_brillo.Width - 4, sup + 6);
            v_margen_brillo.Location = new Point(v_brillo.Location.X - v_margen_brillo.Width - 4, sup + 6);
            v_reduccion.Location = new Point(v_margen_brillo.Location.X - v_reduccion.Width - 4, sup + 6);

            b_escalar_ok.Location = new Point(ancho - b_escalar_ok.Width - 4, sup + b_volver.Height + 10);
            b_escalar_esc.Location = new Point(b_escalar_ok.Location.X - b_escalar_esc.Width - 4, sup + b_volver.Height + 10);
            b_escalar.Location = new Point(b_escalar_esc.Location.X - b_escalar.Width - 4, sup + b_volver.Height + 4);

            r_nombre_marca_1.Location = new Point(ancho - r_nombre_marca_1.Width - 4, b_escalar.Location.Y + b_escalar.Height + 4);
            b_ficha_hyperleda.Location = new Point(ancho - b_ficha_hyperleda.Width - 4, r_nombre_marca_1.Location.Y + r_nombre_marca_1.Height + 4);
            de_catalogo_1.Location = new Point(b_ficha_hyperleda.Location.X - de_catalogo_1.Width - 4, r_nombre_marca_1.Location.Y + r_nombre_marca_1.Height + 12);
            ar_catalogo_1.Location = new Point(de_catalogo_1.Location.X - de_catalogo_2.Width - 4, r_nombre_marca_1.Location.Y + r_nombre_marca_1.Height + 12);
            r_nombre_marca_2.Location = new Point(ancho - r_nombre_marca_1.Width - 4, ar_catalogo_1.Location.Y + b_ficha_hyperleda.Height + 4);
            b_ficha_sao.Location = new Point(ancho - b_ficha_sao.Width - 4, r_nombre_marca_2.Location.Y + r_nombre_marca_2.Height + 4);
            de_catalogo_2.Location = new Point(b_ficha_sao.Location.X - de_catalogo_2.Width - 4, r_nombre_marca_2.Location.Y + r_nombre_marca_2.Height + 12);
            ar_catalogo_2.Location = new Point(de_catalogo_2.Location.X - ar_catalogo_2.Width - 4, r_nombre_marca_2.Location.Y + r_nombre_marca_2.Height + 12);

            // Interfaz espectro

            b_limpiar.Location = new Point(b_redibuja.Location.X - b_limpiar.Width - 4, sup);
            b_picos.Location = new Point(b_limpiar.Location.X - b_picos.Width - 4, sup);
            v_hueco.Location = new Point(b_picos.Location.X - v_hueco.Width - 4, sup + 6);
            v_z.Location = new Point(v_hueco.Location.X - v_z.Width - 4, sup + 6);
            lista_elegidas.Location = new Point(v_z.Location.X - lista_elegidas.Width - 4, sup + 6);
            lista_elegibles.Location = new Point(lista_elegidas.Location.X - lista_elegibles.Width - 4, sup + 6);

            // Lienzo

            int m_sup = b_ver_crpix.Location.Y + b_ver_crpix.Size.Height + 4;
            int m_izq = v_sup_y.Location.X + v_sup_y.Size.Width + 4;
            ancho_lienzo = ancho - m_izq - 4 - r_nombre_marca_1.Width - 8;
            alto_lienzo = alto - m_sup - 8;
            lienzo.Location = new Point(m_izq, m_sup);
            lienzo.Size = new Size(ancho_lienzo, alto_lienzo);

            r_y.Text = string.Empty;
            /*r_y.Refresh();
            r_nombre_marca_1.Text = string.Empty;
            r_nombre_marca_2.Text = string.Empty;*/

            IniciaControles();
            LeeEspectros();
            CambiaIdioma();
        }
        public void LeeEspectros()
        {
            FileStream fe = new FileStream(Path.Combine(fits.sendaApp, "espectros_atomicos.csv"), FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader r = new StreamReader(fe);
            lista_elegibles.Items.Clear();
            string linea;
            string[] sd;
            while (!r.EndOfStream)
            {
                linea = r.ReadLine();
                sd = linea.Split(';');
                lista_elegibles.Items.Add(string.Format("{0,10:f3}  {1}", sd[2], sd[3]));
            }
            r.Close();

        }
        public void CambiaIdioma()
        {
            sel_catalogo.Items.Clear();
            sel_catalogo.Items.Add(Idioma.msg[Idioma.lengua, 27]);
            sel_catalogo.Items.Add(Idioma.msg[Idioma.lengua, 28]);
            sel_catalogo.Items.Add(Idioma.msg[Idioma.lengua, 29]);
        }
        public void IniciaControles()
        {
            lienzo.Size = new Size(ancho_lienzo, alto_lienzo);
            r_x.Text = string.Empty;
            r_y.Refresh();
            pixels_x.Text = string.Empty;
            pixels_y.Text = string.Empty;
            crpix_1.Text = string.Empty;
            crpix_2.Text = string.Empty;
            crval_1.Text = string.Empty;
            crval_2.Text = string.Empty;
            cunit_1.Text = string.Empty;
            cunit_2.Text = string.Empty;
            v_izq_x.Text = string.Empty;
            v_sup_y.Text = string.Empty;
            v_dch_x.Text = string.Empty;
            v_inf_y.Text = string.Empty;
            p_raton_x.Text = string.Empty;
            p_raton_y.Text = string.Empty;
            raton_x.Text = string.Empty;
            raton_y.Text = string.Empty;
            raton_brillo.Text = string.Empty;
            marca_sel[0] = -1;
            r_nombre_marca_1.Text = string.Empty;
            ar_catalogo_1.Text = string.Empty;
            de_catalogo_1.Text = string.Empty;
            ar_catalogo_2.Text = string.Empty;
            de_catalogo_2.Text = string.Empty;
            marca_sel[1] = -1;
            r_nombre_marca_2.Text = string.Empty;

            label1.Visible = false;
            b_salva_cimas.Visible = false;
            b_ver_crpix.Visible = false;
            v_reduccion.Visible = false;
            v_margen_brillo.Visible = false;
            v_brillo.Visible = false;
            b_cimas.Visible = false;
            sel_catalogo.Visible = false;
            b_ficha_hyperleda.Visible = false;
            b_ficha_sao.Visible = false;
            b_escalar.Visible = false;
            b_escalar_esc.Visible = false;
            b_escalar_ok.Visible = false;
            b_picos.Visible = false;
            b_limpiar.Visible = false;
            v_hueco.Visible = false;
            v_z.Visible = false;
            lista_elegibles.Visible = false;
            lista_elegidas.Visible = false;
            FinEscalar();
            if (picos != null) picos.Clear();
            if (cimas != null) cimas.Clear();
            lista_elegidas.Items.Clear();
        }
        public void SelInterfaz(int cual)
        {
            var que = cual switch
            {
                0 => true,
                _ => false,
            };
            label1.Visible = que;
            b_ver_crpix.Visible = que;
            v_reduccion.Visible = que;
            v_margen_brillo.Visible = que;
            v_brillo.Visible = que;
            b_cimas.Visible = que;
            sel_catalogo.Visible = que;
            b_ficha_hyperleda.Visible = que;
            b_ficha_sao.Visible = que;
            b_escalar.Visible = que;
            b_escalar_esc.Visible = que;
            b_escalar_ok.Visible = que;

            b_picos.Visible = !que;
            b_limpiar.Visible = !que;
            v_hueco.Visible = !que;
            v_z.Visible = !que;
            lista_elegibles.Visible = !que;
            lista_elegidas.Visible = !que;
        }
        private void R_y_Paint(object sender, PaintEventArgs e)
        {
            if (fits.CTYPE == null) return;
            var g = e.Graphics;
            g.DrawString(fits.CTYPE[1], new Font("Segoe UI", 9, FontStyle.Bold, GraphicsUnit.Point), Brushes.Blue, 0, 0, new StringFormat(StringFormatFlags.DirectionVertical));
        }
        private void B_volver_Click(object sender, EventArgs e)
        {
            fits.TopLevel = true;
            fits.BringToFront();
            fits.Focus();
        }
        private void B_redibuja_Click(object sender, EventArgs e)
        {
            if (fits.NAXIS == 3) Redibuja(i3);
            else Redibuja();
        }
        private void B_salvar_Click(object sender, EventArgs e)
        {
            if (fits.img == null) return;
            SaveFileDialog ficheroescritura = new SaveFileDialog()
            {
                Filter = "PNG |*.png;*.bmp|TODO |*.*",
                FilterIndex = 1
            };
            if (ficheroescritura.ShowDialog() == DialogResult.OK)
            {
                fits.Disponible(false);
                if (fits.alta_resolucion.Checked)
                {
                    fits.img.Save(ficheroescritura.FileName, ImageFormat.Bmp);
                }
                else
                {
                    fits.img.Save(ficheroescritura.FileName, ImageFormat.Png);
                }
                fits.Disponible(true);
                Console.Beep();
            }
        }
        private void B_hyperleda_Click(object sender, EventArgs e)
        {
            Hyperleda();
        }
        private void B_sao_Click(object sender, EventArgs e)
        {
            Smithsonian();
        }
        private void B_cimas_Click(object sender, EventArgs e)
        {
            if (multiplicador_y > 1) return;
            if (fits.NAXIS == 0 || fits.NAXISn == null || fits.NAXISn[0] == 0 || estadistica == null) return;

            const int R_MAX = 1000;

            double brillo_minimo;
            if (v_brillo.Text.Trim().Length == 0)
            {
                brillo_minimo = estadistica.max_abs;
            }
            else
            {
                brillo_minimo = Convert.ToDouble(v_brillo.Text.Trim());
            }
            int reduccion;
            if (v_reduccion.Text.Trim().Length == 0)
            {
                reduccion = 1;
            }
            else
            {
                reduccion = Convert.ToByte(v_reduccion.Text.Trim());
            }
            double dif_brillo;
            if (v_reduccion.Text.Trim().Length == 0)
            {
                // Diferencias mayores del 1% del brillo mínimo

                dif_brillo = 0.01 * brillo_minimo;
            }
            else
            {
                dif_brillo = brillo_minimo * Convert.ToDouble(v_margen_brillo.Text.Trim().Replace(fits.s_millar, fits.s_decimal)) / 100;

                // Para evitar comparaciones con cero

                if (dif_brillo < 1.0e-20) dif_brillo = 1.0e-20;
            }
            fits.Disponible(false);

            // Se trabaja con una copia de los datos originales pasados a double
            // con 'reduccion' mayor que 1 se toma el mayor valor del cuadrado 'reduccion * reduccion'

            int hastax = (fits.NAXISn[0] - reduccion);
            int hastay = (fits.NAXISn[1] - reduccion);
            int nejex = fits.NAXISn[0] / reduccion;
            int nejey = fits.NAXISn[1] / reduccion;
            double[,] copia = new double[nejex, nejey];

            int nx;
            int ny = 0;
            double valor = 0;
            double max;
            int contador = 0;
            for (int i2 = 0; i2 < hastay; i2 += reduccion)
            {
                nx = 0;
                for (int i1 = 0; i1 < hastax; i1 += reduccion)
                {
                    contador++;
                    if (contador % FITS.X10M == 0)
                    {
                        r_nombre_marca_1.Text = string.Format("{0:N0}", contador);
                        Application.DoEvents();
                    }
                    if (reduccion == 1)
                    {
                        valor = Dato(i1, i2);
                        if (double.IsNaN(valor)) valor = 0;
                    }
                    else
                    {
                        max = double.MinValue;
                        for (int k1 = 0; k1 < reduccion; k1++)
                        {
                            for (int k2 = 0; k2 < reduccion; k2++)
                            {
                                valor = Dato(i1 + k1, i2 + k2);
                                if (double.IsNaN(valor)) valor = 0;
                                if (max < valor) max = valor;
                            }
                        }
                    }
                    copia[nx++, ny] = valor;
                }
                ny++;
            }
            int pxv;
            int pyv;
            int nmax;
            int suma_x;
            int suma_y;
            double v;
            double vecino;
            int radio;
            int iguales;
            bool descartar;
            int n_descartables;
            int[,] vecinos_descartables = new int[4 * (R_MAX + 1) * (R_MAX + 1), 2];
            byte[,] descartado = new byte[nejex, nejey];
            Array.Clear(descartado, 0, descartado.Length);
            if (cimas == null)
            {
                cimas = new List<Cima>();
            }
            else
            {
                cimas.Clear();
            }
            b_salva_cimas.Visible = false;
            int ik1;
            int ik2;
            contador = 0;
            for (int i2 = 0; i2 < nejey; i2++)
            {
                for (int i1 = 0; i1 < nejex; i1++)
                {
                    contador++;
                    if (contador % FITS.X1M == 0)
                    {
                        r_nombre_marca_1.Text = string.Format("{0:N0}  {1:N0}", contador, cimas.Count);
                        Application.DoEvents();
                    }

                    // Evitar los descartados

                    if (descartado[i1, i2] == 1) continue;

                    v = copia[i1, i2];
                    if (v < brillo_minimo) continue;

                    // Buscar en el perimetro con un radio 'r'

                    radio = 1;
                    descartar = false;

                    // Zona dentro del radio (que puede ir aumentando)

                    n_descartables = 0;
                    while (true)
                    {
                        iguales = 0;
                        for (int k1 = -radio; k1 <= radio; k1++)
                        {
                            ik1 = i1 + k1;
                            if (ik1 < 0 || ik1 >= nejex) continue;
                            for (int k2 = -radio; k2 <= radio; k2++)
                            {
                                // Saltar a el mismo

                                if (k1 == 0 && k2 == 0) continue;
                                ik2 = i2 + k2;

                                // Saltar los interiores porque se han tratado con el 'r' previo

                                if (Math.Abs(k1) < radio && Math.Abs(k2) < radio) continue;

                                // Saltar los bordes

                                if (ik2 < 0 || ik2 >= nejey) continue;
                                vecino = copia[ik1, ik2];
                                if (vecino > v + dif_brillo)
                                {
                                    descartar = true;
                                    break;
                                }
                                else
                                {
                                    // Vecino descartable por ser menor o igual

                                    vecinos_descartables[n_descartables, 0] = ik1;
                                    vecinos_descartables[n_descartables, 1] = ik2;
                                    n_descartables++;

                                    if (Math.Abs(vecino - v) < dif_brillo) iguales++;
                                }
                            }
                            if (descartar) break;
                        }
                        if (descartar) break; ;
                        if (iguales > 0)
                        {
                            // Hay iguales, ampliar el radio

                            if (radio == R_MAX)
                            {
                                // Descartar por llanura extensa, a él y a sus vecinos

                                descartar = true;
                                for (int i = 0; i < n_descartables; i++)
                                {
                                    descartado[vecinos_descartables[i, 0], vecinos_descartables[i, 1]] = 1;
                                }
                                break;
                            }
                            radio++;
                        }
                        else
                        {
                            // Todos menores, es una cima

                            break;
                        }
                    }
                    if (descartar) continue;

                    // Nueva cima

                    if (reduccion == 1)
                    {
                        if (radio == 1)
                        {
                            cimas.Add(new Cima(radio, i1, i2));
                        }
                        else
                        {
                            // Poner una marca en el centro de gravedad de los máximos valores

                            // Valor máximo

                            max = double.MinValue;
                            for (int k1 = -radio; k1 <= radio; k1++)
                            {
                                ik1 = i1 + k1;
                                if (ik1 < 0 || ik1 >= nejex) continue;
                                for (int k2 = -radio; k2 <= radio; k2++)
                                {
                                    ik2 = i2 + k2;
                                    if (ik2 < 0 || ik2 >= nejey) continue;
                                    valor = copia[ik1, ik2];
                                    if (max < valor) max = valor;
                                }
                            }

                            // Media de las posiciones de los máximos valores

                            nmax = 0;
                            suma_x = 0;
                            suma_y = 0;
                            for (int k1 = -radio; k1 <= radio; k1++)
                            {
                                ik1 = i1 + k1;
                                if (ik1 < 0 || ik1 >= nejex) continue;
                                for (int k2 = -radio; k2 <= radio; k2++)
                                {
                                    ik2 = i2 + k2;
                                    if (ik2 < 0 || ik2 >= nejey) continue;
                                    valor = copia[ik1, ik2];
                                    if (max - valor <= dif_brillo)
                                    {
                                        nmax++;
                                        suma_x += ik1;
                                        suma_y += ik2;
                                    }
                                }
                            }
                            cimas.Add(new Cima(radio, suma_x / nmax, suma_y / nmax));
                        }
                    }
                    else
                    {
                        // Poner una marca en el centro de gravedad de los máximos valores (sin reducción) dentro del cuadro extendido

                        pxv = i1 * reduccion;
                        pyv = i2 * reduccion;
                        max = double.MinValue;
                        for (int k1 = 0; k1 < radio * reduccion; k1++)
                        {
                            ik1 = pxv + k1;
                            for (int k2 = 0; k2 < radio * reduccion; k2++)
                            {
                                ik2 = pyv + k2;
                                if (ik1 >= 0 && ik1 < fits.NAXISn[0] && ik2 >= 0 && ik2 < fits.NAXISn[1])
                                {
                                    valor = Dato(ik1, ik2);
                                    if (max < valor) max = valor;
                                }
                            }
                        }
                        nmax = 0;
                        suma_x = 0;
                        suma_y = 0;
                        for (int k1 = 0; k1 < radio * reduccion; k1++)
                        {
                            ik1 = pxv + k1;
                            for (int k2 = 0; k2 < radio * reduccion; k2++)
                            {
                                ik2 = pyv + k2;
                                if (ik1 >= 0 && ik1 < fits.NAXISn[0] && ik2 >= 0 && ik2 < fits.NAXISn[1])
                                {
                                    valor = Dato(ik1, ik2);
                                    if (max - valor <= dif_brillo)
                                    {
                                        nmax++;
                                        suma_x += ik1;
                                        suma_y += ik2;
                                    }
                                }
                            }
                        }
                        cimas.Add(new Cima(radio, suma_x / nmax, suma_y / nmax));
                    }

                    // Esto no es correcto, habría que librar a los que son cimas

                    for (int i = 0; i < n_descartables; i++)
                    {
                        descartado[vecinos_descartables[i, 0], vecinos_descartables[i, 1]] = 1;
                    }
                }
            }

            // Dibuja las cimas sobre la imagen

            if (cimas.Count > 0)
            {
                b_salva_cimas.Visible = true;
                using Graphics g = Graphics.FromImage(fits.img);
                int bloque = cimas.Count / 100;
                if (bloque < 10) bloque = 10;
                int menos;
                int ancho;
                Rectangle r;
                Cima c;
                contador = 0;
                for (int k = 0; k < cimas.Count; k++)
                {
                    c = cimas[k];
                    menos = (int)(2 * c.radio * escala_x);
                    ancho = 2 * menos;
                    r = new Rectangle(Vx(c.pixelx) - menos, Vy(c.pixely) - menos, ancho, ancho);
                    g.DrawEllipse(lapiz_marca_roja, r);
                    r = new Rectangle(Vx(c.pixelx) - 1, Vy(c.pixely) - 1, 2, 2);
                    g.DrawRectangle(lapiz_marca_roja_1, r);
                    contador++;
                    if (contador % bloque == 0)
                    {
                        r_nombre_marca_1.Text = string.Format("{0:N0} de {1}", contador, cimas.Count);
                        Application.DoEvents();
                    }
                }
            }
            r_nombre_marca_1.Text = string.Empty;
            lienzo.Refresh();
            MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 78], cimas.Count), Idioma.msg[Idioma.lengua, 77], MessageBoxButtons.OK, MessageBoxIcon.Information);
            fits.Disponible(true);
        }
        private void B_salva_cimas_Click(object sender, EventArgs e)
        {
            if (cimas == null || cimas.Count == 0) return;
            SaveFileDialog ficheroescritura = new SaveFileDialog()
            {
                Filter = "CSV (*.csv)|*.csv|TODO (*.*)|*.*",
                FilterIndex = 1
            };
            if (ficheroescritura.ShowDialog() == DialogResult.OK)
            {
                FileStream fe = new FileStream(ficheroescritura.FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                StreamWriter sw = new StreamWriter(fe, Encoding.UTF8);
                Coordenadas coor;
                sw.WriteLine(string.Format("{0};{1};{2};{3};{4}", "radio", "pixel x", "pixel y", "coordenada x", "coordenada y"));
                foreach (Cima c in cimas)
                {
                    coor = PixelAcoordenadas(Vx(c.pixelx), Vy(c.pixely), modo_convertir_px_coor);
                    sw.WriteLine(string.Format("{0};{1};{2};{3};{4}", c.radio, c.pixelx, c.pixely, coor.x, coor.y));
                }
                sw.Close();
                Console.Beep();
            }
        }
        private void B_ver_crpix_Click(object sender, EventArgs e)
        {
            VerReferencia();
        }
        private void B_escalar_Click(object sender, EventArgs e)
        {
            fits.f_referencia = new Form3
            {
                fits = fits
            };
            if (fits.proyeccion[1] == 6)
            {
                fits.f_referencia.Inicializa(fits.CRPIX[0], fits.CRPIX[1], fits.CRVAL[0], fits.CRVAL[1], fits.CDELT[0], fits.CDELT[1]);
            }
            fits.f_referencia.ShowDialog(this);
            if (!fits.f_referencia_cancelado)
            {
                fits.proyeccion[0] = 0;
                fits.proyeccion[2] = 0;
                fits.CD1[0] = 0;
                fits.CD1[1] = 0;
                fits.CD2[0] = 0;
                fits.CD2[0] = 0;
                fits.CRPIX = new double[2];
                fits.CRVAL = new double[2];
                fits.CDELT = new double[2];
                fits.proyeccion[1] = 2 + 2 + 2;
                modo_convertir_px_coor = 1;
                fits.punto_referencia = 4;
                fits.CRPIX[0] = fits.f_referencia_crpix1;
                fits.CRPIX[1] = fits.f_referencia_crpix2;
                fits.CRVAL[0] = fits.f_referencia_crval1;
                fits.CRVAL[1] = fits.f_referencia_crval2;
                fits.CDELT[0] = fits.f_referencia_cdelt1;
                fits.CDELT[1] = fits.f_referencia_cdelt2;
                Redibuja();
                escalando = true;
                esperando_par = false;
                b_escalar.Enabled = false;
                b_escalar_esc.Enabled = true;
                b_escalar_ok.Text = "0";
                MessageBox.Show(Idioma.msg[Idioma.lengua, 80], Idioma.msg[Idioma.lengua, 79], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void B_escalar_esc_Click(object sender, EventArgs e)
        {
            b_escalar_ok.Text = "Ok";
            FinEscalar();
        }
        private void B_escalar_ok_Click(object sender, EventArgs e)
        {
            /*
                // CDELT[0] = (AR0 - AR1) / (p0x - p1x)
                // CRVAL[0] = AR0 - p0x * CDELT[0]
                
                fits.CRPIX[0] = 0;
                fits.CRPIX[1] = 0;

                fits.CDELT[0] = (escalar_ar[0] - escalar_ar[1]) / (escalar_pixelx[0] - escalar_pixelx[1]);
                fits.CRVAL[0] = escalar_ar[0] - escalar_pixelx[0] * fits.CDELT[0];

                fits.CDELT[1] = (escalar_de[0] - escalar_de[1]) / (escalar_pixely[0] - escalar_pixely[1]);
                fits.CRVAL[1] = escalar_de[0] - escalar_pixely[0] * fits.CDELT[1];
            */

            fits.CRPIX[0] = 0;
            fits.CRPIX[1] = 0;
            RegLineal rl;
            rl = RegresionLineal(escalar_pixelx, escalar_ar, escalar_n_puntos);
            fits.CDELT[0] = rl.m;
            fits.CRVAL[0] = rl.a;
            rl = RegresionLineal(escalar_pixely, escalar_de, escalar_n_puntos);
            fits.CDELT[1] = rl.m;
            fits.CRVAL[1] = rl.a;

            Redibuja();
            FinEscalar();
            DialogResult res = MessageBox.Show(Idioma.msg[Idioma.lengua, 104], Idioma.msg[Idioma.lengua, 79], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;
            fits.CreaFITS(fits.FILENAME, fits.FICHERO_FITS, true, true, Idioma.msg[Idioma.lengua, 94]);
        }
        private void B_ficha_hyperleda_Click(object sender, EventArgs e)
        {
            if (fits.NAXIS == 0 || fits.NAXISn == null || fits.NAXISn[0] == 0 || estadistica == null) return;
            if (marca_sel[0] == -1) return;
            int ind = marcas[0][marca_sel[0]].indice;
            FichaHyperleda(ind);
        }
        public Ficha FichaHyperleda(int ind)
        {
            FileStream fe = new FileStream(Path.Combine(fits.sendaApp, fits.fichero_hyperleda), FileMode.Open, FileAccess.Read, FileShare.Read);
            fe.Seek(ind, SeekOrigin.Begin);
            byte[] linea = new byte[1024];
            int nl;
            int nc = -1;
            do
            {
                nc++;
                nl = fe.Read(linea, nc, 1);
            } while (nl == 1 && linea[nc] != '\n' && nc < 1024);
            fe.Close();
            if (nl == 0 || nc == 1024)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 89], Idioma.msg[Idioma.lengua, 88], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            string s = Encoding.Default.GetString(linea, 0, nc).Trim();
            Ficha ficha = new Ficha
            {
                campos = s.Split(';')
            };
            ficha.rotulos = new string[ficha.campos.Length];
            fe = new FileStream(Path.Combine(fits.sendaApp, "camposHL.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader r = new StreamReader(fe);
            nc = 0;
            while (!r.EndOfStream)
            {
                if (nc == ficha.rotulos.Length)
                {
                    MessageBox.Show(Idioma.msg[Idioma.lengua, 90], Idioma.msg[Idioma.lengua, 88], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                ficha.rotulos[nc++] = r.ReadLine();
            }
            r.Close();
            fits.f_ficha_hyperleda = new Form4
            {
                fits = fits
            };
            fits.f_ficha_hyperleda.Datos(ficha.rotulos, ficha.campos);
            fits.f_ficha_hyperleda.ShowDialog(this);
            return ficha;
        }
        private void B_ficha_sao_Click(object sender, EventArgs e)
        {
            /*
              1-  6   ---     I6              SAO number.
                  7   ---     A1              "D" if this SAO entry refers to a duplicate star from the catalog.
              8-  9  hours    I2               Right ascension B1950.0 equinox and epoch.
             10- 11   min     I2               RA
             12- 17   sec     F6.3             RA
             18- 24  sec/yr   F7.4             Annual proper motion in RA.
             25- 26   0.001 arcsec/yr I2(F2.3) Standard deviation of pm (RA).
                 27   ---     A1               A "+" or "-" to indicate that the minutes of time associated with the seconds portion of RA (bytes 28-33) must be increased or decreased by 1, respectively; otherwise blank (if RA2 is the same minute as RA sec in bytes 12-17).
             28- 33   sec     F6.3             Seconds portion (RA2) of RA at original epoch, precessed to 1950.0 (i.e. 1950 position modified by proper motion).
             34- 35  0.01 arcsec/yr   I2(F2.2) Standard deviation of RA2.
             36- 41   years   F6.1             Epoch of RA2.
                 42   ---     A1               Sign of declination.
             43- 44   deg     I2               Declination B1950.0 equinox and epoch.
             45- 46  arcmin   I2               Dec
             47- 51  arcsec   F5.2             Dec
             52- 57  arcsec/yr  F6.3           Annual proper motion mu(delta).
             58- 59  0.001 arcsec/yr I2(F2.3)  Standard deviation of mu(delta).
                 60   ---     A1               A "+" or "-" to indicate that the arcminutes associated with the arcseconds portion of Dec (bytes 61-65) must be increased or decreased by 1, respectively; otherwise blank (if Dec2 is in the same minute as Dec in bytes 47-51).
             61- 65  arcsec   F5.2             Seconds portion (Dec2) of Dec at original epoch, precessed to 1950.0 (i.e. 1950 position modified by the proper motion).
             66- 67   0.01 arcsec   I2(F2.2)   Standard deviation of Dec2.
             68- 73   years   F6.1             Epoch of Dec2.
             74- 76   0.01 arcsec   I3(F3.2)   Standard deviation of position at epoch 1950.0.
             77- 80    mag    F4.1             Photographic magnitude mpg (99.9 if no value present).  When both magnitude fields are 99.9, the miscellaneous code (byte 95) should be checked for possible variability, in which case magnitudes may not be reported.
             81- 84    mag    F4.1             Visual magnitude mv (99.9 if no value present).
             85- 87    ---    A3               Spectral type ("+++" for composite spectra).
             88- 89    ---    I2               Coded source of visual magnitude (see
             90- 91   ---     I2               Coded source of star number and footnotes (see Table 4).
                 92   ---     I1               Coded source of photographic magnitude (see Table 5).
                 93   ---     I1               Coded source of proper motions (see Table 6).
                 94   ---     I1               Coded source of spectral type (see Table 7).
                 95   ---     I1               Coded miscellaneous remarks for duplicity and variability (see Table 8).
                 96   ---     I1               Accuracy of visual magnitude:  0 indicates that the magnitude in the source catalog is reported to 0.00 mag; 1 to 0.0 mag.
                 97   ---     I1               Accuracy of photographic magnitude same coding as for byte 96.
             98- 99   ---     I2               Code for source catalog (see Table 9).
            100-104   ---     I5               Number in source catalog.
            105-106   ---     A2               Durchmusterung (DM) identification BD  Bonner Durchmusterung CD  Cordoba Durchmusterung CP  Cape Photographic Durchmusterung All DM fields are blank if no DM identification is present.
                107   ---     A1               Sign of DM zone.
            108-109   ---     I2 (A2)          DM zone.
            110-114   ---     I5 (A5)          DM number.
            115-116   ---     A2               Component identification if there are two or more SAO stars having the same DM number. For multiple systems included in the "Index Catalogue of Visual Double Stars" (IDS, see Worley 1980) the IDS components are given; for non-IDS stars, components were assigned on the basis of magnitude. If two components of southern double stars are listed, DM numbers from different catalogs are often quoted for the components. In these cases, component identifications are usually given without changing the DM numbers.
                117   ---     A1               Lower case letter identification for BD supplemental stars (Warren and Kress 1980).
            118-123   ---     A6 (I6)          Henry Draper (HD or HDE) Catalogue number
                124   ---     A1 (I1)          HD Code (1, 2, ... for component identifications where more than one star has the same HD number (not necessarily equivalent to A, B, ... or to the component identifications in the IDS; see Table 2))
            125-129   ---     A5 (I5)          Number in the "General Catalogue of 33342 Stars for 1950" (GC, Boss 1937).
            130-139    rad    F10.8            Right ascension (B1950.0).
            140-150    rad    F11.8            Declination (B1950.0).
            151-152   hours   I2               Right ascension for J2000.0.
            153-154    min    I2               RA
            155-160    sec    F6.3             RA
            161-167   sec/yr  F7.4             Annual proper motion in right ascension for J2000.0.
                168    ---    A1               Sign of declination.
            169-170    deg    I2               Declination for J2000.0.
            171-172   arcmin  I2               Dec
            173-177   arcsec  F5.2             Dec
            178-183   arcsec/yr F6.3             Annual proper motion in declination for J2000.0.
            184-193    rad    F10.8            RA (J2000.0)
            194-204    rad    F11.8            Dec (J2000.0)
            */
            if (fits.NAXIS == 0 || fits.NAXISn == null || fits.NAXISn[0] == 0 || estadistica == null) return;
            if (marca_sel[1] == -1) return;
            int ind = marcas[1][marca_sel[1]].indice;
            FichaSAO(ind);
        }
        public Ficha FichaSAO(int ind)
        {
            int nc = cols_sao.Length - 1;
            Ficha ficha = new Ficha
            {
                campos = new string[nc]
            };
            FileStream fe = new FileStream(Path.Combine(fits.sendaApp, fits.fichero_sao), FileMode.Open, FileAccess.Read, FileShare.Read);
            fe.Seek(ind, SeekOrigin.Begin);
            byte[] linea = new byte[cols_sao[nc]];
            fe.Read(linea, 0, linea.Length);
            fe.Close();
            for (int i = 0; i < nc; i++)
            {
                ficha.campos[i] = Encoding.Default.GetString(linea, cols_sao[i] - 1, cols_sao[i + 1] - cols_sao[i]).Trim();
            }
            fits.f_ficha_sao = new Form5
            {
                fits = fits
            };
            fits.f_ficha_sao.Datos(ficha.campos);
            fits.f_ficha_sao.ShowDialog(this);
            return ficha;
        }
        private void Sel_catalogo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (sel_catalogo.SelectedIndex)
            {
                case 1:
                    Hyperleda();
                    break;
                case 2:
                    Smithsonian();
                    break;
            }
        }
        private void FinEscalar()
        {
            b_escalar_esc.Enabled = false;
            b_escalar_ok.Enabled = false;
            escalar_n_puntos = 0;
            esperando_par = false;
            escalando = false;
            b_escalar.Enabled = true;
        }
        private void Hyperleda()
        {
            if (multiplicador_y > 1) return;
            if (fits.NAXIS == 0 || fits.NAXISn == null || fits.NAXISn[0] == 0 || estadistica == null) return;

            if (!acotado_x || !acotado_y)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 82], Idioma.msg[Idioma.lengua, 91], MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
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
            double t;
            if (x_dch < x_izq)
            {
                // Hacer que el mínimo sea realmente el menor

                t = x_dch;
                x_dch = x_izq;
                x_izq = t;
            }
            if (y_inf < y_sup)
            {
                // Hacer que el máximo sea realmente el mayor

                t = y_inf;
                y_inf = y_sup;
                y_sup = t;
            }
            int ri = BuscarAR(fits.indices_hyperleda, x_izq);
            int rf = BuscarAR(fits.indices_hyperleda, x_dch);
            int indice;
            double ar;
            double de;
            Marca m;
            marcas[0] = new List<Marca>();
            int encontradas = 0;
            if (rf >= ri && rf < fits.indices_hyperleda.Count)
            {
                for (int i = ri; i <= rf; i++)
                {
                    indice = fits.indices_hyperleda[i].indice;
                    ar = fits.indices_hyperleda[i].al2000;
                    de = fits.indices_hyperleda[i].de2000;
                    if (ar >= x_izq && ar <= x_dch && de >= y_sup && de <= y_inf)
                    {
                        // Asegurarse que la marca cae integramente dentro de la imagen
                        // Evitando además, los errores de convertir de coordenadas a pixel y biceversa

                        PixelImg pp = CoordenadasApixel(ar, de, modo_convertir_px_coor);
                        if (pp.x - medio_lado_marca < 0 || pp.y - medio_lado_marca < 0)
                        {
                            continue;
                        }
                        if (pp.x + medio_lado_marca >= fits.NAXISn[0] || pp.y + medio_lado_marca >= fits.NAXISn[1])
                        {
                            continue;
                        }
                        m = MarcaObjetoCatalogo(indice, fits.indices_hyperleda[i].tipo, ar, de);
                        marcas[0].Add(m);
                        encontradas++;
                    }
                }
            }
            if (encontradas == 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 83], Idioma.msg[Idioma.lengua, 91], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lienzo.Refresh();
                MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 84], encontradas), Idioma.msg[Idioma.lengua, 91], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Smithsonian()
        {
            if (multiplicador_y > 1) return;
            if (fits.NAXIS == 0 || fits.NAXISn == null || fits.NAXISn[0] == 0 || estadistica == null) return;

            if (!acotado_x || !acotado_y)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 82], Idioma.msg[Idioma.lengua, 81], MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
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
            double t;
            if (x_dch < x_izq)
            {
                // Hacer que el mínimo sea realmente el menor

                t = x_dch;
                x_dch = x_izq;
                x_izq = t;
            }
            if (y_inf < y_sup)
            {
                // Hacer que el máximo sea realmente el mayor

                t = y_inf;
                y_inf = y_sup;
                y_sup = t;
            }
            int ri = BuscarAR(fits.indices_sao, x_izq);
            int rf = BuscarAR(fits.indices_sao, x_dch);
            int indice;
            double ar;
            double de;
            Marca m;
            marcas[1] = new List<Marca>();
            int encontradas = 0;
            if (rf >= ri && rf < fits.indices_sao.Count)
            {
                for (int i = ri; i <= rf; i++)
                {
                    indice = fits.indices_sao[i].indice;
                    ar = fits.indices_sao[i].al2000;
                    de = fits.indices_sao[i].de2000;
                    if (ar >= x_izq && ar <= x_dch && de >= y_sup && de <= y_inf)
                    {
                        // Asegurarse que la marca cae integramente dentro de la imagen
                        // Evitando además, los errores de convertir de coordenadas a pixel y biceversa

                        PixelImg pp = CoordenadasApixel(ar, de, modo_convertir_px_coor);
                        if (pp.x - medio_lado_marca < 0 || pp.y - medio_lado_marca < 0)
                        {
                            continue;
                        }
                        if (pp.x + medio_lado_marca >= fits.NAXISn[0] || pp.y + medio_lado_marca >= fits.NAXISn[1])
                        {
                            continue;
                        }
                        m = MarcaObjetoCatalogo(indice, fits.indices_sao[i].tipo, ar, de);
                        marcas[1].Add(m);
                        encontradas++;
                    }
                }
            }
            if (encontradas == 0)
            {
                MessageBox.Show(Idioma.msg[Idioma.lengua, 83], Idioma.msg[Idioma.lengua, 81], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lienzo.Refresh();
                MessageBox.Show(string.Format(Idioma.msg[Idioma.lengua, 84], encontradas), Idioma.msg[Idioma.lengua, 81], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Lienzo_MouseDown(object sender, MouseEventArgs e)
        {
            if (fits.img == null) return;
            if (fits.es_espectro)
            {
                p_raton_x.Text = string.Format("{0}", e.X);
                p_raton_y.Text = string.Format("{0}", e.Y);
                Coordenadas ce = fits.PuntoEspectro(e.X, e.Y);
                raton_x.Text = string.Format("{0}", ce.x);
                raton_y.Text = string.Format("{0}", ce.y);

                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    // Añadir la línea atómica más cercana

                    AddLineaAtomica(ce.x);
                    Console.Beep();
                }
                return;
            }

            // Pixels en la imagen visualizada

            int px = (int)(e.X * escala_x);
            int py = (int)(e.Y / multiplicador_y * escala_y);

            // El mayor en un radio de 1

            byte v = fits.img.GetPixel(px, py).G;
            int r = 1;
            int npx = px;
            int npy = py;
            byte vecino;
            for (int k1 = -r; k1 <= r; k1++)
            {
                if (px + k1 < 0 || px + k1 >= fits.NAXISn[0]) continue;
                for (int k2 = -r; k2 <= r; k2++)
                {
                    if (k1 == 0 && k2 == 0) continue;
                    if (py + k2 < 0 || py + k2 >= fits.NAXISn[1]) continue;
                    vecino = fits.img.GetPixel(px + k1, py + k2).G;
                    if (vecino > v)
                    {
                        npx = px + k1;
                        npy = py + k2;
                        v = vecino;
                    }
                }
            }
            px = npx;
            py = npy;
            p_raton_x.Text = string.Format("{0}", px);
            p_raton_y.Text = string.Format("{0}", py);
            raton_brillo.Text = string.Format("{0}", Dato(px, py));

            // Coordenadas

            Coordenadas xy = PixelAcoordenadas(Vx(px), Vy(py), modo_convertir_px_coor);
            raton_x.Text = string.Format("{0}", xy.x);
            raton_y.Text = string.Format("{0}", xy.y);
            if (escalando)
            {
                if (escalar_n_puntos == MAX_ESCALAR_PUNTOS)
                {
                    MessageBox.Show(Idioma.msg[Idioma.lengua, 101], Idioma.msg[Idioma.lengua, 79], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (esperando_par)
                {
                    // Punto de la imagen

                    escalar_pixelx[escalar_n_puntos] = px;
                    escalar_pixely[escalar_n_puntos] = py;
                    escalar_n_puntos++;
                    b_escalar_ok.Text = escalar_n_puntos.ToString();
                    if (escalar_n_puntos == 2) b_escalar_ok.Enabled = true;
                    esperando_par = false;
                }
                else
                {
                    // Objeto del catálogo

                    escalar_ar[escalar_n_puntos] = xy.x;
                    escalar_de[escalar_n_puntos] = xy.y;
                    esperando_par = true;
                }
                MarcaEscalar(px, py, esperando_par);
                return;
            }

            // Buscar si corresponde a una marca de catálogo

            marca_sel[0] = -1;
            r_nombre_marca_1.Text = string.Empty;
            ar_catalogo_1.Text = string.Empty;
            de_catalogo_1.Text = string.Empty;
            marca_sel[1] = -1;
            r_nombre_marca_2.Text = string.Empty;
            ar_catalogo_2.Text = string.Empty;
            de_catalogo_2.Text = string.Empty;
            Marca m;
            for (int i = 0; i < N_CATALOGOS; i++)
            {
                if (marcas[i] != null && marcas[i].Count > 0)
                {
                    for (int k = 0; k < marcas[i].Count; k++)
                    {
                        m = marcas[i][k];
                        if (px >= m.pixelx - medio_lado_marca && px <= m.pixelx + medio_lado_marca && py >= m.pixely - medio_lado_marca && py <= m.pixely + medio_lado_marca)
                        {
                            switch (i)
                            {
                                case 0:
                                    marca_sel[0] = k;
                                    r_nombre_marca_1.Text = string.Format("{0} {1}", fits.hyperleda_tipos[m.tipo, 0], NombreMarcaHyperleda(m.indice));
                                    ar_catalogo_1.Text = string.Format("{0}", m.al2000);
                                    de_catalogo_1.Text = string.Format("{0}", m.de2000);
                                    break;
                                case 1:
                                    marca_sel[1] = k;
                                    r_nombre_marca_2.Text = string.Format("{0} {1}", "SAO", NombreMarcaSAO(m.indice));
                                    ar_catalogo_2.Text = string.Format("{0}", m.al2000);
                                    de_catalogo_2.Text = string.Format("{0}", m.de2000);
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
        }
        private int Vx(int x)
        {
            if (fits.invertir_x.Checked) return fits.NAXISn[0] - x;
            return x;
        }
        private double Vx(double x)
        {
            if (fits.invertir_x.Checked) return fits.NAXISn[0] - x;
            return x;
        }
        private int Vy(int y)
        {
            if (fits.invertir_y.Checked) return fits.NAXISn[1] - y;
            return y;
        }
        private double Vy(double y)
        {
            if (fits.invertir_y.Checked) return fits.NAXISn[1] - y;
            return y;
        }

        private bool Geometria()
        {
            SelInterfaz(0);
            escalando = false;
            escalar_n_puntos = 0;
            esperando_par = false;
            r_x.Text = fits.CTYPE[0];
            r_y.Refresh();
            pixels_x.Text = fits.NAXISn[0].ToString();
            pixels_y.Text = fits.NAXISn[1].ToString();
            if (fits.CRPIX != null && fits.CRPIX[0] != -1 && fits.CRVAL != null && fits.CRVAL[0] > double.MinValue)
            {
                crpix_1.Text = Vx(fits.CRPIX[0]).ToString();
                crval_1.Text = fits.CRVAL[0].ToString();
            }
            else
            {
                crpix_1.Text = string.Empty;
                crval_1.Text = string.Empty;
            }
            if (fits.CRPIX != null && fits.CRPIX[1] != -1 && fits.CRVAL != null && fits.CRVAL[1] > double.MinValue)
            {
                crpix_2.Text = Vy(fits.CRPIX[1]).ToString();
                crval_2.Text = fits.CRVAL[1].ToString();
            }
            else
            {
                crpix_2.Text = string.Empty;
                crval_2.Text = string.Empty;
            }
            if (fits.CUNIT != null && fits.CUNIT[0] != null && fits.CUNIT[1] != null)
            {
                cunit_1.Text = fits.CUNIT[0].ToString();
                cunit_2.Text = fits.CUNIT[1].ToString();
            }
            else
            {
                cunit_1.Text = string.Empty;
                cunit_2.Text = string.Empty;
            }

            // Ajusta el lienzo

            AjustaLienzo();
            escala_x = (double)fits.NAXISn[0] / lienzo.Width;
            escala_y = (double)fits.NAXISn[1] / lienzo.Height;

            // Crea la imagen y el flujo de bytes para rellenarla

            if (fits.alta_resolucion.Checked)
            {
                // RGB de 16 bits sin canal alfa
                // 6 bytes (48 bits) por pixel

                fits.img = new Bitmap(ancho_img_datos, alto_img_datos, PixelFormat.Format48bppRgb);
                flujo = new byte[6 * ancho_img_datos * alto_img_datos];
            }
            else
            {
                // RGB de 8 bits sin canal alfa
                // 3 bytes (24 bits) por pixel

                fits.img = new Bitmap(ancho_img_datos, alto_img_datos, PixelFormat.Format24bppRgb);
                flujo = new byte[3 * ancho_img_datos * alto_img_datos];
            }

            // Para que se vean siempre igual

            ancho_marco = (int)(2 * escala_x);
            lado_marca = (int)(12 * escala_x);
            medio_lado_marca = lado_marca / 2;
            lapiz_marca_roja = new Pen(Color.Red, ancho_marco);
            lapiz_marca_verde = new Pen(Color.LightGreen, ancho_marco);
            lapiz_marca_amarilla = new Pen(Color.Yellow, ancho_marco);
            lapiz_marca_azul = new Pen(Color.Blue, ancho_marco);
            lapiz_marca_morada = new Pen(Color.Fuchsia, ancho_marco);

            // Calcular las coordenadas de los vértices de la imagen original

            acotado_x = acotado_y = false;
            v_izq_x.Text = string.Empty;
            v_sup_y.Text = string.Empty;
            v_dch_x.Text = string.Empty;
            v_inf_y.Text = string.Empty;
            if (fits.NAXISn[1] == 1)
            {
                // Vector. Posiblemente un espectro

                modo_convertir_px_coor = 1;
                if (fits.CRPIX != null && fits.CRPIX[0] != -1 && fits.CRVAL != null && fits.CRVAL[0] > double.MinValue && Math.Abs(fits.CDELT[0]) > 1e-20)
                {
                    x_izq = fits.CRVAL[0] - fits.CRPIX[0] * fits.CDELT[0];
                    x_dch = fits.CRVAL[0] + (fits.NAXISn[0] - fits.CRPIX[0]) * fits.CDELT[0];
                    if (fits.invertir_x.Checked)
                    {
                        v_izq_x.Text = string.Format("{0}", x_dch);
                        v_dch_x.Text = string.Format("{0}", x_izq);
                    }
                    else
                    {
                        v_izq_x.Text = string.Format("{0}", x_izq);
                        v_dch_x.Text = string.Format("{0}", x_dch);
                    }
                    acotado_x = true;
                }
                else
                {
                    x_izq = 0;
                    x_dch = 0;
                    fits.CDELT[0] = 0;
                }
                if (fits.CRPIX != null && fits.CRPIX[1] != -1 && fits.CRVAL != null && fits.CRVAL[1] > double.MinValue && Math.Abs(fits.CDELT[1]) > 1e-20)
                {
                    y_sup = fits.CRVAL[1] - fits.CRPIX[1] * fits.CDELT[1];
                    y_inf = fits.CRVAL[1] + (fits.NAXISn[1] - fits.CRPIX[1]) * fits.CDELT[1];
                    v_sup_y.Text = string.Format("{0}", y_sup);
                    v_inf_y.Text = string.Format("{0}", y_inf);
                    acotado_y = true;
                }
                else
                {
                    y_sup = 0;
                    y_inf = 0;
                    fits.CDELT[1] = 0;
                }
            }
            else
            {
                if (fits.proyeccion[0] == 2 + 2 + 2 + 2)
                {
                    // CPRIX, CVAL, CD_1, CD_2
                    // Si están los fits.CD1 y fits.CD2 se les da preferencia

                    modo_convertir_px_coor = 0;
                }
                else if (fits.proyeccion[1] == 2 + 2 + 2)
                {
                    // CPRIX, CVAL, CDELT
                    // Está anulado, se ha convertido a CD_1, CD_2

                    modo_convertir_px_coor = 1;
                }
                else if (fits.proyeccion[2] == 2 + 2 + 2 + 2 + 2)
                {
                    // CPRIX, CVAL, CDELT, PC_1, PC_2

                    modo_convertir_px_coor = 2;
                }
                else
                {
                    modo_convertir_px_coor = -1;
                }
                switch (modo_convertir_px_coor)
                {
                    case 0:

                        // Comprobamos que el determinante no es cero

                        if (fits.CD1[0] * fits.CD2[1] - fits.CD1[1] * fits.CD2[0] != 0)
                        {
                            /*
                             * x_izq, y_sup (pixel 0, 0)
                               | 
                               ****************
                               *              *
                               *              *
                               ****************
                                              |
                                               x_dch, y_inf (pixel fits.NAXISn[0], fits.NAXISn[1])
                            */

                            // En la imagen original (sin invertir)

                            Coordenadas arriba = PixelAcoordenadas(0, 0, modo_convertir_px_coor);
                            x_izq = arriba.x;
                            y_sup = arriba.y;
                            Coordenadas abajo = PixelAcoordenadas(fits.NAXISn[0], fits.NAXISn[1], modo_convertir_px_coor);
                            x_dch = abajo.x;
                            y_inf = abajo.y;

                            if (fits.invertir_x.Checked)
                            {
                                v_izq_x.Text = string.Format("{0}", x_dch);
                                v_dch_x.Text = string.Format("{0}", x_izq);
                            }
                            else
                            {
                                v_izq_x.Text = string.Format("{0}", x_izq);
                                v_dch_x.Text = string.Format("{0}", x_dch);
                            }

                            // A efectos visuales

                            if (fits.invertir_y.Checked)
                            {
                                v_sup_y.Text = string.Format("{0}", y_inf);
                                v_inf_y.Text = string.Format("{0}", y_sup);
                            }
                            else
                            {
                                v_sup_y.Text = string.Format("{0}", y_sup);
                                v_inf_y.Text = string.Format("{0}", y_inf);
                            }
                            acotado_x = acotado_y = true;

                            // Calcular las dimensiones en coordenadas de un pixel con la matriz de rotación-escala

                            UnidadesPixel escala = CalculaCDELT(fits.CD1[0], fits.CD1[1], fits.CD2[0], fits.CD2[1]);
                            fits.CDELT[0] = escala.CDELT1;
                            fits.CDELT[1] = escala.CDELT2;
                        }
                        break;
                    case 1:
                        if (Math.Abs(fits.CDELT[0]) > 1e-20)
                        {
                            x_izq = fits.CRVAL[0] - fits.CRPIX[0] * fits.CDELT[0];
                            x_dch = fits.CRVAL[0] + (fits.NAXISn[0] - fits.CRPIX[0]) * fits.CDELT[0];
                            if (fits.invertir_x.Checked)
                            {
                                v_izq_x.Text = string.Format("{0}", x_dch);
                                v_dch_x.Text = string.Format("{0}", x_izq);
                            }
                            else
                            {
                                v_izq_x.Text = string.Format("{0}", x_izq);
                                v_dch_x.Text = string.Format("{0}", x_dch);
                            }
                            acotado_x = true;
                        }
                        if (Math.Abs(fits.CDELT[1]) > 1e-20)
                        {
                            y_sup = fits.CRVAL[1] - fits.CRPIX[1] * fits.CDELT[1];
                            y_inf = fits.CRVAL[1] + (fits.NAXISn[1] - fits.CRPIX[1]) * fits.CDELT[1];
                            if (fits.invertir_y.Checked)
                            {
                                v_sup_y.Text = string.Format("{0}", y_inf);
                                v_inf_y.Text = string.Format("{0}", y_sup);
                            }
                            else
                            {
                                v_sup_y.Text = string.Format("{0}", y_sup);
                                v_inf_y.Text = string.Format("{0}", y_inf);
                            }
                            acotado_y = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            color_nan = fits.color_nan;
            color_cota_inf = fits.color_cota_inf;
            color_cota_sup = fits.color_cota_sup;
            return true;
        }
        private void AjustaLienzo()
        {
            ancho_img_datos = fits.NAXISn[0];
            alto_img_datos = fits.NAXISn[1];
            lienzo.Size = new Size(ancho_lienzo, alto_lienzo);
            if (lienzo.Width >= ancho_img_datos && lienzo.Height >= alto_img_datos)
            {
                lienzo.Size = new Size(ancho_img_datos, alto_img_datos);
            }
            else if (lienzo.Width < ancho_img_datos && lienzo.Height < alto_img_datos)
            {
                float fx = (float)lienzo.Width / ancho_img_datos;
                float fy = (float)lienzo.Height / alto_img_datos;
                if (fx < fy)
                {
                    float f = (float)alto_img_datos / ancho_img_datos;
                    int x = lienzo.Width;
                    int y = (int)(x * f);
                    lienzo.Size = new Size(x, y);
                }
                else
                {
                    float f = (float)ancho_img_datos / alto_img_datos;
                    int y = lienzo.Height;
                    int x = (int)(y * f);
                    lienzo.Size = new Size(x, y);
                }
            }
            else if (lienzo.Width < ancho_img_datos)
            {
                float f = (float)alto_img_datos / ancho_img_datos;
                lienzo.Size = new Size(lienzo.Width, (int)(lienzo.Width * f));
            }
            else if (lienzo.Height < alto_img_datos)
            {
                float f = (float)ancho_img_datos / alto_img_datos;
                lienzo.Size = new Size((int)(lienzo.Height * f), lienzo.Height);
            }
            if (alto_img_datos < alto_lienzo / 60)
            {
                multiplicador_y = 48;
                alto_img_datos *= multiplicador_y;
                lienzo.Size = new Size(lienzo.Width, alto_img_datos);
            }
            else
            {
                multiplicador_y = 1;
            }
        }

        private void VerReferencia()
        {
            if (fits.img == null) return;
            if (fits.NAXISn[1] == 1)
            {
                if (fits.CRPIX != null && fits.CRPIX[0] != -1 && fits.CRVAL != null && fits.CRVAL[0] > double.MinValue)
                {
                    Graphics g = Graphics.FromImage(fits.img);
                    g.DrawRectangle(lapiz_marca_amarilla, (int)fits.CRPIX[0], 0, lado_marca, alto_img_datos);
                    lienzo.Refresh();
                }
            }
            else
            {
                if (fits.punto_referencia != 4) return;
                Graphics g = Graphics.FromImage(fits.img);
                g.DrawRectangle(lapiz_marca_azul, (int)Vx(fits.CRPIX[0]), (int)Vy(fits.CRPIX[1]) * multiplicador_y, lado_marca, lado_marca);
                lienzo.Refresh();
            }
        }
        private void MarcaEscalar(int px, int py, bool esperando_par)
        {
            if (fits.img == null) return;
            Graphics g = Graphics.FromImage(fits.img);
            g.DrawString(esperando_par ? Idioma.msg[Idioma.lengua, 102] : Idioma.msg[Idioma.lengua, 103], new Font("Verdana", 16, FontStyle.Bold), Brushes.LightBlue, px - lado_marca, py - lado_marca);
            lienzo.Refresh();
        }
        private Marca MarcaObjetoCatalogo(int ind, byte tipo, double ar, double de)
        {
            // Pixeles en la imagen visualizada

            PixelImg pp = CoordenadasApixel(ar, de, modo_convertir_px_coor);
            int px = pp.x;
            int py = pp.y;

            if (px < 0 || py < 0) return null;

            // Dibuja la marca en la imagen visualizada

            Graphics g = Graphics.FromImage(fits.img);
            if (tipo == 0)
            {
                // Galaxia

                g.DrawRectangle(lapiz_marca_roja, px - medio_lado_marca, py - medio_lado_marca, lado_marca, lado_marca);
            }
            else if (tipo == 8)
            {
                // Estrella

                g.DrawRectangle(lapiz_marca_amarilla, px - medio_lado_marca, py - medio_lado_marca, lado_marca, lado_marca);
            }
            else if (tipo >= 200)
            {
                // Estrella SAO

                g.DrawEllipse(lapiz_marca_morada, px - medio_lado_marca, py - medio_lado_marca, lado_marca, lado_marca);
            }
            else
            {
                // Resto

                g.DrawRectangle(lapiz_marca_verde, px - medio_lado_marca, py - medio_lado_marca, lado_marca, lado_marca);
            }

            // Devolver el registro de la marca (datos del pixel en la imagen visualizada)

            return new Marca(ind, tipo, ar, de, px, py);
        }
        public int BuscarAR(List<FITS.IndiceCatalogo> ic, double x)
        {
            int medio = 0;
            int bajo = 0;
            int alto = ic.Count - 1;
            bool encontrado = false;
            while (bajo <= alto && encontrado == false)
            {
                medio = (bajo + alto) / 2;
                if (ic[medio].al2000 == x)
                {
                    encontrado = true;
                    break;
                }
                else if (ic[medio].al2000 > x)
                {
                    alto = medio - 1;
                }
                else
                {
                    bajo = medio + 1;
                }
            }
            if (!encontrado)
            {
                return bajo;
            }
            return medio;
        }
        public string NombreMarcaHyperleda(int ind)
        {
            int val;
            byte[] sb = new byte[128];
            FileStream fe = new FileStream(Path.Combine(fits.sendaApp, fits.fichero_hyperleda), FileMode.Open, FileAccess.Read, FileShare.Read);
            fe.Seek(ind, SeekOrigin.Begin);
            int npyc = 0;
            int nb = 0;
            while (true)
            {
                val = fe.ReadByte();
                if (val == ';')
                {
                    // Lee los 2 primeros campos: nombre, pgc

                    if (npyc == 1) break;
                    npyc++;
                    sb[nb++] = (byte)' ';
                }
                else
                {
                    sb[nb++] = (byte)val;
                }
            }
            fe.Close();
            return Encoding.Default.GetString(sb, 0, nb);
        }
        public string NombreMarcaSAO(int ind)
        {
            int val;
            byte[] sb = new byte[128];
            FileStream fe = new FileStream(Path.Combine(fits.sendaApp, fits.fichero_sao), FileMode.Open, FileAccess.Read, FileShare.Read);
            fe.Seek(ind, SeekOrigin.Begin);

            // Lee los 6 primeros bytes: número SAO

            int nb = 6;
            for (int i = 0; i < nb; i++)
            {
                val = fe.ReadByte();
                sb[i] = (byte)val;
            }
            fe.Close();
            return Encoding.Default.GetString(sb, 0, nb);
        }
        private UnidadesPixel CalculaCDELT(double CD1_1, double CD1_2, double CD2_1, double CD2_2)
        {
            /*

            | a b |            | d -c |
            | c d |  inversa = |-b  a | dividido del determinante (ad - bc)

            */

            double CDELT1 = Math.Sqrt(CD1_1 * CD1_1 + CD2_1 * CD2_1);
            double CDELT2 = Math.Sqrt(CD1_2 * CD1_2 + CD2_2 * CD2_2);
            double determinante = CD1_1 * CD2_2 - CD1_2 * CD2_1;

            // Por convención, si el determinante es negativo CDELT1 debería ser negativo
            // la matriz de datos debería estár alineada con AR y DE

            if (determinante < 0)
            {
                CDELT1 = -CDELT1;
            }
            return new UnidadesPixel(CDELT1, CDELT2);
        }
        private Coordenadas PixelAcoordenadas(int px, int py, int modo)
        {
            double xr;
            double yr;
            switch (modo)
            {
                case 0:
                    double inc_x = px - fits.CRPIX[0];
                    double inc_y = py - fits.CRPIX[1];

                    // Cálculo de la distancia, en coordenadas, al punto de referencia (fits.CRPIX) a partir de la distancia en pixeles
                    /*
                       | inc_ar |   | fits.CD1[0] fits.CD1[1] |   | inc_x |
                       | inc_de | = | fits.CD2[0] fits.CD2[1] | x | inc_y |
                    */

                    double inc_ar = fits.CD1[0] * inc_x + fits.CD1[1] * inc_y;
                    double inc_de = fits.CD2[0] * inc_x + fits.CD2[1] * inc_y;

                    xr = fits.CRVAL[0] + inc_ar;
                    yr = fits.CRVAL[1] + inc_de;
                    break;
                case 1:
                    if (Math.Abs(fits.CDELT[0]) > 1e-20)
                    {
                        xr = fits.CRVAL[0] + fits.CDELT[0] * (px - fits.CRPIX[0]);
                    }
                    else
                    {
                        xr = 0;
                    }
                    if (Math.Abs(fits.CDELT[1]) > 1e-20)
                    {
                        yr = fits.CRVAL[1] + fits.CDELT[1] * (py - fits.CRPIX[1]);
                    }
                    else
                    {
                        yr = 0;
                    }
                    break;
                default:
                    xr = fits.CRVAL[0] + fits.CDELT[0] * (px - fits.CRPIX[0]) * fits.PC1[0] + fits.CDELT[1] * (py - fits.CRPIX[1]) * fits.PC1[1];
                    yr = fits.CRVAL[1] + fits.CDELT[0] * (px - fits.CRPIX[0]) * fits.PC2[0] + fits.CDELT[1] * (py - fits.CRPIX[1]) * fits.PC2[1];
                    break;
            }
            return new Coordenadas(xr, yr);
        }
        private PixelImg CoordenadasApixel(double ar, double de, int modo)
        {
            double inc_ar = ar - fits.CRVAL[0];
            double inc_de = de - fits.CRVAL[1];
            double inc_x;
            double inc_y;
            switch (modo)
            {
                case 0:
                    /*
                    | a b |            | d -c |
                    | c d |  inversa = |-b  a | dividido del determinante (ad - bc)
                    */

                    // Cálculo de la distancia, en pixeles, al punto de referencia (fits.CRPIX) a partir de la distancia en coordenadas
                    /*
                       |  fits.CD2[1] -fits.CD2[0] |   | inc_ar |   | inc_x |
                       | -fits.CD1[1]  fits.CD1[0] | x | inc_de | = | inc_y |
                    */

                    double determinante = fits.CD1[0] * fits.CD2[1] - fits.CD1[1] * fits.CD2[0];
                    inc_x = (fits.CD2[1] * inc_ar - fits.CD2[0] * inc_de) / determinante;
                    inc_y = (-fits.CD1[1] * inc_ar + fits.CD1[0] * inc_de) / determinante;
                    break;
                default:
                    inc_x = inc_ar / fits.CDELT[0];
                    inc_y = inc_de / fits.CDELT[1];
                    break;
            }
            int px = (int)(fits.CRPIX[0] + inc_x);
            int py = (int)(fits.CRPIX[1] + inc_y);
            return new PixelImg(Vx(px), Vy(py));
        }
        /*
        private UnidadesPixel EstimaCDELT(double CD1_1, double CD2_1, double CD2_2)
        {
            // CD1_1 =  CDELT1* cos(CROTA2)
            // CD1_2 = -CDELT2* sin(CROTA2)
            // CD2_1 =  CDELT1* sin(CROTA2)
            // CD2_2 =  CDELT2* cos(CROTA2)

            double tangente1 = CD2_1 / CD1_1;
            double coseno = Math.Sqrt(1.0 / (1.0 + tangente1 * tangente1));
            double CDELT1 = CD1_1 / coseno;
            double CDELT2 = CD2_2 / coseno;
            return new UnidadesPixel(CDELT1, CDELT2);
        }
        */
        private double Dato(int i1, int i2)
        {
            if (fits.NAXIS == 2)
            {
                return clase_dato switch
                {
                    1 => fits.datosb[i1, i2],
                    2 => fits.datoss[i1, i2],
                    3 => fits.datosi[i1, i2],
                    4 => fits.datosf[i1, i2],
                    _ => fits.datosd[i1, i2],
                };
            }
            else
            {
                return clase_dato switch
                {
                    1 => fits.datosb3[i1, i2, i3],
                    2 => fits.datoss3[i1, i2, i3],
                    3 => fits.datosi3[i1, i2, i3],
                    4 => fits.datosf3[i1, i2, i3],
                    _ => fits.datosd3[i1, i2, i3],
                };
            }
        }
        private Rango Estadistica(double min_abs, double max_abs, double media, double dest, bool h)
        {
            // En el eje x del histograma se muestra en lugar del cero la anchura de las barras

            double min_acotado;
            double max_acotado;
            double fp;
            if (fits.v_acotar_min.Text.Trim().Length > 0)
            {
                if (fits.v_acotar_min.Text.Trim().IndexOf('%') != -1)
                {
                    if (!h)
                    {
                        MessageBox.Show(Idioma.msg[Idioma.lengua, 86], Idioma.msg[Idioma.lengua, 85], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        min_acotado = 0;
                        fits.v_acotar_min.Text = min_acotado.ToString();
                    }
                    else
                    {
                        string s = fits.v_acotar_min.Text.Trim().Replace('%', ' ').Trim();
                        min_acotado = Convert.ToDouble(s.Replace(fits.s_millar, fits.s_decimal)) / 100;
                        int suma = 0;
                        for (int i = 1; i < fits.num_histogramas; i++)
                        {
                            suma += fits.histograma[i];
                        }
                        int suma_max = (int)(suma * min_acotado);
                        min_acotado = 0;
                        suma = 0;
                        for (int i = 0; i < fits.num_histogramas; i++)
                        {
                            suma += fits.histograma[i];
                            if (suma >= suma_max)
                            {
                                min_acotado = i == 0 ? 0 : (i - 1) * ancho_valor_histograma;
                                break;
                            }
                        }
                        fits.v_acotar_min.Text = min_acotado.ToString();
                    }
                }
                else
                {
                    min_acotado = Convert.ToDouble(fits.v_acotar_min.Text.Trim().Replace(fits.s_millar, fits.s_decimal));
                }
            }
            else
            {
                min_acotado = min_abs;
            }
            if (fits.v_acotar_max.Text.Trim().Length > 0)
            {
                if (fits.v_acotar_max.Text.Trim().IndexOf('%') != -1)
                {
                    if (!h)
                    {
                        MessageBox.Show(Idioma.msg[Idioma.lengua, 86], Idioma.msg[Idioma.lengua, 87], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        max_acotado = 0;
                        fits.v_acotar_max.Text = min_acotado.ToString();
                    }
                    else
                    {
                        string s = fits.v_acotar_max.Text.Trim().Replace('%', ' ').Trim();
                        max_acotado = Convert.ToDouble(s.Replace(fits.s_millar, fits.s_decimal)) / 100;
                        int suma = 0;
                        for (int i = 1; i < fits.num_histogramas; i++)
                        {
                            suma += fits.histograma[i];
                        }
                        int suma_max = (int)(suma * max_acotado);
                        max_acotado = double.MaxValue;
                        suma = 0;
                        for (int i = 0; i < fits.num_histogramas; i++)
                        {
                            suma += fits.histograma[i];
                            if (suma >= suma_max)
                            {
                                max_acotado = i * ancho_valor_histograma;
                                break;
                            }
                        }
                        fits.v_acotar_max.Text = max_acotado.ToString();
                    }
                }
                else
                {
                    max_acotado = Convert.ToDouble(fits.v_acotar_max.Text.Trim().Replace(fits.s_millar, fits.s_decimal));
                }
            }
            else
            {
                max_acotado = max_abs;
            }

            // No se admiten negativos, la cantidad de color de un pixel es siempre positiva

            if (min_acotado < 0) min_acotado = 0;
            if (max_acotado < 0) max_acotado = min_acotado + 1;
            if (max_acotado <= min_acotado)
            {
                min_acotado = 0;
                max_acotado = max_abs;
                fp = 1;
            }
            else
            {
                if (fits.alta_resolucion.Checked)
                {
                    // La imagen es RGB48, es decir, los canales de color son de dos bytes pero sólo se usan 13 bits (0-8192)

                    fp = 8192 / (double)(max_acotado - min_acotado);
                }
                else
                {
                    // La imagen es RGB24, es decir, los canales de color son de un byte (0-255)

                    fp = byte.MaxValue / (double)(max_acotado - min_acotado);
                }
            }
            return new Rango(min_abs, max_abs, min_acotado, max_acotado, media, dest, fp);
        }
        private int RegistraPixelEnFlujo(int n, double val)
        {
            if (fits.alta_resolucion.Checked)
            {
                byte canal_byte_bajo;
                byte canal_byte_alto;
                if (double.IsNaN(val))
                {
                    flujo[n++] = color_nan.B;
                    flujo[n++] = (byte)(color_nan.B >> 3);
                    flujo[n++] = color_nan.G;
                    flujo[n++] = (byte)(color_nan.G >> 3);
                    flujo[n++] = color_nan.R;
                    flujo[n++] = (byte)(color_nan.R >> 3);
                    return n;
                }
                if (estadistica == null)
                {
                    if (val < 0)
                    {
                        flujo[n++] = color_cota_inf.B;
                        flujo[n++] = (byte)(color_cota_inf.B >> 3);
                        flujo[n++] = color_cota_inf.G;
                        flujo[n++] = (byte)(color_cota_inf.G >> 3);
                        flujo[n++] = color_cota_inf.R;
                        flujo[n++] = (byte)(color_cota_inf.R >> 3);
                        return n;
                    }
                    if (val > uint.MaxValue)
                    {
                        flujo[n++] = color_cota_sup.B;
                        flujo[n++] = (byte)(color_cota_sup.B >> 3);
                        flujo[n++] = color_cota_sup.G;
                        flujo[n++] = (byte)(color_cota_sup.G >> 3);
                        flujo[n++] = color_cota_sup.R;
                        flujo[n++] = (byte)(color_cota_sup.R >> 3);
                        return n;
                    }
                    if (fits.raiz.Checked) val = Math.Sqrt(val < 0 ? 0 : val);
                    canal_byte_bajo = (byte)((ushort)(val < 0 ? 0 : val) & 255);
                    canal_byte_alto = (byte)((ushort)(val < 0 ? 0 : val) >> 3);
                    flujo[n++] = canal_byte_bajo;
                    flujo[n++] = canal_byte_alto;
                    flujo[n++] = canal_byte_bajo;
                    flujo[n++] = canal_byte_alto;
                    flujo[n++] = canal_byte_bajo;
                    flujo[n++] = canal_byte_alto;
                    return n;
                }
                if (val < estadistica.min_acotado)
                {
                    flujo[n++] = color_cota_inf.B;
                    flujo[n++] = (byte)(color_cota_inf.B >> 3);
                    flujo[n++] = color_cota_inf.G;
                    flujo[n++] = (byte)(color_cota_inf.G >> 3);
                    flujo[n++] = color_cota_inf.R;
                    flujo[n++] = (byte)(color_cota_inf.R >> 3);
                    return n;
                }
                if (val > estadistica.max_acotado)
                {
                    flujo[n++] = color_cota_sup.B;
                    flujo[n++] = (byte)(color_cota_sup.B >> 3);
                    flujo[n++] = color_cota_sup.G;
                    flujo[n++] = (byte)(color_cota_sup.G >> 3);
                    flujo[n++] = color_cota_sup.R;
                    flujo[n++] = (byte)(color_cota_sup.R >> 3);
                    return n;
                }
                if (fits.raiz.Checked) val = Math.Sqrt(val < 0 ? 0 : val);
                ushort valus = (ushort)(((val < 0 ? 0 : val) - estadistica.min_acotado) * estadistica.fp);
                canal_byte_bajo = (byte)(valus & 255);
                canal_byte_alto = (byte)(valus >> 8);
                flujo[n++] = canal_byte_bajo;
                flujo[n++] = canal_byte_alto;
                flujo[n++] = canal_byte_bajo;
                flujo[n++] = canal_byte_alto;
                flujo[n++] = canal_byte_bajo;
                flujo[n++] = canal_byte_alto;
                return n;
            }

            // Resolución normal

            byte canal;
            if (double.IsNaN(val))
            {
                flujo[n++] = color_nan.B;
                flujo[n++] = color_nan.G;
                flujo[n++] = color_nan.R;
                return n;
            }
            if (estadistica == null)
            {
                if (val < 0)
                {
                    flujo[n++] = color_cota_inf.B;
                    flujo[n++] = color_cota_inf.G;
                    flujo[n++] = color_cota_inf.R;
                    return n;
                }
                if (val > 255)
                {
                    flujo[n++] = color_cota_sup.B;
                    flujo[n++] = color_cota_sup.G;
                    flujo[n++] = color_cota_sup.R;
                    return n;
                }
                if (fits.raiz.Checked) val = Math.Sqrt(val < 0 ? 0 : val);
                flujo[n++] = canal = (byte)(val < 0 ? 0 : val);
                flujo[n++] = canal;
                flujo[n++] = canal;
                return n;
            }
            if (val < estadistica.min_acotado)
            {
                flujo[n++] = color_cota_inf.B;
                flujo[n++] = color_cota_inf.G;
                flujo[n++] = color_cota_inf.R;
                return n;
            }
            if (val > estadistica.max_acotado)
            {
                flujo[n++] = color_cota_sup.B;
                flujo[n++] = color_cota_sup.G;
                flujo[n++] = color_cota_sup.R;
                return n;
            }
            if (fits.raiz.Checked) val = Math.Sqrt(val < 0 ? 0 : val);
            canal = (byte)(estadistica.fp * (val < 0 ? 0 : val - estadistica.min_acotado));

            // Blue, Green, Red

            flujo[n++] = canal;
            flujo[n++] = canal;
            flujo[n++] = canal;
            return n;
        }
        private void CreaImagen(Bitmap bitmap)
        {
            // Se crea por líneas ya que puede haber bytes de separación entre líneas en la imagen
            // por motivos de alineación en memora a multiplos de 4.

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr_img = data.Scan0;

            // 'data.Stride' = longitud de la línea en la imagen
            // Las líneas se almacenan en la imagen redondeadas al multiplo de 4 más próximo para mantener el
            // correcto alineamiento en memoria. El signo indica si la imagen se rellena de arriba-abajo o a la inversa

            int lon_linea_datos = data.Width * Image.GetPixelFormatSize(data.PixelFormat) / 8;
            for (int y = 0; y < data.Height; y++)
            {
                Marshal.Copy(flujo, y * lon_linea_datos, ptr_img, lon_linea_datos);
                ptr_img += data.Stride;
            }
            bitmap.UnlockBits(data);
        }
        private void ContadorLineas(int i2)
        {
            if (i2 % 1000 == 0)
            {
                v_brillo.Text = string.Format("{0:N0}", i2);
                Application.DoEvents();
            }
        }
        public void ExtraeFlujo(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr_img = data.Scan0;
            int nbytes = Math.Abs(data.Stride) * data.Height;
            flujo = new byte[nbytes];
            int bytesPorPixel = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            int lon_linea_datos = data.Width * Image.GetPixelFormatSize(data.PixelFormat) / 8;
            for (int y = 0; y < data.Height; y++)
            {
                Marshal.Copy(ptr_img, flujo, y * lon_linea_datos, lon_linea_datos);
                ptr_img += data.Stride;
            }
            bitmap.UnlockBits(data);

            // Crea la matriz de datos de tipo float

            fits.datosf = new float[data.Width, data.Height];
            float val;
            int puntero = 0;
            v_brillo.Text = string.Format("{0:N0}", 0);
            Application.DoEvents();
            for (int i2 = 0; i2 < data.Height; i2++)
            {
                ContadorLineas(i2);
                for (int i1 = 0; i1 < data.Width; i1++)
                {
                    val = 0;

                    // Hacer la media de los canales de color

                    for (int k = 0; k < bytesPorPixel; k++)
                    {
                        val += flujo[puntero++];
                    }
                    fits.datosf[i1, i2] = val / bytesPorPixel;
                }
            }
            v_brillo.Text = string.Empty;
        }

        public bool Dibuja(object[,] datos, bool h)
        {
            fits.Disponible(false);
            Geometria();
            CargaFlujo(datos, h);
            CreaImagen(fits.img);

            // Mostrar la imagen

            lienzo.Image = fits.img;
            fits.Disponible(true);
            return true;
        }
        private Rango NormalizaDatos(object[,] datos, bool h)
        {
            double val;
            double min_abs = byte.MaxValue;
            double max_abs = byte.MinValue;
            double media = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.tipoDato == 0)
                    {
                        val = fits.byPorDato switch
                        {
                            1 => (byte)datos[i1, i2],
                            2 => (int)datos[i1, i2],
                            _ => (int)datos[i1, i2],
                        };
                    }
                    else
                    {
                        val = fits.byPorDato switch
                        {
                            4 => (double)datos[i1, i2],
                            _ => (double)datos[i1, i2],
                        };
                    }
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(val);
                        if (double.IsNaN(val)) continue;
                    }
                    media += val;
                    if (max_abs < val) max_abs = val;
                    if (min_abs > val) min_abs = val;
                }
            }
            if (max_abs == 0)
            {
                // Para evitar divisiones por 0 en la construcción del histograma

                ancho_valor_histograma = 1;
            }
            else
            {
                ancho_valor_histograma = max_abs / (fits.num_histogramas - 1);
            }
            int n = fits.NAXISn[0] * fits.NAXISn[1];
            media /= n;
            double df;
            double dest = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.tipoDato == 0)
                    {
                        val = fits.byPorDato switch
                        {
                            1 => (byte)datos[i1, i2],
                            2 => (int)datos[i1, i2],
                            _ => (int)datos[i1, i2],
                        };
                    }
                    else
                    {
                        val = fits.byPorDato switch
                        {
                            4 => (double)datos[i1, i2],
                            _ => (double)datos[i1, i2],
                        };
                    }
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(val);
                        if (double.IsNaN(val)) continue;
                    }
                    df = val - media;
                    dest += df * df;
                    if (h)
                    {
                        if (val < 0)
                        {
                            fits.histograma[0]++;
                        }
                        else
                        {
                            fits.histograma[(int)(val / ancho_valor_histograma)]++;
                        }
                    }
                }
            }
            dest = Math.Sqrt(dest / n);
            return Estadistica(min_abs, max_abs, media, dest, h);
        }
        private void CargaFlujo(object[,] datos, bool h)
        {
            estadistica = null;
            if (fits.normalizar.Checked)
            {
                estadistica = NormalizaDatos(datos, h);
                if (h) fits.DibujaHistograma();
            }
            int n = 0;
            string s = v_brillo.Text;
            v_brillo.Text = string.Format("{0:N0}", n);
            Application.DoEvents();
            if (fits.invertir_y.Checked)
            {
                for (int i2 = fits.NAXISn[1] - 1; i2 >= 0; i2--)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            else
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            v_brillo.Text = s;
        }
        private void LeeFila(object[,] datos, int i2, ref int n)
        {
            double val;
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = 0; i1 < ancho_img_datos; i1++)
                {
                    if (fits.tipoDato == 0)
                    {
                        val = fits.byPorDato switch
                        {
                            1 => (double)datos[i1, i2],
                            2 => (double)datos[i1, i2],
                            _ => (double)datos[i1, i2],
                        };
                    }
                    else
                    {
                        val = fits.byPorDato switch
                        {
                            4 => (double)datos[i1, i2],
                            _ => (double)datos[i1, i2],
                        };
                    }
                    n = RegistraPixelEnFlujo(n, val);
                }
            }
            ContadorLineas(i2);
        }
        private void LeeFilaIx(object[,] datos, int i2, ref int n)
        {
            double val;
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = ancho_img_datos - 1; i1 >= 0; i1--)
                {
                    if (fits.tipoDato == 0)
                    {
                        val = fits.byPorDato switch
                        {
                            1 => (double)datos[i1, i2],
                            2 => (double)datos[i1, i2],
                            _ => (double)datos[i1, i2],
                        };
                    }
                    else
                    {
                        val = fits.byPorDato switch
                        {
                            4 => (double)datos[i1, i2],
                            _ => (double)datos[i1, i2],
                        };
                    }
                    n = RegistraPixelEnFlujo(n, val);
                }
            }
            ContadorLineas(i2);
        }

        public bool Dibuja(byte[,] datos, bool h)
        {
            fits.Disponible(false);
            Geometria();
            CargaFlujo(datos, h);
            CreaImagen(fits.img);

            // Mostrar la imagen

            lienzo.Image = fits.img;
            fits.Disponible(true);
            return true;
        }
        private Rango NormalizaDatos(byte[,] datos, bool h)
        {
            double val;
            double min_abs = double.MaxValue;
            double max_abs = double.MinValue;
            double media = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    media += val;
                    if (max_abs < val) max_abs = val;
                    if (min_abs > val) min_abs = val;
                }
            }
            if (max_abs == 0)
            {
                // Para evitar divisiones por 0 en la construcción del histograma

                ancho_valor_histograma = 1;
            }
            else
            {
                ancho_valor_histograma = max_abs / (fits.num_histogramas - 1);
            }
            int n = fits.NAXISn[0] * fits.NAXISn[1];
            media /= n;
            double df;
            double dest = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (double.IsNaN(datos[i1, i2])) continue;
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    df = val - media;
                    dest += df * df;
                    if (h)
                    {
                        if (val < 0)
                        {
                            fits.histograma[0]++;
                        }
                        else
                        {
                            fits.histograma[(int)(val / ancho_valor_histograma)]++;
                        }
                    }
                }
            }
            dest = Math.Sqrt(dest / n);
            return Estadistica(min_abs, max_abs, media, dest, h);
        }
        private void CargaFlujo(byte[,] datos, bool h)
        {
            estadistica = null;
            if (fits.normalizar.Checked)
            {
                estadistica = NormalizaDatos(datos, h);
                if (h) fits.DibujaHistograma();
            }
            int n = 0;
            string s = v_brillo.Text;
            v_brillo.Text = string.Format("{0:N0}", n);
            Application.DoEvents();
            if (fits.invertir_y.Checked)
            {
                for (int i2 = fits.NAXISn[1] - 1; i2 >= 0; i2--)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            else
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            v_brillo.Text = s;
        }
        private void LeeFila(byte[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = 0; i1 < ancho_img_datos; i1++)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }
        private void LeeFilaIx(byte[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = ancho_img_datos - 1; i1 >= 0; i1--)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }

        public bool Dibuja(short[,] datos, bool h)
        {
            fits.Disponible(false);
            Geometria();
            CargaFlujo(datos, h);
            CreaImagen(fits.img);

            // Mostrar la imagen

            lienzo.Image = fits.img;
            fits.Disponible(true);
            return true;
        }
        private Rango NormalizaDatos(short[,] datos, bool h)
        {
            double val;
            double min_abs = double.MaxValue;
            double max_abs = double.MinValue;
            double media = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    media += val;
                    if (max_abs < val) max_abs = val;
                    if (min_abs > val) min_abs = val;
                }
            }
            if (max_abs == 0)
            {
                // Para evitar divisiones por 0 en la construcción del histograma

                ancho_valor_histograma = 1;
            }
            else
            {
                ancho_valor_histograma = max_abs / (fits.num_histogramas - 1);
            }
            int n = fits.NAXISn[0] * fits.NAXISn[1];
            media /= n;
            double df;
            double dest = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (double.IsNaN(datos[i1, i2])) continue;
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    df = val - media;
                    dest += df * df;
                    if (h)
                    {
                        if (val < 0)
                        {
                            fits.histograma[0]++;
                        }
                        else
                        {
                            fits.histograma[(int)(val / ancho_valor_histograma)]++;
                        }
                    }
                }
            }
            dest = Math.Sqrt(dest / n);
            return Estadistica(min_abs, max_abs, media, dest, h);
        }
        private void CargaFlujo(short[,] datos, bool h)
        {
            estadistica = null;
            if (fits.normalizar.Checked)
            {
                estadistica = NormalizaDatos(datos, h);
                if (h) fits.DibujaHistograma();
            }
            int n = 0;
            string s = v_brillo.Text;
            v_brillo.Text = string.Format("{0:N0}", n);
            Application.DoEvents();
            if (fits.invertir_y.Checked)
            {
                for (int i2 = fits.NAXISn[1] - 1; i2 >= 0; i2--)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            else
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            v_brillo.Text = s;
        }
        private void LeeFila(short[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = 0; i1 < ancho_img_datos; i1++)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }
        private void LeeFilaIx(short[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = ancho_img_datos - 1; i1 >= 0; i1--)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }

        public bool Dibuja(int[,] datos, bool h)
        {
            fits.Disponible(false);
            Geometria();
            CargaFlujo(datos, h);
            CreaImagen(fits.img);

            // Mostrar la imagen

            lienzo.Image = fits.img;
            fits.Disponible(true);
            return true;
        }
        private Rango NormalizaDatos(int[,] datos, bool h)
        {
            double val;
            double min_abs = double.MaxValue;
            double max_abs = double.MinValue;
            double media = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    media += val;
                    if (max_abs < val) max_abs = val;
                    if (min_abs > val) min_abs = val;
                }
            }
            if (max_abs == 0)
            {
                // Para evitar divisiones por 0 en la construcción del histograma

                ancho_valor_histograma = 1;
            }
            else
            {
                ancho_valor_histograma = max_abs / (fits.num_histogramas - 1);
            }
            int n = fits.NAXISn[0] * fits.NAXISn[1];
            media /= n;
            double df;
            double dest = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (double.IsNaN(datos[i1, i2])) continue;
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    df = val - media;
                    dest += df * df;
                    if (h)
                    {
                        if (val < 0)
                        {
                            fits.histograma[0]++;
                        }
                        else
                        {
                            fits.histograma[(int)(val / ancho_valor_histograma)]++;
                        }
                    }
                }
            }
            dest = Math.Sqrt(dest / n);
            return Estadistica(min_abs, max_abs, media, dest, h);
        }
        private void CargaFlujo(int[,] datos, bool h)
        {
            estadistica = null;
            if (fits.normalizar.Checked)
            {
                estadistica = NormalizaDatos(datos, h);
                if (h) fits.DibujaHistograma();
            }
            int n = 0;
            string s = v_brillo.Text;
            v_brillo.Text = string.Format("{0:N0}", n);
            Application.DoEvents();
            if (fits.invertir_y.Checked)
            {
                for (int i2 = fits.NAXISn[1] - 1; i2 >= 0; i2--)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            else
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            v_brillo.Text = s;
        }
        private void LeeFila(int[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = 0; i1 < ancho_img_datos; i1++)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }
        private void LeeFilaIx(int[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = ancho_img_datos - 1; i1 >= 0; i1--)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }

        public bool Dibuja(float[,] datos, bool h)
        {
            fits.Disponible(false);
            Geometria();
            CargaFlujo(datos, h);
            CreaImagen(fits.img);

            // Mostrar la imagen

            lienzo.Image = fits.img;
            fits.Disponible(true);
            return true;
        }
        private Rango NormalizaDatos(float[,] datos, bool h)
        {
            double val;
            double min_abs = double.MaxValue;
            double max_abs = double.MinValue;
            double media = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (float.IsNaN(datos[i1, i2])) continue;
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    media += val;
                    if (max_abs < val) max_abs = val;
                    if (min_abs > val) min_abs = val;
                }
            }
            if (max_abs == 0)
            {
                // Para evitar divisiones por 0 en la construcción del histograma

                ancho_valor_histograma = 1;
            }
            else
            {
                ancho_valor_histograma = max_abs / (fits.num_histogramas - 1);
            }
            int n = fits.NAXISn[0] * fits.NAXISn[1];
            media /= n;
            double df;
            double dest = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (float.IsNaN(datos[i1, i2])) continue;
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    df = val - media;
                    dest += df * df;
                    if (h)
                    {
                        if (val < 0)
                        {
                            fits.histograma[0]++;
                        }
                        else
                        {
                            fits.histograma[(int)(val / ancho_valor_histograma)]++;
                        }
                    }
                }
            }
            dest = Math.Sqrt(dest / n);
            return Estadistica(min_abs, max_abs, media, dest, h);
        }
        private void CargaFlujo(float[,] datos, bool h)
        {
            estadistica = null;
            if (fits.normalizar.Checked)
            {
                estadistica = NormalizaDatos(datos, h);
                if (h) fits.DibujaHistograma();
            }
            int n = 0;
            string s = v_brillo.Text;
            v_brillo.Text = string.Format("{0:N0}", n);
            Application.DoEvents();

            /*int pases = multiplicador_y == 1 ? fits.NAXISn[1] / 3 : fits.NAXISn[1];
            for (int i2 = 0; i2 < pases; i2++)
            {
                Prueba(ref n);
            }*/

            if (fits.invertir_y.Checked)
            {
                for (int i2 = fits.NAXISn[1] - 1; i2 >= 0; i2--)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            else
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            v_brillo.Text = s;
        }
        private void LeeFila(float[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = 0; i1 < ancho_img_datos; i1++)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }
        private void LeeFilaIx(float[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = ancho_img_datos - 1; i1 >= 0; i1--)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }

        public bool Dibuja(double[,] datos, bool h)
        {
            fits.Disponible(false);
            Geometria();
            CargaFlujo(datos, h);
            CreaImagen(fits.img);

            // Mostrar la imagen

            lienzo.Image = fits.img;
            fits.Disponible(true);
            return true;
        }
        private Rango NormalizaDatos(double[,] datos, bool h)
        {
            double val;
            double min_abs = double.MaxValue;
            double max_abs = double.MinValue;
            double media = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (double.IsNaN(datos[i1, i2])) continue;
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    media += val;
                    if (max_abs < val) max_abs = val;
                    if (min_abs > val) min_abs = val;
                }
            }
            if (max_abs == 0)
            {
                // Para evitar divisiones por 0 en la construcción del histograma

                ancho_valor_histograma = 1;
            }
            else
            {
                ancho_valor_histograma = max_abs / (fits.num_histogramas - 1);
            }
            int n = fits.NAXISn[0] * fits.NAXISn[1];
            media /= n;
            double df;
            double dest = 0;
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (double.IsNaN(datos[i1, i2])) continue;
                    if (fits.raiz.Checked)
                    {
                        val = Math.Sqrt(datos[i1, i2]);
                        if (double.IsNaN(val)) continue;
                    }
                    else
                    {
                        val = datos[i1, i2];
                    }
                    df = val - media;
                    dest += df * df;
                    if (h)
                    {
                        if (val < 0)
                        {
                            fits.histograma[0]++;
                        }
                        else
                        {
                            fits.histograma[(int)(val / ancho_valor_histograma)]++;
                        }
                    }
                }
            }
            dest = Math.Sqrt(dest / n);
            return Estadistica(min_abs, max_abs, media, dest, h);
        }
        private void CargaFlujo(double[,] datos, bool h)
        {
            estadistica = null;
            if (fits.normalizar.Checked)
            {
                estadistica = NormalizaDatos(datos, h);
                if (h) fits.DibujaHistograma();
            }
            int n = 0;
            string s = v_brillo.Text;
            v_brillo.Text = string.Format("{0:N0}", n);
            Application.DoEvents();
            if (fits.invertir_y.Checked)
            {
                for (int i2 = fits.NAXISn[1] - 1; i2 >= 0; i2--)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            else
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    if (fits.invertir_x.Checked)
                    {
                        LeeFilaIx(datos, i2, ref n);
                    }
                    else
                    {
                        LeeFila(datos, i2, ref n);
                    }
                }
            }
            v_brillo.Text = s;
        }
        private void LeeFila(double[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = 0; i1 < ancho_img_datos; i1++)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }
        private void LeeFilaIx(double[,] datos, int i2, ref int n)
        {
            for (int i0 = 0; i0 < multiplicador_y; i0++)
            {
                for (int i1 = ancho_img_datos - 1; i1 >= 0; i1--)
                {
                    n = RegistraPixelEnFlujo(n, datos[i1, i2]);
                }
            }
            ContadorLineas(i2);
        }

        // Tres dimensiones

        public bool Dibuja(byte[,,] datos, bool h, int i3)
        {
            byte[,] datos2 = new byte[fits.NAXISn[0], fits.NAXISn[1]];
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i1 < fits.NAXISn[1]; i2++)
                {
                    datos2[i1, i2] = datos[i1, i2, i3];
                }
            }
            Dibuja(datos2, h);
            return true;
        }
        public bool Dibuja(short[,,] datos, bool h, int i3)
        {
            short[,] datos2 = new short[fits.NAXISn[0], fits.NAXISn[1]];
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    datos2[i1, i2] = datos[i1, i2, i3];
                }
            }
            Dibuja(datos2, h);
            return true;
        }
        public bool Dibuja(int[,,] datos, bool h, int i3)
        {
            int[,] datos2 = new int[fits.NAXISn[0], fits.NAXISn[1]];
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    datos2[i1, i2] = datos[i1, i2, i3];
                }
            }
            Dibuja(datos2, h);
            return true;
        }
        public bool Dibuja(float[,,] datos, bool h, int i3)
        {
            float[,] datos2 = new float[fits.NAXISn[0], fits.NAXISn[1]];
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    datos2[i1, i2] = datos[i1, i2, i3];
                }
            }
            Dibuja(datos2, h);
            return true;
        }
        public bool Dibuja(double[,,] datos, bool h, int i3)
        {
            double[,] datos2 = new double[fits.NAXISn[0], fits.NAXISn[1]];
            for (int i1 = 0; i1 < fits.NAXISn[0]; i1++)
            {
                for (int i2 = 0; i2 < fits.NAXISn[1]; i2++)
                {
                    datos2[i1, i2] = datos[i1, i2, i3];
                }
            }
            Dibuja(datos2, h);
            return true;
        }

        public void Redibuja()
        {
            if (fits.es_espectro)
            {
                fits.DibujaEspectro();
                return;
            }
            Array.Clear(fits.histograma, 0, fits.histograma.Length);
            if (fits.NAXIS == 2)
            {
                switch (clase_dato)
                {
                    case 1:
                        Dibuja(fits.datosb, true);
                        break;
                    case 2:
                        Dibuja(fits.datoss, true);
                        break;
                    case 3:
                        Dibuja(fits.datosi, true);
                        break;
                    case 4:
                        Dibuja(fits.datosf, true);
                        break;
                    default:
                        Dibuja(fits.datosd, true);
                        break;
                }
            }
        }
        public void Redibuja(int i3)
        {
            if (fits.es_espectro)
            {
                fits.DibujaEspectro();
                return;
            }
            Array.Clear(fits.histograma, 0, fits.histograma.Length);
            if (fits.NAXIS == 3)
            {
                switch (clase_dato)
                {
                    case 1:
                        Dibuja(fits.datosb3, true, i3);
                        break;
                    case 2:
                        Dibuja(fits.datoss3, true, i3);
                        break;
                    case 3:
                        Dibuja(fits.datosi3, true, i3);
                        break;
                    case 4:
                        Dibuja(fits.datosf3, true, i3);
                        break;
                    default:
                        Dibuja(fits.datosd3, true, i3);
                        break;
                }
            }
        }

        private RegLineal RegresionLineal(double[] x, double[] y, int n)
        {
            double a;
            double m;
            double sumax = 0;
            double sumay = 0;
            double sumx2 = 0;
            double sumay2 = 0;
            double sumaxy = 0;
            double xi;
            double yi;
            for (var i = 0; i < n; i++)
            {
                xi = x[i];
                yi = y[i];
                sumaxy += xi * yi;
                sumax += xi;
                sumay += yi;
                sumx2 += xi * xi;
                sumay2 += yi * yi;
            }
            double ssx = sumx2 - (sumax * sumax / n);
            double sco = sumaxy - ((sumax * sumay) / n);
            double mediax = sumax / n;
            double mediay = sumay / n;
            m = sco / ssx;
            a = mediay - (m * mediax);

            //double rNumerator = (n * sumaxy) - (sumax * sumay);
            //double rDenom = (n * sumx2 - (sumax * sumax)) * (n * sumay2 - (sumay * sumay));
            //double r = rNumerator / Math.Sqrt(rDenom);
            //double r2 = r * r;

            return new RegLineal(a, m);
        }

        private void Lineas_elegibles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lista_elegibles.SelectedItem != null && lista_elegibles.SelectedIndex >= 0) lista_elegidas.Items.Add(lista_elegibles.SelectedItem);
        }
        private void Lineas_elegidas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lista_elegidas.SelectedItem != null && lista_elegidas.SelectedIndex >= 0) lista_elegidas.Items.RemoveAt(lista_elegidas.SelectedIndex);
        }
        private void B_picos_Click(object sender, EventArgs e)
        {
            bool ctrl = ModifierKeys.HasFlag(Keys.Control);
            fits.BuscaPicos();
            if (picos.Count > 0)
            {
                if (ctrl)
                {
                    foreach (Pico p in picos)
                    {
                        double xm = fits.es_actual.x[p.indice];
                        AddLineaAtomica(xm);
                    }
                }
                if (fits.NAXIS == 3) Redibuja(i3);
                else Redibuja();
            }
        }
        private void B_limpiar_Click(object sender, EventArgs e)
        {
            if (picos != null) picos.Clear();
            lista_elegidas.Items.Clear();
            if (fits.NAXIS == 3) Redibuja(i3);
            else Redibuja();
        }
        private void AddLineaAtomica(double v)
        {
            string linea;
            double xm;
            double xma = -1;
            for (int i = 0; i < lista_elegibles.Items.Count; i++)
            {
                linea = lista_elegibles.Items[i].ToString();
                xm = Convert.ToDouble(linea.Substring(0, 10).Trim());
                if (xm > v)
                {
                    if (Math.Abs(xm - v) < Math.Abs(xma - v))
                    {
                        lista_elegidas.Items.Add(linea);
                    }
                    else
                    {
                        lista_elegidas.Items.Add(lista_elegibles.Items[i - 1].ToString());
                    }
                    break;
                }
                xma = xm;
            }
        }
    }
}

