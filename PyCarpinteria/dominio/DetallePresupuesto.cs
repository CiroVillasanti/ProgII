using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PII_Clase1.dominio
{
    class DetallePresupuesto
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }

        public DetallePresupuesto()
        {
            Producto = new Producto();
            Cantidad = 0;
        }

        public double CalcularSubtotal()
        {
            return Producto.Precio * Cantidad;
        }
    }
}
