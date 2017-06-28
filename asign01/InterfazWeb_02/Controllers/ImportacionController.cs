using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterfazWeb_02.Models;
using InterfazWeb_02.Clases;
using System.IO;

namespace InterfazWeb_02.Controllers
{
    public class ImportacionController : Controller
    {
        //
        // GET: /Importacion/

        public ActionResult Index()
        {
            return View();
        }

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
                lista.Add(Path.GetFileName(s) );

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

            try
            {
                c = new Conexion(Conexion.datosConexion,excel,ciclo);
                c.Sheet = sheet;
                ListaGrupos grupos = new ListaGrupos(c.GetGrupos(Session["ciclo"].ToString()));

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
            Conexion c = new Conexion(Conexion.datosConexion, excelDir+file);

            string[] sheets = c.Sheets();

            return new JsonResult() { Data = sheets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
