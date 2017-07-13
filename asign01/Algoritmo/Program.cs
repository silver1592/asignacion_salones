using Algoritmo02.Clases;
using OrigenDatos.Clases;
using System;
using System.Collections.Generic;

namespace Algoritmo02
{
    class Program
    {
        private static float avance;
        private static Conexion c;

        static void Main(string[] args)
        {
            inicializaConeccion();
            TimeSpan stop;
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
            Algoritmo alg;
            ChecaEmpalmes emp;
            PreAsignacion pre;
            ListaGrupos grupos = c.GetGrupos("2016-2017/II");
            ListaSalones salones = new ListaSalones(c,c.Salones());
            ListaGrupos gruposActuales;

            for (int i=7;i<22;i++)
            {
                gruposActuales = new ListaGrupos(grupos.EnHora(i, i + 1));

                emp = new ChecaEmpalmes(grupos, salones);
                emp.ejecuta();
                grupos.Actualiza(emp.Grupos);

                pre = new PreAsignacion(grupos, salones);
                pre.preferencial();
                pre.semestres_anteriores();
                grupos.Actualiza(pre.Grupos);

                alg = new Algoritmo(grupos,salones, i,5,50);
                alg.AsignaSalones();
                grupos.Actualiza(alg.GruposAsignados);

                c.UpdateGrupo(gruposActuales);
            }
            stop = new TimeSpan(DateTime.Now.Ticks);
            Console.Write("***Pulsa una tecla para continuar****\n");
            Console.WriteLine("Tiempo del proceso: "+(stop.Subtract(start).TotalMilliseconds/1000)+" segundos");

            while (Console.ReadKey().Key!= ConsoleKey.Enter) ;
        }

        public static void RecojeErrores(List<Grupo> grupos)
        {
            Console.WriteLine("-----Se tubo problema al asignar " + grupos.Count + " grupos-----");

            foreach(Grupo g in grupos)
                Console.WriteLine("----> Materia= " + g.Cve_materia + " Grupo= " + g.num_Grupo);
        }

        public static void Estado(string detalles, float porcentaje)
        {
            avance += porcentaje;
            Console.WriteLine(detalles);
            Console.WriteLine(avance);
        }

        private static void inicializaConeccion()
        {
            string excelDir = @"C:\Users\Fernando\_DD\Mega\UASLP\Sandra\Sistema de Asignacion de Salones\Referencias y Documentos\2016-2017_II\";
            string nombreArchivo = "SIAMMAT16172-FINAL.xlsx";
            //string nombreHoja = "E_2017_01_12";

            c = new Conexion(Conexion.datosConexion, excelDir+nombreArchivo);

            if (c.Autenticacion())
                Console.WriteLine("Coneccion realizada");
            else
            {
                Console.WriteLine("Error al conectar");
                throw new Exception("Error al conectar con la base de datos");
            }
        }
    }
}
