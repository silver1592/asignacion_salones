using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Algoritmo02.Heredados;

namespace Algoritmo02.Clases
{
    public class Asignacion
    {
        #region Variables
        private ListaGrupos grupos;
        private ListaSalones salones;
        private int hora;

        public ListaGrupos gruposAsignados;
        public ListaGrupos Grupos { get { return gruposAsignados; } }
        #endregion

        #region constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ciclo">Ciclo escolar que se va a asignar</param>
        /// <param name="hora">Hora que se se va a asignar</param>
        /// <param name="excel">Si se va a usar un excel para obtener los grupos o de la base de datos</param>
        public Asignacion(ListaGrupos _grupos,ListaSalones _salones, int _hora)
        {
            this.grupos = _grupos;
            this.salones = _salones;
            this.hora = _hora;
        }
        #endregion

        /// <summary>
        /// Proceso principal del algoritmo
        /// Aqui se va desarrollando el algoritmo y se van haciendo las llamadas a los demas metodos para su realizacion.
        /// </summary>
        public void EjecutaAlgoritmo()
        {
            #region Algoritmo
            iniciaAlgoritmoGenetico();

            #endregion
        }

        #region Procesos de asignacion

        private void iniciaAlgoritmoGenetico()
        {
            Algoritmo inst_algoritmo;
            ListaGrupos GruposSinAsignar = new ListaGrupos(grupos.SinAsignar("111111",hora));

            if (GruposSinAsignar.Count() != 0)
            {
                try
                {
                    inst_algoritmo = new Algoritmo(GruposSinAsignar, salones, hora);
                    gruposAsignados = inst_algoritmo.AsignaSalones();
                    /*
                    foreach (Grupo g in gruposAsignados)
                    {
                        g.Update("Algoritmo");
                        grupos.Update(g);
                    }

                    Finalizado(inst_algoritmo.Errores);
                    */
                    //return gruposAsignados;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error de ejecucion (asignacionSalones): " + ex);
                    throw ex;
                }
            }
        }
        #endregion
    }
}