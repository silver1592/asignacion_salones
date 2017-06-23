using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrigenDatos.Clases;

namespace ProyectTest
{
    [TestClass]
    public class OrigenDatos_Test
    {
        [TestMethod]
        public void Conexion_Default_Test()
        {
            //Arrange o planteamiento            
            bool valido = true;

            //Act o prueba
            Conexion c = new Conexion();
            var res = c.Autenticacion();

            //Assert o Confirmacion
            Assert.AreEqual(valido, res);
        }

        [TestMethod]
        public void Conexion_CreaExel_Test()
        {
            try
            {
                Conexion c = new Conexion();
                Grupo g = new Grupo(c.Querry("Select * from ae_horario where cve_materia = '1001'").Rows[0], c.DGrupos);
                LibroExcel excel = new LibroExcel(@"C:\Users\Fernando\Source\Repos\asignacion_salones\asign01\InterfazWeb_02\Archivos\miexcel.xlsx", "2016-2017/II", "T");
                ListaGrupos grupos = new ListaGrupos();

                grupos.Add(g);
                excel.EscribeGrupos(grupos, "prueba");
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
