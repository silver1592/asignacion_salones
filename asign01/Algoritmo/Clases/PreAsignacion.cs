using Algoritmo02.Heredados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo02.Clases
{
    public class PreAsignacion
    {
        private ListaGrupos grupos;
        private ListaSalones salones;
        public ListaGrupos Grupos { get { return grupos; } }

        public PreAsignacion(ListaGrupos _g, ListaSalones _s)
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
            ListaGrupos gruposPreferencial = new ListaGrupos(grupos.SalonFijo());

            if (gruposPreferencial.Count() != 0)
                foreach(Grupo g in gruposPreferencial)
                    g.salonPreferencial();
        }

        /// <summary>
        /// Asigna salones tomando en cuenta el semestre pasado, aqui no checa empalmes, solo si es el mismo profesor, grupo, y horario
        /// </summary>
        public void semestres_anteriores()
        {
            foreach (Grupo g in grupos)
                g.Salon = g.salonAnioPasado();
        }
    }
}
