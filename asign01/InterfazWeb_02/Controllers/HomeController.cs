using InterfazWeb_02.Clases;
using Algoritmo02.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterfazWeb_02.Models;
using System.IO;

namespace InterfazWeb_02.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Menu()
        {
            return View();
        }

        public ActionResult _Semestres()
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            //string[] semestres = c.Semestres();
            string[] semestres = new string[0];

            return View(semestres);
        }

        [HttpPost]
        public JsonResult ConexionValida(string semestre)
        {
            Conexion c;

            bool res = false;

            c = new Conexion(Conexion.datosConexion);
            if (c.ExisteSemestre(semestre))
            {
                if (Session["ciclo"] == null)
                    Session.Add("ciclo", semestre);
                else
                    Session["ciclo"] = semestre;
                res = true;
            }

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Importacion
        public ActionResult _SeleccionExcel()
        {
            List<LibroExcel> libros = new List<LibroExcel>();
            string[] pathElements;
            string fileName;

            foreach (string s in System.IO.Directory.GetFiles(Server.MapPath("~/Archivos/")))
            {
                pathElements = s.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                fileName = pathElements[pathElements.Length - 1];
                libros.Add(new LibroExcel(Server.MapPath("~/Archivos/"), fileName));
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

        [HttpPost]
        public ActionResult SubirExcel(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string archivo = file.FileName;
                string dir = Server.MapPath("~/Archivos/") + archivo;

                file.SaveAs(dir);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult CargaExcel_BD(string excel, string sheet, string ciclo)
        {
            object[] res = new object[] { true, "" };
            Conexion c;
            string excelDir = Server.MapPath("~/Archivos/") + excel;

            try
            {
                c = new Conexion(Conexion.datosConexion, excelDir, ciclo);
                c.Sheet = sheet;
                ListaGrupos grupos = new ListaGrupos(c.GetGrupos(ciclo));

                foreach (Grupo g in grupos)
                    if (c.ExisteBD(g))
                        c.Comando(g.qUpdate);
                    else
                        c.Comando(g.qInsert);
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
        public JsonResult EjecutaOperaciones(string hora, string empalmes, string preasignacion, string otrosSemestres, string algoritmo, string individuos, string generacion)
        {
            string res = "asignacion fallida";

            try
            {
                string ciclo = Session["ciclo"].ToString();

                Conexion c = new Conexion(Conexion.datosConexion);
                ListaGrupos grupos = new ListaGrupos(c.GetGruposIni(ciclo, Convert.ToInt32(hora), false));
                ListaSalones salones = new ListaSalones(c, c.Salones(), Convert.ToInt32(hora));

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

                c.UpdateGrupo(grupos, Session["ResSheet"].ToString());

                res = "Asignacion de " + hora + " completada";
            }
            catch (Exception ex)
            {
                res += "\n" + ex.Message;
            }

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion
    }
}
