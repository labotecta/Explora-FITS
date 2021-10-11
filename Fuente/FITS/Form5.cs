using ExploraFITS;
using System;
using System.Text;
using System.Windows.Forms;

namespace ExploraFits
{
    public partial class Form5 : Form
    {
        public FITS fits;

        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            CambiaIdioma();
        }
        private void CambiaIdioma()
        {
            b_portapapeles.Text = Idioma.msg[Idioma.lengua, 26];
            r1.Text = Idioma.msg[Idioma.lengua, 30];
            r2.Text = Idioma.msg[Idioma.lengua, 31];
            r3.Text = Idioma.msg[Idioma.lengua, 32];
            r4.Text = Idioma.msg[Idioma.lengua, 33];
            r5.Text = Idioma.msg[Idioma.lengua, 34];
            r6.Text = Idioma.msg[Idioma.lengua, 35];
            r6.Text = Idioma.msg[Idioma.lengua, 36];
            r8.Text = Idioma.msg[Idioma.lengua, 37];
            r9.Text = Idioma.msg[Idioma.lengua, 38];
            r10.Text = Idioma.msg[Idioma.lengua, 39];
            r11.Text = Idioma.msg[Idioma.lengua, 40];
            r12.Text = Idioma.msg[Idioma.lengua, 41];
            r13.Text = Idioma.msg[Idioma.lengua, 42];
            r14.Text = Idioma.msg[Idioma.lengua, 43];
            r15.Text = Idioma.msg[Idioma.lengua, 44];
            r16.Text = Idioma.msg[Idioma.lengua, 45];
            r17.Text = Idioma.msg[Idioma.lengua, 46];
            r18.Text = Idioma.msg[Idioma.lengua, 47];
            r19.Text = Idioma.msg[Idioma.lengua, 48];
            r20.Text = Idioma.msg[Idioma.lengua, 49];
            r21.Text = Idioma.msg[Idioma.lengua, 50];
            r22.Text = Idioma.msg[Idioma.lengua, 51];
            r23.Text = Idioma.msg[Idioma.lengua, 52];
        }
        public void Datos(string[] datos)
        {
            v1.Text = datos[0];
            v2.Text = datos[1];
            v3.Text = datos[22];
            v4.Text = datos[23];
            v5.Text = datos[24];
            v6.Text = datos[25];
            v7.Text = datos[26];
            v8.Text = datos[28];
            v9.Text = datos[29];
            v10.Text = datos[31];
            v11.Text = datos[32];
            v12.Text = datos[33];
            v13.Text = datos[34];
            v14.Text = datos[35];
            v15.Text = datos[36];
            v16.Text = datos[37];
            v17.Text = datos[38];
            v18.Text = datos[39];
            v19.Text = datos[41];
            v20.Text = datos[55];
            v21.Text = datos[49];
            v22.Text = datos[56];
            v23.Text = datos[54];
            r2.Visible = !string.IsNullOrEmpty(v2.Text.Trim());
        }
        private void B_portapapeles_Click(object sender, System.EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0};{1}\n", r1.Text, v1.Text);
            sb.AppendFormat("{0};{1}\n", r2.Text, v2.Text);
            sb.AppendFormat("{0};{1}\n", r3.Text, v3.Text);
            sb.AppendFormat("{0};{1}\n", r4.Text, v4.Text);
            sb.AppendFormat("{0};{1}\n", r5.Text, v5.Text);
            sb.AppendFormat("{0};{1}\n", r6.Text, v6.Text);
            sb.AppendFormat("{0};{1}\n", r7.Text, v7.Text);
            sb.AppendFormat("{0};{1}\n", r8.Text, v8.Text);
            sb.AppendFormat("{0};{1}\n", r9.Text, v9.Text);
            sb.AppendFormat("{0};{1}\n", r10.Text, v10.Text);
            sb.AppendFormat("{0};{1}\n", r11.Text, v11.Text);
            sb.AppendFormat("{0};{1}\n", r12.Text, v12.Text);
            sb.AppendFormat("{0};{1}\n", r13.Text, v13.Text);
            sb.AppendFormat("{0};{1}\n", r14.Text, v14.Text);
            sb.AppendFormat("{0};{1}\n", r15.Text, v15.Text);
            sb.AppendFormat("{0};{1}\n", r16.Text, v16.Text);
            sb.AppendFormat("{0};{1}\n", r17.Text, v17.Text);
            sb.AppendFormat("{0};{1}\n", r18.Text, v18.Text);
            sb.AppendFormat("{0};{1}\n", r19.Text, v19.Text);
            sb.AppendFormat("{0};{1}\n", r20.Text, v20.Text);
            sb.AppendFormat("{0};{1}\n", r21.Text, v21.Text);
            sb.AppendFormat("{0};{1}\n", r22.Text, v22.Text);
            sb.AppendFormat("{0};{1}\n", r23.Text, v23.Text);
            Clipboard.SetText(sb.ToString());
            Console.Beep();
        }
    }
}
