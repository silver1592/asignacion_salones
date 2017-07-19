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
            this.salones = salones;
            this.grupos.SetSalones(this.salones);
            this.hora = hora;
            tamPoblacion = _tamPoblacion;
            generaciones = _generaciones;
            errores = new ListaVariables() ;

            poblacion = new Individuo[tamPoblacion];
            mejorPoblacion = new Individuo[tamPoblacion];

            try
            {
                for (int i = 0; i < tamPoblacion; i++)
                {
                    poblacion[i] = new Individuo(Grupos, hora);
                    poblacion[i].asignaSalones(Salones);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error de ejecucion (constructor Algoritmo)\n No existen salones validos para el grupo: " + ex.Message);
            }
        }

        /// <summary>
        /// Marca el inicio del algoritmo
        /// </summary>
        /// <returns>Lista con los grupos asignados</returns>
        public void AsignaSalones()
        {
            try
            {
                rescate();
                //Generacion
                for (int g = 0; g < generaciones; g++)
                { 
                    //Mutacion
                    //se mantiene un grupo de individuos congelados
                    foreach(Individuo i in poblacion)
                        i.mutacion();

                    seleccion();

                    rescate();
                }

                gruposAsignados = mejorRespuesta();
            }
            catch (Exception ex)
            {
                //MessageBox.Show();
                throw new Exception("Error de ejecucion (AsignaSalones): " + ex);
            }
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
        /// --03/06/2016 si el valor que retorna es nulo significa que ninguna asignacion es valida
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
        /// --03/06/2016 si el individuo tiene valor de -1 sera eliminado
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

        /// <summary>
        /// mensaje llamado cuando el evento Warning se activa y manda un grupo al que no se le pudo encontrar un salon al cual asignarsele
        /// </summary>
        /// <param name="grupo">Grupo con el uqe se tubo problemas</param>
        private void AddWarning(Grupo grupo)
        {
            var query = errores.Busca(grupo.Cve_materia, grupo.num_Grupo);

            if(query!=null)
                errores.Add(grupo);
        }
    }
}