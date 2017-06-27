using OrigenDatos.Estructuras;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo02.Heredados
{
    public class Grupo:OrigenDatos.Clases.Grupo
    {
        #region Constructores
        public Grupo(OrigenDatos.Clases.Grupo g,Conexion c=null, ListaSalones salones=null) :base(g,c,salones){ }

        public Grupo(DataRow r, IDictionary<string, string> h, IDictionary<string, string> def=null, Conexion c=null, ListaSalones salones=null) : base(r,h,def,c,salones) { }
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

        public string salonAnioPasado()
        {
            Grupo sel =null;
            Grupo gAux;
            foreach (OrigenDatos.Clases.Grupo g in otrosSemestres)
            {
                gAux = new Grupo(g);
                if (gAux.fCiclo % 2 == this.fCiclo % 2)
                    if (sel == null || sel.fCiclo < gAux.fCiclo)
                        sel = gAux;
            }

            return sel != null ? sel.ciclo : "";
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
        public bool salonPreferencial()
        {
            if (salon_fijo != null && salon_fijo != "")
            {
                salon = salon_fijo;

                return true;
            }

            return false;
        }
   }
}
