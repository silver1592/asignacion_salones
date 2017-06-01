using Algoritmo02.Clases;
using System;
using System.Collections.Generic;
using OrigenDatos.Clases;

namespace Algoritmo02
{
    class Program
    {
        private static float avance;
        private static Conexion c;

        static void Main(string[] args)
        {
            TimeSpan stop;
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
            Asignacion temp;
            string ciclo = "2016-2017/I";

            inicializaConeccion();

            for(int i=7;i<22;i++)
            {
                avance = 0;
                //temp = new Asignacion(G, i);
                //Eventos
                //temp.Estado += new Asignacion.estado(Estado);
                //temp.Finalizado += new Asignacion.Finaliza(RecojeErrores);

                //algoritmo
                //temp.EjecutaAlgoritmo();
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
            string excelDir = @"C:\Users\Fernando\_DD\Mega\UASLP\Sandra\Sistema de Asignacion de Salones\Referencias y Documentos\";
            string nombreArchivo = "SIAMMAT16172-FINAL.xlsx";
            string nombreHoja = "Query_1";

            c = new Conexion(Conexion.datosConexionPrueba, excelDir, nombreArchivo, nombreHoja);


            if (c.Autenticacion())
                Console.WriteLine("Coneccion realizada");
            else
            {
                Console.WriteLine("Error al conectar");
                throw new Exception("Error al conectar con la base de datos");
            }

            //Grupo.Coneccion = c;
            //Salon.Coneccion = c;
            //Algoritmo.Coneccion = c;
            //Asignacion.Conexion = c;
        }
    }
}
