using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Algoritmo02.Heredados
{
    public class ListaGrupos : OrigenDatos.Clases.ListaGrupos
    {
        #region Constuctores
        public ListaGrupos() : base(){}

        public ListaGrupos(List<Grupo> grupos)
        {
            this.grupos = new List<OrigenDatos.Clases.Grupo>();
            foreach (Grupo g in grupos)
                this.grupos.Add(g);
        }

        public ListaGrupos(ListaGrupos grupos)
        {
            this.grupos = new List<OrigenDatos.Clases.Grupo>();
            foreach (Grupo g in grupos.grupos)
                this.grupos.Add(new Grupo(g));
        }

        public ListaGrupos(OrigenDatos.Clases.ListaGrupos grupos)
        {
            this.grupos = new List<OrigenDatos.Clases.Grupo>();
            foreach (Grupo g in grupos)
                this.grupos.Add(new Grupo(g));
        }

        public ListaGrupos(Conexion c, DataTable dtGrupos, ListaSalones salones) : base(c, dtGrupos, salones) { }
        #endregion

        #region consultas
        public ListaGrupos EnSalonesFijos()
        {
            var query = from g in grupos
                        where g.Salon_fijo == g.Salon
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos MejorPuntuacion(Salon s, int limite = 1)
        {
            var query = from g in grupos
                        where g.SalonValido(s) > 0
                        orderby g.SalonValido(s)
                        select (Grupo)g;

            return new ListaGrupos(query.Take(limite).ToList());
        }

        public ListaGrupos QuierenPlantabaja()
        {
            var query = from g in grupos
                        where g.PlantaBaja
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos OtrosSemestres(string salon)
        {
            var query = from g in grupos
                        where g.AsignacionSemestresAnteriores(salon)
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos Horario(int hora)
        {
            var query = from g in grupos
                        where ((Grupo)g).horario(hora)
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        public Grupo Busca(string cve_materia, int num_Grupo)
        {
            var query = from g in grupos
                        where g.Cve_materia == cve_materia && g.num_Grupo == num_Grupo
                        select g;

            if (query.Count() != 0)
                return (Grupo)query.ToList()[0];

            return null;
        }

        public Grupo MejorPara(Salon salon)
        {
            ListaGrupos aux = this;

            if (salon.plantaBaja)
                aux = aux.QuierenPlantabaja();

            //Salon de otros semestres
            if (aux.Count() > 1)
                aux = aux.OtrosSemestres(salon.Cve_espacio);

            //Mejor puntuacion de equipamiento
            if (aux.Count() > 1)
                aux = aux.MejorPuntuacion(salon);

            return aux.Count() != 0 ? (Grupo)aux[0] : null;
        }
        #endregion

        #region Operaciones
        internal void Ejecuta(Func<Grupo, object> p)
        {
            foreach(Grupo g in grupos)
                p(g);
        }

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
