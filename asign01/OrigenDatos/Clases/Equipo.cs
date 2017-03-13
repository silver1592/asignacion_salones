using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrigenDatos.Clases
{
    class Equipo
    {
        public int Id { get { return id; } }
        private int id;

        private string descripcion;
        public string Descripcion { get { return descripcion; } }

        public Equipo(int id, string descripcion)
        {
            this.id = id;
            this.descripcion = descripcion;
        }

        public override string ToString()
        {
            return id + ".-" + descripcion;
        }

        public static List<Equipo> ToEquipo(DataTable datos, string idCol = "cve_equipo",string descripcioCol="equipo")
        {
            List<Equipo> lista = new List<Equipo>();
            int _id;
            string _descripcion;

            foreach (DataRow r in datos.Rows)
            {
                _id = Convert.ToInt32(r[idCol].ToString());
                _descripcion = r[descripcioCol].ToString();
                lista.Add(new Equipo(_id, _descripcion));
            }

            return lista;
        }
    }
}
