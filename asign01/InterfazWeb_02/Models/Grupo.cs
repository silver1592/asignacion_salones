using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrigenDatos;
using Algoritmo02;

namespace InterfazWeb_02.Models
{
    public class Grupo : Algoritmo02.Heredados.Grupo
    {
        public Grupo (OrigenDatos.Clases.Grupo copia) : base(copia) { }

        public Grupo(Algoritmo02.Heredados.Grupo copia,Materia materia) : base(copia) { }
    }
}