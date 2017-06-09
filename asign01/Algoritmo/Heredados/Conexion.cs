using System;
using System.Data;

namespace Algoritmo02.Heredados
{
    public class Conexion : OrigenDatos.Clases.Conexion
    {
        public Conexion(string Datos, string excelDireccion = null, string archivoEntrada = null, string hoja = "SIAMDIF", string ciclo = "2016-2017/II", string tipo = ""): base(Datos, excelDireccion, archivoEntrada,hoja, ciclo,tipo){}
    }
}
