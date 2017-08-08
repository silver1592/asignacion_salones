using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Models
{
    public class Salon
    {
        public Algoritmo02.Clases.ListaVariables grupos;
        public OrigenDatos.Clases.Salon salon;

        public Salon(OrigenDatos.Clases.Salon _salon, Algoritmo02.Clases.ListaVariables _grupos)
        {
            grupos = _grupos;
            salon = _salon;
        }
    }
}