using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OrigenDatos.Clases;

namespace Algoritmo01.Clases
{
    public class Asignacion
    {
        #region Variables
        private string ciclo;
        private int hora;
        private static Conexion conexion;

        private List<Grupo> GruposAsignados;
        private List<Grupo> GruposSinAsignar;
        private List<Salon> Salones;

        public static Conexion Conexion { set { conexion = value; } }
        #endregion

        #region Eventos
        /// <summary>
        /// Usado para informar a otros procesos en que parte del algoritmo va.
        /// </summary>
        /// <param name="procesoActual">Descripcion de lo que se esta haciendo en ese momento</param>
        /// <param name="porcentajeAdd">Porcentaje de avance que lleva el algoritmo para concluir su ejecucion (</param>
        public delegate void estado(string procesoActual, float porcentajeAdd=0);
        public event estado Estado;

        public delegate void Finaliza(List<Grupo> Errores);
        public event Finaliza Finalizado;
        #endregion

        #region constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ciclo">Ciclo escolar que se va a asignar</param>
        /// <param name="hora">Hora que se se va a asignar</param>
        /// <param name="excel">Si se va a usar un excel para obtener los grupos o de la base de datos</param>
        public Asignacion(string ciclo, int hora)
        {
            this.hora = hora;
            this.ciclo = ciclo;
        }
        #endregion

        /// <summary>
        /// Proceso principal del algoritmo
        /// Aqui se va desarrollando el algoritmo y se van haciendo las llamadas a los demas metodos para su realizacion.
        /// </summary>
        public void EjecutaAlgoritmo()
        {
            #region Pre Asignacion
            PreparacionDatos();
            Estado("Finalizo la busqueda de informacion en la base de datos.", 30);
            /// No cambiar el orden del proceso, primero va asignacion preferencial y luego el chequeo de empalmes
            /// De modificarlo pueden surgir empalmes en los salonees
            ///
            
            Estado("Iniciando ejecucion del algoritmo en el horario de " + hora);
            Estado("Asignando salones a los profesores con salones especificos.");
            AsignacionPreferencal();
            Estado("Termino la asignacion Preferencial.",10);

            Estado("Checando si hay empames en la asignacion actual");
            Empalmes("e1");
            Estado("Termino el chequeo de empalmes", 10);
            #endregion

            #region Algoritmo
            PreparacionDatos();
            Estado("Ejecutando algoritmo genetico");
            iniciaAlgoritmoGenetico();
            Estado("Termino la ejecucion del algoritmo genetico", 25);
            #endregion

            #region eliminacion de empalmes
            //Ultimo chequeo de empalmes
            PreparacionDatos();

            Estado("Checando si hay empames en la asignacion actual");
            Empalmes("e2");
            Estado("Termino el chequeo de empalmes",10);
            #endregion

            #region guardando asignacion
            Estado("Guardando Asginacion", 10);
            conexion.Update();
            #endregion

            Estado("Finalizo la ejecucion del algoritmo en el horario de " + hora,5);
        }

        #region Datos
        private void PreparacionDatos()
        {
            Estado("Consultado listado de salones y grupos en la base de datos");
            RecuperaDatos();
            Estado("Recuperando horarios de los salones");
            foreach (Salon s in Salones)
                s.ObtenHorario(GruposAsignados);
        }

        /// <summary>
        /// Hace las consultas necesarias en la base de datos y crea los objetos necesarios para almacenar esta informacion
        /// </summary>
        private void RecuperaDatos()
        {
            Salones = RecuperaSalones();
            GruposSinAsignar = RecuperaGruposSinAsignar();
            GruposAsignados = RecuperarGruposAsignados();
        }

        private List<Grupo> RecuperaGruposSinAsignar()
        {
            List<Grupo> grupos = null;
            
            grupos = Grupo.GruposSinAsignar("1111111", hora, ciclo);

            return grupos;
        }

        private List<Salon> RecuperaSalones()
        {
            List<Salon> salones = null;

            salones = Salon.Salones(conexion,hora);

            return salones;
        }

        private List<Grupo> RecuperarGruposAsignados()
        {
            List<Grupo> grupos = null;

            grupos = Grupo.Grupos_Empalmados(hora, ciclo);

            return grupos;
        }
        #endregion

        #region Procesos de asignacion
        /// <summary>
        /// Busca en los grupos sin asignar los profesores que tengan un salon asignado, los asigna y los remueve de la lista.
        /// </summary>
        /// <returns></returns>
        private List<Grupo> AsignacionPreferencal()
        {
            List<Grupo> gruposPreferencial = null;

            if (GruposSinAsignar.Count != 0)
            {
                var Preferencial = from g in GruposSinAsignar
                                   where g.Salon_fijo != null || g.Salones_posibles.Count != 0
                                   select g;
                gruposPreferencial = Preferencial.ToList();

                foreach (Grupo g in gruposPreferencial)
                {
                    if (g.salonPreferencial(Salones))
                    {
                        GruposSinAsignar.Remove(g);
                        g.Update("Asignacion preferencial");
                    }
                }
            }

            return gruposPreferencial;
        }

        private List<List<Grupo>> GruposEmpalmados()
        {
            List<List<Grupo>> res = new List<List<Grupo>>();
            List<Grupo> aux;
            foreach (Salon s in Salones)
            {
                aux = GruposDelSalon(s.Cve_espacio);

                if(aux.Count>1)
                    res.Add(aux);
            }

            return res;
        }

