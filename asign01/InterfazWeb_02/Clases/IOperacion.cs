using OrigenDatos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazWeb_02.Clases
{
    public delegate void DelEjecuta();

    interface IOperacion
    {
        IList<Grupo> Resultado { get; }
        IList<Grupo> Errores { get; }

        DelEjecuta Ejecuta { get; }
    }
}
