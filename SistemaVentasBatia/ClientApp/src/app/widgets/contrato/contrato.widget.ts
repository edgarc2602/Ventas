import { Component, Inject, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from '../../stores/StoreUser';
import { saveAs } from 'file-saver';
import { Catalogo } from '../../models/catalogo';
import { DatePipe } from '@angular/common';
import { ItemN } from '../../models/item';
import { ToastWidget } from '../toast/toast.widget';
import { Prospecto } from '../../models/prospecto';
import { ClienteContrato } from '../../models/clientecontrato';
import { DireccionResponseAPI } from '../../models/direccionresponseapi';

declare var bootstrap: any;

@Component({
    selector: 'contrato-widget',
    templateUrl: './contrato.widget.html',
    providers: [DatePipe]
})
export class ContratoWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @Output('contratoEvent') sendEvent = new EventEmitter<number>();
    direccionAPI: DireccionResponseAPI = {
        message: '',
        error: false,
        codigoPostal: {
            estado: '',
            idEstado: 0,
            estadoAbreviatura: '',
            municipio: '',
            idMunicipio: 0,
            centroReparto: '',
            codigoPostal: '',
            colonias: []
        }
    };
    contrato: ClienteContrato = {} as ClienteContrato;
    model: Prospecto = {} as Prospecto;
    empresas: Catalogo[] = [];
    edos: Catalogo[] = [];
    muns: Catalogo[] = [];
    docs: ItemN[] = [];
    idCotizacion: number = 0;
    idProspecto: number = 0;
    validacion: boolean = false;
    isLoading: boolean = false;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser, private dtpipe: DatePipe) {
        http.get<Catalogo[]>(`${url}api/catalogo/getestado`).subscribe(response => {
            this.edos = response;
        }, err => console.log(err));
    }

    nuevo() {
        this.lerr = {};
        let fec: Date = new Date();
        this.contrato = {
            idClienteContrato: 0, idEmpresa: 0, idProspecto: 0, idCotizacion: 0, constitutivaEscrituraPublica: '', constitutivaFecha: '', constitutivaLicenciado: '', constitutivaNumeroNotario: '', constitutivaFolioMercantil: '', constitutivaIdEstado: 0,
            poderEscrituraPublica: '', poderFecha: '', poderLicenciado: '', poderNumeroNotario: '', poderIdEstado: 0, polizaMonto: 0, polizaMontoLetra: '', polizaEmpresa: '', polizaNumero: '', contratoVigencia: '', empresaContactoNombre: '',
            empresaContactoCorreo: '', empresaContactoTelefono: '', clienteDireccion: '', clienteColonia: 0, clienteColoniaDescripcion: '', clienteMunicipio: 0, clienteMunicipioDescripcion: '', clienteEstado: 0, cp: '', clienteRegistroPatronal: '',
            clienteRazonSocial: '', clienteRfc: '', clienteEmail: '', clienteContactoNombre: '', clienteContactoTelefono: '', clienteRepresentante: ''
        }
    }

    existe() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/ObtenerEmpresas`).subscribe(response => {
            this.empresas = response;
        }, err => console.log(err));

        this.http.get<ClienteContrato>(`${this.url}api/cliente/ObtenerDatosExistentesClienteContrato/${this.idProspecto}`).subscribe(response => {
            this.contrato = response;
            this.http.get<Prospecto>(`${this.url}api/prospecto/ObtenerDatosExistentesProspecto/${this.idProspecto}`).subscribe(response => {
                this.contrato.clienteRazonSocial = response.nombreComercial;
                this.contrato.clienteRfc = response.rfc;
                this.getDireccionAPI();
            }, err => console.log(err));
        }, err => console.log(err));
    }

    guardarDatosContrato() {
        this.http.post<boolean>(`${this.url}api/cliente/InsertarDatosClienteContrato`, this.contrato).subscribe(response => {
            this.isLoading = false;
        }, err => console.log(err));
    }

    g

    guardar() {
        this.lerr = {};
        if (this.valida()) {
            this.guardarDatosContrato();
            this.isLoading = false;
            this.sendEvent.emit(1);
            this.close();
        }
    }


    generarContratoBase() {
        this.isLoading = true;
        this.http.post(`${this.url}api/report/DescargarContratoDOCX/${this.idCotizacion}`, this.contrato, { responseType: 'arraybuffer' })
            .subscribe(
                (data: ArrayBuffer) => {
                    const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' });
                    saveAs(blob, 'Contrato ' + this.contrato.clienteRazonSocial + '.docx');
                    this.isLoading = false;
                    this.close();
                    this.toastWidget.isErr = false;
                    this.toastWidget.errMessage = 'Contrato descargado';
                    this.toastWidget.open();
                    error => {
                        console.error('Error al obtener el archivo DOCX', error);
                        this.isLoading = false;
                        this.toastWidget.isErr = true;
                        this.toastWidget.errMessage = 'Ocurri\u00F3 un error';
                        this.toastWidget.open();
                    }
                });
    }

    valida() {
        this.contrato.clienteMunicipioDescripcion = 'w'
        this.contrato.clienteMunicipioDescripcion = 'w';
        this.contrato.idCotizacion = this.idCotizacion;
        this.contrato.idProspecto = this.idProspecto;
        this.validacion = true;
        if (this.contrato.idEmpresa == 0 || this.contrato.idEmpresa == undefined) {
            this.lerr['idEmpresa'] = ['Empresa es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.empresaContactoNombre == '' || this.contrato.empresaContactoNombre == undefined) {
            this.lerr['empresaContactoNombre'] = ['Nombre es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.empresaContactoCorreo == '' || this.contrato.empresaContactoNombre == undefined) {
            this.lerr['empresaContactoCorreo'] = ['Email es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.empresaContactoTelefono == '' || this.contrato.empresaContactoTelefono == undefined) {
            this.lerr['empresaContactoTelefono'] = ['Teléfono es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteRazonSocial == '' || this.contrato.clienteRazonSocial == undefined) {
            this.lerr['clienteRazonSocial'] = ['Razón social es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteRfc == '' || this.contrato.clienteRfc == undefined) {
            this.lerr['clienteRfc'] = ['RFC es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.contratoVigencia == '' || this.contrato.contratoVigencia == undefined || this.contrato.contratoVigencia == null) {
            this.lerr['contratoVigencia'] = ['Vigencia de contrato es obligatoria'];
            this.validacion = false;
        }
        if (this.contrato.cp == '' || this.contrato.cp == undefined) {
            this.lerr['cp'] = ['Codigo postal es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteEstado == 0 || this.contrato.cp == undefined) {
            this.lerr['clienteEstado'] = ['Estado es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteMunicipio == 0 || this.contrato.clienteMunicipio == undefined) {
            this.lerr['clienteMunicipio'] = ['Municipio es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteColoniaDescripcion == '' || this.contrato.clienteColoniaDescripcion == undefined) {
            this.lerr['clienteColoniaDescripcion'] = ['Colonia es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteDireccion == '' || this.contrato.clienteDireccion == undefined) {
            this.lerr['clienteDireccion'] = ['Dirección es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.constitutivaEscrituraPublica == '' || this.contrato.constitutivaEscrituraPublica == undefined) {
            this.lerr['constitutivaEscrituraPublica'] = ['No. Escritura pública es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.constitutivaFecha == '' || this.contrato.constitutivaFecha == undefined || this.contrato.constitutivaFecha == null) {
            this.lerr['constitutivaFecha'] = ['Fecha es obligatoria'];
            this.validacion = false;
        }
        if (this.contrato.constitutivaLicenciado == '' || this.contrato.constitutivaLicenciado == undefined) {
            this.lerr['constitutivaLicenciado'] = ['Licenciado es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.constitutivaNumeroNotario == '' || this.contrato.constitutivaNumeroNotario == undefined) {
            this.lerr['constitutivaNumeroNotario'] = ['No.Notario público es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.constitutivaFolioMercantil == '' || this.contrato.constitutivaFolioMercantil == undefined) {
            this.lerr['constitutivaFolioMercantil'] = ['Folio mercantil es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.poderEscrituraPublica == '' || this.contrato.poderEscrituraPublica == undefined) {
            this.lerr['poderEscrituraPublica'] = ['No. Escritura pública es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.poderFecha == '' || this.contrato.poderFecha == undefined || this.contrato.poderFecha == null) {
            this.lerr['poderFecha'] = ['Fecha es obligatoria'];
            this.validacion = false;
        }
        if (this.contrato.poderLicenciado == '' || this.contrato.poderLicenciado == undefined) {
            this.lerr['poderLicenciado'] = ['Licenciado es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.poderNumeroNotario == '' || this.contrato.poderNumeroNotario == undefined) {
            this.lerr['poderNumeroNotario'] = ['No.Notario público es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.polizaEmpresa == '' || this.contrato.polizaEmpresa == undefined) {
            this.lerr['polizaEmpresa'] = ['Empresa es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.polizaMonto == 0 || this.contrato.polizaMonto == undefined) {
            this.lerr['polizaMonto'] = ['Monto es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.polizaMontoLetra == '' || this.contrato.polizaMontoLetra == undefined) {
            this.lerr['polizaMontoLetra'] = ['Monto en letra es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.polizaNumero == '' || this.contrato.polizaNumero == undefined) {
            this.lerr['polizaNumero'] = ['Numero de póliza es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteRepresentante == '' || this.contrato.clienteRepresentante == undefined || this.contrato.clienteRepresentante == null) {
            this.lerr['clienteRepresentante'] = ['Representante es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteEmail == '' || this.contrato.clienteEmail == undefined) {
            this.lerr['clienteEmail'] = ['Email es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteContactoNombre == '' || this.contrato.clienteContactoNombre == undefined) {
            this.lerr['clienteContactoNombre'] = ['Nombre es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.clienteContactoTelefono == '' || this.contrato.clienteContactoTelefono == undefined) {
            this.lerr['clienteContactoTelefono'] = ['Teléfono es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.constitutivaIdEstado == 0 || this.contrato.constitutivaIdEstado == undefined) {
            this.lerr['constitutivaIdEstado'] = ['Estado es obligatorio'];
            this.validacion = false;
        }
        if (this.contrato.poderIdEstado == 0 || this.contrato.poderIdEstado == undefined) {
            this.lerr['poderIdEstado'] = ['Estado es obligatorio'];
            this.validacion = false;
        }
        return this.validacion;
    }

    ferr(nm: string) {
        let fld = this.lerr[nm];
        if (fld)
            return true;
        else
            return false;
    }

    terr(nm: string) {
        let fld = this.lerr[nm];
        let msg: string = fld.map((x: string) => "-" + x);
        return msg;
    }

    open(idCotizacion: number, idProspecto: number) {
        this.isLoading = false;
        this.nuevo();
        this.contrato.idProspecto = idProspecto;
        this.contrato.idCotizacion = idCotizacion;
        this.idCotizacion = idCotizacion;
        this.idProspecto = idProspecto;
        this.existe();
        let docModal = document.getElementById('modalContrato');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalContrato');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    loadMun() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getmunicipio/${this.contrato.clienteEstado}`).subscribe(response => {
            this.muns = response;
        }, err => console.log(err));
    }

    getDireccionAPI() {
        if (this.contrato.cp.length === 5) {
            this.direccionAPI = {
                message: '',
                error: false,
                codigoPostal: {
                    estado: '',
                    idEstado: 0,
                    estadoAbreviatura: '',
                    municipio: '',
                    idMunicipio: 0,
                    centroReparto: '',
                    codigoPostal: '',
                    colonias: []
                }
            };
            this.http.get<DireccionResponseAPI>(`${this.url}api/direccion/GetDireccionAPI/${this.contrato.cp}`).subscribe(response => {
                this.direccionAPI = response
                this.contrato.clienteEstado = this.direccionAPI.codigoPostal.idEstado;
                this.loadMun();
                this.contrato.clienteMunicipio = this.direccionAPI.codigoPostal.idMunicipio;
            })
        }
        else {
            this.direccionAPI = {
                message: '',
                error: false,
                codigoPostal: {
                    estado: '',
                    idEstado: 0,
                    estadoAbreviatura: '',
                    municipio: '',
                    idMunicipio: 0,
                    centroReparto: '',
                    codigoPostal: '',
                    colonias: []
                }
            };
        }
    }
}