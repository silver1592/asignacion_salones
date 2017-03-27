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
    }
}
