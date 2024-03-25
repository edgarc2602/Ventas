import { Prospecto } from './prospecto';

export interface ListaProspecto {
    prospectos: Prospecto[];
    idEstatusProspecto: number;
    keywords: string;
    pagina: number;
    rows: number;
    numPaginas: number;
}