using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Models
{
    public class Salon
    {
        public Algoritmo02.Clases.ListaVariables grupos;
        private OrigenDatos.Clases.Salon salon;
        private string cve_espacio;

        public string Cve_espacio { get { return salon != null ? salon.Cve_espacio : cve_espacio; } }
        public string Cupo { get { return salon != null ? salon.Cupo.ToString() : "Desconocido"; } }
        public bool Asignable { get { return salon != null ? salon.Asignable : false; } }
        public bool Empalme { get { return salon != null ? salon.empalme : true; } }

        public Salon(OrigenDatos.Clases.Salon _salon, Algoritmo02.Clases.ListaVariables _grupos, string _cve_espacio)
        {
            grupos = _grupos;
            salon = _salon;
            cve_espacio = _cve_espacio;
        }
    }
}