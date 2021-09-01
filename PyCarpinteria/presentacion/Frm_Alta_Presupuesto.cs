using PII_Clase1.dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PII_Clase1
{
    public partial class Frm_Alta_Presupuesto : Form
    {

        private Presupuesto oPresupuesto;
        public Frm_Alta_Presupuesto()
        {
            InitializeComponent();
        }

        private void Frm_Alta_Presupuesto_Load(object sender, EventArgs e) 
        {
            SqlConnection cnn = new SqlConnection(@"Data Source = localhost; Initial Catalog = carpinteria_db; Integrated Security = True");
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_PROXIMO_ID";

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@next";
            param.SqlDbType = SqlDbType.Int;
            param.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();

            lblNumero.Text = param.Value.ToString();

            SqlCommand cmd2 = new SqlCommand("SP_CONSULTAR_PRODUCTOS", cnn);

            DataTable table = new DataTable();
            table.Load(cmd2.ExecuteReader());

            cnn.Close();

            cboProductos.DataSource = table;
            cboProductos.DisplayMember = table.Columns[1].ColumnName;
            cboProductos.ValueMember = table.Columns[0].ColumnName;
            cboProductos.DropDownStyle = ComboBoxStyle.DropDownList;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDto.Text = "0";
            txtCliente.Text = "Consumidor final";

            oPresupuesto = new Presupuesto();


        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Desea agregar los productos?", "Confirmación", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                DetallePresupuesto item = new DetallePresupuesto();
                item.Cantidad = (int)nudCantidad.Value;
                DataRowView oDataRow = (DataRowView)cboProductos.SelectedItem;

                Producto oProducto = new Producto();
                oProducto.IdProducto = Int32.Parse(oDataRow[0].ToString());
                oProducto.Nombre = oDataRow[1].ToString();
                oProducto.Precio = Double.Parse(oDataRow[2].ToString());

                item.Producto = oProducto;

                oPresupuesto.AgregarDetalle(item);

                if (ExisteProductoEnGrilla(oProducto.Nombre))
                {
                    MessageBox.Show("No se puede insertar un producto existente", "Error de validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else 
                {
                    dgvDetalle.Rows.Add(new object[] { oProducto.IdProducto, oProducto.Nombre, oProducto.Precio, item.Cantidad, item.CalcularSubtotal(), });
                    CalcularTotales();
                }
             }
        }

        private void CalcularTotales() {

            double subTotal = oPresupuesto.CalcularTotal();
            double desc = (Double.Parse(txtDto.Text)) * subTotal / 100;
            double total = subTotal - desc;
            lblSubtotal.Text = "Subtotal: " + subTotal.ToString();
            lblDto.Text = "Descuento: " + desc.ToString();
            lblTotal.Text = "Total: " + total.ToString();
    
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalle.CurrentCell.ColumnIndex == 5) {

                oPresupuesto.QuitarDetalle(dgvDetalle.CurrentRow.Index);
                dgvDetalle.Rows.Remove(dgvDetalle.CurrentRow);
                CalcularTotales();
            
            }
        }

        private bool ExisteProductoEnGrilla(string producto) {

            foreach (DataGridViewRow fila in dgvDetalle.Rows) {

                if (fila.Cells[1].Value.ToString().Equals(producto)) {

                    return true;
                }
            }
            return false;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            double subTotal = oPresupuesto.CalcularTotal();
            double desc = (Double.Parse(txtDto.Text)) * subTotal / 100;
            double total = subTotal - desc;

            SqlTransaction newt = null; 

            SqlConnection cnn1 = new SqlConnection(@"Data Source = localhost; Initial Catalog = carpinteria_db; Integrated Security = True");
            cnn1.Open();
            newt = cnn1.BeginTransaction();
            SqlCommand cmd1 = new SqlCommand();
            cmd1.Connection = cnn1;
            cmd1.Transaction = newt;
            cmd1.CommandType = CommandType.StoredProcedure;

            cmd1.CommandText = "SP_INSERTAR_MAESTRO";

            SqlParameter paramMaestro = new SqlParameter();
            SqlParameter paramMaestro1 = new SqlParameter();
            SqlParameter paramMaestro2 = new SqlParameter();
            SqlParameter paramMaestro3 = new SqlParameter();

            paramMaestro.ParameterName = "@cliente";
            paramMaestro.SqlDbType = SqlDbType.VarChar;
            paramMaestro.Direction = ParameterDirection.Input;
            paramMaestro.Value = txtCliente.Text;

            paramMaestro1.ParameterName = "@dto";
            paramMaestro1.SqlDbType = SqlDbType.Float;
            paramMaestro1.Direction = ParameterDirection.Input;
            paramMaestro1.Value = desc;

            paramMaestro2.ParameterName = "@total";
            paramMaestro2.SqlDbType = SqlDbType.Float;
            paramMaestro2.Direction = ParameterDirection.Input;
            paramMaestro2.Value = total;

            paramMaestro3.ParameterName = "@presupuesto_nro"; 
            paramMaestro3.SqlDbType = SqlDbType.Int;
            paramMaestro3.Direction = ParameterDirection.Output;

            cmd1.Parameters.Add(paramMaestro);

            cmd1.Parameters.Add(paramMaestro2);

            cmd1.Parameters.Add(paramMaestro3);

            cmd1.Parameters.Add(paramMaestro1);

            
            cmd1.ExecuteNonQuery();
            newt.Commit();
            cnn1.Close();


            int nroPresupuesto = (int)paramMaestro3.Value;
            int numDetalle = 1;

            SqlConnection cnn = new SqlConnection(@"Data Source = localhost; Initial Catalog = carpinteria_db; Integrated Security = True");
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
               
                cmd.CommandText = "SP_INSERTAR_DETALLE";

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@presupuesto_nro";
                param.SqlDbType = SqlDbType.Int;
                param.Direction = ParameterDirection.Input;

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@detalle";
                param2.SqlDbType = SqlDbType.Int;
                param2.Direction = ParameterDirection.Input;

                SqlParameter param3 = new SqlParameter();
                param3.ParameterName = "@id_producto";
                param3.SqlDbType = SqlDbType.Int;
                param3.Direction = ParameterDirection.Input;

                SqlParameter param4 = new SqlParameter();
                param4.ParameterName = "@cantidad";
                param4.SqlDbType = SqlDbType.Int;
                param4.Direction = ParameterDirection.Input;

                param.Value = nroPresupuesto;
                param2.Value = numDetalle;
                numDetalle++;

                param3.Value = Convert.ToInt32(fila.Cells[0].Value.ToString());

                param4.Value = Convert.ToInt32(fila.Cells[3].Value.ToString());

                cmd.Parameters.Add(param);

                cmd.Parameters.Add(param2);

                cmd.Parameters.Add(param3);

                cmd.Parameters.Add(param4);

                cmd.ExecuteNonQuery();

            }

            cnn.Close();

        }


    }

    
}
