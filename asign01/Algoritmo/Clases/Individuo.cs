using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OrigenDatos.Clases;

namespace Algoritmo02.Clases
{
    /// <summary>
    /// Esta clase es para manipular la informacion que los individuos dan al algoritmo
    /// --02/06/2016 Aqui se crearan los metodos para la mutacion, seleccion y asignacion del individuo (solo los ciclos)
    /// </summary>
    public class Individuo : IList<Variable>
    {
        #region IList
        public bool IsReadOnly
        {
            get
            {
                return ((IList<Variable>)cromosomas).IsReadOnly;
            }
        }

        public Variable this[int index]
        {
            get
            {
                return ((IList<Variable>)cromosomas)[index];
            }

            set
            {
                ((IList<Variable>)cromosomas)[index] = value;
            }
        }

        public int IndexOf(Variable item)
        {
            return ((IList<Variable>)cromosomas).IndexOf(item);
        }

        public void Insert(int index, Variable item)
        {
            ((IList<Variable>)cromosomas).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Variable>)cromosomas).RemoveAt(index);
        }

        public void Add(Variable item)
        {
            ((IList<Variable>)cromosomas).Add(item);
        }

        public void Clear()
        {
            ((IList<Variable>)cromosomas).Clear();
        }

        public bool Contains(Variable item)
        {
            return ((IList<Variable>)cromosomas).Contains(item);
        }

        public void CopyTo(Variable[] array, int arrayIndex)
        {
            ((IList<Variable>)cromosomas).CopyTo(array, arrayIndex);
        }

        public bool Remove(Variable item)
        {
            return ((IList<Variable>)cromosomas).Remove(item);
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            return ((IList<Variable>)cromosomas).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Variable>)cromosomas).GetEnumerator();
        }
        #endregion

        #region Atributos
        private List<Variable> cromosomas;
        private ListaSalones salones;
        private List<Variable> Errores;
        private int cromosomas_a_Mutar = 5;     //Cantidad de grupos a mutar
        private int intentos = 10;  //Cantidad de intentos que tiene un grupo para encontrar un salon valido

        public ListaGrupos Grupos
        {
            get
            {
                ListaGrupos lista = new ListaGrupos();

                foreach (Variable v in cromosomas)
                    lista.Add(v);

                return lista;
            }
        }

        /// <summary>
        /// Retorna el valor obtenido para la funcion objetivo del individuo
        /// </summary>
        public float valor
        {
            get
            {
                float fRes = 0;

                foreach (Variable v in cromosomas)
                    fRes += v.Puntos;

                return fRes;
            }
        }

        public int Count
        {
            get
            {
                return ((IList<Variable>)cromosomas).Count;
            }
        }
        #endregion

        #region Constructores
        /// <summary>
        /// Constructor del Individuo. 
        /// Inicializa el arreglo de cromosomas.
        /// </summary>
        /// <param name="salonesDisponibles">Lista de salones a considerar para la interacion del algoritmo</param>
        public Individuo(ListaGrupos gruposPorAsignar,int  hora)
        {
            cromosomas = new List<Variable>();

            foreach(Grupo g in gruposPorAsignar)
                cromosomas.Add(new Variable(g,hora));
        }

        public Individuo(Individuo individuo)
        {
            cromosomas = new List<Variable>();
            salones = new ListaSalones(individuo.salones);
            foreach (Variable v in individuo.cromosomas)
                cromosomas.Add(new Variable(v));
        }
        #endregion

        #region Asignacion de Salones
        /// <summary>
        /// Asigna los grupos a los salones usando un random y los salones marcados.
        /// --08/06/2016 Modifique la parte de las variables y tambien los metodos, este ahora asinara salones a los grupos.
        /// </summary>
        public void asignaSalones(ListaSalones Salones)
        {
            Errores = new List<Variable>();

            salones = new ListaSalones(Salones);

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
                        salon = salones[val];
                        if (v.Cupo < salon.Cupo && salon.Disponible_para_grupo(v))
                        {
                            v.Salon = salon;
                            r.Reinicia();
                            break;
                        }
                    }
                    else
                    {
                        r.Reinicia();
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
            List<Variable> asignando;

            var query1 = from g in cromosomas
                         where g.Area == i.ToString()
                         orderby g.Cupo descending
                         select g;

            asignando = query1.ToList();

            if (asignando.Count != 0)
                asignandoXArea(asignando, new ListaSalones(salones.EnArea(i)));
        }

        private void asignandoXArea(List<Variable> asignando, ListaSalones s)
        {
            Grupo gAnterior;
            Salon sAnterior;

            foreach (Variable v in asignando)
            {
                gAnterior = v.GHoraAnterior;
                if (gAnterior== null)
                    asignacionAleatoria(v, s);
                else
                {
                    sAnterior = s.busca(gAnterior.Cve_espacio);
                    if (sAnterior!=null && sAnterior.Disponible_para_grupo(v))
                        v.Salon = sAnterior;
                    else
                        asignacionAleatoria(v, s);
                }
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
                    salon = s[val];
                    if (v.EsValido(salon) && salon.Disponible_para_grupo(v))
                    {
                        v.Salon = salon;
                        r.Reinicia();
                        break;
                    }
                }
                else
                {
                    Errores.Add(v);
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
        public void Mutacion()
        {
            Aleatorio rGrupo = new Aleatorio(cromosomas.Count);
            Aleatorio rSalon;
            int iSalonSelec=0, iGrupoSelec,i;
            Variable grupo;

            for (int c = 0; c < cromosomas_a_Mutar && c < cromosomas.Count; c++)
            {
                //Selecciona un grupo aleatoriamente
                iGrupoSelec = rGrupo.Next();
                rGrupo.aceptado(iGrupoSelec);
                grupo = cromosomas[iGrupoSelec];

                rSalon = new Aleatorio(salones.Count);

                //loop intentos
                for (i=0;i<intentos;i++)
                {
                    iSalonSelec = rSalon.Next();    //selecciona un salon aleatoriamente
                    if (iSalonSelec == -1) break;   //Si ya se recorrieron todos los salones entonces lo deja como esta

                    if (AsignaEnSalon(grupo, salones[iSalonSelec]))
                        break;
                }
            }
        }

        private float promedio(List<Variable> lista)
        {
            float f = 0 ;

            foreach (Variable v in lista)
                f += v.Puntos;

            f = f / lista.Count;

            return f;
        }

        /// <summary>
        /// Checa si el salon se puede asignar, y si no evalua los cambios que hay que hacer y si los hace
        /// </summary>
        /// <param name="grupo">Grupo a asignar</param>
        /// <param name="s">Salon en que se quiere asignar</param>
        /// <returns></returns>
        private bool AsignaEnSalon(Variable grupo, Salon s)
        {
            try
            {
                //Checa si es apto para el grupo
                if (grupo.CalculaPuntos(s) <= 0 || 
                    grupo.Cupo >= s.Cupo)
                    return false;

                if (s.Disponible(grupo.horario))
                {
                    grupo.Salon = s;
                    return true;
                }

                //Obtiene los grupos que estan asignados de este salon
                ListaVariables enSalon = new ListaVariables(Grupos.EnSalon(s.Cve_espacio));

                //Obtiene los grupos con los que se crusaria
                ListaVariables conEmpalme = enSalon.Empalmados(grupo);

                //Compara el maximo puntaje de los grupos contra el que tendria estre grupo
                if (grupo.CalculaPuntos(s) > conEmpalme.MaxPuntos())
                {
                    //Quita la asignacon de los grupos y asigna al nuevo grupo
                    foreach (Variable g in conEmpalme)
                        g.Salon = null;

                    if (s.Disponible(grupo.horario))    //Checa si no hay problemas con grupos de otras horas
                    {
                        Grupos.Actualiza(conEmpalme);   //actualiza los grupos en otros horarios

                        grupo.Salon = s;

                        return true;
                    }
                    else
                    {
                        foreach (Variable g in conEmpalme)
                            g.Salon = s;
                    }
                }
            }
            catch (Exception) { }

            return false;
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
                if (var == grupo)
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