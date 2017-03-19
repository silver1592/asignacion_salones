using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaz_02.Heredados
{
    class Conexion : Algoritmo01.Heredados.Conexion
    {
        public Conexion(string Datos, string excelDireccion = null, string archivoEntrada = null, string hoja = "SIAMDIF", string ciclo = "2016-2017/II", string tipo = "") : base(Datos, excelDireccion, archivoEntrada, hoja, ciclo, tipo) { }

        /// <summary>
        /// Obtiene el listado de equipo (catalogo de equipo)
        /// </summary>
        /// <returns></returns>
        public DataTable Equipo()
        {
            string textoCmd = "SELECT * FROM [asignacion].[asignacion].[ae_cat_equipo]";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene la informacion de todos los profesores
        /// </summary>
        /// <param name="rpe"></param>
        /// <returns></returns>
        public DataTable Profesores()
        {
            //string textoCmd = "SELECT [rpe],[titulo]+' '+[nombre] as 'Nombre' FROM [vae_cat_profesor]";
            string textoCmd = "SELECT [rpe],[nombre] as 'Nombre' FROM [vae_cat_profesor]";
            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene de la base de datos las materias que se encuentren en la base de datos
        /// </summary>
        /// <param name="cve_materia">clave similar</param>
        /// <param name="nombre">nombre de la materia</param>
        /// <returns></returns>
        public DataTable Materias()
        {
            string textoCmd = "SELECT cve_area as Area, cve_materia as Materia, materia as Nombre  FROM [vae_cat_materia]";
            DataTable datos = Querry(textoCmd);

            return datos;
        }
    }
}
