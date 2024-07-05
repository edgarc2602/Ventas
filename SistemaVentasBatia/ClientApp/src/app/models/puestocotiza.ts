export interface PuestoCotiza {
    idPuestoDireccionCotizacion: number;
    idPuesto: number;
    idDireccionCotizacion: number;
    idCotizacion: number;
    cantidad?: number;
    idTurno: number;
    idSalario: number;
    hrInicio: string;
    hrFin: string;
    diaInicio: number;
    diaFin: number;
    fechaAlta: string;
    idPersonal: number;
    sueldo: number;
    vacaciones: number;
    primaVacacional: number;
    imss: number;
    isn: number;
    aguinaldo: number;
    total: number;

    jornada: number;
    jornadadesc: string;
    idTabulador: number;
    idClase: number;
    idZona: number;
    diaFestivo: boolean;
    festivo: number;
    bonos: number;
    vales: number;
    diaDomingo: boolean;
    domingo: number;

    diaCubreDescanso: boolean;
    cubreDescanso: number;
    hrInicioFin: string;
    hrFinFin: string;
    diaInicioFin: number;
    diaFinFin: number;
    diaDescanso: number;
    diasEvento: number;
    incluyeMaterial: boolean;

}