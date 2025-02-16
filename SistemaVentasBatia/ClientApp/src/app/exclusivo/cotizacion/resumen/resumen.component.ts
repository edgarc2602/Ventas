﻿import { Component, Inject, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { CotizaResumenLim } from 'src/app/models/cotizaresumenlim';
import { ItemN } from 'src/app/models/item';
import { DireccionCotizacion } from 'src/app/models/direccioncotizacion';
import { ListaDireccion } from 'src/app/models/listadireccion';
import { ListaPuesto } from 'src/app/models/listapuesto';
import { ListaMaterial } from 'src/app/models/listamaterial';
import { ListaServicio } from 'src/app/models/listaservicio';
import { Catalogo } from 'src/app/models/catalogo';
import { MaterialAddWidget } from 'src/app/widgets/materialadd/materialadd.widget';
import { MaterialWidget } from 'src/app/widgets/material/material.widget';
import { DireccionWidget } from 'src/app/widgets/direccion/direccion.widget';
import { PuestoWidget } from 'src/app/widgets/puesto/puesto.widget';
import { ServicioAddWidget } from 'src/app/widgets/servicioadd/servicioadd.widget';
import { Cotizacionupd } from 'src/app/models/cotizacionupd';
import { Router } from '@angular/router';
import { ReportService } from 'src/app/report.service';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import { saveAs } from 'file-saver';
import { PuestoLayoutWidget } from 'src/app/widgets/puestolayout/puestolayout.widget';
import Swal from 'sweetalert2';
import { MarcaVenta } from 'src/app/widgets/marcaventa/marcaventa.widget';
import { ContratoWidget } from '../../../widgets/contrato/contrato.widget';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { Prospecto } from '../../../models/prospecto';
import { DatePipe } from '@angular/common';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { ConfirmacionWidget } from 'src/app/widgets/confirmacion/confirmacion.widget'
import { SubirContratoClienteWidget } from '../../../widgets/subircontratocliente/subircontratocliente.widget';
import { ClienteWidget } from '../../../widgets/cliente/cliente.widget';
import { ProductoGeneralWidget } from '../../../widgets/productogeneral/productogeneral.widget';
import { MaterialPuesto } from '../../../models/materialpuesto';


@Component({
    selector: 'resumen',
    templateUrl: './resumen.component.html',
    animations: [fadeInOut],
    providers: [DatePipe]
})
export class ResumenComponent implements OnInit, OnDestroy {
    @ViewChild(MaterialAddWidget, { static: false }) proAdd: MaterialAddWidget;
    @ViewChild(MaterialWidget, { static: false }) proPue: MaterialWidget;
    @ViewChild(DireccionWidget, { static: false }) dirAdd: DireccionWidget;
    @ViewChild(PuestoWidget, { static: false }) pueAdd: PuestoWidget;
    @ViewChild(ServicioAddWidget, { static: false }) serAdd: ServicioAddWidget;
    @ViewChild(PuestoLayoutWidget, { static: false }) puelay: PuestoLayoutWidget;
    @ViewChild(MarcaVenta, { static: false }) marven: MarcaVenta;
    @ViewChild(ContratoWidget, { static: false }) contrato: ContratoWidget;
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @ViewChild(ConfirmacionWidget, { static: false }) confirma: ConfirmacionWidget;
    @ViewChild(ClienteWidget, { static: false }) clienteWidget: ClienteWidget;
    @ViewChild(SubirContratoClienteWidget, { static: false }) subirCont: SubirContratoClienteWidget;
    @ViewChild(ProductoGeneralWidget, { static: false }) productoG: ProductoGeneralWidget;
    @ViewChild('resumen', { static: false }) resumen: ElementRef;
    @ViewChild('pdfCanvas', { static: true }) pdfCanvas: ElementRef;
    @ViewChild('fileInputDir', { static: false }) fileInputDir: ElementRef<HTMLInputElement>;
    @ViewChild('fileInputPlan', { static: false }) fileInputPlan: ElementRef<HTMLInputElement>;
    @ViewChild('fileInputProductoExtra', { static: false }) fileInputProductoExtra: ElementRef<HTMLInputElement>;
    @ViewChild('resumenCot', { static: false }) resumenContainer: ElementRef;
    @ViewChild('tbdirecciones', { static: false }) tbdireciones: ElementRef;
    @ViewChild('tbpuestos', { static: false }) tbpuestos: ElementRef;

    model: CotizaResumenLim = {
        idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, prestaciones: 0, provisiones: 0,
        material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0,
        subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', utilidadPor: '', indirectoPor: '', csvPor: '', comisionSV: 0, comisionExt: 0, comisionExtPor: '', polizaCumplimiento: false, polizaPor: '', totalPolizaCumplimiento: 0, idEstatus: 0, diasEvento: 0
    };
    modelDir: DireccionCotizacion = {
        idCotizacion: 0, idDireccionCotizacion: 0, idDireccion: 0, nombreSucursal: ''
    };
    modelcot: Cotizacionupd = {
        idCotizacion: 0, indirecto: '', utilidad: '', comisionSV: '', comisionExt: '', polizaPor: ''
    };
    dirs: ItemN[] = [];
    cotdirs: Catalogo[] = [];
    indust: Catalogo[] = [];
    docs: ItemN[] = [];
    lsdir: ListaDireccion = {} as ListaDireccion;
    lspue: ListaPuesto = {
        puestosDireccionesCotizacion: [], direccionesCotizacion: [], idCotizacion: 0, idDireccionCotizacion: 0, idPuestoDireccionCotizacion: 0, length: 0, empleados: 0, pagina: 1, rows: 0, numPaginas: 0
    };
    lsmat: ListaMaterial = {} as ListaMaterial;
    lsher: ListaMaterial = {} as ListaMaterial;
    lsser: ListaServicio = {} as ListaServicio;
    modelpros: Prospecto = {} as Prospecto;
    selDireccion: number = 0;
    selPuesto: number = 0;
    selMatDir: number = 0;
    selMatPue: number = 0;
    edit: number = 0;
    isGen: number = 0;
    isSuc: number = 0;
    autorizacion: number = 0;
    idCotN: number = 0;
    idpro: number = 0;
    idope: number = 0;
    idDC: number = 0;
    sDir: boolean = false;
    isLoadinglay: boolean = false;
    isLoading: boolean = false;
    validaciones: boolean = false;
    totalDia: number = 0;
    allTabsOpen = true;
    selTipo: string = 'material';
    txtMatKey: string = '';
    indirectoValue: string = this.model.utilidadPor;
    utilidadValue: string = this.model.indirectoPor;
    CSV: string = this.model.csvPor;
    ComisionExtValue: string = this.model.comisionExtPor;
    urlF: string = '';
    pdfUrl: string;
    validaMess: string = '';
    nombreSucursal: string = '';
    puesto: string = '';
    reportData: Blob;
    lerr: any = {};
    sub: any;
    incluyeProducto: boolean = false;


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private route: ActivatedRoute, private rtr: Router, private reportService: ReportService, public user: StoreUser, private dtpipe: DatePipe, private sinU: StoreUser) {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        this.nuevo();
        this.lsdir = {
            pagina: 1, idCotizacion: this.model.idProspecto, idProspecto: this.model.idProspecto, idDireccion: 0, direcciones: [], rows: 0, numPaginas: 0
        };
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`, { headers }).subscribe(response => {
            this.autorizacion = response;
        }, err => {
            this.validaError(err);
        });
        http.get<Catalogo[]>(`${url}api/catalogo/ObtenerCatalogoTiposdeIndustria`, { headers }).subscribe(response => {
            this.indust = response;
        }, err => {
            this.validaError(err);
        });
    }
    ngOnInit(): void {
        this.sub = this.route.params.subscribe(params => {
            let idcot: number = +params['id'];
            this.existe(idcot);
        });
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }

    validaError(err: any) {
        if (err.status === 401) {
            this.errorToast('⚠️ No autorizado. Inicia sesión nuevamente.');
            this.rtr.navigate(['']);
        } else {
            this.errorToast('Ocurri\u00F3 un error');
        }
    }
    getHeaders() {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        return headers;
    }

    nuevo() {
        this.model = {
            idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, prestaciones: 0, provisiones: 0, material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0, subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0,
            idServicio: 0, nombreComercial: '', utilidadPor: '', indirectoPor: '', csvPor: '', comisionSV: 0, comisionExt: 0, comisionExtPor: '', polizaCumplimiento: false, totalPolizaCumplimiento: 0, polizaPor: '', idEstatus: 0, diasEvento: 0
        };
        let fec: Date = new Date();
        this.modelpros = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '', representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [], idPersonal: this.sinU.idPersonal, idEstatusProspecto: 0, idTipoIndustria: 0
        };
        this.docs.forEach(d => d.act = false);
    }

    existe(id: number) {
        this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${id}`, { headers: this.getHeaders() }).subscribe(response => {
            this.model = response;
            this.totalDia = (response.subTotal + response.indirecto + response.utilidad + response.comisionSV + response.comisionExt + response.totalPolizaCumplimiento) / this.model.diasEvento;
            this.http.get<Prospecto>(`${this.url}api/prospecto/${this.model.idProspecto}`, { headers: this.getHeaders() }).subscribe(response => {
                this.modelpros = response;
                this.docs = this.modelpros.listaDocumentos;
            }, err => {
                this.validaError(err);
            });
            this.getAllDirs();
            this.getDirs();
            this.getPlan();
        }, err => {
            this.validaError(err);
        });
    }

    onFileChangeDir(event: any): void {
        this.isLoadinglay = true;
        const file = event.target.files[0];
        const formData = new FormData();
        formData.append('file', file);
        this.http.post<boolean>(`${this.url}api/cargamasiva/CargarDirecciones/${this.model.idCotizacion}/${this.model.idProspecto}`, formData, { headers: this.getHeaders() }).subscribe((response) => {
            this.okToast('Direcciones cargadas correctamente');
            this.getAllDirs();
            this.getDirs();
            this.isLoadinglay = false;
        }, err => {
            this.validaError(err);
            this.isLoadinglay = false;
            this.errorToast('Ocurri\u00F3 un error al cargar el layout, verifique la informaci\u00F3n')
        });
        this.fileInputDir.nativeElement.value = '';
    }

    onFileChangePlan(event: any): void {
        this.isLoadinglay = true;
        const file = event.target.files[0];
        const formData = new FormData();
        formData.append('file', file);
        this.http.post<boolean>(`${this.url}api/cargamasiva/CargarPlantilla/${this.model.idCotizacion}`, formData, { headers: this.getHeaders() }).subscribe((response) => {
            setTimeout(() => {
                this.getPlan();
                this.isLoadinglay = false;
                this.detenerCarga();
                this.okToast('Plantillas cargadas correctamente');
                //Swal.fire({
                //    icon: 'success',
                //    timer: 500,
                //    showConfirmButton: false,
                //});
            }, 300);
        }, err => {
            setTimeout(() => {
                this.validaError(err);
                this.isLoadinglay = false;
                this.detenerCarga();
                this.errorToast('Ocurri\u00F3 un error al cargar el layout, verifique la informaci\u00F3n');
                //Swal.fire({
                //    title: 'Error',
                //    text: 'Ocurri\u00F3 un error al cargar el layout, verifique la informaci\u00F3n',
                //    icon: 'error',
                //    timer: 2000,
                //    showConfirmButton: false,
                //});
            }, 300);
        });
        this.fileInputPlan.nativeElement.value = '';
    }

    onFileChangeProductoExtra(event: any): void {
        this.isLoadinglay = true;
        const file = event.target.files[0];
        const formData = new FormData();
        formData.append('file', file);
        this.http.post<boolean>(`${this.url}api/cargamasiva/CargaLayoutProductoExtra/${this.model.idCotizacion}/${this.selTipo}/${this.user.idPersonal}`, formData, { headers: this.getHeaders() }).subscribe((response) => {
            setTimeout(() => {
                this.getMat(this.selTipo);
                this.isLoadinglay = false;
                this.detenerCarga();
                this.okToast('Productos cargados correctamente');
                //Swal.fire({
                //    icon: 'success',
                //    timer: 500,
                //    showConfirmButton: false,
                //});
            }, 300);
        }, err => {
            setTimeout(() => {
                this.validaError(err);
                this.isLoadinglay = false;
                this.detenerCarga();
                this.errorToast(err.error.message);
                //Swal.fire({
                //    title: 'Error',
                //    text: 'Ocurri\u00F3 un error al cargar el layout, verifique la informaci\u00F3n',
                //    icon: 'error',
                //    timer: 2000,
                //    showConfirmButton: false,
                //});
            }, 300);
        });
        this.fileInputProductoExtra.nativeElement.value = '';
    }

    descargarLayoutDirectorio() {
        this.http.post(`${this.url}api/cargamasiva/DescargarLayoutDirectorio`, null, { headers: this.getHeaders(), responseType: 'blob' }).subscribe((response: Blob) => {
            saveAs(response, 'LayoutDirectorio.xlsx');
            this.okToast('Layout descargado');
        }, err => {
            this.validaError(err);
        });
    }

    descargarLayoutPlantilla() {
        this.http.post(`${this.url}api/cargamasiva/DescargarLayoutPlantilla/${this.model.idCotizacion}`, null, { headers: this.getHeaders(), responseType: 'blob' }).subscribe((response: Blob) => {
            saveAs(response, 'LayoutPlantilla.xlsx');
            this.okToast('Layout descargado');
        }, err => {
            this.validaError(err);
        });
    }

    descargarLayoutProductoExtra() {
        this.http.post(`${this.url}api/cargamasiva/DescargarLayoutProductoExtra/${this.model.idCotizacion}`, null, { headers: this.getHeaders(), responseType: 'blob' }).subscribe((response: Blob) => {
            saveAs(response, 'Layout' + this.selTipo + '_Extra.xlsx');
            this.okToast('Layout descargado');
        }, err => {
            this.validaError(err);
        });
    }

    propuestaSuministroInsumos() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.get(`${this.url}api/report/DescargarPropuestaInsumos/${this.model.idCotizacion}`, { headers: this.getHeaders(), responseType: 'arraybuffer' })
                .subscribe(
                    (data: ArrayBuffer) => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast('Propuesta descargada');
                        }, 300);
                        const file = new Blob([data], { type: 'application/pdf' });
                        const fileURL = URL.createObjectURL(file);
                        const width = 800;
                        const height = 550;
                        const left = window.innerWidth / 2 - width / 2;
                        const top = window.innerHeight / 2 - height / 2;
                        const newWindow = window.open(fileURL, '_blank', `width=${width}, height=${height}, top=${top}, left=${left}`);
                        if (newWindow) {
                            newWindow.focus();
                        } else {
                            alert('La ventana emergente ha sido bloqueada. Por favor, permite ventanas emergentes para este sitio.');
                        }
                    },
                    error => {
                        this.validaError(error);
                        this.detenerCarga();
                        setTimeout(() => {
                            this.errorToast('Ocurri\u00F3 un error');
                        }, 300);
                        console.error('Error al obtener el archivo PDF', error);
                    }
                );
        }, 300);
    }

    propuestaEvento() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.get(`${this.url}api/report/DescargarPropuestaEvento/${this.model.idCotizacion}`, { headers: this.getHeaders(), responseType: 'arraybuffer' })
                .subscribe(
                    (data: ArrayBuffer) => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast('Propuesta descargada');
                        }, 300);
                        const file = new Blob([data], { type: 'application/pdf' });
                        const fileURL = URL.createObjectURL(file);
                        const width = 800;
                        const height = 550;
                        const left = window.innerWidth / 2 - width / 2;
                        const top = window.innerHeight / 2 - height / 2;
                        const newWindow = window.open(fileURL, '_blank', `width=${width}, height=${height}, top=${top}, left=${left}`);
                        if (newWindow) {
                            newWindow.focus();
                        } else {
                            alert('La ventana emergente ha sido bloqueada. Por favor, permite ventanas emergentes para este sitio.');
                        }
                    },
                    error => {
                        this.validaError(error);
                        this.detenerCarga();
                        setTimeout(() => {
                            this.errorToast('Ocurri\u00F3 un error');
                        }, 300);
                        console.error('Error al obtener el archivo PDF', error);
                    }
                );
        }, 300);
    }

    savePros(event) {
        this.model.idProspecto = event;
        this.getAllDirs();
    }

    guardaPros() {
        this.quitarFocoDeElementos2();
        this.modelpros.listaDocumentos = this.docs;
        this.lerr = {};
        if (this.validaPros()) {
            this.iniciarCarga();
            setTimeout(() => {
                this.http.put<Prospecto>(`${this.url}api/prospecto`, this.modelpros, { headers: this.getHeaders() }).subscribe(response => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Prospecto actualizado')
                    }, 300);
                }, err => {
                    this.validaError(err);
                    this.detenerCarga();
                    setTimeout(() => {
                        this.errorToast('Ocurri\u00F3 un error');
                    }, 300);
                    if (err.error) {
                        for (let key in err.error) {
                            if (err.error.hasOwnProperty(key)) {
                                this.lerr[key] = [err.error[key]];
                            }
                        }
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                    console.log(err);
                });
            }, 300);
        }
    }

    validaPros() {
        this.validaciones = true;
        if (this.modelpros.nombreComercial == '' || this.modelpros.nombreComercial == null) {
            this.lerr['NombreComercial'] = ['Nombre Comercial es requerido'];
            this.validaciones = false;
        }
        if (this.modelpros.rfc.length > 13) {
            this.lerr['RfcLenght'] = ['El RFC no puede contener m\u00E1s de 13 caracteres'];
            this.validaciones = false;
        }
        if (this.modelpros.nombreContacto == '' || this.modelpros.nombreContacto == null) {
            this.lerr['NombreContacto'] = ['Contacto es requerido'];
            this.validaciones = false;
        }
        if (this.modelpros.emailContacto == '' || this.modelpros.emailContacto == null) {
            this.lerr['EmailContacto'] = ['Email es requerido'];
            this.validaciones = false;
        }
        if (this.modelpros.numeroContacto == '' || this.modelpros.numeroContacto == null) {
            this.lerr['NumeroContacto'] = ['Tel. Contacto es requerido'];
            this.validaciones = false;
        }
        return this.validaciones;
    }

    savePlan(event) {
        this.getPlan();
        this.actualizarDatos();
    }

    getAllDirs() {
        this.http.get<ItemN[]>(`${this.url}api/direccion/getcatalogo/${this.model.idProspecto}`, { headers: this.getHeaders() }).subscribe(response => {
            this.dirs = response;
        }, err => {
            this.validaError(err);
        });
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getsucursalbycot/${this.model.idCotizacion}`, { headers: this.getHeaders() }).subscribe(response => {
            this.cotdirs = response;
        }, err => {
            this.validaError(err);
        });
    }

    getDirs() {
        //this.lsdir.direcciones = [];
        this.isLoading = true;
        this.http.get<ListaDireccion>(`${this.url}api/cotizacion/limpiezadirectorio/${this.model.idCotizacion}/${this.lsdir.pagina}`, { headers: this.getHeaders() }).subscribe(response => {
            this.lsdir = response;
            this.isLoading = false;
        }, err => {
            this.isLoading = false;
            this.validaError(err);
        });
    }

    saveDir($event) {
        if ($event != 0) {
            this.modelDir.idDireccion = $event;
            this.addDir(0);
        }
        this.getAllDirs();
        this.getDirs();
    }

    addDir(edit: number) {
        this.lerr = {};
        this.modelDir.idCotizacion = this.model.idCotizacion;
        this.modelDir.idDireccionCotizacion = 0;
        if (this.modelDir.idDireccion != 0) {
            this.iniciarCarga();
            setTimeout(() => {
                this.http.post<DireccionCotizacion>(`${this.url}api/cotizacion/agregardireccion`, this.modelDir, { headers: this.getHeaders() }).subscribe(response => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Direcci\u00F3n agregada');
                    }, 300);
                    this.getDirs();
                }, err => {
                    this.detenerCarga();
                    this.validaError(err);
                    if (err.error) {
                        if (edit == 1) {
                            setTimeout(() => {
                                this.errorToast(err.error.message);
                            }, 300);
                        }
                    }
                    console.log(err)
                });
            }, 300);
        }
        else {
            if (this.modelDir.idDireccion == 0) {
                this.lerr['SLidDireccion'] = ['Seleccione una direcci\u00F3n'];

            }
        }
    }

    getPlan() {
        this.isLoading = true;
        this.http.get<ListaPuesto>(`${this.url}api/cotizacion/${this.model.idCotizacion}/0/0/${this.lspue.pagina}`, { headers: this.getHeaders() }).subscribe(response => {
            this.lspue = response;
            this.isLoading = false;
        }, err => {
            this.validaError(err);
            this.isLoading = false;
            console.log(err);
        });
        this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${this.model.idCotizacion}`, { headers: this.getHeaders() }).subscribe(response => {
            this.model = response;
        }, err => {
            this.validaError(err);
        });
    }

    addPlan(idDireccionCotizacion: number, nombreSucursal: string, idServicio: number) {
        this.selDireccion = idDireccionCotizacion;
        this.pueAdd.openAdd(this.model.idCotizacion, idDireccionCotizacion, nombreSucursal, this.model.diasEvento, idServicio);
    }

    updPlan(idPuesto: number, nombreSucursal: string, idServicio: number) {
        this.pueAdd.openEdit(this.model.idCotizacion, idPuesto, nombreSucursal, this.model.diasEvento, idServicio);
    }

    removePlan(id: number) {
        this.http.delete<boolean>(`${this.url}api/puesto/${id}`, { headers: this.getHeaders() }).subscribe(response => {
            if (response) {
                this.okToast('Puesto eliminado');
                this.getPlan();
            }
        }, err => {
            this.validaError(err);
        });
    }

    filtroPlan(id: number) {
        let list = this.lspue.puestosDireccionesCotizacion.filter(p => p.idDireccionCotizacion == id);
        return list;
    }

    getMat(tb: string) {
        this.isLoading = true;
        this.selTipo = tb;
        let fil: string = (this.txtMatKey != '' ? 'keywords=' + this.txtMatKey : '');
        if (fil.length > 0) fil += '&';
        fil += 'idDir=' + this.selMatDir + '&idPues=' + this.selMatPue;
        this.http.get<ListaMaterial>(`${this.url}api/${tb}/${this.model.idCotizacion}/${this.lsmat.pagina == undefined ? 1 : this.lsmat.pagina}?${fil}`, { headers: this.getHeaders() }).subscribe(response => {
            this.lsmat = response;
            this.isLoading = false;

        }, err => {
            this.isLoading = false;
            this.validaError(err);
        });
        this.isLoading = false;
    }

    getNewDir() {
        this.dirAdd.open(this.model.idProspecto, 0);
    }

    getExistDir(idDireccionCotizacion: number) {
        this.dirAdd.open(this.model.idProspecto, idDireccionCotizacion);
    }

    getMatPues(id: number, dir: number, tp: string, nombreSucursal?: string, puesto?: string) {
        if (nombreSucursal != undefined) {
            this.nombreSucursal = nombreSucursal;
        }
        if (puesto != undefined) {
            this.puesto = puesto;
        }
        this.edit = 0;
        this.selPuesto = id;
        this.selDireccion = dir;
        this.selTipo = tp;
        this.proPue.open(this.model.idCotizacion, dir, id, tp, this.edit, nombreSucursal, puesto, this.model.idEstatus, this.model.idServicio);
    }

    newMate(event) {
        this.cloMate();
        this.sDir = false;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, event, this.model.idServicio, this.selTipo, false, this.edit, this.nombreSucursal, this.puesto, this.model.idServicio, this.model.diasEvento);
    }

    saveMate(event) {
        this.getMat(this.selTipo);
    }

    getNewMat(tp: string) {
        this.selPuesto = 0;
        this.selDireccion = 0;
        this.sDir = true;
        this.selTipo = tp;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, 0, this.model.idServicio, tp, true, this.edit, "", "", this.model.idServicio, this.model.diasEvento);
    }

    selNewMat(id: number, tp: string, edit: number) {
        this.edit = 1;
        this.sDir = true;
        this.selTipo = tp;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, id, this.model.idServicio, tp, true, this.edit, "", "", this.model.idServicio, this.model.diasEvento);
    }

    removeMat(id: number) {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.delete<boolean>(`${this.url}api/${this.selTipo}/${id}`).subscribe(response => {
                if (response) {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Producto eliminado');
                    }, 300);
                    this.getMat(this.selTipo);
                }
            }, err => {
                this.validaError(err);
                this.detenerCarga();
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);
            });
        }, 300);
    }

    cloMate() {
        this.proPue.close();
    }

    matPagina(event) {
        this.lsmat.pagina = event;
        this.getMat(this.selTipo);
    }

    dirPagina(event) {
        this.lsdir.pagina = event;
        this.getDirs();
        if (this.tbdireciones) {
            const container = this.tbdireciones.nativeElement;
            container.scrollTop = 0;
        }
    }
    puePagina(event) {
        this.lspue.pagina = event;
        this.getPlan();
        if (this.tbpuestos) {
            const container = this.tbpuestos.nativeElement;
            container.scrollTop = 0;
        }
    }

    toggleAllTabs() {
        this.allTabsOpen = !this.allTabsOpen;
        this.quitarFocoDeElementos2();
    }

    actualizarIndirectoUtilidad() {
        this.modelcot.idCotizacion = this.model.idCotizacion;
        this.modelcot.indirecto = this.model.indirectoPor.toString();
        this.modelcot.utilidad = this.model.utilidadPor.toString();
        this.modelcot.comisionSV = this.model.csvPor.toString();
        this.modelcot.comisionExt = this.model.comisionExtPor.toString();
        this.modelcot.comisionExt = this.model.comisionExtPor.toString();
        this.modelcot.polizaPor = this.model.polizaPor.toString();
        this.quitarFocoDeElementos2();
        if (this.model.idCotizacion != 0) {
            this.iniciarCarga();
            setTimeout(() => {
                this.http.post<boolean>(`${this.url}api/cotizacion/ActualizarIndirectoUtilidadService`, this.modelcot, { headers: this.getHeaders() }).subscribe(response => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Porcentajes actualizados');
                    }, 300);
                    this.existe(this.model.idCotizacion)
                    this.getDirs();
                }, err => {
                    this.detenerCarga();
                    this.validaError(err);
                    setTimeout(() => {
                        this.errorToast('Ocurri\u00F3 un error');
                    }, 300);
                });
            }, 300);
        }
    }

    servicio(id: number) {
        this.serAdd.open(id, this.model.idCotizacion, this.model.idServicio);
    }

    return($event) {
        if ($event == true) {

            this.getMatPues(this.selPuesto, this.selDireccion, this.selTipo);
        }
        else {
            this.getMat(this.selTipo);
        }
    }

    formatoDocumento(documento: string, idReporte: number) {
        Swal.fire({
            title: documento,
            text: 'Seleccione un formato de descarga',
            icon: 'info',
            showCancelButton: true,
            showDenyButton: true,
            showConfirmButton: true,
            denyButtonText: '<i class="fa-solid fa-file-pdf fa-xl me-2"></i>PDF',
            confirmButtonText: '<i class="fa-solid fa-file-word fa-xl me-2"></i>WORD',
            cancelButtonText: '<i class="fa-solid fa-file-powerpoint fa-xl me-2"></i>PPTX',
            customClass: {
                confirmButton: 'btn-pdf',
                denyButton: 'btn-word',
                cancelButton: 'btn-pptx',
                popup: 'custom-swal-width'
            }
        }).then((result) => {
            if (result.isDenied) {
                this.descargarReporte(documento, idReporte, 'pdf');
            } else if (result.isConfirmed) {
                this.descargarReporte(documento, idReporte, 'word');
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                this.descargarReporte(documento, idReporte, 'powerpoint');
            }
        });
    }

    descargarReporte(nombreArchivo: string, idReporte: number, formato: string) {
        this.existe(this.model.idCotizacion)
        this.iniciarCarga();
        setTimeout(() => {
            this.http.get(`${this.url}api/report/DescargarReporteCotizacion/${this.model.idCotizacion}/${idReporte}/${formato}`, { headers: this.getHeaders(), responseType: 'arraybuffer' })
                .subscribe(
                    (data: ArrayBuffer) => {
                        var formatoArchivo = '';
                        switch (formato) {
                            case "pdf":
                                formatoArchivo = 'application/pdf';
                                break;
                            case "word":
                                formatoArchivo = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';
                                break;
                            case "powerpoint":
                                formatoArchivo = 'application/vnd.openxmlformats-officedocument.presentationml.presentation';
                                break;
                        }
                        const file = new Blob([data], { type: formatoArchivo });
                        const fileURL = URL.createObjectURL(file);
                        const a = document.createElement('a');
                        a.href = fileURL;
                        a.download = nombreArchivo + "-" + this.model.nombreComercial;
                        document.body.appendChild(a);
                        a.click();
                        URL.revokeObjectURL(fileURL);
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast('Descargado correctamente');
                        }, 300);
                    },
                    error => {
                        this.detenerCarga();
                        this.validaError(error);
                        setTimeout(() => {
                            this.errorToast('Ocurri\u00F3 un error al descargar');
                        }, 300);
                        console.error('Error al obtener el archivo', error);
                    }
                );
        }, 300);
    }

    arrayBufferToDataUrl(buffer: ArrayBuffer): string {
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const dataUrl = URL.createObjectURL(blob);
        return dataUrl;
    }

    getServ() {
        this.http.get<ListaServicio>(`${this.url}api/material/ObtenerListaServiciosCotizacion/${this.model.idCotizacion}/${this.selMatDir}`, { headers: this.getHeaders() }).subscribe(response => {
            this.lsser = response;
            this.isGen = 0;
            this.isSuc = 0;
            for (let ser of this.lsser.serviciosCotizacion) {
                if (ser.idDireccionCotizacion != 0) {
                    this.isSuc = 1;
                }
                else {
                    this.isGen = 1;
                }
            }
        }, err => {
            this.validaError(err);
        });
    }

    eliSer(id: number) {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.delete(`${this.url}api/material/EliminarServicioCotizacion/${id}`, { headers: this.getHeaders() }).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast('Servicio eliminado');
                }, 300);
                this.getServ();

            }, err => {
                this.detenerCarga();
                this.validaError(err);
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);
            });
        }, 300);
    }

    goBack() {
        window.history.back();
    }

    quitarFocoDeElementos(event: MouseEvent): void {
        const target = event.target as HTMLElement;
        if (!target.matches('button, input[type="text"]')) {
            const elementos = document.querySelectorAll('button, input[type="text"]');
            elementos.forEach((elemento: HTMLElement) => {
                elemento.blur();
            });
        }
    }

    openPueLay() {
        this.puelay.open();
    }

    quitarFocoDeElementos2(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');

        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }

    marcarVenta() {
        this.marven.open(this.model.idCotizacion);
    }

    returnMarcaCotizacion($event) {

    }
    actualizarDatos() {
        this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${this.model.idCotizacion}`, { headers: this.getHeaders() }).subscribe(response => {
            this.model = response;
        }, err => console.log(err));
    }

    modalContrato() {
        this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${this.model.idCotizacion}`, { headers: this.getHeaders() }).subscribe(response => {
            this.model = response;
            if (response.idEstatus == 6) {
                this.model.idEstatus == response.idEstatus;
                this.contrato.open(this.model.idCotizacion, this.model.idProspecto);
            }
            else {
                this.errorToast("Esta cotizaci\u00F3n a\u00FAn no est\u00E1 validada por direcci\u00F3n")
            }
        }, err => {
            this.validaError(err);
        });
    }

    descargarDatosCotizacion() {
        this.isLoading = true;
        this.http.post(`${this.url}api/cargamasiva/DescargarDatosCotizacion/${this.model.idCotizacion}`, null, { headers: this.getHeaders(), responseType: 'blob' }).subscribe((response: Blob) => {
            this.okToast('Datos descargados');
            saveAs(response, 'DatosCotizacion_Id' + this.model.idCotizacion + '.xlsx');
            this.isLoading = false;
        },
            error => {
                console.error('Error al descargar el archivo:', error);
                this.isLoading = false;
                this.validaError(error);
            }
        );
    }

    cargarContratoCliente($event) {
        this.subirCont.open(this.model.idCotizacion, this.model.idProspecto);
    }
    subirContratoEvent($event) {

    }

    openFormularioCliente($event) {
        this.clienteWidget.open(this.model.idCotizacion, this.model.idProspecto, this.model.idServicio);
    }

    //Abrir modal cofirmacion
    confirmaDuplicarCotizacion(idCotizacion: number) {
        this.idpro = idCotizacion;
        const tipo = 'duplicarCotizacion';
        const titulo = 'Confirmaci\u00F3n';
        const mensaje = 'Se crear\u00E1 un duplicado con las especificaciones se\u00F1aladas previamente';
        Swal.fire({
            title: "Productos de cotizaci\u00F3n",
            text: "\u00BFDesea que la nueva cotizaci\u00F3n incluya todos los productos?",
            icon: 'info',
            showCancelButton: true,
            confirmButtonText: 'S\u00ED',
            cancelButtonText: 'No',
            customClass: {
                popup: 'custom-swal-width'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                this.incluyeProducto = true;
                this.confirma.open(tipo, titulo, mensaje)
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                this.incluyeProducto = false;
                this.confirma.open(tipo, titulo, mensaje)
            }
        });
    }

    confirmaEliminarDireccionCotizacion(idDireccionCotizacion: number) {
        this.idDC = idDireccionCotizacion;
        const tipo = 'eliminarDireccionCotizacion';
        const titulo = 'Eliminar directorio';
        const mensaje = 'Se eliminar\u00E1n los items relacionados';
        this.confirma.open(tipo, titulo, mensaje);
    }

    confirmaEliminarPuestoDireccionCotizacion(idPuestoDireccionCotizacion: number) {
        this.idope = idPuestoDireccionCotizacion;
        const tipo = 'eliminarPuestoDireccionCotizacion'
        const titulo = 'Eliminar puesto';
        const mensaje = 'Se eliminar\u00E1n los registros relacionados';
        this.confirma.open(tipo, titulo, mensaje);
    }
    confirmaAutorizarCotizacion() {
        const tipo = 'autorizarCotizacion'
        const titulo = 'Autorizar cotizaci\u00F3n';
        const mensaje = 'Al autorizar la cotizaci\u00F3n el vendedor podr\u00E1 convertir el prospecto a cliente';
        this.confirma.open(tipo, titulo, mensaje);
    }
    confirmaRemoverAutorizacionCotizacion() {
        const tipo = 'removerAutorizacionCotizacion'
        const titulo = 'Remover autorizaci\u00F3n';
        const mensaje = 'El estatus de la cotizaci\u00F3n pasara a ser: "Activa"';
        this.confirma.open(tipo, titulo, mensaje);
    }


    //Respuesta de confirmacion
    confirmacionEvent($event) {
        if ($event == 'duplicarCotizacion') {
            this.duplicarCotizacion();
        }
        if ($event == 'eliminarDireccionCotizacion') {
            this.eliminaDireccionCotizacion();
        }
        if ($event == 'eliminarPuestoDireccionCotizacion') {
            this.eliminarPuestoDireccionCotizacion();
        }
        if ($event == 'autorizarCotizacion') {
            this.autorizarCotizacion();
        }
        if ($event == 'removerAutorizacionCotizacion') {
            this.removerAutorizacionCotizacion();
        }
    }


    //acciones de confirmacion
    duplicarCotizacion() {
        this.model.idCotizacion = this.model.idCotizacion;
        if (this.model.idCotizacion != 0) {
            this.iniciarCarga();
            setTimeout(() => {
                this.http.get<number>(`${this.url}api/cotizacion/DuplicarCotizacion/${this.incluyeProducto}/${this.model.idCotizacion}`, { headers: this.getHeaders() }).subscribe(response => {
                    this.idCotN = response;
                    if (this.resumenContainer) {
                        const container = this.resumenContainer.nativeElement;
                        container.scrollTop = 0;
                    }
                    this.rtr.navigate(['/exclusivo/resumen/', this.idCotN]);
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Ahora se encuentra en la nueva cotizaci\u00F3n');
                    }, 300);

                }, err => {
                    this.detenerCarga();
                    this.validaError(err);
                    setTimeout(() => {
                        this.errorToast('Ocurri\u00F3 un error');
                    }, 300);
                    console.log(err);
                });
            }, 300);
        }
    }

    eliminarPuestoDireccionCotizacion() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.delete<boolean>(`${this.url}api/puesto/${this.idope}`, { headers: this.getHeaders() }).subscribe(response => {
                if (response) {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Puesto eliminado');
                    }, 300);
                    this.getPlan();
                }
            }, err => {
                this.detenerCarga();
                this.validaError(err);
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);
            });
        }, 300);
    }

    eliminaDireccionCotizacion() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.get<boolean>(`${this.url}api/cotizacion/EliminarDireccionCotizacion/${this.idDC}`, { headers: this.getHeaders() }).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast('Direcci\u00F3n eliminada');
                }, 300);
                this.getDirs();
            }, err => {
                this.detenerCarga();
                this.validaError(err);
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);
            });
        }, 300);

    }

    autorizarCotizacion() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.post<boolean>(`${this.url}api/cotizacion/AutorizarCotizacion`, this.model.idCotizacion, { headers: this.getHeaders() }).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast('Cotizaci\u00F3n ' + this.model.idCotizacion.toString() + ' autorizada');
                    this.actualizarDatos();
                }, 300);
                this.getDirs();
            }, err => {
                this.detenerCarga();
                this.validaError(err);
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);
            });
        }, 300);
    }

    removerAutorizacionCotizacion() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.post<boolean>(`${this.url}api/cotizacion/RemoverAutorizacionCotizacion`, this.model.idCotizacion, { headers: this.getHeaders() }).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast('Cotizaci\u00F3n ' + this.model.idCotizacion.toString() + ' actualizada');
                    this.actualizarDatos();
                }, 300);
                this.getDirs();
            }, err => {
                this.detenerCarga();
                this.validaError(err);
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);
            });
        }, 300);
    }

    //errors
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


    //imports
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
    openCargaContratoWidget($event) {
        if ($event) {
            this.actualizarDatos();
        }
    }

    openProductoMasivo(tipo: number) {
        this.productoG.open(tipo, this.model.idServicio, this.model.idCotizacion);
    }

    returnProductoGeneral($event) {
        this.getPlan();
        this.actualizarDatos();
    }
}   
