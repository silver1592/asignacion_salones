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
            Conexion c = new Conexion(Conexion.datosConexion, this);
            ListaGrupos grupos = new ListaGrupos(c.GetGrupos(Session["semestre"].ToString()));

            return PartialView(grupos);
        }
    }
}
