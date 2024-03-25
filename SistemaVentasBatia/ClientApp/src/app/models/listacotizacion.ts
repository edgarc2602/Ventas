import { CotizacionMin } from './cotizacionmin';

export interface ListaCotizacion {
    idProspecto: number;
    idServicio: number;
    pagina: number;
    numPaginas: number;
    rows: number;
    cotizaciones: CotizacionMin[];
    idEstatusCotizacion: number;
    idAlta: string;
    total: number;
}