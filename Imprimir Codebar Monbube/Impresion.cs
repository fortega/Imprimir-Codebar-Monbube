using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imprimir_Codebar_Monbube
{
    class Impresion
    {

        public string Codebar { get; set; }
        public int Num { get; set; }
        public string SKU { get; set; }
        public string NombreProducto { get; set; }
        public int ValorNeto { get; set; }
        public string Variante { get; set; }

        public Impresion(string codebar, int n)
            : this(codebar, n, null)
        {
        }

        public Impresion(string codebar, int n, string desc)
            : this(codebar, n, desc, null, 0, null)
        {
        }

        public Impresion(string codebar, int n, string desc, string NombreProducto, int ValorNeto, string Variante)
        {
            this.Codebar = codebar;
            this.Num = n;
            this.SKU = desc;
            this.NombreProducto = NombreProducto;
            this.ValorNeto = ValorNeto;
            this.Variante = Variante;
        }

        public static explicit operator Impresion(string t)
        {
            string[] v = t.Split(Properties.Settings.Default.ImpresionSep);

            if (v.Length == 2)
            {
                return new Impresion(v[0], int.Parse(v[1]));
            }
            else if (v.Length == 3)
            {
                return new Impresion(v[0], int.Parse(v[1]), v[2]);
            }

            throw new Exception("Mal formato: " + t);
        }
    }
}