        private List<Grupo> GruposDelSalon(string salon)
        {
            var query = from g in GruposAsignados
                        where g.Salon == salon && g.horario(hora)
                        select g;

            return query.ToList<Grupo>();
        }

        private bool HayEmpalme(List<Grupo> lista)
        {
            //se obtiene el salon al que se hace referencia
            Salon salon = buscaSalon(lista[0].Salon);

            //Posible error
            if (salon == null) { throw new Exception("Error al ejecutar los empalmes (" + salon.Cve_espacio + ")", null); }

            if (salon.empalme) return false;
            //Checa el salon
            foreach (Grupo g1 in lista)
                foreach (Grupo g2 in lista)
                    if (g1!=g2 && g1.empalme(g2))
                        return true;

            return false;
        }

        private void guardaMovimiento(List<Grupo> lista, string mensaje="empalme")
        {
            string observaciones;

            foreach (Grupo g in lista)
            {
                observaciones = mensaje;
                if (g.Salon == "")
                    GruposSinAsignar.Add(g);
                GruposAsignados.Remove(g);

                g.Update(observaciones);
            }
        }

        /// <summary>
        /// Busca los empalmes en el horario designado y comienza a acomodarlos
        /// </summary>
        /// <returns></returns>
        private void Empalmes(string mensaje_plantilla="")
        {
            List<List<Grupo>> empalmados = new List<List<Grupo>>();
            List<Grupo> checando = new List<Grupo>();
            List<Grupo> Temp;

            //obtiene grupos de grupos empalmados
            empalmados = GruposEmpalmados();

            int i=1;
            string mensaje;
            while (empalmados.Count != 0)
            {
                checando = empalmados[0];
                mensaje = "";
                //Chequeo de empalme
                if (HayEmpalme(empalmados[0]))
                {
                    #region solucion de empalmes
                    
                    //Solucion de empalme
                    ///selecciona el grupo que se va a quedar con el salon si es que hay algun preferencial
                    var salonPref = from g in checando
                                    where g.Salon_fijo == g.Salon
                                    select g;

                    Temp = salonPref.ToList();

                    ///Si hay conflicto en el preferencial
                    if (Temp.Count > 1)
                    {
                        var count = from g in Temp
                                    where g.RPE != checando[0].RPE
                                    select g;

                        if (count.ToList().Count > 0)
                            //throw new Exception("Verifique Salones preferenciales en la base de datos, hay un problema con el salon " + checando[0].Salon);
                            foreach(Grupo g in  Temp)
                                g.Salon = "-";
                    }
                    ///Solo uno tiene preferencia, y a ese se le va a dar
                    else if (Temp.Count == 1)
                    {
                        foreach (Grupo g in checando)
                            if (Temp[0] != g)
                                g.Salon = "";
                    }
                    /// se elegira por otro medio
                    else
                        checando = corrigeEmpalmes(checando);

                    mensaje = mensaje_plantilla + "_" + hora + "_" + i;
                    #endregion
                }

                //Guardar informacion
                guardaMovimiento(checando, mensaje);

                empalmados.Remove(checando);
                i++;
            }
        }

        /// <summary>
        /// Proceso que se ejecuta cuando indiscutiblemente hay un empalme entre 2 grupos, analiza cual es el que se debe de cambiar y cua no
        /// </summary>
        /// <param name="checando">Grupos con empalme</param>
        /// <returns>Misma lista de grupos con el empalme corregido</returns>
        private List<Grupo> corrigeEmpalmes(List<Grupo> checando)
        {
            ///Checa si estan en el horario
            ///por precaucion
            var query0 = from g in checando
                         where g.horario(hora)
                         select g;

            if (query0.ToList().Count == checando.Count)
            {
                var query = from g in checando
                            orderby g.AsignacionSemestreAnterior(hora) descending
                            select g;
                if (query.ToList().Count != 0)
                {
                    Grupo sel = query.ToList()[0];

                    foreach (Grupo g in query)
                        if (g != sel && sel.empalme(g))
                            g.Salon = "";
                }
            }
            else
            {
                var otrasHoras = from g in checando
                                 where g.horario(hora)
                                 select g;

                foreach(Grupo g in otrasHoras)
                {
                    var q = from gr in query0
                            where gr.empalme(g)
                            select gr;

                    foreach (Grupo gr in q)
                        gr.Salon = "";
                }
            }

            return checando;
        }

        private void iniciaAlgoritmoGenetico()
        {
            List<Grupo> gruposAsignados;
            Algoritmo inst_algoritmo;

            if (GruposSinAsignar.Count != 0)
            {
                try
                {
                    inst_algoritmo = new Algoritmo(GruposSinAsignar, Salones, hora);
                    gruposAsignados = inst_algoritmo.AsignaSalones();

                    foreach (Grupo g in gruposAsignados)
                    {
                        g.Update("Algoritmo");
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

        #region busqueda
        /// <summary>
        /// Busca un salon en la lista de salones del sistema.
        /// </summary>
        /// <param name="cve_espacio"></param>
        /// <returns></returns>
        private Salon buscaSalon(string cve_espacio)
        {
            var busqueda = from s in Salones
                           where s.Cve_espacio == cve_espacio
                           select s;

            List<Salon> resQuery = busqueda.ToList();

            if (resQuery.Count == 1)
                return resQuery[0];
            else
            {
                return null;
                throw new Exception("Ocurrio un error al buscar el salon \nNo se encontro nunguna conincidencia o fueron mas de 1");
            }
        }
        #endregion
    }
}