using System;
using System.Data;

namespace Algoritmo01.Heredados
{
    public class Conexion : OrigenDatos.Clases.Conexion
    {
        public Conexion(string Datos, string excelDireccion = null, string archivoEntrada = null, string hoja = "SIAMDIF", string ciclo = "2016-2017/II", string tipo = ""): base(Datos, excelDireccion, archivoEntrada,hoja, ciclo,tipo){}

        /// <summary>
        /// Obtiene el grupo especificado por los parametros
        /// </summary>
        /// <param name="cve_materia"></param>
        /// <param name="grupo"></param>
        /// <param name="tipo"></param>
        /// <param name="ciclo"></param>
        /// <returns></returns>
        public DataTable Grupo(string cve_materia, int grupo, string tipo, string ciclo)
        {
            DataTable datos = null;

            if (!excel)
            {
                ///ver lo de funciones almacenadas.
                string textoCmd = "SELECT * "
                                  + "FROM [asignacion].[ae_horario] "
                                  + "where cve_materia = '" + cve_materia + "' and grupo = " + grupo.ToString() + " and tipo = '" + tipo + "' and ciclo = '" + ciclo + "'; ";

                datos = Querry(textoCmd);
            }
            else
                datos = Excel.GetGrupo(Excel.RawGrupos, cve_materia, grupo.ToString());

            return datos;

        }

        public void Update(string nombre = null)
        {
            if (excel)
                Excel.Escribe_Horario_Excel(nombre != null ? nombre : "asignacion_" + DateTime.Today.ToString("yyyy_MM_dd"),
                                            Excel.Direccion + "\\asignacion_2016-2017-II.xlsx");
        }
    }
}
