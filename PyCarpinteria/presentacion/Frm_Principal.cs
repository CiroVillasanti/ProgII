using PyCarpinteria.presentacion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PII_Clase1
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void nuevoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Frm_Alta_Presupuesto frmNuevo = new Frm_Alta_Presupuesto();
            frmNuevo.ShowDialog();
        }

        private void consultarPresupuestosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConsultarPresupuestos frmConsultarPresupuesto = new FormConsultarPresupuestos();
            frmConsultarPresupuesto.ShowDialog();
        }
    }
}
