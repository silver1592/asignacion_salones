using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterfazWeb_02.Models;

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
            return PartialView();
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
        public JsonResult SetSession_OrigenDatos(string excel, string sheet, string bd)
        {
            if(!Convert.ToBoolean(bd))
            {
                Session.Add("excel", excel);
                Session.Add("sheet", excel);
            }

            Session.Add("usaExcel", !Convert.ToBoolean(bd));

            return new JsonResult() { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult CargaExcel_BD(string excel, string sheet, string semestre)
        {
            //TODO: Metodo para leer los grupos del excel dado

            //foreach(Grupo g in grupos)
            {
                //TODO: Metodo para checar si existe el grupo en la base de datos
                //TODO: Si existe hacer update
                //TODO: Si no existe hacer insert
            }

            //TODO: Resultado negativo
            return new JsonResult() { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
