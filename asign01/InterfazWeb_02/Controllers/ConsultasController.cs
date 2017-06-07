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
        string nombreArchivo = "SIAMMAT16172-FINAL.xlsx";
        string nombreHoja = "E_2017_01_12";
        string semestre = "2016-2017/II";
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
            Conexion c = new Conexion(Conexion.datosConexionPrueba, excelDir, nombreArchivo, nombreHoja);
            ListaGrupos grupos = c.GetGrupos(semestre);

            return PartialView(grupos);
        }

        [HttpPost]
        public JsonResult PaginasExcel(string file)
        {
            string excelDir = Server.MapPath("~/Archivos/");
            Conexion c = new Conexion(Conexion.datosConexionPrueba, excelDir, nombreArchivo, nombreHoja);
            string[] sheets = c.GetExcel.Sheets;

            return new JsonResult() { Data = sheets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
