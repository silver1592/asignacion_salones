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
        public ListaGrupos() : base() { }

        public ListaGrupos(List<OrigenDatos.Clases.Grupo> grupos)
        {
            this.grupos = new List<OrigenDatos.Clases.Grupo>();
            foreach (Grupo g in grupos)
                this.grupos.Add(new Grupo(g));
        }

        public ListaGrupos(ListaGrupos grupos)
        {
            this.grupos = new List<OrigenDatos.Clases.Grupo>();
            foreach (Grupo g in grupos.grupos)
                this.grupos.Add(new Grupo(g));
        }

        public List<Grupo> List()
        {
            List<Grupo> list = new List<Grupo>();

            foreach (Grupo g in grupos)
                list.Add(g);

            return list;
        }
    }
}