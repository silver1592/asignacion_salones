using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Models
{
    public class Materia: Algoritmo02.Clases.Materia
    {
        public Materia(DataRow r) : base(r) { }
    }
}