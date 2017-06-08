using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Algoritmo02.Heredados;

namespace Algoritmo02.Clases
{
    /// <summary>
    /// Esta clase es para manipular la informacion que los individuos dan al algoritmo
    /// --02/06/2016 Aqui se crearan los metodos para la mutacion, seleccion y asignacion del individuo (solo los ciclos)
    /// </summary>
    public class Individuo
    {
        private Conexion Consultas;
        public Conexion Coneccion { set { Consultas = value; } }

        /// <summary>
        /// Usado para informar que un grupo no fue posible de ubicar al principio.
        /// </summary>
        public delegate void Alerta(Grupo grupo);
        public event Alerta Warning;

        public delegate void Problema(Variable var);
        public event Problema problema;
        private void agregaError(Variable v){ Errores.Add(v); }

        private List<Variable> Errores;

        #region Atributos
        private List<Variable> cromosomas;
        public List<Variable> Cromosomas { get { return cromosomas; } }
        private ListaSalones salones;
        private int cromosomas_a_Mutar = 10;

        /// <summary>
        /// Retorna el valor obtenido para la funcion objetivo del individuo
        /// --03/062016 si es -1 el individuo es invalido
        /// </summary>
        public float valor
        {
            get
            {
                float fRes = 0;

                foreach (Variable v in cromosomas)
                    fRes += v.puntos;

                return fRes;
            }
        }
        #endregion

        /// <summary>
        /// Constructor del Individuo. 
        /// Inicializa el arreglo de cromosomas.
        /// </summary>
        /// <param name="salonesDisponibles">Lista de salones a considerar para la interacion del algoritmo</param>
        public Individuo(ListaGrupos gruposPorAsignar, int  hora)
        {
            cromosomas = new List<Variable>();
            problema += new Problema(agregaError);
            Errores = new List<Variable>();

            for(int i=0; i<gruposPorAsignar.Count();i++)
                cromosomas.Add(new Variable((Grupo)gruposPorAsignar[i],hora));
        }

        #region Asignacion de Salones
        /// <summary>
        /// Asigna los grupos a los salones usando un random y los salones marcados.
        /// --08/06/2016 Modifique la parte de las variables y tambien los metodos, este ahora asinara salones a los grupos.
        /// </summary>
        public void asignaSalones(ListaSalones Salones)
        {
            this.salones = Salones;

            for(int i=2;i<=7;i++)
            {
                separacionXArea(i);
            }

            separacionXArea(1);

            arreglaErrores();
        }

        /// <summary>
        /// Checa la lista de errores y todos los grupos que no pudo asignar se asignaran ahora en cualquier otra parte
        /// </summary>
        private void arreglaErrores()
        {
            Aleatorio r = new Aleatorio(salones.Count);
            int val;
            Salon salon;

            foreach (Variable v in Errores)
            {
                do
                {
                    val = r.Next();

                    if (val != -1)
                    {
                        salon = salones.GetSalon(val);
                        if (v.Grupo.Cupo < salon.Cupo && salon.Disponible_para_grupo(v.Grupo))
                        {
                            v.Salon = salon;
                            r.Reinicia();
                            break;
                        }
                    }
                    else
                    {
                        r.Reinicia();
                        //throw new Exception("No se encontron salones para el grupo " + v.Grupo.Cve_materia + v.Grupo.num_Grupo.ToString());
                        break;
                    }

                } while (true);
            }
        }

        /// <summary>
        /// Toma los grupos de el area designada por el numero i, y manda llamar la asignacion de los salones de esa area
        /// </summary>
        /// <param name="i"></param>
        private void separacionXArea(int i)
        {
            ListaSalones s;
            List<Variable> asignando;

            var query1 = from g in cromosomas
                         where g.Grupo.Area == i.ToString()
                         orderby g.Grupo.Cupo descending
                         select g;

            asignando = query1.ToList();

            if (asignando.Count != 0)
            {
                s = (ListaSalones)salones.EnArea(i);

                asignandoXArea(asignando, s);
            }
        }

        private void asignandoXArea(List<Variable> asignando, ListaSalones s)
        {
            foreach (Variable v in asignando)
                if (!v.salonHoraAnterior(s))
                    asignacionAleatoria(v, s);
                else
                {
                    //Asignacion preferencial
                }
        }

