﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterfazWeb_02.Models;
using InterfazWeb_02.Clases;

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
        public JsonResult SetSession_OrigenDatos(string excel, string sheet, string ciclo,string bd)
        {
            Session.Add("excel", excel);
            Session.Add("sheet", sheet);

            Session.Add("ciclo", ciclo);
            Session.Add("usaExcel", !Convert.ToBoolean(bd));

            return new JsonResult() { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult CargaExcel_BD()
        {
            object[] res = new object[] { true, "" };
            Conexion c;
            string excel = Session["excel"].ToString();
            string sheet = Session["sheet"].ToString();
            string ciclo = Session["ciclo"].ToString();
            bool db = Convert.ToBoolean(Session["usaExcel"].ToString());

            try
            {
                c = new Conexion(Conexion.datosConexion, this);
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
        public JsonResult ExcelValid()
        {
            Conexion c;

            object[] res = new object[] { false, "No conectado" };
            string excel = Session["excel"] != null ? Session["excel"].ToString() : "";
            string sheet = Session["sheet"] != null ? Session["sheet"].ToString() : "";
            string ciclo = Session["ciclo"] != null ? Session["ciclo"].ToString() : "";
            bool db = Session["usaExcel"] != null ? !Convert.ToBoolean(Session["usaExcel"].ToString()) : true;

            if (ciclo == "")
                return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            try
            {
                c = new Conexion(Conexion.datosConexion, this);
                res[1] = db ? "Base de datos" : excel + "-" + sheet;
                res[0] = true;

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
