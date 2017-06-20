using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Models
{
    public class Profesor : Algoritmo02.Clases.Profesor
    {
        public Profesor(DataRow r) : base(r) { }

        public Profesor(Algoritmo02.Clases.Profesor p) : base(p) { }

        public Profesor(int rpe) : base(rpe) { }
    }
}