using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Algoritmo01.Heredados;

namespace Algoritmo01.Clases
{
    public class Algoritmo
    {
        private static Conexion Consultas;
        public static Conexion Coneccion { set { Consultas = value; } }

        private List<Grupo> grupos;
        private List<Salon> salones;
        private int hora;

        private List<Grupo> Grupos
        {
            get
            {
                List<Grupo> res = new List<Grupo>();

                foreach (Grupo g in grupos)
                    res.Add(new Grupo(g));

                return res;
            }
        }
        private List<Salon> Salones
        {
            get
            {
                List<Salon> res=new List<Salon>();

                foreach(Salon s in salones)
                    res.Add(new Salon(s));

                return res;
            }
        }

        private int tamPoblacion = 5;
        private int generaciones = 50;

        private static int individuos_Viejos = 3;
        private int individuosCongelados = 1;

        private Individuo[] poblacion;

        private List<Grupo> errores;
        public List<Grupo> Errores { get { return errores; } }

        /// <summary>
        /// Constructor de la iteracion del algoritmo
        /// </summary>
        /// <param name="grupos">Lista de grupos a asignar</param>
        /// <param name="salones">Lista de salones disponibles para asignar</param>
        public Algoritmo(List<Grupo> grupos, List<Salon> salones, int hora, string cicloAnt = "2015-2016/I")
        {
            this.grupos = grupos;
            this.salones = salones;
            this.hora = hora;
            errores = new List<Grupo>();

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

        private List<Grupo> individuo_OtrosSemestres(List<Grupo> g, string ciclo, out int[] res)
        {
            DataTable asignacionAnterior;
            Grupo temp;
            List<Grupo> grupos_temp = new List<Grupo>();
            int count = salones.Count;

            res = new int[count];
            //res = inicializa(res.Length, -1);

            while (g.Count != 0)
            {
                asignacionAnterior = Consultas.Grupo(g[0].Cve_materia, g[0].num_Grupo, "T", ciclo);

                if (asignacionAnterior.Rows.Count != 0 && asignacionAnterior.Rows[0]["salon"].ToString() != "")
                {
                    try
                    {
                        temp = new Grupo(g[0].Cve_materia, g[0].num_Grupo, "T", ciclo);
                        res[salones.IndexOf(Salon.buscaSalon(temp.Salon, salones))] = grupos.IndexOf(g[0]);
                        g.Remove(g[0]);
                    }
                    catch
                    {
                        grupos_temp.Add(g[0]);
                        g.Remove(g[0]);
                    }
                }
                else
                {
                    grupos_temp.Add(g[0]);
                    g.Remove(g[0]);
                }
            }

            return new List<Grupo>(grupos_temp);
        }

        /// <summary>
        /// mensaje llamado cuando el evento Warning se activa y manda un grupo al que no se le pudo encontrar un salon al cual asignarsele
        /// </summary>
        /// <param name="grupo">Grupo con el uqe se tubo problemas</param>
        private void AddWarning(Grupo grupo)
        {
            var query = from g in errores
                        where g.Cve_materia == grupo.Cve_materia && g.num_Grupo == grupo.num_Grupo
                        select g;

            if(query.Count()==0)
                errores.Add(grupo);
        }
    }
}
