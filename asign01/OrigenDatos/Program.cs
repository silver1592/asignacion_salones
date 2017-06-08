using OrigenDatos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrigenDatos
{
    class Program
    {
        private static Conexion c;

        static void Main(string[] args)
        {
            inicializaConeccion();
            string cad;
            string aux, aux2, aux3;

            do
            {
                Console.WriteLine("Elige una opcion a realizar:");
                Console.WriteLine("\t1.- Ejecuta Asignacion preferencial");
                Console.WriteLine("\t2.- Checa los empalmes");
                Console.WriteLine("\t3.- Soluciona empalmes");
                Console.WriteLine("\t4.- Muestra Horario de un salon");
                Console.WriteLine("\t5.- Muestra horario de un profesor");
                Console.WriteLine("\t6.- Busca salon disponible de hI a hF que esten en un conjunto D");
                Console.WriteLine("\t-.- Ejecuta SQL");
                Console.WriteLine("\t0.- Salir");

                cad = Console.ReadLine();

                switch(cad)
                {
                    case "1":
                        Console.WriteLine("Funcion no implementada");
                    break;
                    case "2":
                        Console.WriteLine("Ejecutando chequeo de empalmes");
                        foreach (ListaGrupos empalme in c.GetExcel.Grupos.Empalmes())
                            imprimeLista(empalme);
                    break;
                    case "3":
                        Console.WriteLine("Funcion no implementada");
                    break;
                    case "4":
                        Console.Write("Salon:");
                        aux = Console.ReadLine();
                        Console.WriteLine("Obteniendo horario del salon");
                        Console.WriteLine("*****************************");
                        imprimeHorario(c.GetExcel.Grupos.EnSalon(aux).Grupos);
                    break;
                    case "5":
                        Console.Write("RPE:");
                        aux = Console.ReadLine();
                        Console.WriteLine("Obteniendo horario del profesor");
                        Console.WriteLine("*******************************");
                        foreach (Grupo g in c.GetExcel.Grupos.ConProfesor(aux).Grupos)
                            Console.WriteLine(g.ToString());
                    break;
                    case "6":
                        Console.Write("Hora inicial:");
                        aux = Console.ReadLine();
                        Console.Write("Hora final:");
                        aux2 = Console.ReadLine();
                        Console.Write("Dias:");
                        aux3 = Console.ReadLine();

                        salonesDisponibles(Convert.ToInt32(aux), Convert.ToInt32(aux2), aux3);
                    break;
                    case "-":
                        Console.Write("Escribe busqueda:");
                        aux = Console.ReadLine();
                        c.Querry(aux);
                    break;
                }
            } while (cad != "0");
        }

        private static void inicializaConeccion()
        {
            string excelDir = @"C:\Users\Fernando\_DD\Mega\UASLP\Sandra\Sistema de Asignacion de Salones\Referencias y Documentos\2016-2017_II\";
            string nombreArchivo = "SIAMMAT16172-FINAL.xlsx";
            string nombreHoja = "Query_1";

            c = new Conexion(Conexion.datosConexionPrueba, excelDir, nombreArchivo, nombreHoja, tipo: "T");


            if (c.Autenticacion())
                Console.WriteLine("Coneccion realizada");
            else
            {
                Console.WriteLine("Error al conectar");
                throw new Exception("Error al conectar con la base de datos");
            }
        }

        private static void imprimeLista(ListaGrupos lista)
        {
            Console.WriteLine("****************************************");
            Console.WriteLine(lista.ToString());
        }

        private static void imprimeHorario(List<Grupo> g)
        {
            int[,] horario = new int[6,14];

            for(int i = 0; i < 14; i++)
                foreach(Grupo grupo in g)
                    for(int dia = 0; dia<6; dia++)
                        if (grupo.horario_ini[dia] <= i + 7 && grupo.horario_fin[dia] > i + 7)
                            horario[dia, i] += 1;

            for (int i = 0; i < 15; i++)
            {
                if (i == 0)
                    Console.WriteLine("\tL\tM\tMi\tJ\tV\tS");
                else
                {
                    Console.Write(i +7 - 1);
                    for (int d = 0; d < 6; d++)
                        Console.Write("\t" + horario[d, i - 1]);
                }

                Console.WriteLine();
            }
                
        }

        private static void salonesDisponibles(int ini, int fin, string dias)
        {
            ListaSalones salones = new ListaSalones(c, c.Salones(), ini);
            ListaSalones sDisponibles = salones.Disponibles(ini, fin, dias);

            foreach (Salon s in sDisponibles.Salones)
                Console.WriteLine(s.ToString());
        }
    }
}