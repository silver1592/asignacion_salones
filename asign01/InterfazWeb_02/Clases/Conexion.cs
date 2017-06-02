using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrigenDatos;
using System.Data;
using System.IO;

namespace InterfazWeb_02.Clases
{
    public class Conexion : Algoritmo02.Heredados.Conexion
    {
        public Conexion(string Datos, string excelDireccion = null, string archivoEntrada = null, string hoja = "SIAMDIF", string ciclo = "2016-2017/II", string tipo = "") : base(Datos, excelDireccion, archivoEntrada, hoja, ciclo, tipo) { }

        public ListaGrupos GetGrupos(string semestre)
        {
            ListaGrupos res = null;

            if (!excel)
            {
                DataTable dt = Querry("SELECT * FROM[asignacion].[ae_horario] where ciclo = '" + semestre + "'");
                res = new ListaGrupos(Excel.AsList(dt));
            }
            else
            {
                res = new ListaGrupos(Excel.Grupos);
            }

            return res;
        }
    }
}