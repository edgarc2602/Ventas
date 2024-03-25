import { PuestoCotizaMin } from './puestocotizamin';
import { DireccionMin } from './direccionmin';

export interface ListaPuesto {
    puestosDireccionesCotizacion: PuestoCotizaMin[];
    direccionesCotizacion: DireccionMin[];
    idCotizacion: number;
    idDireccionCotizacion: number;
    idPuestoDireccionCotizacion: number;
    length: number;
    empleados: number;
}