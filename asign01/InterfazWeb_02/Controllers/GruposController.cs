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
            ListaVariables list = new ListaVariables(new ListaGrupos(
                c.Grupos_Light(Session["ciclo"].ToString(), Convert.ToInt32(ini), Convert.ToInt32(fin)),
                c.Profesores(),
                c.Materias()));
            ListaSalones s = new ListaSalones(c, c.Salones());

            list = list.EnDias(dias);
            list.SetSalones(s);

            return PartialView(list);
        }

        public ActionResult _Grupos()
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            ListaVariables list = new ListaVariables(
                new ListaGrupos(c.Grupos_Light(Session["ciclo"].ToString()),
                                c.Profesores(),
                                c.Materias()));
            ListaSalones s = new ListaSalones(c, c.Salones());
            list.SetSalones(s);

            return PartialView(list);
        }

        [HttpGet]
        public ActionResult Grupo(int cve_full)
        {
            Conexion c = new Conexion(Conexion.datosConexion);
            try
            {
                Variable g = new Variable(c.Grupo(cve_full.ToString(), Session["ciclo"].ToString()), 0);
                Profesor p = c.Profesor(g.RPE);
                Materia m = c.Materia(g.Cve_materia);
                Salon s=null;

                if (g.Cve_espacio != "")
                    s = c.Salon(g.Cve_espacio);

                g.Salon = s;

                return View(new object[] { g,p,m});
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ModificaGrupo(string grupo, string salon)
        {
            Conexion c = new Conexion();
            Variable g = new Variable(c.Grupo(grupo, Session["ciclo"].ToString()),0);
            Salon s = c.Salon(salon);

            g.Salon = s;

            c.Querry(g.qUpdate);

            return View();
        }
    }
}