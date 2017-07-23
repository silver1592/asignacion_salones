using OrigenDatos.Clases;
using OrigenDatos.Estructuras;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Algoritmo02.Clases
{
    public class Variable : Grupo
    {
        private Salon salon;    //Salon asignado
        private int Hora;       //Hora en la que se esta asignando

        public float fCiclo     //Convierte el cilco a puntos para que sea facil distinguir cual es el mas actual
        {
            get
            {
                string aux;
                int yIni;
                int yFin;
                float semPar = .5f;

                if (ciclo.Contains("/II"))
                {
                    aux = ciclo.Replace("/II", "");
                    semPar = 2;
                }
                else
                    aux = ciclo.Replace("/I", "");

                yIni = Convert.ToInt32(aux.Split('-')[0]);
                yFin = Convert.ToInt32(aux.Split('-')[1]);

                if (yIni >= 2014)
                    return ((yIni - 2014) * 10 + semPar);
                else
                    return -1;
            }
        }
        public Salon Salon      //Salon asignado
        {
            get { return salon; }
            set
            {
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
        public bool Valido      //Checa si la asignacion es valida
        {
            get
            {
                return EsValido(Salon);
            }
        }
        public float puntosEquipo//Retorna los puntos que se tiene con el equipo instalado en el salon y los que necesita el grupo
        {
            get
            {
                if (salon == null) return 0;
                float pT = this.valorTotalEquipo;
                float pE = this.valorEquipo(salon);

                if (pT == 0)
                    return 1;

                return pE / pT;
            }
        }

        public string salonAnioPasado()
        {
            Variable sel = null;
            Variable v;
            foreach (Grupo g in otrosSemestres)
            {
                v = new Variable(g, 0);
                if (v.fCiclo % 2 == this.fCiclo % 2)
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
            else
                Salon = null; 
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

            //Checa el area. Max = 5
            puntos += s.PrioridadArea(Area) * 5 / 10;

            //Checa la hora anterior. Max = 3
            if (GHoraAnterior != null && GHoraAnterior.Cve_espacio == s.Cve_espacio)
                puntos += 3;

            //Checa el equipo instalado. Max = 2
            puntos += valorEquipo(s) * 2 / 10;

            //Diferencia de cupo del grupo entre el del salon dividido entre 3 y restado a los puntos totales. Extra
            puntos -= Math.Abs(Cupo - salon.Cupo) / 3;

            return puntos;
        }

        public float valorEquipo(Salon salon)
        {
            int res = 0;

            foreach (Requerimiento_Valor req in requerimientos_Salon)
                if (salon.Equipo.Contains(req.requerimiento))
                    res += req.valor;

            return res;
        }

        public bool horario(int hora)
        {
            if (lunes_ini != 0 && lunes_ini <= hora && lunes_fin > hora)
                return true;
            if (martes_ini != 0 && martes_ini <= hora && martes_fin > hora)
                return true;
            if (miercoles_ini != 0 && miercoles_ini <= hora && miercoles_fin > hora)
                return true;
            if (jueves_ini != 0 && jueves_ini <= hora && jueves_fin > hora)
                return true;
            if (viernes_ini != 0 && viernes_ini <= hora && viernes_fin > hora)
                return true;
            if (sabado_ini != 0 && sabado_ini <= hora && sabado_fin > hora)
                return true;

            return false;
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

        public bool EnDias(string dias="111111")
        {
            for (int i = 0; i < 6; i++)
                if ((dias[i] == '1' && this.Dias[i] == '1') || dias[i] == '0')
                    return true;

            return false;
        }

        public bool EnDia(int dia)
        {
            if (this.Dias[dia] == '1')
                return true;

            return false;
        }
    }
}