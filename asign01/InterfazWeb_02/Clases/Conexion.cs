using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrigenDatos;
using System.Data;
using System.IO;
using InterfazWeb_02.Models;

namespace InterfazWeb_02.Clases
{
    public class Conexion : Algoritmo02.Heredados.Conexion
    {
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
    }
}