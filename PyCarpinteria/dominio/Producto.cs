using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PII_Clase1.dominio
{
    class Producto
    {
        // Cuando compila genera el atributo: private int _IdProducto
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public bool Activo { get; set; }

        public Producto()
        {

        }

        public override string ToString()
        {
            return IdProducto.ToString() + "-" + Nombre;
        }

    }
}
