using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Models
{
    public class Profesor
    {
        private int rpe;
        private string titulo;
        private string nombre;

        public int RPE { get { return rpe; } }
        public string Titulo { get { return titulo; } }
        public string Nombre { get { return nombre; } }

        public Profesor(DataRow r)
        {
            rpe = Convert.ToInt32(r["rpe"].ToString());
            titulo = r["titulo"].ToString();
            nombre = r["nombre"].ToString();
        }

        public Profesor(int _rpe)
        {
            rpe = _rpe;
            titulo = "";
            nombre = "--------------------";
        }
    }
}