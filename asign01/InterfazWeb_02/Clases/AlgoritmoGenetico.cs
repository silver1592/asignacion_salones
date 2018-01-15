using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Algoritmo02.Clases;
using OrigenDatos.Clases;

namespace InterfazWeb_02.Clases
{
    public class AlgoritmoGenetico : Algoritmo, IOperacion
    {
        public AlgoritmoGenetico(ListaGrupos grupos, ListaSalones salones, int hora, int _tamPoblacion = 5, int _generaciones = 50) : base(grupos, salones, hora, _tamPoblacion, _generaciones){ }

        public IList<Grupo> Errores => new List<Grupo>();

        public IList<Grupo> Resultado => GruposAsignados;

        public DelEjecuta Ejecuta => AsignaSalones;
    }
}