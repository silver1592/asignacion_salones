
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrigenDatos.Clases
{
    /// <summary>
    /// Esta clase se hiso para no estar manejando tantas listas y para que la hisiera de filtro o buscador para siertas situaciones
    /// </summary>
    public class ListaSalones : IList<Salon>
    {
        protected List<Salon> salones;

        #region IList
        public int Count { get { return salones.Count; } }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<Salon>)salones).IsReadOnly;
            }
        }

        public Salon this[int index]
        {
            get
            {
                return ((IList<Salon>)salones)[index];
            }

            set
            {
                ((IList<Salon>)salones)[index] = value;
            }
        }

        public int IndexOf(Salon item)
        {
            return ((IList<Salon>)salones).IndexOf(item);
        }

        public void Insert(int index, Salon item)
        {
            ((IList<Salon>)salones).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Salon>)salones).RemoveAt(index);
        }

        public void Add(Salon item)
        {
            ((IList<Salon>)salones).Add(item);
        }

        public void Clear()
        {
            ((IList<Salon>)salones).Clear();
        }

        public bool Contains(Salon item)
        {
            return ((IList<Salon>)salones).Contains(item);
        }

        public void CopyTo(Salon[] array, int arrayIndex)
        {
            ((IList<Salon>)salones).CopyTo(array, arrayIndex);
        }

        public bool Remove(Salon item)
        {
            return ((IList<Salon>)salones).Remove(item);
        }

        public IEnumerator<Salon> GetEnumerator()
        {
            return ((IList<Salon>)salones).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Salon>)salones).GetEnumerator();
        }

        #endregion

        #region Constructores e inicializadores
        /// <summary>
        /// Solo crea la lista, falta inicializar la conexion
        /// </summary>
        public ListaSalones()
        {
            salones = new List<Salon>();
        }

        /// <summary>
        /// Inicializa todos los componentes con algun valor
        /// </summary>
        /// <param name="c">Conexion a utilizar</param>
        /// <param name="salones">Lista de salones a utilizar</param>
        public ListaSalones(List<Salon> salones)
        {
            SetSalones(salones);
        }

        public ListaSalones(DataTable dt)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        from DataRow r in dt.Rows
                        where r["cve_espacio"].ToString() == s.Cve_espacio
                        select s;

            temp = query.ToList<Salon>();

            SetSalones(temp);
        }

        /// <summary>
        /// Inicializa la coneccion y crea los salones a partir de una tabla
        /// </summary>
        /// <param name="c">Conexion a utilizar</param>
        /// <param name="dtSalones">Tabla de salones</param>
        public ListaSalones(Conexion c, DataTable dtSalones, int hora=0)
        {
            salones = new List<Salon>();

            foreach(DataRow rSalon in dtSalones.Rows)
                salones.Add(Salon.ToSalon(rSalon, hora, c));
        }

        public void SetSalones(List<Salon> salones)
        {
            this.salones = salones;
        }
        #endregion

        #region basico
        public Salon Get(int index)
        {
            return salones[index];
        }

        public Salon busca(string id_salon)
        {
            Salon res = null;

            foreach (Salon s in salones)
                if (s.Cve_espacio == id_salon)
                    res = s;

            return res;
        }
        #endregion

        #region consultas
        public ListaSalones Disponibles(int hora_ini, int hora_fin, string dias)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.Disponible(hora_ini, hora_fin, dias)
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones Disponibles(int[] hora_ini, int[] hora_fin)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.Disponible(hora_ini, hora_fin)
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones ConEquipo(int idEquipo)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.ContieneEquipo(idEquipo)
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones EnArea(int area)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.PrioridadArea(area.ToString())>0
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones EnEdifico(string edificio)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.Edificio == edificio
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones Validos(Grupo g)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where g.SalonValido(s)>0
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones Preferenciales(Grupo g)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where g.Salones_posibles.busca(s.Cve_espacio)!=null
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SinEmpalmeCon(Grupo g)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.EmpalmesCon(g).Count()>0
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }
        #endregion
    }
}
