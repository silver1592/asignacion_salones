using OrigenDatos.Clases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo02.Clases
{
    public class ListaVariables : ListaGrupos, IList<Variable>
    {
        #region IList
        Variable IList<Variable>.this[int index]
        {
            get
            {
                return new Variable(grupos[index],0);
            }

            set
            {
                grupos[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return grupos.Count;
            }
        }

        public void Add(Variable item)
        {
            grupos.Add(item);
        }

        public bool Contains(Variable item)
        {
            return grupos.Contains(item);
        }

        public void CopyTo(Variable[] array, int arrayIndex)
        {
            grupos.CopyTo(array, arrayIndex);
        }

        public int IndexOf(Variable item)
        {
            return grupos.IndexOf(item);
        }

        public void Insert(int index, Variable item)
        {
            grupos.Insert(index, item);
        }

        public bool Remove(Variable item)
        {
            return grupos.Remove(item);
        }

        IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
        {
            List<Variable> var = new List<Variable>();

            foreach (Grupo g in grupos)
                var.Add(new Variable(g,0));

            return var.GetEnumerator();
        }

        public void SetSalones(ListaSalones salones)
        {
            foreach(Variable v in this)
                v.Salon = salones.busca(v.Cve_espacio);
        }
        #endregion

        #region Constructores

        public ListaVariables() : base() { }
        public ListaVariables(ListaVariables grupos, IList<Profesor> Profesores=null, IList<Materia> Materias=null):base(grupos,Profesores!=null ? Profesores :grupos.profesores,Materias!=null ? Materias: grupos.materias){}

        public ListaVariables(IList<Variable> variables, IList<Profesor> Profesores = null, IList<Materia> Materias = null) : base()
        {
            grupos = new List<Grupo>();
            this.profesores = Profesores;
            this.materias = Materias;
            foreach (Variable v in variables)
                grupos.Add(v);
        }

        public ListaVariables(ListaGrupos grupos)
        {
            materias = grupos.Materias;
            profesores = grupos.Profesores;
            foreach (Grupo g in grupos)
                this.grupos.Add(new Variable(g,0));
        }
        #endregion

        /// <summary>
        /// Obtiene los grupos con mejor puntiacion con cierto salon
        /// </summary>
        /// <param name="s"></param>
        /// <param name="limite"></param>
        /// <returns></returns>
        public ListaVariables MejorPuntuacion(Salon s, int limite = 1)
        {
            var query = from g in this as IList<Variable>
                        where g.SalonValido(s) > 0
                        orderby g.SalonValido(s) descending
                        select g;

            return new ListaVariables(query.Take(limite).ToList());
        }

        public ListaVariables Empalmados()
        {
            var query = from g in this as IList<Variable>
                        from g1 in grupos
                        where g.cve_full!=g1.cve_full && g.empalme(g1)
                        select g;

            return new ListaVariables(query.Distinct().ToList());
        }

        public ListaVariables Empalmados(Grupo grupo)
        {
            var query = from g in this as IList<Variable>
                        where g.empalme(grupo)
                        select g;

            return new ListaVariables(query.ToList());
        }

        public ListaVariables EnHoras(int hora_ini, int hora_fin, string salon, string dias)
        {
            var query = from g in this as IList<Variable>
                        where g.EnHora(hora_ini, hora_fin, dias) && g.Cve_espacio == salon
                        select g;

            return new ListaVariables(query.ToList());
        }

        public ListaVariables EnHoras(int hora_ini, int hora_fin)
        {
            var query = from g in this as IList<Variable>
                        where g.EnHora(hora_ini, hora_fin)
                        select g;

            return new ListaVariables(query.ToList());
        }

        #region Consultas Salones
        /// <summary>
        /// busca salon a salon los que esten ocupados entre las horas designadas y los dias
        /// </summary>
        /// <param name="salones">Grupo de salones validos para checar</param>
        /// <param name="ini">hora inicial para el rango de horas</param>
        /// <param name="fin">hora final para el rango de horas</param>
        /// <param name="dias">dias que se van a buscar. L-M-Mi-J-V-S Marcar con un 1 los dias que quieres obtener</param>
        /// <returns></returns>
        public ListaSalones Ocupados(ListaSalones salones, int ini, int fin, string dias = "111111")
        {
            var query = from s in salones
                        where EnHoras(ini, fin, s.Cve_espacio, dias).Count == 0
                        select s;

            return new ListaSalones(query.ToList());
        }
        #endregion

        /// <summary>
        /// Checa cada salon y obtiene los grupos que esten empalmados y los mete en una lista
        /// </summary>
        /// <param name="g">Lista de grupos</param>
        /// <returns>Lista de grupos empalmados por salon</returns>
        public List<ListaVariables> AgrupaGruposEmpalmados()
        {
            var query = from g in Agrupados_Salon()
                        where g.Count()!=0 && (g[0].Cve_espacio!="" && g[0].Cve_espacio!=null)
                        select new ListaVariables(g);

            var query2 = from g in query
                         select g.Empalmados();

            var query3 = from g in query2
                         where g.Count > 1
                         select g;

            return query3.ToList();
        }

        /// <summary>
        /// Obtiene el grupo que tenga mas puntos para el salon
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public Variable MejorPara(Salon salon)
        {
            ListaVariables aux = this;

            if (salon.plantaBaja)
                aux = new ListaVariables(aux.QuierenPlantabaja());
            if (aux.Count == 0)
                aux = this;

            //Mejor puntuacion de equipamiento
            if (aux.Count > 1)
                aux = aux.MejorPuntuacion(salon);

            //Salon de otros semestres
            if (aux.Count > 1)
                aux = new ListaVariables(aux.AsignacionOtrosSemestres(salon.Cve_espacio));
            if (aux.Count == 0)
                aux = this;

            return aux.Count != 0 ? (aux as IList<Variable>)[0] : null;
        }

        /// <summary>
        /// Obtiene los grupos que esten asignados a tales dias
        /// </summary>
        /// <param name="dias">Cadena de boleanos que indican los dias (LMmiJVS)</param>
        /// <returns></returns>
        public ListaVariables EnDias(string dias = "111111")
        {
            var query = from g in this as IList<Variable>
                        where g.EnDias(dias)
                        select g;

            return new ListaVariables(query.ToList(), profesores, materias);
        }

        public ListaVariables EnDia(int dia)
        {
            var query = from g in this as IList<Variable>
                        where g.EnDia(dia)
                        select g;

            return new ListaVariables(query.ToList(), profesores, materias);
        }

        public ListaVariables OrdenarPorCiclo()
        {
            var query = from g in this as IList<Variable>
                        orderby g.fCiclo descending
                        select g;

            return new ListaVariables(query.ToList(),profesores,materias);
        }
    }
}