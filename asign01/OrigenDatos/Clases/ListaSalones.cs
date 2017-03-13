
using System;
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
    public class ListaSalones
    {
        private List<Salon> salones;

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
            salones = new List<Salon>();
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

        #region consultas
        public Salon buscaSalon(string id_salon)
        {
            Salon res = null;

            foreach (Salon s in salones)
                if (s.Cve_espacio == id_salon)
                    res = s;

            return res;
        }

        public ListaSalones SalonesDisponibles(int hora_ini, int hora_fin, string dias)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.Disponible(hora_ini, hora_fin, dias)
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SalonesDisponibles(int[] hora_ini, int[] hora_fin)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.Disponible(hora_ini, hora_fin)
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SalonesConEquipo(int idEquipo)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.ContieneEquipo(idEquipo)
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SalonesArea(int area)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.PrioridadArea(area.ToString())>0
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SalonesEdifico(string edificio)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.Edificio == edificio
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SalonesValidos(Grupo g)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where g.SalonValido(s)>0
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SalonesPreferenciales(Grupo g)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where g.Salones_posibles.buscaSalon(s.Cve_espacio)!=null
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones SalonesSinEmpalmeCon(Grupo g)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        where s.EmpalmesCon(g).Count>0
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }

        public ListaSalones Salones(DataTable dt)
        {
            List<Salon> temp = new List<Salon>();

            var query = from Salon s in salones
                        from DataRow r in dt.Rows
                        where r["cve_espacio"].ToString() == s.Cve_espacio
                        select s;

            temp = query.ToList<Salon>();

            ListaSalones resultado = new ListaSalones(temp);
            return resultado;
        }
        #endregion
    }
}
