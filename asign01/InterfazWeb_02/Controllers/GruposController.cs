using OrigenDatos.Clases;
using Algoritmo02.Clases;
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
            Conexion c = new Conexion(Conexion.datosConexion);
            List<Materia> ms = new List<Materia>();
            var materias = c.Materias();

            return PartialView(ms);
        }

        [HttpPost]
        public ActionResult _Grupos(string ini, string fin, string dias)
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            ListaVariables list = new ListaVariables(c.Grupos_Light(Session["ciclo"].ToString(), Convert.ToInt32(ini), Convert.ToInt32(fin)));

            list = list.EnDias(dias);

            return PartialView(list);
        }

        public ActionResult _Grupos()
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            ListaGrupos list = c.Grupos_Light(Session["ciclo"].ToString());

            return PartialView(list);
        }

        [HttpGet]
        public ActionResult Grupo(string cve_full)
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            Grupo g = c.Grupo(cve_full, Session["ciclo"].ToString());

            return View(g);
        }
    }
}