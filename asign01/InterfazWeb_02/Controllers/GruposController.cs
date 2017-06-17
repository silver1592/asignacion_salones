using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InterfazWeb_02.Controllers
{
    public class GruposController : Controller
    {
        // GET: Grupos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Busqueda()
        {
            return PartialView();
        }
    }
}