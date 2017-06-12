using Algoritmo02.Heredados;
using System.Collections.Generic;
using System.Linq;

namespace Algoritmo02.Clases
{
    public class ChecaEmpalmes
    {
        private ListaGrupos grupos;
        private ListaSalones salones;
        private int hora;

        public ListaGrupos Grupos { get { return grupos; } }

        public ChecaEmpalmes(ListaGrupos _grupos, ListaSalones _salones)
        {
            grupos = _grupos;
            salones = _salones;
        }

        /// <summary>
        /// Busca los empalmes en el horario designado y comienza a acomodarlos
        /// </summary>
        /// <returns></returns>
        public void ejecuta(string mensaje_plantilla = "")
        {
            List<ListaGrupos> empalmados = new List<ListaGrupos>();
            ListaGrupos checando = new ListaGrupos();
            ListaGrupos Temp;
            Grupo g;
            Salon s;

            //obtiene grupos de grupos empalmados
            empalmados = GruposEmpalmados();

            foreach(ListaGrupos empalme in empalmados)
            {
                //Chequeo de empalme
                if (empalme.HayEmpalme())
                {
                    s = new  Salon(salones.busca(empalme[0].Salon));
                    #region solucion de empalmes

                    ///Solucion de empalme
                    ///selecciona el grupo que se va a quedar con el salon si es que hay algun preferencial
                    Temp = empalme.EnSalonesFijos();


                    if (Temp.Count() > 1) { }  ///Si hay conflicto en el preferencial
                    else if (Temp.Count() == 1)///Solo uno tiene preferencia, y a ese se le va a dar
                    {
                        foreach (Grupo grupo in empalme)
                            if (grupo.Salon_fijo == grupo.Salon)
                                grupo.Salon = "";
                    }
                    else    /// Si no hay preferencial entonces se elegira por otro medio
                    {
                        g = empalme.MejorPara(s);
                        if (g != null)
                        {
                            foreach (Grupo a in empalme)
                                a.Salon = "";
                            g.Salon = s.Cve_espacio;
                        }
                    }
                    #endregion
                }
            }
        }

        private List<ListaGrupos> GruposEmpalmados()
        {
            List<ListaGrupos> res = salones.AgrupaGruposEmpalmados(grupos);

            return res;
        }
    }
}
