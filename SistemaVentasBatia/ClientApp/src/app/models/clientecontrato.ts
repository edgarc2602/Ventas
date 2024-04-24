export interface ClienteContrato {
    idClienteContrato: number;
    idEmpresa: number;
    idProspecto: number;
    idCotizacion: number;

    //cliente datos
    constitutivaEscrituraPublica: string;
    constitutivaFecha: string;
    constitutivaLicenciado: string;
    constitutivaNumeroNotario: string;
    constitutivaFolioMercantil: string;
    constitutivaIdEstado: number;
    poderEscrituraPublica: string;
    poderFecha: string;
    poderLicenciado: string;
    poderNumeroNotario: string;
    poderIdEstado: number;
    clienteRegistroPatronal: string;
    //poliza
    polizaMonto: number;
    polizaMontoLetra: string;
    polizaEmpresa: string;
    polizaNumero: string;
    //vigencia
    contratoVigencia: string;
    //contrato empresa
    empresaContactoNombre: string;
    empresaContactoCorreo: string;
    empresaContactoTelefono: string;
    //domicilio fiscal
    clienteDireccion: string;
    clienteColonia: number;
    clienteColoniaDescripcion: string;
    clienteMunicipio: number;
    clienteMunicipioDescripcion: string;
    clienteEstado: number;
    cp: string;
    clienteRazonSocial: string;
    clienteRfc: string;
    clienteEmail: string;
    clienteContactoNombre: string;
    clienteContactoTelefono: string;
    clienteRepresentante: string;
}