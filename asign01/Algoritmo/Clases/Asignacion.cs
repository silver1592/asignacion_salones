using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Algoritmo01.Heredados;

namespace Algoritmo01.Clases
{
    public class Asignacion
    {
        #region Variables
        private ListaGrupos grupos;
        private ListaSalones salones;
        private int hora;
        #endregion

        #region Eventos
        /// <summary>
        /// Usado para informar a otros procesos en que parte del algoritmo va.
        /// </summary>
        /// <param name="procesoActual">Descripcion de lo que se esta haciendo en ese momento</param>
        /// <param name="porcentajeAdd">Porcentaje de avance que lleva el algoritmo para concluir su ejecucion (</param>
        public delegate void estado(string procesoActual, float porcentajeAdd=0);
        public event estado Estado;

        public delegate void Finaliza(ListaGrupos Errores);
        public event Finaliza Finalizado;
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
            #region Pre Asignacion
            /// No cambiar el orden del proceso, primero va asignacion preferencial y luego el chequeo de empalmes
            /// De modificarlo pueden surgir empalmes en los salones
            AsignacionPreferencial a = new AsignacionPreferencial(grupos, salones);
            a.ejecuta();

            Empalmes("e1");
            #endregion

            #region Algoritmo
            iniciaAlgoritmoGenetico();

            #endregion

            #region eliminacion de empalmes
            Empalmes("e2");
            #endregion
        }

        #region Procesos de asignacion        

        private List<Grupo> GruposDelSalon(string salon)
        {
            var query = from g in GruposAsignados
                        where g.Salon == salon && g.horario(hora)
                        select g;

            return query.ToList<Grupo>();
        }

        private void iniciaAlgoritmoGenetico()
        {
            List<Grupo> gruposAsignados;
            Algoritmo inst_algoritmo;
            ListaGrupos GruposSinAsignar = (ListaGrupos)grupos.GruposSinAsignar("111111",hora);

            if (GruposSinAsignar.Count() != 0)
            {
                try
                {
                    inst_algoritmo = new Algoritmo(GruposSinAsignar, salones, hora);
                    gruposAsignados = inst_algoritmo.AsignaSalones();

                    foreach (Grupo g in gruposAsignados)
                    {
                        g.Update("Algoritmo");
                        grupos.Update(g);
                    }

                    Finalizado(inst_algoritmo.Errores);
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