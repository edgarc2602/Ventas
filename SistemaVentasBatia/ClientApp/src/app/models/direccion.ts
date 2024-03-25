export interface Direccion {
    idDireccion: number;
    idProspecto: number;
    idCotizacion: number;
    nombreSucursal: string;
    idTipoInmueble: number;
    idEstado: number;
    
    municipio: string;
    idMunicipio: number;
    ciudad: string;
    colonia: string;
    domicilio: string;
    referencia: string;
    codigoPostal: string;
    idDireccionCotizacion: number;
    frontera: boolean;
}