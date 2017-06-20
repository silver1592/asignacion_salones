using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo02.Clases
{
    public class Materia
    {
        protected string nombre;
        protected string cve;
        protected int area;

        public string Nombre { get { return nombre; } }
        public string CVE { get { return cve; } }
        public int Area { get { return area; } }

        public Materia(DataRow r)
        {
            cve = r["cve_materia"].ToString();
            nombre = r["materia"].ToString();
            area = Convert.ToInt32(r["cve_area"].ToString());
        }

        public Materia(string nombre, string cve, int area)
        {
            this.nombre = nombre;
            this.cve = cve;
            this.area = area;
        }
    }
}
