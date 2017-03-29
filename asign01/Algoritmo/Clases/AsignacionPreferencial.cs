using Algoritmo01.Heredados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo01.Clases
{
    public class AsignacionPreferencial
    {
        private ListaGrupos grupos;
        private ListaSalones salones;
        public ListaGrupos Grupos { get { return grupos; } }

        public AsignacionPreferencial(ListaGrupos _g, ListaSalones _s)
        {
            grupos = _g;
            salones = _s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="absoluto">Si quiere que se cambie al salon preferencial sin importar si ya tiene salon asignado</param>
        /// <returns></returns>
        public void ejecuta(bool absoluto = false)
        {
            ListaGrupos gruposPreferencial = grupos.GruposPreferencial();

            if (gruposPreferencial.Count() != 0)
            {
                gruposPreferencial.Ejecuta((Grupo g) =>
                {
                    g.salonPreferencial(new ListaSalones(salones));
                    return null;
                });
            }

            grupos.Update(gruposPreferencial);
        }
    }
}
