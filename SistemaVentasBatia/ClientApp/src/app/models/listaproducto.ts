import { ProductoPrecioEstado } from "./productoprecioestado";
import { ProductoFamilia } from "./productofamilia";
export interface ListaProducto {
    productos: ProductoPrecioEstado[];
    familias: ProductoFamilia[];
    pagina: number;
    numPaginas: number;
    rows: number;
    idProveedor: number;
    proveedor: string;
}