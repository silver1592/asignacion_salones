using System.Collections.Generic;
using System.Data;
using InterfazWeb_02.Models;
using System;
using System.Web;

namespace InterfazWeb_02.Clases
{
    public class Conexion : Algoritmo02.Heredados.Conexion
    {
        public Conexion(string datos) : base()
        {
            DatosConexion = datos;
        }

        public Conexion(string Datos, string excelDireccion = null, string ciclo = "2016-2017/II", string tipo = "") : base(Datos, excelDireccion, ciclo, tipo) { }

        public string[] Semestres()
        {
            string query = "SELECT distinct ciclo FROM[asignacion].[ae_horario] order by ciclo desc";
            List<string> res = new List<string>();

            DataTable dt = Querry(query);

            foreach (DataRow r in dt.Rows)
                res.Add(r[0].ToString());

            return res.ToArray();
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