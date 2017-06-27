using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Algoritmo02.Clases;

namespace Algoritmo02.Heredados
{
    public class ListaGrupos : OrigenDatos.Clases.ListaGrupos
    {
        protected List<Profesor> profesores;
        protected List<Materia> materias;

        public List<Profesor> Profesores { get { return profesores; } }
        public List<Materia> Materias { get { return materias; } }

        #region Constuctores
        public ListaGrupos() : base(){}

        public ListaGrupos(IList<OrigenDatos.Clases.Grupo> grupos)
        {
            this.grupos = new List<OrigenDatos.Clases.Grupo>();
            foreach (Grupo g in grupos)
                this.grupos.Add(new Grupo(g));
        }

        public void Actualiza(ListaGrupos _grupos)
        {
            if (this == _grupos)
                return;

            Grupo temp;
            int index=0;

            foreach(Grupo g in _grupos)
            {
                temp = Busca(g.Cve_materia, g.num_Grupo);
                index = IndexOf(temp);
                grupos[index] = g;
            }
        }

        public ListaGrupos(Conexion c, DataTable dtGrupos, ListaSalones salones) : base(c, dtGrupos, salones) { }

        public ListaGrupos(IList<OrigenDatos.Clases.Grupo> grupos, List<Materia> materias, List<Profesor> profesores, Conexion c=null, ListaSalones salones=null):base()
        {
            this.materias = materias;
            this.profesores = profesores;

            this.grupos = new List<OrigenDatos.Clases.Grupo>();

            foreach (OrigenDatos.Clases.Grupo g in grupos)
                this.grupos.Add(new Grupo(g,c,salones));
        }
        #endregion

        #region consultas
        /// <summary>
        /// Obtiene los Grupos que requieran un salon en especifico
        /// </summary>
        /// <returns></returns>
        public ListaGrupos EnSalonesFijos()
        {
            var query = from g in grupos
                        where g.Salon_fijo == g.Salon
                        select (OrigenDatos.Clases.Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene los grupos con mejor puntiacion con cierto salon
        /// </summary>
        /// <param name="s"></param>
        /// <param name="limite"></param>
        /// <returns></returns>
        public ListaGrupos MejorPuntuacion(Salon s, int limite = 1)
        {
            var query = from g in grupos
                        where g.SalonValido(s) > 0
                        orderby g.SalonValido(s)
                        select (OrigenDatos.Clases.Grupo)g;

            return new ListaGrupos(query.Take(limite).ToList());
        }

        /// <summary>
        /// Obtiene los grupos que requieren planta baja
        /// </summary>
        /// <returns></returns>
        public ListaGrupos QuierenPlantabaja()
        {
            var query = from g in grupos
                        where g.PlantaBaja
                        select (OrigenDatos.Clases.Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene los grupos que han estado en el salon en otros semestres
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public ListaGrupos AsignacionOtrosSemestres(string salon)
        {
            var query = from g in grupos
                        where g.AsignacionSemestresAnteriores(salon)!=null
                        select (OrigenDatos.Clases.Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene los grupos que inicial a cierta hora
        /// </summary>
        /// <param name="hola"></param>
        /// <returns></returns>
        public ListaGrupos InicialALas(int hora)
        {
            var query = from g in this
                        where g.hora_ini == hora
                        select g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene el grupo que coincida
        /// </summary>
        /// <param name="cve_materia"></param>
        /// <param name="num_Grupo"></param>
        /// <returns></returns>
        public Grupo Busca(string cve_materia, int num_Grupo)
        {
            var query = from g in grupos
                        where g.Cve_materia == cve_materia && g.num_Grupo == num_Grupo
                        select g;

            if (query.Count() != 0)
                return (Grupo)query.ToList()[0];

            return null;
        }

        /// <summary>
        /// Obtiene el grupo que tenga mas puntos para el salon
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public Grupo MejorPara(Salon salon)
        {
            ListaGrupos aux = this;

            if (salon.plantaBaja)
                aux = aux.QuierenPlantabaja();

            //Salon de otros semestres
            if (aux.Count() > 1)
                aux = aux.AsignacionOtrosSemestres(salon.Cve_espacio);

            //Mejor puntuacion de equipamiento
            if (aux.Count() > 1)
                aux = aux.MejorPuntuacion(salon);

            return aux.Count() != 0 ? (Grupo)aux[0] : null;
        }
        #endregion

        #region Operaciones
        public void Update(Grupo g)
        {
            Grupo gAux = Busca(g.Cve_materia, g.num_Grupo);

            if (gAux != null)
            {
                grupos.Remove(gAux);
                grupos.Add(g);
            }
            else
            {
                throw new Exception("Error al actualizar un grupo. El grupo solicitado no se encuentra (A01-H-LG-UPDATE)");
            }
        }

        public void Update(ListaGrupos g)
        {
            foreach (Grupo grupo in grupos)
                Update(grupo);
        }
        #endregion

        public bool EnSalones(ListaSalones salones, Grupo grupo)
        {
            foreach(Grupo g in grupos)
            {
                var query = from Salon sal in salones
                            where sal.Cve_espacio == g.Salon && grupo.SalonValido(sal)>0 && sal.Disponible_para_grupo(grupo)
                            select sal;

                if (query.Count() != 0)
                    return true;
            }

            return false;
        }
    }
}
