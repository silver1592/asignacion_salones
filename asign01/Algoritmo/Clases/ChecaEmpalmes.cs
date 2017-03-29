using Algoritmo01.Heredados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo01.Clases
{s
    public class ChecaEmpalmes
    {
        private ListaGrupos grupos;
        private ListaSalones salones;

        /// <summary>
        /// Busca los empalmes en el horario designado y comienza a acomodarlos
        /// </summary>
        /// <returns></returns>
        public void ejecuta(string mensaje_plantilla = "")
        {
            List<List<Grupo>> empalmados = new List<List<Grupo>>();
            List<Grupo> checando = new List<Grupo>();
            List<Grupo> Temp;

            //obtiene grupos de grupos empalmados
            empalmados = GruposEmpalmados();

            int i = 1;
            string mensaje;
            while (empalmados.Count != 0)
            {
                checando = empalmados[0];
                mensaje = "";
                //Chequeo de empalme
                if (HayEmpalme(empalmados[0]))
                {
                    #region solucion de empalmes

                    //Solucion de empalme
                    ///selecciona el grupo que se va a quedar con el salon si es que hay algun preferencial
                    var salonPref = from g in checando
                                    where g.Salon_fijo == g.Salon
                                    select g;

                    Temp = salonPref.ToList();

                    ///Si hay conflicto en el preferencial
                    if (Temp.Count > 1)
                    {
                        var count = from g in Temp
                                    where g.RPE != checando[0].RPE
                                    select g;

                        if (count.ToList().Count > 0)
                            //throw new Exception("Verifique Salones preferenciales en la base de datos, hay un problema con el salon " + checando[0].Salon);
                            foreach (Grupo g in Temp)
                                g.Salon = "-";
                    }
                    ///Solo uno tiene preferencia, y a ese se le va a dar
                    else if (Temp.Count == 1)
                    {
                        foreach (Grupo g in checando)
                            if (Temp[0] != g)
                                g.Salon = "";
                    }
                    /// se elegira por otro medio
                    else
                        checando = corrigeEmpalmes(checando);

                    mensaje = mensaje_plantilla + "_" + hora + "_" + i;
                    #endregion
                }

                //Guardar informacion
                guardaMovimiento(checando, mensaje);

                empalmados.Remove(checando);
                i++;
            }
        }

        private List<List<Grupo>> GruposEmpalmados()
        {
            List<List<Grupo>> res = new List<List<Grupo>>();
            List<Grupo> aux;
            foreach (Salon s in Salones)
            {
                aux = GruposDelSalon(s.Cve_espacio);

                if (aux.Count > 1)
                    res.Add(aux);
            }

            return res;
        }

        private bool HayEmpalme(List<Grupo> lista)
        {
            //se obtiene el salon al que se hace referencia
            Salon salon = buscaSalon(lista[0].Salon);

            //Posible error
            if (salon == null) { throw new Exception("Error al ejecutar los empalmes (" + salon.Cve_espacio + ")", null); }

            if (salon.empalme) return false;
            //Checa el salon
            foreach (Grupo g1 in lista)
                foreach (Grupo g2 in lista)
                    if (g1 != g2 && g1.empalme(g2))
                        return true;

            return false;
        }

        /// <summary>
        /// Proceso que se ejecuta cuando indiscutiblemente hay un empalme entre 2 grupos, analiza cual es el que se debe de cambiar y cua no
        /// </summary>
        /// <param name="checando">Grupos con empalme</param>
        /// <returns>Misma lista de grupos con el empalme corregido</returns>
        private List<Grupo> corrigeEmpalmes(List<Grupo> checando)
        {
            ///Checa si estan en el horario
            ///por precaucion
            var query0 = from g in checando
                         where g.horario(hora)
                         select g;

            if (query0.ToList().Count == checando.Count)
            {
                var query = from g in checando
                            orderby g.AsignacionSemestreAnterior(hora) descending
                            select g;
                if (query.ToList().Count != 0)
                {
                    Grupo sel = query.ToList()[0];

                    foreach (Grupo g in query)
                        if (g != sel && sel.empalme(g))
                            g.Salon = "";
                }
            }
            else
            {
                var otrasHoras = from g in checando
                                 where g.horario(hora)
                                 select g;

                foreach (Grupo g in otrasHoras)
                {
                    var q = from gr in query0
                            where gr.empalme(g)
                            select gr;

                    foreach (Grupo gr in q)
                        gr.Salon = "";
                }
            }

            return checando;
        }
    }
}
