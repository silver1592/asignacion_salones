using OrigenDatos.Clases;
using Algoritmo02.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using InterfazWeb_02.Clases;

namespace InterfazWeb_02.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        /// <summary>
        /// Ciclo a analizar
        /// </summary>
        public string Ciclo
        {
            get
            {
                return Session["ciclo"] != null ? Session["ciclo"].ToString() : "";
            }
            set
            {
                if (Session["ciclo"] == null)
                    Session.Add("ciclo", value);
                else
                    Session["ciclo"] = value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Vista parcial del menu de navegacion
        /// </summary>
        /// <returns></returns>
        public ActionResult _Menu()
        {
            return View();
        }

        /// <summary>
        /// Vista parcial del ComboBox con los semestres listados en la base de datos
        /// </summary>
        /// <returns></returns>
        public ActionResult _Semestres()
        {
            string[] semestres = new string[0];
            try
            {
                Conexion c = new Conexion(Conexion.datosConexion);
                semestres = c.Semestres();
            }
            catch(Exception ex)
            {

            }

            return View(semestres);

        }

        /// <summary>
        /// Checa si el semestre es valido
        /// </summary>
        /// <param name="semestre"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ConexionValida(string semestre)
        {
            Conexion c;

            bool res = false;

            c = new Conexion(Conexion.datosConexion);
            if (c.Semestre_Valido(semestre))
            {
                Ciclo = semestre;

                res = true;
            }

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Importacion
        public ActionResult _SeleccionExcel()
        {
            List<string> libros = new List<string>();
            string[] pathElements;
            string fileName;

            foreach (string s in Directory.GetFiles(Server.MapPath("~/Archivos/"), "*.xls*"))
            {
                pathElements = s.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                fileName = pathElements[pathElements.Length - 1];
                libros.Add(fileName);
            }

            return PartialView(libros);
        }

        public ActionResult _CargaExcel()
        {
            List<string> lista = new List<string>();

            foreach (string s in Directory.GetFiles(Server.MapPath("~/Archivos/")))
                lista.Add(Path.GetFileName(s));

            return PartialView(lista);
        }

        public ActionResult _EliminaExcel(string nombre)
        {
            string dir = Server.MapPath("~/Archivos/") + nombre;
            if (Directory.Exists(dir))
            {

            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LimpiaBaseDatos()
        {
            Conexion c = new Conexion(Conexion.datosConexion);

            c.EliminaDatos(Ciclo);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubirExcel(HttpPostedFileBase archivo)
        {
            //TODO:checar que funcione
            if (archivo != null)
            {
                string file = archivo.FileName;
                string dir = Server.MapPath("~/Archivos/") + file;

                archivo.SaveAs(dir);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult CargaExcel_BD(string excel, string sheet, string ciclo)
        {
            //TODO: Hacer validaciones de ciclo por si se anota mal
            object[] res = new object[] { true, "" };
            Conexion c;
            string excelDir = Server.MapPath("~/Archivos/") + excel;

            try
            {
                c = new Conexion(Conexion.datosConexion, excelDir, ciclo);
                c.Sheet = sheet;
                ListaGrupos grupos = c.IGrupos_Light(ciclo);

                c.Grupos_Carga(grupos,null);
            }
            catch (Exception ex)
            {
                res[0] = false;
                res[1] = ex.Message;
            }
            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult PaginasExcel(string file)
        {
            string excelDir = Server.MapPath("~/Archivos/");
            Conexion c = new Conexion(Conexion.datosConexion, excelDir + file);

            string[] sheets;

            try
            {
                sheets = c.Sheets;
            }
            catch (Exception ex)
            {
                sheets = new string[0];
            }

            return new JsonResult() { Data = sheets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Operaciones
        public ActionResult _Operaciones()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult EjecutaOperaciones(string hora, int[] operaciones,string ciclo,string excel, string hoja)
        {
            //-empalmes -preasignacion -otrosSemestres -algoritmo
            string res = "<strong>Asignacion Fallida</strong>\n";
            string detalles = "";

            try
            {
                Conexion c = new Conexion(Conexion.datosConexion,Server.MapPath("~/Archivos/"+excel),ciclo);
                ListaVariables grupos = new ListaVariables(c.Grupos_EmpiezanA(ciclo, Convert.ToInt32(hora), false));
                ListaSalones salones = new ListaSalones(c, c.Salones(), Convert.ToInt32(hora));
                salones.SetHorarios(c, Ciclo);

                int numero_Operacion = 0;
                IOperacion operacion = null;

                foreach(int op in operaciones)
                {
                    numero_Operacion++;

                    switch((byte)op)  //Fabrica abstracta
                    {
                        case (byte)EOperaciones.algoritmoGenetico:
                            operacion = new AlgoritmoGenetico(grupos, salones, Convert.ToInt32(hora), 100, 1000);
                        break;
                        case (byte)EOperaciones.empalmes:
                            operacion = new RevisionEmpalmes(grupos, salones);
                        break;
                        case (byte)EOperaciones.otrosSemestres:
                            operacion = new AsignacionOtrosSemestres(grupos, salones);
                        break;
                        case (byte)EOperaciones.preasignacion:
                            operacion = new AsignacionPreferencial(grupos, salones);
                        break;
                    }

                    if (operacion != null)
                    {
                        operacion.Ejecuta();
                        if (operacion.Resultado.Count != 0)
                        {
                            grupos.Actualiza(operacion.Resultado);
                            detalles += Grupos2Table(operacion.Resultado.OrderBy(g=>g.Cve_espacio).ToList(), operacion.NombreOperacion);
                        }
                        else
                            detalles += string.Format("{0} sin grupos modificados", operacion.NombreOperacion);

                        detalles += "<br>";
                    }
                }

                c.Grupos_Carga(grupos, hoja,c.Materias_AsDictionary(),c.Profesores_AsDicctionary());

                res = string.Format("Asignacion de {0} completada<br>{1}",hora,detalles);
            }
            catch (Exception ex)
            {
                res += "\n" + ex.Message;
            }

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private string Grupos2Table(IList<Grupo> grupos,string nombre)
        {
            string res = string.Format("<table class='table-bordered'><thead><tr><th colspan='5'><h3>{0}</h3></th></tr>",nombre) +
                "<tr><th>Materia</th><th>Profesor</th><th>horario</th><th>Salon actual</th><th>salon anterior</th></tr></thead><tbody>";
            

            foreach(Grupo g in grupos)
                res += string.Format(
                    "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                    g.Cve_materia,g.RPE,g.Dias,g.Cve_espacio,g.SalonBD);

            res += "</tbody></table>";
            return res;
        }

        public JsonResult Exporta(string excel, string sheet)
        {
            string res = "<strong>Operacion fallida</strong>\n";

            try
            {
                string ciclo = Ciclo;
                string path = Server.MapPath("~/Archivos/"+excel);

                Conexion c = new Conexion(Conexion.datosConexion, path, ciclo);
                ListaVariables grupos = new ListaVariables(c.Grupos(ciclo,bExcel:false));

                c.Grupos_Carga(grupos, sheet, c.Materias_AsDictionary(), c.Profesores_AsDicctionary());

                res = "Exportacion completada </br><strong>Guardado en "+excel+"->"+sheet+"</strong>";
            }
            catch (Exception ex)
            {
                res += "\n" + ex.Message;
            }
            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult EliminaArchivo(string archivo)
        {
            bool res = false;
            string path = Server.MapPath("~/Archivos/" + archivo);

            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                    res = true;
                }
                catch
                {
                    res = false;
                }

            }
            else
                res = true;

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}
