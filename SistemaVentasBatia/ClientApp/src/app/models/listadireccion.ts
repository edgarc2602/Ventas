import { DireccionMin } from './direccionmin';

export interface ListaDireccion {
    idProspecto: number;
    idCotizacion: number;
    idDireccion: number;
    pagina: number;
    rows: number;
    numPaginas: number; 
    direcciones: DireccionMin[];
}