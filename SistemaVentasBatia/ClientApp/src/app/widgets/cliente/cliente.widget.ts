import { Component, OnChanges, Input, SimpleChanges, Inject, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { Direccion } from '../../models/direccion';
import { Catalogo } from '../../models/catalogo';
declare var bootstrap: any;
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { Prospecto } from '../../models/prospecto';
import { Cliente } from '../../models/cliente';
import { StoreUser } from 'src/app/stores/StoreUser';
import { error } from 'protractor';
import { saveAs } from 'file-saver';
import { ClienteContrato } from '../../models/clientecontrato';


@Component({
    selector: 'cliente-widget',
    templateUrl: './cliente.widget.html',
    providers: [DatePipe]
})
export class ClienteWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('clienteEvent') sendEvent = new EventEmitter<any>();
    @ViewChild('contratoInput', { static: false }) contratoInput!: ElementRef;

    idCotizacion: number = 0;
    idProspecto: number = 0;
    idServicio: number = 0;
    validacion: boolean = false;
    isLoading: boolean = false;
    lerr: any = {};
    prospecto: Prospecto = {} as Prospecto;
    model: Cliente = {} as Cliente;
    contrato: ClienteContrato = {} as ClienteContrato;
    ejecutivos: Catalogo[] = [];
    empresas: Catalogo[] = [];
    gerentes: Catalogo[] = [];

    tipoContrato: boolean = false;
    idClienteGenerado: number = 0;
    contratoSeleccionado: File | null = null;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe, private user: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/obtenerCatalogoEjecutivos`).subscribe(response => {
            this.ejecutivos = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/obtenerEmpresas`).subscribe(response => {
            this.empresas = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/ObtenerCatalogoGerentesLimpieza`).subscribe(response => {
            this.gerentes = response;
        }, err => console.log(err));
    }


    nuevo() {
        this.lerr = {};
        let fec: Date = new Date();
        this.model = {
            idServicio: 0, idCotizacion: 0, idProspecto: 0, idPersonal: 0, idCliente: 0, codigo: '', idTipo: 1, nombreComercial: '', contacto: '', departamento: '', puesto: '', email: '', telefonos: '', idEjecutivo: 0, idGerenteLimpieza: 0, idEmpresaPagadora: 0, facturacion: '0', tipoFacturacion: '0',
            credito: 0, diasFacturacion: 0, fechaInicio: null, vigencia: 0, fechaTermino: null, porcentajeMateriales: 0, porcentajeIndirectos: 0, diaLimiteFacturar: 0, totalSucursales: 0, totalEmpleados: 0, incluyeMaterial: false, incluyeHerramienta: false,
            deductivaMaterial: false, deductivaServicio: false, deductivaPlantilla: false, deductivaPlazoEntrega: false
        }
        this.prospecto = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '', representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [], idPersonal: this.user.idPersonal, idEstatusProspecto: 0
        };
        this.contratoSeleccionado = null;
    }

    guarda() {
        this.lerr = {};
        if (this.valida()) {
            this.iniciarCarga();
            this.model.idServicio = this.idServicio;
            this.model.fechaInicio = new Date(this.model.fechaInicio);
            this.model.idCotizacion = this.idCotizacion;
            this.model.idProspecto = this.idProspecto;
            this.model.idPersonal = this.user.idPersonal;
            this.http.post<number>(`${this.url}api/cliente/ConvertirProspectoACliente/${this.user.direccionIP}`, this.model).subscribe(response => {
                this.idClienteGenerado = response;
                if (this.tipoContrato == true) {
                    this.cargarContratoCliente();
                }
                else {
                    this.generarContratoBaseCliente();
                }
            }, err => {
                this.detenerCarga();
                console.log(err);
            });
        }
    }

    generarContratoBaseCliente() {
        //obtener datos contrato
        this.http.get<ClienteContrato>(`${this.url}api/cliente/ObtenerDatosExistentesClienteContrato/${this.idProspecto}`).subscribe(response => {
            this.contrato = response;
            this.http.get<Prospecto>(`${this.url}api/prospecto/ObtenerDatosExistentesProspecto/${this.idProspecto}`).subscribe(response => {
                this.contrato.clienteRazonSocial = response.nombreComercial;
                this.contrato.clienteRfc = response.rfc;
                //enviar contrato y descargar copia
                this.http.post(`${this.url}api/report/GenerarYDescargarContratoBaseCliente/${this.idCotizacion}/${this.idClienteGenerado}`, this.contrato, { responseType: 'arraybuffer' })
                    .subscribe(
                        (data: ArrayBuffer) => {
                            const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' });
                            saveAs(blob, 'Contrato_' + this.model.nombreComercial + '.docx');
                            this.okToast('Cliente generado correctamente');
                            this.detenerCarga();
                            this.close();
                            error => {
                                this.errorToast('Ocurri\u00F3 un error');
                                this.detenerCarga();
                                console.error('Error al obtener el archivo DOCX', error);
                            }
                        });
            }, err => {
                console.log(err);
                this.errorToast('Ocurri\u00F3 un error');
                this.detenerCarga();
            });
        }, err => {
            console.log(err);
            this.errorToast('Ocurri\u00F3 un error');
            this.detenerCarga();
        });

    }
    cargarContratoCliente() {
        const formData = new FormData();
        formData.append('contratoSeleccionado', this.contratoSeleccionado);
        formData.append('idClienteGenerado', this.idClienteGenerado.toString());
        formData.append('nombreComercial', this.model.nombreComercial);

        this.http.post<boolean>(`${this.url}api/cliente/InsertarContratoCliente`, formData).subscribe(response => {
            this.okToast('Cliente generado correctamente');
            this.detenerCarga();
            this.close();
        }, err => {
            this.errorToast('Ocurri\u00F3 un error');
            this.detenerCarga();
        });
    }

    onContratoSeleccionado(event: any) {
        this.contratoSeleccionado = event.target.files[0];
    }
    limpiarContrato() {
        this.contratoSeleccionado = null;
        this.contratoInput.nativeElement.value = '';
    }

    valida() {
        this.validacion = true;
        if (this.model.nombreComercial == '') {
            this.lerr['NombreComercial'] = ['Nombre Comercial es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idEjecutivo == 0 || this.model.idEjecutivo == null) {
            this.lerr['IdEjecutivo'] = ['Ejecutivo es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idGerenteLimpieza == 0 || this.model.idGerenteLimpieza == null) {
            this.lerr['IdGerenteLimpieza'] = ['Gerente limpieza es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idEmpresaPagadora == 0 || this.model.idEmpresaPagadora == null) {
            this.lerr['IdEmpresaPagadora'] = ['Empresa pagadora es obligatorio'];
            this.validacion = false;
        } 1
        if (this.model.facturacion == '0' || this.model.facturacion == null || this.model.facturacion == undefined) {
            this.lerr['Facturacion'] = ['Facturacion es obligatorio'];
            this.validacion = false;
        }
        if (this.model.tipoFacturacion == '0' || this.model.tipoFacturacion == null || this.model.tipoFacturacion == undefined) {
            this.lerr['TipoFacturacion'] = ['Tipo de facturacion es obligatorio'];
            this.validacion = false;
        }
        if (this.model.diasFacturacion == 0 || this.model.diasFacturacion == null) {
            this.lerr['DiasFacturacion'] = ['Dias de facturacion es obligatorio'];
            this.validacion = false;
        }
        if (this.model.fechaInicio == null || this.model.fechaInicio == null) {
            this.lerr['FechaInicio'] = ['Fecha de inicio es obligatoria'];
            this.validacion = false;
        }
        if (this.model.vigencia == 0 || this.model.vigencia == null) {
            this.lerr['Vigencia'] = ['Vigencia es obligatorio'];
            this.validacion = false;
        }
        if (this.model.fechaTermino == null || this.model.fechaTermino == null) {
            this.lerr['FechaTermino'] = ['Fecha de termino es obligatoria'];
            this.validacion = false;
        }
        if (this.tipoContrato == true) {
            if (this.contratoSeleccionado == null) {
                this.lerr['ContratoSeleccionado'] = ['Seleccione un contrato'];
                this.validacion = false;
            }
        }

        return this.validacion;
    }

    open(idCotizacion: number, idProspecto: number, idServicio: number) {
        this.idCotizacion = idCotizacion;
        this.idProspecto = idProspecto;
        this.idServicio = idServicio;
        this.nuevo();
        this.obtenerTotalSucursalesCotizacion();
        this.obtenerTotalEmpleadosCotizacion();
        this.obtenerInformacionProspecto();
        let docModal = document.getElementById('modalCliente');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    obtenerInformacionProspecto() {
        this.http.get<Prospecto>(`${this.url}api/prospecto/${this.idProspecto}`).subscribe(response => {
            this.model.nombreComercial = response.nombreComercial;
        }, err => console.log(err));
    }
    obtenerTotalSucursalesCotizacion() {
        this.http.get<number>(`${this.url}api/cotizacion/ObtenerTotalSucursalesCotizacion/${this.idCotizacion}`).subscribe(response => {
            this.model.totalSucursales = response;
        }, err => console.log(err));
    }
    obtenerTotalEmpleadosCotizacion() {
        this.http.get<number>(`${this.url}api/cotizacion/ObtenerTotalEmpleadosCotizacion/${this.idCotizacion}`).subscribe(response => {
            this.model.totalEmpleados = response;
        }, err => console.log(err));
    }

    calcularFechaTermino() {
        if (this.model.fechaInicio != null && this.model.vigencia != null && this.model.vigencia != 0) {
            this.model.vigencia = Number(this.model.vigencia.toString().substr(0, 3));
            let fechaTermino = new Date(this.model.fechaInicio);
            fechaTermino.setMonth(fechaTermino.getMonth() + this.model.vigencia);
            let fechaTerminoFormateada = fechaTermino.toISOString().substr(0, 10);

            this.model.fechaTermino = new Date(fechaTerminoFormateada);
        }
    }


    close() {
        let docModal = document.getElementById('modalCliente');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
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

    okToast(message: string) {
        this.toastWidget.errMessage = message;
        this.toastWidget.isErr = false;
        this.toastWidget.open();
    }

    errorToast(message: string) {
        this.toastWidget.isErr = true;
        this.toastWidget.errMessage = message;
        this.toastWidget.open();
    }

    iniciarCarga() {
        this.isLoading = true;
        this.cargaWidget.open(true);
    }

    detenerCarga() {
        this.isLoading = false;
        this.cargaWidget.open(false);
    }
}