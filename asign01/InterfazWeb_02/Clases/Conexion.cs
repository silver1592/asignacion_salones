using System.Collections.Generic;
using System.Data;
using InterfazWeb_02.Models;
using System;
using System.Web;

namespace InterfazWeb_02.Clases
{
    public class Conexion : Algoritmo02.Heredados.Conexion
    {
        public Conexion(string datos, System.Web.Mvc.Controller controller) : base()
        {
            DatosConexion = datos;
            string dir = controller.Server.MapPath("~/Archivos/");
            string file = controller.Session["excel"] != null ? controller.Session["excel"].ToString() : "";
            this.Sheet = controller.Session["sheet"] != null ? controller.Session["sheet"].ToString() : "";
            string ciclo = controller.Session["ciclo"].ToString();
            bool bd = !Convert.ToBoolean(controller.Session["usaExcel"].ToString());

            if (!bd)
                Excel = new LibroExcel(dir, file, ciclo, "T");
            else if(!ExisteSemestre(ciclo))
                throw new Exception("No hay datos de ese semestre");

        }

        public Conexion(string Datos, string excelDireccion = null, string ciclo = "2016-2017/II", string tipo = "") : base(Datos, excelDireccion, ciclo, tipo) { }

        internal string[] Sheets()
        {
            return Excel.GetStringSheets();
        }

        public bool ExisteBD(Grupo g)
        {
            string query = "select * from ae_horario where cve_materia="+g.Cve_materia+ " and grupo="+g.num_Grupo+" and ciclo='"+ g.Ciclo+"'";

            DataTable dt = Querry(query);

            return dt.Rows.Count != 0;
        }

        public bool ExisteSemestre(string semestre)
        {
            DataTable dt = Querry("select count(*) from ae_horario where ciclo = '" + semestre + "'");
            if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 0)
                return false;

            return true;

        }

        internal void Insert(Grupo g)
        {
            string query = "insert into ae_horario (" + ") values (" + ")";

            Comando(query);
        }

        /// <summary>
        /// Inserta Grupos en la base de datos
        /// </summary>
        /// <param name="grupos"></param>
        public void InsertaGrupos(ListaGrupos grupos)
        {

        }
    }
}