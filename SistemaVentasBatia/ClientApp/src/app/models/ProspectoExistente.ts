import { ItemN } from './item';
export interface ProspectoExistente {
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
    usuarioAlta: string;
    idEstatusProspecto: number;
    idTipoIndustria: number;
    /*polizaCumplimiento: boolean;*/

    //poderRepresentanteLegal: string;
    //actaConstitutiva: string;
    //registroPatronal: string;
    //empresaVenta: number;

    idGrupoActivo: number;
}