using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PII_Clase1.dominio
{
    class Presupuesto
    {
        public int PresupuestoNro { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public double CostoMO { get; set; }
        public double Descuento { get; set; }
        public DateTime FechaBaja { get; set; }

        public List<DetallePresupuesto> Detalles { get; }

        public Presupuesto()
        {
            // Generar una relación 1 a muchos entre detallepresupuesto y presupuesto
            Detalles = new List<DetallePresupuesto>();
        }
        public void AgregarDetalle(DetallePresupuesto detalle)
        {
            Detalles.Add(detalle);
        }

        public void QuitarDetalle(int indice) {

            Detalles.RemoveAt(indice);
        }

        public double CalcularTotal() {

            double subtotal = 0;

            foreach (DetallePresupuesto detalle in Detalles) {
                
                subtotal = subtotal + detalle.CalcularSubtotal();
             
            }

           
            return subtotal;

        } 
    }
}
