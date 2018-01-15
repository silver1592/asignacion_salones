using Algoritmo02.Clases;
using OrigenDatos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterfazWeb_02.Clases
{
    public class RevisionEmpalmes : ChecaEmpalmes,IOperacion
    {
        public RevisionEmpalmes(ListaGrupos _grupos, IList<Salon> _salones) : base(_grupos, _salones){ }

        public IList<Grupo> Resultado => EmpalmesResueltos;

        public IList<Grupo> Errores => new List<Grupo>();

        public DelEjecuta Ejecuta => ejecuta;
    }
}