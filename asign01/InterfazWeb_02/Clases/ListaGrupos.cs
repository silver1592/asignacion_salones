using InterfazWeb_02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Algoritmo02;

namespace InterfazWeb_02.Clases
{
    public class ListaGrupos : Algoritmo02.Heredados.ListaGrupos
    {
        public ListaGrupos(Algoritmo02.Heredados.ListaGrupos grupos)
        {
            materias = grupos.Materias;
            profesores = grupos.Profesores;
            this.grupos = new List<OrigenDatos.Clases.Grupo>();

            foreach (OrigenDatos.Clases.Grupo g in grupos)
                this.grupos.Add(new Grupo(g));
        }

        public ListaGrupos(List<OrigenDatos.Clases.Grupo> grupos) : base(grupos) { }

        public Materia buscaMateria(string cve_materia)
        {
            var query = from m in materias
                        where m.CVE == cve_materia
                        select m;

            if (query.Count() > 0)
                return new Materia(query.ToList()[0]);
            else
                throw new Exception("No se encontro la materia");
        }

        internal ListaGrupos ImpartenMateria(string cve)
        {
            var query = from g in this
                        where g.Cve_materia == cve
                        select g;

            return new ListaGrupos(query.ToList());
        }

        internal ListaGrupos NoGrupo(int noGrupo)
        {
            var query = from g in this
                        where g.num_Grupo == noGrupo
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos NoRepetidos()
        {
            var query = from g in this
                        select g;

            return new ListaGrupos(query.GroupBy(p=> new {p.Cve_materia, p.num_Grupo, p.Ciclo}).Select(g=>g.First()).ToList());
        }

        /// <summary>
        /// Busca la materia del grupo que se encuentra en el indice [index]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Materia buscaMateria(int index)
        {
            Grupo g = (Grupo)this[index];

            return buscaMateria(g.Cve_materia);
        }

        public Profesor buscaProfesor(string RPE)
        {
            int rpe = Convert.ToInt32(RPE);
            var query = from p in profesores
                        where p.RPE == rpe
                        select p;

            if (query.Count() > 0)
                return new Profesor(query.ToList()[0]);
            else
                return new Profesor(rpe);
                //throw new Exception("No se encontro el RPE");
        }

        /// <summary>
        /// Busca al profesor del grupo que se encuentra en el indice [index]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Profesor buscaProfesor(int index)
        {
            Grupo g = (Grupo)this[index];

            return buscaProfesor(g.RPE.ToString());
        }

        /// <summary>
        /// Obtiene los grupos que esten asignados a tales dias
        /// </summary>
        /// <param name="dias">Cadena de boleanos que indican los dias (LMmiJVS)</param>
        /// <returns></returns>
        public ListaGrupos EnDias(string dias="111111")
        {
            var query = from g in this
                        where g.EnDias(dias)
                        select (OrigenDatos.Clases.Grupo)g;

            return new ListaGrupos(query.ToList());
        }
    }
}