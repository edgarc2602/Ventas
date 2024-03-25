export interface PuestoCotizaMin {
    idPuestoDireccionCotizacion: number;
    idDireccionCotizacion: number;
    jornadaDesc: string;
    jornada : number;
    hrInicio: object;
    hrFin: object;
    diaInicio: string;
    diaFin: string;
    sueldo: number;
    vacaciones: number;
    primaVacacional: number;
    imss: number;
    isn: number;
    aguinaldo: number;
    total: number;
    turno: string;
    puesto: string;
    idCotizacion: number;

    diaFestivo: number;
    festivo: number;
    bonos: number;
    vales: number;
    diaDomingo: number;
    domingo: number;
}