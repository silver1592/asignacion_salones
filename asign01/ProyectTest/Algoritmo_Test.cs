using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algoritmo02.Clases;

namespace ProyectTest
{
    /// <summary>
    /// Descripción resumida de Algoritmo_Test
    /// </summary>
    [TestClass]
    public class Algoritmo_Test
    { 
        //Prueba de aleatorio
        [TestMethod]
        public void Aleatorio_Test()
        {
            int max=10, num;
            List<int> aceptados= new List<int>();
            Aleatorio aleatorio = new Aleatorio(max);
            for(int i = 0; i < max; i++)
            {
                num = aleatorio.Next();
                aleatorio.aceptado(num);
                if (aceptados.Contains(num))
                    Assert.Fail();
                aceptados.Add(num);
            }
        }

        //Prueba de ejecucion del algoritmo

        //Prueba de chequeo de empalmes

        //Prueba de PreAsignacion

        //Prueba de constructores de variable

        //Prueba de metodos de variable

        //prueba de constructores de individuo

        //prueba de metodos de individuo
    }
}
