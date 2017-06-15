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

        public Materia buscaMateria(string cve_materia)
        {
            var query = from m in materias
                        where m.CVE == cve_materia
                        select m;

            if (query.Count() > 0)
                return (Materia)query.ToList()[0];
            else
                throw new Exception("No se encontro la materia");
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
            var query = from p in profesores
                        where p.RPE == Convert.ToInt32(RPE)
                        select p;

            if(query.Count() > 0)
                return (Profesor)query.ToList()[0];
            else
                throw new Exception("No se encontro el RPE");
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
    }
}