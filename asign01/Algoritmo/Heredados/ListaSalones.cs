using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Algoritmo02.Heredados
{
    /// <summary>
    /// Esta clase se hiso para no estar manejando tantas listas y para que la hisiera de filtro o buscador para siertas situaciones
    /// </summary>
    public class ListaSalones : OrigenDatos.Clases.ListaSalones
    {
        #region Constructores e inicializadores
        /// <summary>
        /// Solo crea la lista, falta inicializar la conexion
        /// </summary>
        public ListaSalones() : base () { }

        public ListaSalones(IList<OrigenDatos.Clases.Salon> salones)
        {
            this.salones = new List<OrigenDatos.Clases.Salon>();
            foreach (OrigenDatos.Clases.Salon s in salones)
                this.salones.Add(new Salon(s));
        }

        /// <summary>
        /// Inicializa la coneccion y crea los salones a partir de una tabla
        /// </summary>
        /// <param name="c">Conexion a utilizar</param>
        /// <param name="dtSalones">Tabla de salones</param>
        public ListaSalones(Conexion c, DataTable dtSalones, int hora=0) : base(c,dtSalones,hora){}
        #endregion

        #region consultas
        /// <summary>
        /// Checa cada salon y obtiene los grupos que esten empalmados y los mete en una lista
        /// </summary>
        /// <param name="g">Lista de grupos</param>
        /// <returns>Lista de grupos empalmados por salon</returns>
        public List<ListaGrupos> AgrupaGruposEmpalmados(ListaGrupos g)
        {
            ListaGrupos aux;
            List<ListaGrupos> res = new List<ListaGrupos>();

            foreach (OrigenDatos.Clases.Salon s in salones)
            {
                aux = new ListaGrupos(g.EnSalon(s.Cve_espacio).Empalmados());

                if(aux.Count()!=0)
                    res.Add(aux);
            }

            return res;
        }
        #endregion
    }
}
