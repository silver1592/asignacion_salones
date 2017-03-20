using System.Collections.Generic;
using System.Data;

namespace Interfaz_02.Heredados
{
    public class ListaGrupos : Algoritmo01.Heredados.ListaGrupos
    {
        public ListaGrupos() : base(){ }

        public ListaGrupos(List<Grupo> grupos)
        {
            this.grupos = new List<OrigenDatos.Clases.Grupo>();
            foreach (Grupo g in grupos)
                this.grupos.Add(g);
        }

        public ListaGrupos(Conexion c, DataTable dtGrupos, ListaSalones salones): base(c, dtGrupos, salones) { }
    }
}