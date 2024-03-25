import { ItemN } from './item';
export interface Prospecto {
    idProspecto: number;
    nombreComercial: string;
    razonSocial: string;
    rfc: string;
    domicilioFiscal: string;
    representanteLegal: string;
    telefono: string;
    fechaAlta: string;
    nombreContacto: string;
    emailContacto: string;
    numeroContacto: string;
    extContacto: string;
    listaDocumentos: ItemN[];
    idCotizacion: number;
    idPersonal: number;
    idEstatusProspecto: number;
    polizaCumplimiento: boolean;
}