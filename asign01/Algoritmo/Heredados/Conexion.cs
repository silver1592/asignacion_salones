using Algoritmo02.Clases;
using System;
using System.Collections.Generic;
using System.Data;

namespace Algoritmo02.Heredados
{
    public class Conexion : OrigenDatos.Clases.Conexion
    {
        public Conexion() : base() { }

        public Conexion(string Datos, string excelDireccion = null, string ciclo = "2016-2017/II", string tipo = ""): base(Datos, excelDireccion, ciclo,tipo){}

        /// <summary>
        /// Obtiene los grupos de la base de datos o del excel
        /// </summary>
        /// <param name="semestre">Semestre del cual se obtendran los grupos, si es excel los inicializara a ese semestre</param>
        /// <param name="ini"></param>
        /// <param name="fin"></param>
        /// <param name="bExcel"></param>
        /// <returns></returns>
        public ListaGrupos GetGrupos(string semestre, int ini = 7, int fin=22,bool bExcel=true)
        {
            ListaGrupos res = null;
            List<OrigenDatos.Clases.Grupo> grupos;
            List<Materia> materias = GetMaterias();
            List<Profesor> profesores = GetProfesores();

            if (Excel==null || !bExcel)
            {
                DataTable dt = Querry("SELECT DISTINCT * FROM  [asignacion].[Grupos_a_las] (" + ini+","+fin+") where ciclo = '" + semestre + "'");

                grupos = AsList(dt);
                res = new ListaGrupos(grupos, materias, profesores,this);
            }
            else
            {
                res = new ListaGrupos(Excel.GetGrupos(hoja), materias, profesores,this);
            }

            return res;
        }

        public ListaGrupos GetGruposIni(string semestre, int ini, bool bExcel)
        {
            ListaGrupos res = null;
            List<OrigenDatos.Clases.Grupo> grupos;
            List<Algoritmo02.Clases.Materia> materias = GetMaterias();
            List<Algoritmo02.Clases.Profesor> profesores = GetProfesores();

            if (Excel == null || !bExcel)
            {
                DataTable dt = Querry("SELECT DISTINCT *  FROM [asignacion].[ae_Grupos_ini] (" + ini + ") where ciclo = '" + semestre + "'");

                grupos = AsList(dt);
                res = new ListaGrupos(grupos, materias, profesores, this);
            }
            else
            {
                res = new ListaGrupos(Excel.GetGrupos(hoja), materias, profesores, this);
            }

            return res;
        }
        public ListaGrupos GetLightGrupos(string semestre, int ini=7, int fin=22,bool bExcel = true)
        {
            ListaGrupos res = null;
            List<OrigenDatos.Clases.Grupo> grupos;
            List<Materia> materias = GetMaterias();
            List<Profesor> profesores = GetProfesores();

            if (Excel == null || !bExcel)
            {
                DataTable dt = Querry("SELECT * FROM  [asignacion].[Grupos_a_las] (" + ini + "," + fin + ") where ciclo = '" + semestre + "'");

                grupos = AsList(dt);
                res = new ListaGrupos(grupos, materias, profesores);
            }
            else
            {
                res = new ListaGrupos(Excel.GetGrupos(hoja), materias, profesores);
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

        public Dictionary<string, string> GetMateriasAsDictionary()
        {
            Dictionary<string,string> materias = new Dictionary<string, string>();
            DataTable dt = Querry("SELECT * FROM [asignacion].[dbo].[vae_cat_materia]");

            foreach (DataRow r in dt.Rows)
                materias.Add(r["cve_materia"].ToString(),r["materia"].ToString());

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

        public Dictionary<int,string> GetProfesoresAsDicctionary()
        {
            Dictionary<int, string> profesores = new Dictionary<int, string>();
            DataTable dt = Querry("SELECT * FROM [asignacion].[dbo].[vae_cat_profesor]");

            foreach (DataRow r in dt.Rows)
                profesores.Add(Convert.ToInt32(r["rpe"].ToString()), r["nombre"].ToString());

            return profesores;
        }

        public List<OrigenDatos.Clases.Grupo> AsList(DataTable dt)
        {
            List<OrigenDatos.Clases.Grupo> g = new List<OrigenDatos.Clases.Grupo>();
            foreach (DataRow r in dt.Rows)
                g.Add(new Grupo(r, DGruposBD));

            return g;
        }
    }
}
