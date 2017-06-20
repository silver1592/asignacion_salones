using Algoritmo02.Clases;
using System;
using System.Collections.Generic;
using System.Data;

namespace Algoritmo02.Heredados
{
    public class Conexion : OrigenDatos.Clases.Conexion
    {
        public Conexion() : base() { }

        public Conexion(string Datos, string excelDireccion = null, string archivoEntrada = null, string hoja = null, string ciclo = "2016-2017/II", string tipo = ""): base(Datos, excelDireccion, archivoEntrada,hoja, ciclo,tipo){}

        public ListaGrupos GetGrupos(string semestre, int ini = 7, int fin=22,bool bExcel=true)
        {
            ListaGrupos res = null;
            List<OrigenDatos.Clases.Grupo> grupos;
            List<Materia> materias = GetMaterias();
            List<Profesor> profesores = GetProfesores();

            if (Excel==null || !bExcel)
            {
                DataTable dt = Querry("SELECT * FROM  [asignacion].[Grupos_a_las] ("+ini+","+fin+") where ciclo = '" + semestre + "'");

                grupos = AsList(dt);
                res = new ListaGrupos(grupos, materias, profesores,this);
            }
            else
            {
                res = new ListaGrupos(Excel.Grupos, materias, profesores,this);
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

        public List<OrigenDatos.Clases.Grupo> AsList(DataTable dt)
        {
            List<OrigenDatos.Clases.Grupo> g = new List<OrigenDatos.Clases.Grupo>();
            foreach (DataRow r in dt.Rows)
                g.Add(new Grupo(r, DGruposBD));

            return g;
        }
    }
}
