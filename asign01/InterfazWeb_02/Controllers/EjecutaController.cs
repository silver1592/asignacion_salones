using Algoritmo02.Clases;
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

                Conexion c = new Conexion(Conexion.datosConexion);
                ListaGrupos grupos = new ListaGrupos(c.GetGruposIni(ciclo,Convert.ToInt32(hora),false));
                ListaSalones salones = new ListaSalones(c,c.Salones(), Convert.ToInt32(hora));

                if(Convert.ToBoolean(empalmes))
                {
                    ChecaEmpalmes emp = new ChecaEmpalmes(grupos, salones);
                    emp.ejecuta();

                    grupos.Actualiza(emp.Grupos);
                }

                if(Convert.ToBoolean(preasignacion) || Convert.ToBoolean(otrosSemestres))
                {
                    PreAsignacion pre = new PreAsignacion(grupos, salones);
                    if(Convert.ToBoolean(preasignacion))
                        pre.preferencial();
                    if(Convert.ToBoolean(otrosSemestres))
                        pre.semestres_anteriores();

                    grupos.Actualiza(pre.Grupos);
                }

                if(Convert.ToBoolean(algoritmo))
                {
                    Algoritmo alg = new Algoritmo(grupos, salones, Convert.ToInt32(hora), 5, 50);
                    alg.AsignaSalones();

                    grupos.Actualiza(alg.GruposAsignados);
                }

                c.UpdateGrupo(grupos, Session["ResSheet"].ToString());

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
