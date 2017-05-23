using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InterfazWeb.Controllers
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
            return PartialView();
        }

        public ActionResult _CargaExcel()
        {
            return PartialView();
        }
    }
}
