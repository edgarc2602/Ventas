import { ItemN } from './item';

export interface Cotizacion {
    idCotizacion: number;
    idProspecto: number;
    idServicio: number;
    total: number;
    fechaAlta: string;
    idCotizacionOriginal: number;
    idPersonal: number;
    listaServicios: ItemN[];
    listaTipoSalarios: ItemN[];
    salTipo: number;
}