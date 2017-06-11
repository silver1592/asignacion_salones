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
            datosConexion = datos;
            string dir = controller.Server.MapPath("~/Archivos/");
            string file = controller.Session["excel"].ToString();
            string sheet = controller.Session["sheet"].ToString();
            string ciclo = controller.Session["ciclo"].ToString();
            bool bd = Convert.ToBoolean(controller.Session["usaExcel"].ToString());

            if (!bd)
            {
                Excel = new LibroExcel(dir, file, ciclo, "T");
                Excel.setHojaHorarios(sheet);
            }
        }

        public Conexion(string Datos, string excelDireccion = null, string archivoEntrada = null, string hoja = "SIAMDIF", string ciclo = "2016-2017/II", string tipo = "") : base(Datos, excelDireccion, archivoEntrada, hoja, ciclo, tipo) { }

        public ListaGrupos GetGrupos(string semestre)
        {
            ListaGrupos res = null;
            List<Materia> materias = GetMaterias();
            List<Profesor> profesores = GetProfesores();

            if (!excel)
            {
                DataTable dt = Querry("SELECT * FROM[asignacion].[ae_horario] where ciclo = '" + semestre + "'");
                res = new ListaGrupos(Excel.AsList(dt),materias,profesores);
            }
            else
            {
                res = new ListaGrupos(Excel.Grupos, materias, profesores);
            }

            return res;
        }

        public List<Materia> GetMaterias()
        {
            List<Materia> materias = new List<Materia>();
            DataTable dt = Querry("SELECT * FROM [asignacion].[dbo].[vae_cat_materia]");

            foreach (DataRow r in dt.Rows)
                materias.Add(new Materia(r));

            return materias;
        }

        public List<Profesor> GetProfesores()
        {
            List<Profesor> profesores = new List<Profesor>();
            DataTable dt = Querry("SELECT * FROM [asignacion].[dbo].[vae_cat_profesor]");

            foreach (DataRow r in dt.Rows)
                profesores.Add(new Profesor(r));

            return profesores;
        }

        public bool ExisteBD(Grupo g)
        {
            //TODO: Checar quuery
            string query = "select * from ae_horario where cve_materia="+g.Cve_materia+ " and grupo="+g.num_Grupo+" and ciclo='"+ g.Ciclo+"'";

            DataTable dt = Querry(query);

            return dt.Rows.Count != 0;
        }

        internal void Insert(Grupo g)
        {
            string query = "insert into ae_horario (" + ") values (" + ")";

            Comando(query);
        }
    }
}