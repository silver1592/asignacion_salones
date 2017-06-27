using Algoritmo02.Heredados;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Clases
{
    public class ListaSalones : Algoritmo02.Heredados.ListaSalones
    {
        /// <summary>
        /// Solo crea la lista, falta inicializar la conexion
        /// </summary>
        public ListaSalones() : base () { }

        /// <summary>
        /// Inicializa todos los componentes con algun valor
        /// </summary>
        /// <param name="c">Conexion a utilizar</param>
        /// <param name="salones">Lista de salones a utilizar</param>
        public ListaSalones(IList<OrigenDatos.Clases.Salon> salones) : base(salones){ }

        public ListaSalones(Conexion c, DataTable dtSalones, int hora = 0) : base(c, dtSalones, hora) { }
    }
}