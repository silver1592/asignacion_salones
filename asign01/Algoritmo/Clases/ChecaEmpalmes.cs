using Algoritmo01.Heredados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmo01.Clases
{
    public class ChecaEmpalmes
    {
        private ListaGrupos grupos;
        private ListaSalones salones;
        private int hora;

        public ChecaEmpalmes(ListaGrupos _grupos, ListaSalones _salones, int hora)
        {
            grupos = _grupos;
            salones = _salones;
            this.hora = hora;
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
            string extra;
            Grupo g;
            Salon s;

            //obtiene grupos de grupos empalmados
            empalmados = GruposEmpalmados();

            string mensaje;
            foreach(ListaGrupos empalme in empalmados)
            {
                extra = "";    
                mensaje = "";
                //Chequeo de empalme
                if (empalme.HayEmpalme())
                {
                    s = (Salon)salones.buscaSalon(empalme.GetGrupo(0).Salon);
                    #region solucion de empalmes

                    //Solucion de empalme
                    ///selecciona el grupo que se va a quedar con el salon si es que hay algun preferencial
                    Temp = empalme.EnSalonesFijos();

                    ///Si hay conflicto en el preferencial
                    if (Temp.Count() > 1)
                    {
                        //throw new Exception("Verifique Salones preferenciales en la base de datos, hay un problema con el salon " + Temp.GetGrupo(0).Salon);
                        extra = "hay un problema con el salon " + s.Cve_espacio;
                    }
                    ///Solo uno tiene preferencia, y a ese se le va a dar
                    else if (Temp.Count() == 1)
                        empalme.Ejecuta((grupo) =>
                        {
                            if (grupo.Salon_fijo == grupo.Salon)
                                grupo.Salon = "";

                            return null;
                        });
                    /// Si no hay preferencial entonces se elegira por otro medio
                    else
                    {
                        g = empalme.MejorPara(s);

                    }

                    mensaje = extra=="" ? mensaje_plantilla + "_" + hora + "_" : extra;
                    #endregion
                }
            }
        }

        private List<ListaGrupos> GruposEmpalmados()
        {
            List<ListaGrupos> res = salones.Agrupa(grupos);

            return res;
        }
    }
}
