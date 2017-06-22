using InterfazWeb_02.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InterfazWeb_02.Controllers
{
    public class EjecutaController : Controller
    {
        //
        // GET: /Ejecuta/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Operaciones()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult EjecutaOperaciones(string hora,string empalmes, string preasignacion, string otrosSemestres, string algoritmo, string individuos, string generacion)
        {
            string res = "asignacion fallida";

            try
            {
                string ciclo = Session["ciclo"].ToString();

                Conexion c = new Conexion(Conexion.datosConexion,this);
                ListaGrupos grupos = new ListaGrupos(c.GetGrupos(ciclo,Convert.ToInt32(hora),Convert.ToInt32(hora)+1));

                //TODO: Hacer metodo que guarde en un excel el resultado

                res = "Asignacion de " + hora + " completada";
            }
            catch (Exception ex)
            {
                res += "\n" + ex.Message;
            }

            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
