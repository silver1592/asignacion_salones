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

        [HttpPost]
        public ActionResult _Grupos(string ini="7", string fin="22",string cve = "", string grp="0", string dias="111111")
        {
            return PartialView();
        }
    }
}