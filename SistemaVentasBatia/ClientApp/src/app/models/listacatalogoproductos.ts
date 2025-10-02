import { Catalogo } from "./catalogo";
export interface listacatalogoproductos

{
    productos: Catalogo[];
    keywords: string;
    pagina: number;
    numPaginas: number;
    rows: number;
}