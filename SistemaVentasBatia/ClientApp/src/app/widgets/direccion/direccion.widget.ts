import { Component, OnChanges, Input, SimpleChanges, Inject, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Direccion } from '../../models/direccion';
import { Catalogo } from '../../models/catalogo';
declare var bootstrap: any;
import { ToastWidget } from '../toast/toast.widget';
import { DireccionResponseAPI } from '../../models/direccionresponseapi';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';

@Component({
    selector: 'direc-widget',
    templateUrl: './direccion.widget.html'
})
export class DireccionWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('smEvent') sendEvent = new EventEmitter<any>();
    model: Direccion = {} as Direccion;
    tips: Catalogo[] = [];
    edos: Catalogo[] = [];
    tabs: Catalogo[] = [];
    muns: Catalogo[] = [];
    idD: number = 0;
    idP: number = 0;
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
    validacion: boolean = false;
    isLoading: boolean = false;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {
        http.get<Catalogo[]>(`${url}api/catalogo/getestado`).subscribe(response => {
            this.edos = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getinmuebletipo`).subscribe(response => {
            this.tips = response;
        }, err => console.log(err));
    }

    getDireccionAPI() {
        if (this.model.codigoPostal.length === 5) {
            this.iniciarCarga();
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
            this.http.get<DireccionResponseAPI>(`${this.url}api/direccion/GetDireccionAPI/${this.model.codigoPostal}`).subscribe(response => {
                this.detenerCarga();
                this.direccionAPI = response
                this.model.idEstado = this.direccionAPI.codigoPostal.idEstado;
                this.loadMun();
                this.model.idMunicipio = this.direccionAPI.codigoPostal.idMunicipio;
            }, err => {
                this.detenerCarga();
                console.log(err);
            });
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

    nuevo() {
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
        this.model = {
            idDireccion: 0, idProspecto: this.idP, idCotizacion: 0, nombreSucursal: '',
            idTipoInmueble: 0, idEstado: 0, municipio: '', idMunicipio: 0, ciudad: '', colonia: '',
            domicilio: '', referencia: '', codigoPostal: '', idDireccionCotizacion: 0, frontera: false
        };
    }

    refresh() {
        this.http.get<Direccion>(`${this.url}api/direccion/${this.idD}/0`).subscribe(response => {
            this.model = response;
            this.chgEdo();
        }, err => console.log(err));
    }

    guarda() {
        for (let mun of this.muns) {
            if (mun.id == this.model.idMunicipio) {
                this.model.municipio = mun.descripcion
            }
        }
        this.lerr = {};
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                if (this.model.idDireccion == 0) {
                    this.http.post<Direccion>(`${this.url}api/direccion`, this.model).subscribe(response => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast('Direcci\u00F3n creada');
                        }, 300);
                        this.sendEvent.emit(response.idDireccion);
                        this.close();
                    }, err => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.errorToast('Ocurri\u00F3 un error');
                        }, 300);
                        if (err.error) {
                            if (err.error.errors) {
                                this.lerr = err.error.errors;
                            }
                        }
                        console.log(err);
                    });
                } else {
                    this.http.put<Direccion>(`${this.url}api/direccion`, this.model).subscribe(response => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast('Direcci\u00F3n actualizada');
                        }, 300);
                        this.close();
                        this.sendEvent.emit(0);
                    }, err => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.errorToast('Ocurri\u00F3 un error');
                        }, 300);
                        if (err.error) {
                            if (err.error.errors) {
                                this.lerr = err.error.errors;
                            }
                        }
                        console.log(err);
                    });
                }
            }, 300);
        }
    }

    chgEdo() {
        this.http.get<Catalogo[]>(`${this.url}api/tabulador/getbyedo/${1}`).subscribe(response => {
            this.tabs = response;
        }, err => console.log(err));
    }

    loadMun() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getmunicipio/${this.model.idEstado}`).subscribe(response => {
            this.muns = response;
        }, err => console.log(err));
    }

    valida() {
        this.validacion = true;
        if (this.model.nombreSucursal == '') {
            this.lerr['NombreSucursal'] = ['Nombre de sucursal es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idTipoInmueble == 0) {
            this.lerr['IdTipoInmueble'] = ['Tipo de inmueble es obligatorio'];
            this.validacion = false;
        }
        if (this.model.codigoPostal == '') {
            this.lerr['CodigoPostal'] = ['Codigo postal es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idEstado == 0) {
            this.lerr['IdEstado'] = ['Estado es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idMunicipio == 0) {
            this.lerr['IdMunicipio'] = ['Municipio es obligatorio'];
            this.validacion = false;
        }
        if (this.model.colonia == '') {
            this.lerr['Colonia'] = ['Colonia es obligatorio'];
            this.validacion = false;
        }
        if (this.model.ciudad == '') {
            this.lerr['Ciudad'] = ['Ciudad es obligatorio'];
            this.validacion = false;
        }
        if (this.model.domicilio == '') {
            this.lerr['Domicilio'] = ['Domicilio es obligatorio'];
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

    existe() {
        this.http.get<Direccion>(`${this.url}api/direccion/obtenerdireccion/${this.idD}/${this.idP}`).subscribe(response => {
            this.model = response;
            this.getDireccionAPI();
            this.loadMun();
        }, err => {
            console.log(err);
            if (err.error) {
                if (err.error.errors) {
                    this.lerr = err.error.errors;
                }
            }
        });
        let docModal = document.getElementById('modalAgregarDireccion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    open(pro: number, dir: number) {
        this.lerr = {};
        this.idP = pro;
        this.idD = dir;
        if (dir != 0) {
            this.existe();
        }
        else if (dir == 0) {
            this.nuevo();
        }
        else {
            this.refresh();
        }
        let docModal = document.getElementById('modalAgregarDireccion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
        this.chgEdo();
    }

    close() {
        let docModal = document.getElementById('modalAgregarDireccion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
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