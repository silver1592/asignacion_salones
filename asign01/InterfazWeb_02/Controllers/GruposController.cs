using InterfazWeb_02.Clases;
using InterfazWeb_02.Models;
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
            Conexion c = new Conexion(Conexion.datosConexion, this);
            List<Materia> ms = new List<Materia>();
            var materias = c.GetMaterias();
            foreach (Algoritmo02.Clases.Materia m in materias)
                ms.Add(new Materia(m));

            return PartialView(ms);
        }

        [HttpPost]
        public ActionResult _Grupos(string ini="7", string fin="22",string cve = "", string grp="0", string dias="111111")
        {
            Conexion c = new Conexion(Conexion.datosConexion, this);
            ListaGrupos list = new ListaGrupos(c.GetGrupos(Session["ciclo"].ToString(), Convert.ToInt32(ini), Convert.ToInt32(fin)));

            list = list.EnDias(dias);

            if (cve != "")
                list = list.ImpartenMateria(cve);

            if (grp != "0")
                list = list.NoGrupo(Convert.ToInt32(grp));

            return PartialView(list);
        }

        public ActionResult _Grupos()
        {
            Conexion c = new Conexion(Conexion.datosConexion, this);
            ListaGrupos list = new ListaGrupos(c.GetLightGrupos(Session["ciclo"].ToString()));

            return PartialView(list);
        }
    }
}