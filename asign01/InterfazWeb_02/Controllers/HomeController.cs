using OrigenDatos.Clases;
using Algoritmo02.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

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
        public JsonResult EjecutaOperaciones(string hora, string empalmes, string preasignacion, string otrosSemestres, string algoritmo, string individuos, string generacion, string excel, string hoja)
        {
            string res = "<strong>Asignacion Fallida</strong>\n";

            try
            {
                string ciclo = Ciclo;

                Conexion c = new Conexion(Conexion.datosConexion,Server.MapPath("~/Archivos/"+excel),ciclo);
                ListaVariables grupos = new ListaVariables(c.Grupos_EmpiezanA(ciclo, Convert.ToInt32(hora), false));
                ListaSalones salones = new ListaSalones(c, c.Salones(), Convert.ToInt32(hora));
                salones.SetHorarios(c, Ciclo);

                if (Convert.ToBoolean(empalmes))
                {
                    ChecaEmpalmes emp = new ChecaEmpalmes(grupos, salones);
                    emp.ejecuta();

                    grupos.Actualiza(emp.Grupos);
                }

                if (Convert.ToBoolean(preasignacion) || Convert.ToBoolean(otrosSemestres))
                {
                    PreAsignacion pre = new PreAsignacion(grupos, salones);
                    if (Convert.ToBoolean(preasignacion))
                        pre.preferencial();
                    if (Convert.ToBoolean(otrosSemestres))
                        pre.semestres_anteriores();

                    grupos.Actualiza(pre.Grupos);
                }

                if (Convert.ToBoolean(algoritmo))
                {
                    Algoritmo alg = new Algoritmo(grupos, salones, Convert.ToInt32(hora), 5, 50);
                    alg.AsignaSalones();

                    grupos.Actualiza(alg.GruposAsignados);
                }

                c.Grupos_Carga(grupos, hoja,c.Materias_AsDictionary(),c.Profesores_AsDicctionary());

                res = "Asignacion de " + hora + " completada";
            }
            catch (Exception ex)
            {
                res += "\n" + ex.Message;
            }

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
