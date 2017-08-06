using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OrigenDatos.Clases;

namespace Algoritmo02.Clases
{
    public class Algoritmo
    {
        private ListaVariables grupos;
        private ListaVariables gruposAsignados;
        private ListaSalones salones;
        private int hora;

        private ListaGrupos Grupos { get { return new ListaGrupos(grupos); } }
        public ListaGrupos GruposAsignados { get { return gruposAsignados; } }
        private ListaSalones Salones { get { return new ListaSalones(salones); } }

        private int tamPoblacion;
        private int generaciones;

        private Individuo[] poblacion;
        private Individuo[] mejorPoblacion;

        private ListaVariables errores;
        public ListaVariables Errores { get { return errores; } }

        /// <summary>
        /// Constructor de la iteracion del algoritmo
        /// </summary>
        /// <param name="grupos">Lista de grupos a asignar</param>
        /// <param name="salones">Lista de salones disponibles para asignar</param>
        public Algoritmo(ListaGrupos grupos, ListaSalones salones, int hora,int _tamPoblacion = 5, int _generaciones =50)
        {
            this.grupos = new ListaVariables(grupos.SinAsignar());
            this.grupos = this.grupos.EnHoras(hora, hora + 1);
            this.grupos = this.grupos.EnDias();
            this.salones = salones.Asignables();
            this.hora = hora;
            tamPoblacion = _tamPoblacion;
            generaciones = _generaciones;
            errores = new ListaVariables();

            poblacion = new Individuo[tamPoblacion];
            mejorPoblacion = new Individuo[tamPoblacion];

            for (int i = 0; i < tamPoblacion; i++)
            {
                poblacion[i] = new Individuo(Grupos, hora);
                try
                {
                  poblacion[i].asignaSalones(Salones);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error de ejecucion (constructor Algoritmo)\n No existen salones validos para el grupo: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Marca el inicio del algoritmo
        /// </summary>
        /// <returns>Lista con los grupos asignados</returns>
        public void AsignaSalones()
        {
            rescate();
            
            //Generacion
            for (int g = 0; g < generaciones; g++)
            {
                foreach (Individuo i in poblacion)
                {
                    try { i.mutacion(); }
                    catch (Exception ex)
                    { throw new Exception("Error de ejecucion (AsignaSalones): " + ex); }
                }

                seleccion();

                rescate();
            }

            gruposAsignados = mejorRespuesta();
        }

        /// <summary>
        /// Obtiene los individuos y los compara entre los que tiene en mejores, para ir almacenando a los mejores individuos de cada generacion
        /// </summary>
        private void rescate()
        {
            for (int i = 0; i < poblacion.Length; i++)
                if (mejorPoblacion[i] == null || poblacion[i].valor > mejorPoblacion[i].valor)
                    mejorPoblacion[i] = new Individuo(poblacion[i]);
        }

        /// <summary>
        /// Checa los individuos y selecciona la respuesta mejor evaluada
        /// </summary>
        /// <returns>Respuesta mejor evaluada.</returns>
        private ListaVariables mejorRespuesta()
        {
            Individuo seleccionado = mejorPoblacion[0];

            foreach(Individuo i in mejorPoblacion)
                if(i.valor > seleccionado.valor)
                    seleccionado = i;

            foreach(Individuo i in poblacion)
                if (i.valor > seleccionado.valor)
                    seleccionado = i;

            return new ListaVariables(seleccionado.Grupos);
        }

        /// <summary>
        /// recorre la poblacion y dependiendo de un random y el valor que tenga el individuo se decide si se elimina o no
        /// </summary>
        private void seleccion()
        {
            Random r = new Random();
            Individuo temp;

            for (int i = 0; i < poblacion.Length; i++)
                if (poblacion[i].valor < r.Next(15))
                {
                    temp = new Individuo(Grupos, hora);
                    temp.asignaSalones(Salones);
                    poblacion[i] = temp;
                }
        }
    }
}