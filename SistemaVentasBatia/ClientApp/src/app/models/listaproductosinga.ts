import { productosinga } from "./productosinga";
export interface listaproductosinga {
    productos: productosinga[];
    keywords: string
    pagina: number;
    numPaginas: number;
    rows: number;
}