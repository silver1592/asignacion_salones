using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Algoritmo02.Heredados;

namespace Algoritmo02.Clases
{
    public class Algoritmo
    {
        private static Conexion Consultas;
        public static Conexion Coneccion { set { Consultas = value; } }

        private ListaGrupos grupos;
        private ListaSalones salones;
        private int hora;

        private ListaGrupos Grupos
        {
            get
            {
                return new ListaGrupos(grupos);
            }
        }
        private ListaSalones Salones
        {
            get
            {
                return new ListaSalones(salones);
            }
        }

        private int tamPoblacion = 5;
        private int generaciones = 50;

        private int individuosCongelados = 1;

        private Individuo[] poblacion;

        private ListaGrupos errores;
        public ListaGrupos Errores { get { return errores; } }

        /// <summary>
        /// Constructor de la iteracion del algoritmo
        /// </summary>
        /// <param name="grupos">Lista de grupos a asignar</param>
        /// <param name="salones">Lista de salones disponibles para asignar</param>
        public Algoritmo(ListaGrupos grupos, ListaSalones salones, int hora)
        {
            this.grupos = grupos;
            this.salones = salones;
            this.hora = hora;
            errores = new ListaGrupos() ;

            poblacion = new Individuo[tamPoblacion];

            try
            {
                for (int i = 0; i < tamPoblacion; i++)
                {
                    poblacion[i] = new Individuo(Grupos, hora);
                    poblacion[i].Warning += new Individuo.Alerta(AddWarning);
                    /*
                    if (individuos_Viejos > i)
                        poblacion[i].asignaSalones(Salones, cicloAnt);
                    else*/
                        poblacion[i].asignaSalones(Salones);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error de ejecucion (constructor Algoritmo)\n No existen salones validos para el grupo: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Marca el inicio del algoritmo
        /// --03/06/2016 Tal vez sea mejor hacerlos hilos para que no se conjele la pantalla y para poder mostrar resultados en tiempo real.
        /// </summary>
        /// <returns></returns>
        public List<Grupo> AsignaSalones()
        {
            try
            {
                //Generacion
                for (int g = 0; g < generaciones; g++)
                {
                    //Mutacion
                    //se mantiene un grupo de individuos congelados
                    for (int individuo = 0; individuo < poblacion.Length; individuo++)
                        if (individuo > individuosCongelados - 1)
                            poblacion[individuo].mutacion();

                    seleccion();
                }

                return mejorRespuesta();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error de ejecucion (AsignaSalones): " + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Checa los individuos y selecciona la respuesta mejor evaluada
        /// --03/06/2016 si el valor que retorna es nulo significa que ninguna asignacion es valida
        /// </summary>
        /// <returns>Respuesta mejor evaluada.</returns>
        private List<Grupo> mejorRespuesta()
        {
            Individuo seleccionado = poblacion[0];
            List<Grupo> gruposAsignados = null;

            for (int i = 0; i < poblacion.Length; i++)
            {
                if (seleccionado != null)
                {
                    if (poblacion != null && poblacion[i].valor > seleccionado.valor)
                        seleccionado = poblacion[i];
                }
                else
                    seleccionado = poblacion[i];
            }

            if (seleccionado.valor > -1)
            {
                gruposAsignados = new List<Grupo>();
                foreach (Variable i in seleccionado.Cromosomas)
                    gruposAsignados.Add(i.Grupo);

                return gruposAsignados;
            }
            else
            {
                throw new Exception("no se asigno correctamente el horario");
            }
        }

        /// <summary>
        /// recorre la poblacion y dependiendo de un random y el valor que tenga el individuo se decide si se elimina o no
        /// --03/06/2016 si el individuo tiene valor de -1 sera eliminado
        /// </summary>
        private void seleccion()
        {
            Random r = new Random();
            Individuo temp;


            for (int i = individuosCongelados; i < tamPoblacion; i++)
                if (poblacion[i].valor < r.Next(15))
                    try
                    {
                        temp = new Individuo(Grupos, hora);
                        temp.asignaSalones(Salones);
                        poblacion[i] = temp;
                    }
                    catch { }
        }

        /// <summary>
        /// mensaje llamado cuando el evento Warning se activa y manda un grupo al que no se le pudo encontrar un salon al cual asignarsele
        /// </summary>
        /// <param name="grupo">Grupo con el uqe se tubo problemas</param>
        private void AddWarning(Grupo grupo)
        {
            var query = errores.GetGrupo(grupo.Cve_materia, grupo.num_Grupo);

            if(query!=null)
                errores.Add(grupo);
        }
    }
}