using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Algoritmo02.Clases;
using OrigenDatos.Clases;

namespace InterfazWeb_02.Clases
{
    public class AsignacionPreferencial : Algoritmo02.Clases.PreAsignacion, IOperacion
    {
        public AsignacionPreferencial(ListaVariables Grupos, ListaSalones Salones) : base(Grupos, Salones){ }

        IList<Grupo> IOperacion.Resultado => GruposModificados;

        IList<Grupo> IOperacion.Errores => new List<Grupo>();

        DelEjecuta IOperacion.Ejecuta => preferencial;

        string IOperacion.NombreOperacion => "Asignacion preferencial";
    }
}