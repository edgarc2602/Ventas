export interface CotizaResumenLim {
    idCotizacion: number;
    idProspecto: number;
    idServicio: number;
    salario: number;
    cargaSocial: number;
    prestaciones: number;
    provisiones: number;
    material: number;
    uniforme: number;
    equipo: number;
    herramienta: number;
    servicio: number;
    subTotal: number;
    indirecto: number;
    utilidad: number;
    comisionSV: number;
    comisionExt: number;
    total: number;
    idCotizacionOriginal: number;

    nombreComercial: string;
    utilidadPor: string;
    indirectoPor: string;
    csvPor: string;
    comisionExtPor: string;
    polizaCumplimiento: boolean;
    totalPolizaCumplimiento: number;
    idEstatus: number;
    diasEvento: number;
}