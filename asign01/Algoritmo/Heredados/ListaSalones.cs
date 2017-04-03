using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Algoritmo01.Heredados
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

        /// <summary>
        /// Inicializa todos los componentes con algun valor
        /// </summary>
        /// <param name="c">Conexion a utilizar</param>
        /// <param name="salones">Lista de salones a utilizar</param>
        public ListaSalones(List<Salon> salones)
        {
            this.salones = new List<OrigenDatos.Clases.Salon>();
            foreach(Salon s in salones)
                this.salones.Add(s);
        }

        public ListaSalones(ListaSalones salones)
        {
            this.salones = new List<OrigenDatos.Clases.Salon>();
            foreach (Salon s in salones.salones)
                this.salones.Add(new Salon(s));
        }

        public ListaSalones(DataTable dt):base(dt){}

        /// <summary>
        /// Inicializa la coneccion y crea los salones a partir de una tabla
        /// </summary>
        /// <param name="c">Conexion a utilizar</param>
        /// <param name="dtSalones">Tabla de salones</param>
        public ListaSalones(Conexion c, DataTable dtSalones, int hora=0) : base(c,dtSalones,hora){}
        #endregion

        public List<Salon> GetList()
        {
            List<Salon> sal = new List<Salon>();
            foreach(Salon s in salones)
            {
                sal.Add(new Salon(s));
            }

            return sal;
        }

        public Salon GetSalon(int val)
        {
            return (Salon)(salones[val]);
        }

        public List<ListaGrupos> Agrupa(ListaGrupos g)
        {
            ListaGrupos aux;
            List<ListaGrupos> res = new List<ListaGrupos>();

            foreach (Salon s in salones)
            {
                aux = new ListaGrupos((ListaGrupos)g.GruposEnSalon(s.Cve_espacio).Grupos_Empalmados());

                if(aux.Count()!=0)
                    res.Add(aux);
            }

            return res;
        }

        #region consultas

        #endregion
    }
}
