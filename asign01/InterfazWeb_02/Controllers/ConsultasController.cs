using InterfazWeb_02.Clases;
using InterfazWeb_02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InterfazWeb_02.Controllers
{
    public class ConsultasController : Controller
    {
        //
        // GET: /Consultas/

        public ConsultasController():base()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Grupos()
        {
            string excelDir = Server.MapPath("~/Archivos/");
            Conexion c = new Conexion(Conexion.datosConexionPrueba, excelDir, Session["excel"].ToString(), Session["sheet"].ToString());
            ListaGrupos grupos = c.GetGrupos(Session["semestre"].ToString());

            return PartialView(grupos);
        }

        [HttpPost]
        public JsonResult PaginasExcel(string file)
        {
            string excelDir = Server.MapPath("~/Archivos/");
            Conexion c = new Conexion(Conexion.datosConexionPrueba, excelDir, Session["excel"].ToString(), Session["sheet"].ToString());
            string[] sheets = c.GetExcel.Sheets;

            return new JsonResult() { Data = sheets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        
        public JsonResult ExcelSelected()
        {
            string excel = Session["excel"].ToString();
            string sheet = Session["sheet"].ToString();
            bool db = Convert.ToBoolean(Session["usaExcel"].ToString());

            return new JsonResult() { Data = db ? new string[] {excel,sheet} : new string[] { "SQL Server" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ExcelValid()
        {
            string excel = Session["excel"].ToString();
            string sheet = Session["sheet"].ToString();
            bool db = Convert.ToBoolean(Session["usaExcel"].ToString());

            //TODO: Hacer un metodo que cheque si se puede leer el excel o si tiene un formato invlido que regrese el error

            return new JsonResult() { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
