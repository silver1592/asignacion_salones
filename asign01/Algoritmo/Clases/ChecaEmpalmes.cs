using OrigenDatos.Clases;
using System.Collections.Generic;
using System.Linq;

namespace Algoritmo02.Clases
{
    public class ChecaEmpalmes
    {
        private ListaVariables grupos;
        private ListaSalones salones;

        public ListaVariables Grupos { get { return grupos; } }

        public ChecaEmpalmes(ListaGrupos _grupos, IList<Salon> _salones)
        {
            grupos = new ListaVariables(_grupos);
            salones = new ListaSalones(_salones);
        }

        /// <summary>
        /// Busca los empalmes en el horario designado y comienza a acomodarlos
        /// </summary>
        /// <returns></returns>
        public void ejecuta(string mensaje_plantilla = "")
        {
            List<ListaVariables> empalmados = new List<ListaVariables>();
            ListaGrupos checando = new ListaGrupos();
            ListaGrupos Temp;
            Grupo g;
            Salon s;

            //obtiene grupos de grupos empalmados
            empalmados = GruposEmpalmados();

            foreach(ListaVariables empalme in empalmados)
            {
                //Chequeo de empalme
                if (empalme.Empalmados().Count!=0)
                {
                    s = new  Salon(salones.busca(empalme[0].Cve_espacio));
                    #region solucion de empalmes

                    ///Solucion de empalme
                    ///selecciona el grupo que se va a quedar con el salon si es que hay algun preferencial
                    Temp = empalme.EnSalonesFijos();


                    if (Temp.Count() > 1) { }  ///Si hay conflicto en el preferencial
                    else if (Temp.Count() == 1)///Solo uno tiene preferencia, y a ese se le va a dar
                    {
                        foreach (Grupo grupo in empalme)
                            if (grupo.Salon_fijo == grupo.Cve_espacio)
                                grupo.Cve_espacio = "";
                    }
                    else    /// Si no hay preferencial entonces se elegira por otro medio
                    {
                        g = empalme.MejorPara(s);
                        if (g != null)
                        {
                            foreach (Grupo a in empalme)
                                a.Cve_espacio = "";
                            g.Cve_espacio = s.Cve_espacio;
                        }
                    }
                    #endregion
                }
            }
        }

        private List<ListaVariables> GruposEmpalmados()
        {
            List<ListaVariables> res = grupos.AgrupaGruposEmpalmados();

            return res;
        }
    }
}