        private void asignacionAleatoria(Variable v,ListaSalones s)
        {
            Aleatorio r = new Aleatorio(s.Count);
            int val;
            Salon salon;

            do
            {
                val = r.Next();

                if (val != -1)
                {
                    salon = s.GetSalon(val);
                    if (v.Grupo.SalonValido(salon) > 0 && salon.Disponible_para_grupo(v.Grupo))
                    {
                        v.Salon = salon;
                        r.Reinicia();
                        break;
                    }
                }
                else
                {
                    //r.Reinicia();
                    //throw new Exception("No se encontron salones para el grupo " + v.Grupo.Cve_materia + v.Grupo.num_Grupo.ToString());
                    Warning(v.Grupo);
                    problema(v);
                    r.Reinicia();
                    break;
                }

            } while (true);
        }
        #endregion

        #region Mutacion
        /// <summary>
        /// Proceso de mutacion del individuo
        /// </summary>
        /// <param name="congelado"></param>
        public void mutacion()
        {
            Aleatorio rGrupo = new Aleatorio(cromosomas.Count);
            Aleatorio rSalon;
            int salonSelec;
            int grupoSelec;
            Salon selecSal;
            Salon temp;
            ListaSalones salonesXArea;

            Variable grupo1;
            List<Variable> grupo2;

            for (int c = 0; c < cromosomas_a_Mutar && c < cromosomas.Count; c++)
            {
                try
                {
                    //Selecciona un grupo aleatoriamente y guarda sus datos
                    grupoSelec = rGrupo.Next();
                    rGrupo.aceptado(grupoSelec);
                    grupo1 = cromosomas[grupoSelec];

                    //selecciona un salon aleatorio para el cambio
                    salonesXArea = SalonesXArea(grupo1.Grupo.Area);
                    rSalon = new Aleatorio(salonesXArea.Count);

                    do { salonSelec = rSalon.Next(); }
                    while (grupo1.calcula_PuntosSalon(salonesXArea.GetSalon(salonSelec)) < 0);

                    grupo2 = buscaSalon(salonesXArea.GetSalon(salonSelec).Cve_espacio);
                    selecSal = salonesXArea.GetSalon(salonSelec);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Mutacion\n" + ex.Message);
                    break;
                }

                ///Checa si es un cambio beneficioso
                ///
                //-Corregir-
                /*
                if (selecSal.puntos + grupo1.puntos < selecSal.puntosCon(grupo1.Grupo) + grupo1.calcula_PuntosSalon(selecSal))
                {
                    //Checa si esta permitido el cambio (si los grupos estan dentro de las variables)
                    ListaGrupos list = (ListaGrupos)selecSal.EmpalmesCon(grupo1.Grupo);

                    var query = from g in grupo2
                                where list.Contains(g.Grupo)
                                select g;

                    if (query.ToList().Count == list.Count)
                    {
                        temp = grupo1.Salon;
                        grupo1.Salon = selecSal;

                        foreach (Variable g in grupo2)
                            if (g.Grupo.empalme(grupo1.Grupo))
                                g.Salon = temp;
                    }
                }*/
            }
        }

        public ListaSalones SalonesXArea(string area)
        {
            return (ListaSalones)salones.EnArea(Convert.ToInt32(area));
        }

        private float promedio(List<Variable> lista)
        {
            float f = 0 ;

            foreach (Variable v in lista)
                f += v.puntos;

            f = f / lista.Count;

            return f;
        }

        #endregion

        #region Utilidades
        /// <summary>
        /// Busca las variable correspondientes al salon solisitado
        /// </summary>
        /// <param name="salon">salon a buscar</param>
        /// <returns>Variable del salon, o null en caso de no encontrarlo</returns>
        private List<Variable> buscaSalon(string salon)
        {
            var query = from v in cromosomas
                        where v.Salon != null && v.Salon.Cve_espacio == salon
                        select v;

            return query.ToList() ;
        }

        /// <summary>
        /// Busca la variable correspondiente al grupo solisitado
        /// --08/06/2016 agregada por el cambio en las variables
        /// </summary>
        /// <param name="grupo">Grupo a buscar</param>
        /// <returns>Variable del grupo, o null en caso de no encontrarlo</returns>
        private Variable buscaGrupo(Grupo grupo)
        {
            foreach (Variable var in cromosomas)
                if (var.Grupo == grupo)
                    return var;

            return null;
        }

        #region nuevo
        /// <summary>
        /// Busca todos los Grupos que tengan asignado el salon
        /// La pense para buscar los grupos con el salon y hacer el horario, pero eso mejor se lo asigne al grupo individualmente.
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        private List<Variable> busca_Grupos(string salon)
        {
            var query = from v in cromosomas
                        where v.Salon.Cve_espacio==salon
                        select v;

            return query.ToList();
        }
        #endregion

        #endregion
    }
}
