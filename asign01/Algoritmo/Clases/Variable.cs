﻿using OrigenDatos.Clases;
using OrigenDatos.Estructuras;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Algoritmo02.Clases
{
    public class Variable : Grupo
    {
        /// <summary>
        /// Limite de puntos que puede tener el area
        /// </summary>
        public static int MaxPuntos_Area = 6;
        /// <summary>
        /// Puntos adicionales cuando un grupo cae en el mismo de la hora anterior
        /// </summary>
        public static int MaxPuntos_HoraAnterior = 3;
        /// <summary>
        /// Limite de puntos para cuando se tiene el equipo
        /// </summary>
        public static int MaxPuntos_Equipo = 2;
        /// <summary>
        /// Limite de puntos para cuando cae en el mismo salon que el semestre anterior
        /// </summary>
        public static int MaxPuntos_SemestreAnterior = 4;

        private Salon salon;    //Salon asignado
        private int Hora;       //Hora en la que se esta asignando
        public float fCiclo     //Convierte el cilco a puntos para que sea facil distinguir cual es el mas actual
        {
            get
            {
                string aux;
                int yIni;

                if (ciclo.Contains("/II"))
                    aux = ciclo.Replace("/II", "");
                else
                    aux = ciclo.Replace("/I", "");

                yIni = Convert.ToInt32(aux.Split('-')[0]);

                if (yIni >= 2014)
                    return yIni - 2014;
                else
                    return -1;
            }
        }
        public bool SemestrePar { get { return ciclo.Contains("/II"); } }   //Checa si el semestre es par
        public Salon Salon      //Salon asignado
        {
            get { return salon; }
            set
            {
                if (salon != null)
                    salon.ElminaGrupo(this);
                salon = value;

                if (salon != null)
                {
                    Cve_espacio = salon.Cve_espacio;
                    value.AsignaGrupo(this);
                }
                else
                    Cve_espacio = "";
            }
        }
        public float Puntos     //Puntos que tiene esta asignacion
        {
            get
            {
                if (salon != null)
                {
                    return CalculaPuntos(salon);
                }
                else
                    return 0;
            }
        }
        public int valorTotalEquipo
        {
            get
            {
                int res = 0;

                foreach (Requerimiento_Valor req in requerimientos_Salon)
                    res = req.valor;

                return res;
            }
        }
        public bool Valido      //Checa si la asignacion es valida
        {
            get
            {
                return EsValido(Salon);
            }
        }
        public string salonAnioPasado(int limite=1)//Limite es para que chece la diferencia entre fCiclo y por ejemplo, solo cuente los de hace un año
        {
            Variable sel = null;
            Variable v;
            foreach (Grupo g in otrosSemestres)
            {
                v = new Variable(g, 0);
                if (v.SemestrePar == SemestrePar && fCiclo -limite<= v.fCiclo)
                    if (sel == null || sel.fCiclo < v.fCiclo)
                        sel = v;
            }

            return sel != null ? sel.Cve_espacio : "";
        }

        #region Constuctores
        /// <summary>
        /// Constructor de una variable (cromosoma) del individuo
        /// --No es necesario asignarle un salon
        /// </summary>
        /// <param name="salon">Salon que va a representar</param>
        public Variable(Grupo grupo, int hora):base(grupo)
        {
            Hora = hora;
            salon = null;
        }

        public Variable(Variable v):base(v)
        {
            if (v.salon != null)
                Salon = new Salon(v.salon);
            Hora = v.Hora;
        }
        #endregion

        /// <summary>
        /// Calcula los puntos que tiene el grupo con el salon solicitado
        /// </summary>
        /// <param name="s">Salon con que se quiere checar</param>
        /// <returns>Valor que se obtubo de los puntos. Max = 10</returns>
        public float CalculaPuntos(Salon s)
        {
            float puntos = 0;
            if (s == null) return 0;

            //Max = 6
            puntos += PuntosArea(s) * MaxPuntos_Area / 10;

            //Max = 3
            puntos += PuntosHoraAnterior(s);

            //Max = 2
            puntos += ValorEquipo(s) * MaxPuntos_Equipo / 10;

            //Extra (max = 4)
            puntos += ValorSemestrePasado(s) / MaxPuntos_SemestreAnterior;

            //Diferencia de cupo del grupo entre el del salon dividido entre 4 y restado a los puntos totales. Extra
            puntos -= Math.Abs(DiferenciaCupo(s)) / 4;

            return puntos;
        }

        /// <summary>
        /// Calcula los puntos de area que proporciona el salon, si no hay salon trata de evaluar con el asignado en la variable
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public float PuntosArea(Salon salon=null)
        {
            Salon s = salon != null ? salon : Salon;
            if (s != null)
                return s.PrioridadArea(Area);

            return 0;
        }

        /// <summary>
        /// Checa la hora anterior
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public float PuntosHoraAnterior(Salon salon=null)
        {
            Salon s = salon != null ? salon : Salon;
            if (s != null)
                if (GHoraAnterior != null && GHoraAnterior.Cve_espacio == s.Cve_espacio)
                    return MaxPuntos_HoraAnterior;

            return 0;
        }
        /// <summary>
        /// Checa que haya estado en ese salon el año pasado
        /// </summary>
        /// <param name="salon"></param>
        /// <returns>Puntos dependiendo del semestre en el que estubo</returns>
        public float ValorSemestrePasado(Salon salon =null)
        {
            Salon s = salon != null ? salon : Salon;
            if (s != null)
            {
                ListaGrupos semestres = otrosSemestres.EnSalon(s.Cve_espacio);
                if (semestres.Count() != 0)
                    return (new ListaVariables(semestres).OrdenarPorCiclo() as IList<Variable>)[0].fCiclo;
            }
            return 0;
        }

        /// <summary>
        /// Obtiene la diferencia de espacio que existe entre el salon y el cupo del grupo
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public float DiferenciaCupo(Salon salon=null)
        {
            Salon s = salon != null ? salon : Salon;
            if (s != null)
                return Cupo - s.Cupo;

            return 0;
        }

        /// <summary>
        /// Calcula el valor del equipo comparandolo con el del salon
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public float ValorEquipo(Salon salon=null)
        {
            int res = 0;

            Salon s = salon != null ? salon : Salon;

            if (s != null)
            {
                foreach (Requerimiento_Valor req in requerimientos_Salon)
                    if (s.Equipo.Contains(req.requerimiento))
                        res += req.valor;

                return valorTotalEquipo != 0 ? res * 10 / valorTotalEquipo : res;
            }

            return res;
        }

        /// <summary>
        /// Checa si tiene un grupo con salon fijo, y de ser asi lo asigna, sino regresa false
        /// </summary>
        /// <returns></returns>
        public bool salonPreferencial()
        {
            if (salon_fijo != null && salon_fijo != "")
            {
                Cve_espacio = salon_fijo;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checa si el grupo es valido para asignarse en ese salon
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public bool EsValido(Salon salon)
        {
            if (salon == null) return false;
            else if (Salon_fijo != "" && Salon_fijo != salon.Cve_espacio) return false;
            else if (salones_Posibles.Count != 0 && salones_Posibles.busca(salon.Cve_espacio) == null) return false;
            else if (PlantaBaja && (!salon.plantaBaja || salon.Edificio == "T")) return false;
            else if (Cupo > salon.Cupo) return false;

            return true;
        }

        /// <summary>
        /// Checa si hay empalme con el grupo que se pasa por parametro
        /// </summary>
        /// <param name="grupo">Grupo a checar si hay empalme</param>
        /// <returns>Regresa true si hay un empalme entre los grupos.</returns>
        public bool Empalme(Grupo grupo)
        {
            if (this == grupo)
                return false;

            Variable v = new Variable(grupo, 0);

            if ((lunes_ini >= v.lunes_ini && lunes_ini < v.lunes_fin) ||
                (lunes_fin <= v.lunes_fin && lunes_fin > v.lunes_ini))
                return true;

            if ((martes_ini >= v.martes_ini && martes_ini < v.martes_fin) ||
                (martes_fin <= v.martes_fin && martes_fin > v.martes_ini))
                return true;

            if ((miercoles_ini >= v.miercoles_ini && miercoles_ini < v.miercoles_fin) ||
                (miercoles_fin <= v.miercoles_fin && miercoles_fin > v.miercoles_ini))
                return true;

            if ((jueves_ini >= v.jueves_ini && jueves_ini < v.jueves_fin) ||
                (jueves_fin <= v.jueves_fin && jueves_fin > v.jueves_ini))
                return true;

            if ((viernes_ini >= v.viernes_ini && viernes_ini < v.viernes_fin) ||
                (viernes_fin <= v.viernes_fin && viernes_fin > v.viernes_ini))
                return true;

            if ((sabado_ini >= v.sabado_ini && sabado_ini < v.sabado_fin) ||
                (sabado_fin <= v.sabado_fin && sabado_fin > v.sabado_ini))
                return true;

            return false;
        }

        /// <summary>
        /// Checa si el grupo se encuentra asignado en las horas y dias marcados
        /// </summary>
        /// <param name="ini">hora de inicio</param>
        /// <param name="fin">hora de fin</param>
        /// <param name="dias">Dias en los que se desea checar</param>
        /// <returns></returns>
        public bool EnHora(int ini, int fin, string dias = "111111")
        {
            if (dias[0] == '1')
                if ((lunes_ini >= ini && lunes_ini < fin) ||
                    (lunes_fin <= fin && lunes_fin > ini))
                    return true;

            if (dias[1] == '1')
                if ((martes_ini >= ini && martes_ini < fin) ||
                (martes_fin <= fin && martes_fin > ini))
                    return true;

            if (dias[2] == '1')
                if ((miercoles_ini >= ini && miercoles_ini < fin) ||
                (miercoles_fin <= fin && miercoles_fin > ini))
                    return true;

            if (dias[3] == '1')
                if ((jueves_ini >= ini && jueves_ini < fin) ||
                (jueves_fin <= fin && jueves_fin > ini))
                    return true;

            if (dias[4] == '1')
                if ((viernes_ini >= ini && viernes_ini < fin) ||
                (viernes_fin <= fin && viernes_fin > ini))
                    return true;

            if (dias[5] == '1')
                if ((sabado_ini >= ini && sabado_ini < fin) ||
                (sabado_fin <= fin && sabado_fin > ini))
                    return true;

            return false;
        }

        /// <summary>
        /// Checa si el grupo se encuentra en los dias solicitados
        /// </summary>
        /// <param name="dias"></param>
        /// <returns></returns>
        public bool EnDias(string dias="111111")
        {
            for (int i = 0; i < 6; i++)
                if ((dias[i] == '1' && this.Dias[i] == '1') || dias[i] == '0')
                    return true;

            return false;
        }

        /// <summary>
        /// Checa si el grupo se encuentra en los dias solicitados
        /// </summary>
        /// <param name="dia"></param>
        /// <returns></returns>
        public bool EnDia(int dia)
        {
            if (this.Dias[dia] == '1')
                return true;

            return false;
        }
    }
}