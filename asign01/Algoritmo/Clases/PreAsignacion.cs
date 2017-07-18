using OrigenDatos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo02.Clases
{
    public class PreAsignacion
    {
        private ListaVariables grupos;
        private ListaSalones salones;
        public ListaVariables Grupos { get { return grupos; } }

        public PreAsignacion(ListaVariables _g, ListaSalones _s)
        {
            grupos = _g;
            salones = _s;
        }

        /// <summary>
        /// Asigna salon a los que estan marcados con un salon fijo
        /// </summary>
        /// <param name="forzado">Si quiere que se cambie al salon preferencial sin importar si ya tiene salon asignado</param>
        /// <returns></returns>
        public void preferencial()
        {
            ListaVariables gruposPreferencial = new ListaVariables(grupos.SalonFijo());

            if (gruposPreferencial.Count != 0)
                foreach(Variable g in gruposPreferencial)
                    g.salonPreferencial();
        }

        /// <summary>
        /// Asigna salones tomando en cuenta el semestre pasado, aqui no checa empalmes, solo si es el mismo profesor, grupo, y horario
        /// </summary>
        public void semestres_anteriores()
        {
            foreach (Variable g in grupos)
                g.Cve_espacio = g.salonAnioPasado();
        }
    }
}
