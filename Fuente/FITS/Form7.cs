using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExploraFits
{
    public partial class Form7 : Form
    {
        public int nf;
        public Form7()
        {
            InitializeComponent();
        }

        private void b_esc_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Dispose();
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            nf = Convert.ToInt32(fracciones.Text);
            DialogResult = DialogResult.OK;
            Dispose();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            nf = Convert.ToInt32(fracciones.Text);
        }
    }
}
