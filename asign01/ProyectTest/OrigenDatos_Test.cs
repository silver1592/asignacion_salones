﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrigenDatos.Clases;
using System.Linq;

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
        public void Conexion_GetGrupo_Test()
        {
            try
            {
                Conexion c = new Conexion();
                Grupo g = new Grupo(c.Querry("Select * from ae_horario where cve_materia = '1001'").Rows[0], c.DGrupos);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Conexion_NuevoExel_Test()
       {
            try
           { 
                Conexion c = new Conexion();
                Grupo g = new Grupo(c.Querry("Select * from ae_horario where cve_materia = '1001'").Rows[0], c.DGrupos);
                LibroExcel excel = new LibroExcel(@"C:\Users\Fernando\Source\Repos\asignacion_salones\asign01\InterfazWeb_02\Archivos\excel.xlsx", "2016-2017/II", "T");
                ListaGrupos grupos = new ListaGrupos();

                grupos.Add(g);
                excel.EscribeGrupos(grupos, "prueba");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Conexion_Exportacion_Test()
        {
            try
            {
                Conexion c = new Conexion();
                LibroExcel excel = new LibroExcel(@"C:\Users\Fernando\Source\Repos\asignacion_salones\asign01\InterfazWeb_02\Archivos\exp_2016_2017_II.xlsx", "2016-2017/II", "T");
                ListaGrupos grupos = new ListaGrupos(c.Grupos("2016-2017/II",7,8));
                var profesores = c.Profesores_AsDicctionary();
                var materias = c.Materias_AsDictionary();

                excel.EscribeGrupos(grupos, "prueba01_2016_2017_II",materias,profesores);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Conexion_Importacion_Test()
        {
            try
            {
                Conexion c = new Conexion(Conexion.datosConexion, @"C:\Users\Fernando\Source\Repos\asignacion_salones\asign01\InterfazWeb_02\Archivos\exp_2016_2017_II.xlsx", "T");
                c.Sheet = "prueba";
                ListaGrupos grupos = new ListaGrupos(c.Grupos("2016-2017/II"));

                //excel.EscribeGrupos(grupos, "prueba");
                c.Grupos_Carga(grupos);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Conexion_ExportacionPartes_Test()
        {
            int gruposTotales=0;
            int gruposRes = 0;
            try
            {
                Conexion c = new Conexion();
                LibroExcel excel = new LibroExcel(@"C:\Users\Fernando\Source\Repos\asignacion_salones\asign01\InterfazWeb_02\Archivos\exp_2016_2017_II.xlsx", "2016-2017/II", "T");
                ListaGrupos grupos = new ListaGrupos(c.Grupos("2016-2017/I", 7, 8, false));
                ListaGrupos grupos2 = new ListaGrupos(c.Grupos("2016-2017/I", 8, 9, false));

                gruposTotales = grupos.Count() + grupos2.Count();
                excel.EscribeGrupos(grupos, "prueba02");
                excel.EscribeGrupos(grupos2, "prueba02");

                Conexion c2 = new Conexion(Conexion.datosConexion, @"C:\Users\Fernando\Source\Repos\asignacion_salones\asign01\InterfazWeb_02\Archivos\exp_2016_2017_II.xlsx", "2016-2017/I", "T");
                c2.Sheet = "prueba02";

                grupos = new ListaGrupos(c2.Grupos("2016-2017/I"));
                gruposRes = grupos.Count();

                Assert.IsFalse(gruposTotales != gruposRes,"No son los grupos");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
