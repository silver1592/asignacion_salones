using InterfazWeb_02.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            string[] semestres = c.Semestres();

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
    }
}
