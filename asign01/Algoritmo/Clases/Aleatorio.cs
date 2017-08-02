using System;
using System.Collections.Generic;

namespace Algoritmo02.Clases
{
    /// <summary>
    /// Clase creada para evitar chequeos repeetitivos en el algoritmo cuando se usa un random.
    /// </summary>
    public class Aleatorio
    {
        private Random r;
        private List<int> mostrados;
        private List<int> aceptados;
        private int valMaximo;

        /// <summary>
        /// Constructor de la clase, recive el valor maximo que va a tener el objeto para generar los enteros
        /// </summary>
        /// <param name="valor_maximo">Valor maximo de random</param>
        public Aleatorio(int valor_maximo)
        {
            r = new Random();
            mostrados = new List<int>();
            aceptados = new List<int>();
            valMaximo = valor_maximo;
        }

        /// <summary>
        /// Da un numero aleatorio que no haya dado este objeto 
        /// y lo almacena en una lista temporal para saber cuales son los que ah dado
        /// </summary>
        /// <returns>Nuevo numero aleatorio entero</returns>
        public int Next()
        {
            int res = -1;
            int temp = -1;

            do
            {
                temp = r.Next(valMaximo);
                if (mostrados.Count >= valMaximo)
                    break;
            } while (mostrados.Contains(temp));

            if (!mostrados.Contains(temp))
            {
                res = temp;
                mostrados.Add(temp);
            }

            return res;
        }

        /// <summary>
        /// Reinicia el contador y vuelve a generar numeros aleatorios excepto lo que han sido aceptados
        /// </summary>
        public void Reinicia()
        {
            mostrados = new List<int>(aceptados);
        }

        /// <summary>
        /// Guarda el valor para que no se vuelva a generar aunque se reinicie el contador
        /// </summary>
        /// <param name="num">Numero a marcar</param>
        public void aceptado(int num)
        {
            aceptados.Add(num);
        }
    }
}
