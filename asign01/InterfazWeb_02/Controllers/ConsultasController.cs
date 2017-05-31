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
        string excelDir = @"C:\Users\Fernando\_DD\Mega\UASLP\Sandra\Sistema de Asignacion de Salones\Referencias y Documentos\";
        string nombreArchivo = "SIAMMAT16172-FINAL.xlsx";
        string nombreHoja = "Query_1";
        string semestre = "2016-2017/I";
        //
        // GET: /Consultas/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Grupos()
        {
            Conexion c = new Conexion(Conexion.datosConexionPrueba, excelDir, nombreArchivo, nombreHoja);
            ListaGrupos grupos = c.GetGrupos(semestre);
            List<Grupo> gs = grupos.List();

            return PartialView(gs);
        }
    }
}
