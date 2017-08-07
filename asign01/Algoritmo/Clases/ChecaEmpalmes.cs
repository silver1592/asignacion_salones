using OrigenDatos.Clases;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Algoritmo02.Clases
{
    //TODO:Hacer que cheque tambien el empalme con horas siguientes
    public class ChecaEmpalmes
    {
        private ListaVariables grupos;
        private ListaSalones salones;
        private ListaSalones permiteEmpalmes;
        private string ciclo;

        public ListaVariables Grupos { get { return grupos; } }

        public ChecaEmpalmes(ListaGrupos _grupos, IList<Salon> _salones)
        {
            grupos = new ListaVariables(_grupos);
            salones = new ListaSalones(_salones);
            permiteEmpalmes = salones.PermiteEmpalmes();
            if (Grupos.Count != 0)
                ciclo = Grupos[0].Ciclo;
            else
                ciclo = "";
        }

        /// <summary>
        /// Busca los empalmes en el horario designado y comienza a acomodarlos
        /// </summary>
        /// <returns></returns>
        public void ejecuta(string mensaje_plantilla = "")
        {
            List<ListaVariables> empalmados = new List<ListaVariables>();
            ListaGrupos checando = new ListaGrupos();

            //obtiene grupos de grupos empalmados
            empalmados = new ListaVariables(grupos.NoEn(permiteEmpalmes)).AgrupaGruposEmpalmados();

            foreach (ListaVariables empalme in empalmados)
                try
                {
                    ResuelveEmpalme(empalme);
                }
                catch (Exception ex)//Se le quita el salon a todos
                {
                    QuitaSalon(empalme);
                }
        }

        private void ResuelveEmpalme(ListaVariables empalme)
        {
            ListaGrupos Temp;
            ListaVariables aux;
            Salon s;

            aux = empalme.Empalmados();

            //Chequeo de empalme
            if (aux.Count > 1 && permiteEmpalmes.busca(aux[0].Cve_espacio) == null)
            {
                s = salones.busca(empalme[0].Cve_espacio);
                if (s == null) return;  //Si no encuentra el salon es porque es algo como Campo o asi. Se valen los empalmes
                aux.SetSalones(salones);

                Temp = empalme.EnSalonesFijos();

                if (Temp.Count() > 1) { }  //Si hay conflicto en el preferencial
                else if (Temp.Count() == 1)//Solo uno tiene preferencia, y a ese se le va a dar
                    AsignacionPreferencial(empalme, s);
                else    // Si no hay preferencial entonces se elegira por otro medio
                    AsignacionMejorEleccion(empalme, s);
            }

            grupos.Actualiza(empalme);
        }

        /// <summary>
        /// Selecciona entre la lista de grupos el salon que tiene mas puntos en la base de datos para el salon
        /// </summary>
        /// <param name="empalme"></param>
        /// <param name="s"></param>
        private void AsignacionMejorEleccion(ListaVariables empalme, Salon s)
        {
            Variable gOtrosSemestres = null, gAux = null, g =null;
            empalme.SetSalones(salones);

            //Obtiene los gupos validos
            ListaVariables validos = new ListaVariables(empalme);
            //Obtiene los grupos que estaban 
            ListaVariables otrosSemestres = new ListaVariables(validos.AsignacionOtrosSemestres(s.Cve_espacio));
            otrosSemestres.SetSalones(salones);
            validos.SetSalones(salones);
            otrosSemestres = otrosSemestres.OrdenarMejorPuntuacion(s);
            ListaVariables aux = validos.OrdenarMejorPuntuacion(s);

            gAux = aux.Count != 0 ? (aux as IList<Variable>)[0] : null;
            gOtrosSemestres = otrosSemestres.Count != 0 ? (otrosSemestres as IList<Variable>)[0] : null;

            if (gAux != null && gOtrosSemestres != null)
                g = gAux.Puntos > gOtrosSemestres.Puntos ? gAux : gOtrosSemestres;
            else if (gAux != null && gOtrosSemestres == null)
                g = gAux;
            else
                g = gOtrosSemestres;

            QuitaSalon(empalme, g);
        }

        /// <summary>
        /// Busca el grupo que tiene preferencia sobre el salon y a ese se le asigna
        /// </summary>
        /// <param name="empalmes"></param>
        /// <param name="s"></param>
        private void AsignacionPreferencial(ListaVariables empalmes, Salon s)
        {
            Grupo g=null;

            foreach (Grupo grupo in empalmes)
                if (grupo.Salon_fijo == s.Cve_espacio)
                    g = grupo;

            QuitaSalon(empalmes, g);
        }

        private void QuitaSalon(ListaGrupos empalmes, Grupo excepto=null)
        {
            foreach (Grupo g in empalmes)
                if (excepto != null && excepto.cve_full != g.cve_full)
                    g.Cve_espacio = "";
        }
    }
}