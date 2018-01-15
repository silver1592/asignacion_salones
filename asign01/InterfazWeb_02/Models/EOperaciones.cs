using System.ComponentModel;

enum EOperaciones : byte
{
    //-empalmes -preasignacion -otrosSemestres -algoritmo
    [Description("Algoritmo Genetico")]
    algoritmoGenetico = 1,
    [Description("Revision de empalmes")]
    empalmes,
    [Description("Asignacion preferencial")]
    preasignacion,
    [Description("Asignacion de otros semestres")]
    otrosSemestres
}