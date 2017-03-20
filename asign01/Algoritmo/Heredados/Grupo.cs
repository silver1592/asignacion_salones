using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo01.Heredados
{
    public class Grupo:OrigenDatos.Clases.Grupo
    {
        #region Constructores
        /// <summary>
        /// Constructor usado cuando se crea a partir de un excel
        /// </summary>
        /// <param name=""></param>
        public Grupo(string cve_materia, string grupo, string rpe, string tipo, string salon, string li, string lf, string mi, string mf, string Mii, string Mif, string ji, string jf, string vi, string vf, string si, string sf, string cupo, string ciclo):base(cve_materia, grupo, rpe, tipo, salon, li, lf, mi, mf, Mii, Mif, ji, jf, vi, vf, si, sf, cupo, ciclo) { }
        public Grupo(DataRow grupo, DataTable necesidadesGrupo = null, DataTable necesidadesProfesor = null, DataTable salonesPosibles = null, ListaSalones salones = null) : base(grupo, necesidadesGrupo, necesidadesProfesor, salonesPosibles, salones) { }

        public Grupo(Grupo g) :base(g){ }

        public Grupo(DataRow r, Conexion c, ListaSalones salones):base(r, c, salones){ }
        #endregion

        #region Atributos

        public int valorTotalEquipo
        {
            get
            {
                int res = 0;

                foreach (Requerimiento_Valor req in requerimientos_Salon)
                    res = req.valor;

                return res;
            }
        }

        #endregion
        public bool horario(int hora)
        {
            if (lunes_ini != 0 && lunes_ini <= hora && lunes_fin > hora)
                return true;
            if (martes_ini != 0 && martes_ini <= hora && martes_fin > hora)
                return true;
            if (miercoles_ini != 0 && miercoles_ini <= hora && miercoles_fin > hora)
                return true;
            if (jueves_ini != 0 && jueves_ini <= hora && jueves_fin > hora)
                return true;
            if (viernes_ini != 0 && viernes_ini <= hora && viernes_fin > hora)
                return true;
            if (sabado_ini != 0 && sabado_ini <= hora && sabado_fin > hora)
                return true;

            return false;
        }

        /// <summary>
        /// Actualiza la tabla horario con los valores que tiene
        /// </summary>
        public void Update(Conexion c, string observaciones = "")
        {
            this.observaciones = observaciones;
            c.UpdateGrupo(this, observaciones);
        }

        /// <summary>
        /// Actualiza la tabla horario con los valores que tiene
        /// </summary>
        public void Update(string observaciones = "")
        {
            this.observaciones = observaciones;
        }

        public float valorEquipo(Salon salon)
        {
            int res = 0;

            foreach (Requerimiento_Valor req in Requerimientos)
                if (salon.Equipo.Contains(req.requerimiento))
                    res += req.valor;

            return res;
        }

        /// <summary>
        /// Checa si tiene un grupo con salon fijo, y de ser asi lo asigna, sino regresa false
        /// </summary>
        /// <returns></returns>
        public bool salonPreferencial(ListaSalones salones)
        {
            Salon _Salon;
            //Checa si tiene salon fijo
            if (salon_fijo != null && salon_fijo != "")
            {
                salon = salon_fijo;
                _Salon = salones.buscaSalon(salon);
                if (_Salon != null)
                    _Salon.agregaGrupo(this);

                return true;
            }

            return false;
        }

        public float AsignacionSemestreAnterior(int hora)
        {
            /*
            DataTable ciclos = Consultas.asignacionesSemestresAnteriores(cve_materia, rpe, tipo, salon, hora, ciclo);
            float res = -1;

            foreach (DataRow r in ciclos.Rows)
                if (res < cicloToFloat(r["ciclo"].ToString()))
                    res = cicloToFloat(r["ciclo"].ToString());

            return res;
            */
            return 0;
        }
   }
}
