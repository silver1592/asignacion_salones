using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrigenDatos;
using Algoritmo01.Heredados;

namespace InterfazWeb_02.Models
{
    public class Grupo : Algoritmo01.Heredados.Grupo
    {
        public Grupo (OrigenDatos.Clases.Grupo copia) : base(copia) { }

        public Grupo(Algoritmo01.Heredados.Grupo copia) : base(copia) { }
    }
}