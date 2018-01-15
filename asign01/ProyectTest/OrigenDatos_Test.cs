using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrigenDatos.Clases;
using System.Linq;
using System.IO;

namespace ProyectTest
{
    [TestClass]
    public class OrigenDatos_Test
    {
        //Prueba de conexion
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

        //Prueba de lectura de un grupo de la base de datos
        [TestMethod]
        public void Conexion_GetGrupo_Test()
        {
            try
            {
                Conexion c = new Conexion();
                Grupo g = new Grupo(c.Querry("Select * from asignacion.ae_horario where cve_materia = '1001'").Rows[0], c.DGrupos);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        //Prueba de escritura en excel
        [TestMethod]
        public void Conexion_NuevoExel_Test()
       {
            try
           { 
                Conexion c = new Conexion();
                Grupo g = new Grupo(c.Querry("Select * from asignacion.ae_horario where cve_materia = '1001'").Rows[0], c.DGrupos);
                LibroExcel excel = new LibroExcel(@"excel.xlsx", "2016-2017/II", "T");
                ListaGrupos grupos = new ListaGrupos();

                grupos.Add(g);
                excel.EscribeGrupos(grupos, "prueba", c.Materias_AsDictionary(), c.Profesores_AsDicctionary());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        //Prueba de escritura con nombres en Excel
        //Requiere vistas
        [TestMethod]
        public void Conexion_Exportacion_Test()
        {
            try
            {
                Conexion c = new Conexion();
                LibroExcel excel = new LibroExcel(@"exp_2016_2017_II.xlsx", "2016-2017/II", "T");
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
        public void Conexion_Importacion_Excel_Test()
        {
            try
            {
                Conexion c = new Conexion(Conexion.datosConexion, @"exp_2016_2017_II.xlsx", "T");
                c.Sheet = "prueba01_2016_2017_II";
                ListaGrupos grupos = new ListaGrupos(c.Grupos("2016-2017/II"));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        //Prueba de lectura en excel
        [TestMethod]
        public void Conexion_Importacion_Excel_DB_Test()
        {
            try
            {
                Conexion c = new Conexion(Conexion.datosConexion, @"exp_2016_2017_II.xlsx", "T");
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

        //Prueba de escritura en excel con 2 llamadas (Creando una hoja y reutilizando la hoja)
        //Requiere vistas
        [TestMethod]
        public void Conexion_ExportacionPartes_Test()
        {
            int gruposTotales=0;
            int gruposRes = 0;
            try
            {
                Conexion c = new Conexion();
                LibroExcel excel = new LibroExcel(@"exp_2016_2017_II.xlsx", "2016-2017/II", "T");
                ListaGrupos grupos = new ListaGrupos(c.Grupos("2016-2017/I", 7, 8, false));
                ListaGrupos grupos2 = new ListaGrupos(c.Grupos("2016-2017/I", 8, 9, false));

                gruposTotales = grupos.Count() + grupos2.Count();
                excel.EscribeGrupos(grupos, "prueba02");
                excel.EscribeGrupos(grupos2, "prueba02");

                Conexion c2 = new Conexion(Conexion.datosConexion, @"exp_2016_2017_II.xlsx", "2016-2017/I", "T");
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

        //Prueba de lectura de salon
        [TestMethod]
        public void Conexion_GetSalon_Test()
        {
            Conexion c = new Conexion();
            ListaSalones salones = new ListaSalones(c,c.Salones());

            Assert.IsFalse(salones.Count == 0);
        }

        //Prueba de constructores de Grupo
        [TestMethod]
        public void Grupo_Constructores_Test()
        {
            Grupo g1,g2;
            Conexion c = new Conexion();
            ListaSalones s = new ListaSalones(c, c.Salones(), 0);

            g1 = new Grupo(c.Grupo("100101", "2016-2017/I"));
            Assert.IsFalse(g1 == null);
            g1 = new Grupo(c.Grupo("100101", "2016-2017/I"), c);
            Assert.IsFalse(g1 == null);
            g1 = new Grupo(c.Grupo("100101", "2016-2017/I"), null, s);
            Assert.IsFalse(g1 == null);
            g1 = new Grupo(c.Grupo("100101", "2016-2017/I"),c,s);
            Assert.IsFalse(g1 == null);

            g2 = new Grupo(g1);
            Assert.IsFalse(g2 == null);
            g2 = new Grupo(g1,c);
            Assert.IsFalse(g2 == null);
            g2 = new Grupo(g1,null,s);
            Assert.IsFalse(g2 == null);
            g2 = new Grupo(g1, c, s);
            Assert.IsFalse(g2 == null);
        }

        //Prueba de Constructores de Salon
        [TestMethod]
        public void Salon_Constructores_Test()
        {
            Conexion c = new Conexion();
            Salon s1, s2;
            s1 = new Salon(c.Salones().Rows[0], 0, c);
            Assert.IsFalse(s1 == null);
            s2 = new Salon(s1);
            Assert.IsFalse(s2 == null);
        }

        //Prueba de grupos de metodos (No deben retornar null)
        [TestMethod]
        public void Grupo_Metodos_Test()
        {

        }

        //Prueba de salones de Metodos (No deben retornar null)
    }
}
