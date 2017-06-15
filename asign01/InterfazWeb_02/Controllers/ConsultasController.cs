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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Grupos()
        {
            string excelDir = Server.MapPath("~/Archivos/");
            Conexion c = new Conexion(Conexion.datosConexionPrueba, this);
            ListaGrupos grupos = new ListaGrupos(c.GetGrupos(Session["semestre"].ToString()));

            return PartialView(grupos);
        }

        [HttpPost]
        public JsonResult PaginasExcel(string file)
        {
            string excelDir = Server.MapPath("~/Archivos/");
            Conexion c = new Conexion(Conexion.datosConexionPrueba,excelDir,file);

            string[] sheets = c.Sheets();

            return new JsonResult() { Data = sheets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult ExcelSelected()
        {
            string excel = Session["excel"].ToString();
            string sheet = Session["sheet"].ToString();
            bool db = Convert.ToBoolean(Session["usaExcel"].ToString());

            return new JsonResult() { Data = db ? new string[] {excel,sheet} : new string[] { "SQL Server" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult ExcelValid()
        {
            Conexion c;

            object[] res = new object[] { false, "No conectado" };
            string excel = Session["excel"] != null ? Session["excel"].ToString() : "";
            string sheet = Session["sheet"] != null ? Session["sheet"].ToString() : "";
            string ciclo = Session["ciclo"] != null ? Session["ciclo"].ToString() : "";
            bool db = Session["usaExcel"]!=null ? !Convert.ToBoolean(Session["usaExcel"].ToString()):true;

            if(ciclo=="")
                return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            try
            {
                c = new Conexion(Conexion.datosConexionPrueba, this);
                res[1] = db ? "Base de datos" : excel+"-"+sheet;
                res[0] = true;

            }catch(Exception ex)
            {
                res[0] = false;
                res[1] = ex.Message;
            }

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
