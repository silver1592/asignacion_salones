using Algoritmo02.Clases;
using OrigenDatos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Generic.Dictionary<int, string>;

namespace InterfazWeb_02.Controllers
{
    public class SalonController : Controller
    {
        // GET: Salon
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Busqueda()
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            ListaSalones salones = new ListaSalones(c,c.Salones());
            Dictionary<int, string> equipo = c.Equipos();
            object[] model = new object[2];

            model[0] = salones;
            model[1] = equipo;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _Salon(string cve_salon)
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            List<InterfazWeb_02.Models.Salon> ls = new List<InterfazWeb_02.Models.Salon>();
            List<ListaVariables> aux;
            ListaVariables grupos;
            switch (cve_salon) {
                case "empalmes":    //Empalmes
                    grupos = new ListaVariables(c.Grupos(Session["ciclo"].ToString()));
                    aux = grupos.AgrupaGruposEmpalmados();
                    foreach (ListaVariables g in aux)
                        if (g.Count != 0)
                            ls.Add(new InterfazWeb_02.Models.Salon(c.Salon(g[0].Cve_espacio), g,g[0].Cve_espacio));
                    break;
                default:
                    grupos = new ListaVariables(c.Grupos(Session["ciclo"].ToString(), cve_salon));
                    ls.Add(new InterfazWeb_02.Models.Salon(c.Salon(cve_salon),grupos, cve_salon));
                break;
            }

            return PartialView(ls);
        }
    }
}