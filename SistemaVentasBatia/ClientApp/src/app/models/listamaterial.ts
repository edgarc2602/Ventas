import { MaterialMin } from './materialmin';

export interface ListaMaterial {
    idCotizacion: number;
    idDireccionCotizacion: number;
    idPuestoDireccionCotizacion: number;
    keywords: string;
    pagina: number;
    rows: number;
    numPaginas: number; 
    materialesCotizacion: MaterialMin[];
    edit: number;
}