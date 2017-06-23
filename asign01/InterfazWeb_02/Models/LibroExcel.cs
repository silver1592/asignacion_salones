using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Models
{
    public class LibroExcel : OrigenDatos.Clases.LibroExcel
    {
        private string nombre;
        public string Nombre { get { return nombre; } }

        public LibroExcel(string direccion, string archivo, string ciclo ="2016-2017/II", string tipo =""):base(direccion+archivo,ciclo,tipo)
        {
            nombre = archivo;
        }
    }
}