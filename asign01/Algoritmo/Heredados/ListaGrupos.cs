using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Algoritmo01.Heredados
{
    public class ListaGrupos : OrigenDatos.Clases.ListaGrupos
    {
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

        internal ListaGrupos GruposPreferencial()
        {
            var query = from g in grupos
                               where g.Salon_fijo != null || g.Salones_posibles.Count != 0
                               select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        internal void Ejecuta(Func<Grupo, object> p)
        {
            foreach(Grupo g in grupos)
                p(g);
        }

        public ListaGrupos(Conexion c, DataTable dtGrupos, ListaSalones salones): base(c, dtGrupos, salones) {}

        public bool EnSalones(ListaSalones salones, Grupo grupo)
        {
            List<Salon> s = salones.GetList();

            foreach(Grupo g in grupos)
            {
                var query = from sal in s
                            where sal.Cve_espacio == g.Salon && grupo.SalonValido(sal)>0 && sal.Disponible_para_grupo(grupo)
                            select sal;

                if (query.Count<Salon>() != 0)
                    return true;
            }

            return false;
        }

        public Grupo GetGrupo(string cve_materia, int num_Grupo)
        {
            var query = from g in grupos
                        where g.Cve_materia == cve_materia && g.num_Grupo == num_Grupo
                        select g;

            if (query.Count() != 0)
                return (Grupo)query.ToList()[0];

            return null;
        }

        public void Update(Grupo g)
        {
            Grupo gAux = GetGrupo(g.Cve_materia, g.num_Grupo);

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
            foreach(Grupo grupo in grupos)
                Update(grupo);
        }
    }
}
