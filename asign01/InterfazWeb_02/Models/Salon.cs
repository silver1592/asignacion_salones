using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Models
{
    public class Salon
    {
        private OrigenDatos.Clases.Salon salon { get; set; }
        public Algoritmo02.Clases.ListaVariables grupos { get; set; }
        private string cve_espacio;

        public string Cve_espacio { get { return salon != null ? salon.Cve_espacio : cve_espacio; } set { salon.Cve_espacio = value; } }
        [RegularExpression(@"[0-9]*")]
        public string Cupo { get { return salon != null ? salon.Cupo.ToString() : "Desconocido"; } set {salon.Cupo = Convert.ToInt32(value); } }
        public bool Asignable { get { return salon != null ? salon.Asignable : false; } set { salon.Asignable = value;  } }
        public bool Empalme { get { return salon != null ? salon.empalme : true; } set { salon.empalme = value; } }

        public Salon(OrigenDatos.Clases.Salon _salon, Algoritmo02.Clases.ListaVariables _grupos, string _cve_espacio)
        {
            grupos = _grupos;
            salon = _salon;
            cve_espacio = _cve_espacio;
        }
    }
}