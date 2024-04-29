import { Component, Inject, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';
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
import { EliminaWidget } from 'src/app/widgets/elimina/elimina.widget';
import { EliminaOperarioWidget } from 'src/app/widgets/eliminaOperario/eliminaOperario.widget';
import { MaterialOperarioAddWidget } from 'src/app/widgets/materialoperarioadd/materialoperarioadd.widget';
import { EliminaDirectorioWidget } from 'src/app/widgets/eliminadirectorio/eliminadirectorio.widget';
import { ActualizaCotizacionWidget } from 'src/app/widgets/actualizacotizacion/actualizacotizacion.widget';
import { ServicioAddWidget } from 'src/app/widgets/servicioadd/servicioadd.widget';
import { Cotizacionupd } from 'src/app/models/cotizacionupd';
import { Router } from '@angular/router';
import { ReportService } from 'src/app/report.service';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import internal = require('assert');
import { saveAs } from 'file-saver';
import { PuestoLayoutWidget } from 'src/app/widgets/puestolayout/puestolayout.widget';
import Swal from 'sweetalert2';
import { MarcaVenta } from 'src/app/widgets/marcaventa/marcaventa.widget';
import { ContratoWidget } from '../../../widgets/contrato/contrato.widget';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';



@Component({
    selector: 'resumen',
    templateUrl: './resumen.component.html',
    animations: [fadeInOut],
})
export class ResumenComponent implements OnInit, OnDestroy {
    @ViewChild(MaterialAddWidget, { static: false }) proAdd: MaterialAddWidget;
    @ViewChild(MaterialWidget, { static: false }) proPue: MaterialWidget;
    @ViewChild(MaterialOperarioAddWidget, { static: false }) opeMat: MaterialOperarioAddWidget;
    @ViewChild(DireccionWidget, { static: false }) dirAdd: DireccionWidget;
    @ViewChild(PuestoWidget, { static: false }) pueAdd: PuestoWidget;
    @ViewChild(EliminaWidget, { static: false }) eliw: EliminaWidget;
    @ViewChild(ActualizaCotizacionWidget, { static: false }) actCot: ActualizaCotizacionWidget;
    @ViewChild(EliminaOperarioWidget, { static: false }) eliope: EliminaOperarioWidget;
    @ViewChild(EliminaDirectorioWidget, { static: false }) elidir: EliminaDirectorioWidget;
    @ViewChild(ServicioAddWidget, { static: false }) serAdd: ServicioAddWidget;
    @ViewChild(PuestoLayoutWidget, { static: false }) puelay: PuestoLayoutWidget;
    @ViewChild(MarcaVenta, { static: false }) marven: MarcaVenta;
    @ViewChild(ContratoWidget, { static: false }) contrato: ContratoWidget;
    @ViewChild('resumen', { static: false }) resumen: ElementRef;
    @ViewChild('pdfCanvas', { static: true }) pdfCanvas: ElementRef;
    @ViewChild('indirectotxt', { static: false }) indirectotxt: ElementRef;
    @ViewChild('utilidadtxt', { static: false }) utilidadtxt: ElementRef;
    @ViewChild('CSVtxt', { static: false }) CSVtxt: ElementRef;
    @ViewChild('comisionExttxt', { static: false }) comisionExttxt: ElementRef;
    @ViewChild('fileInputDir', { static: false }) fileInputDir: ElementRef<HTMLInputElement>;
    @ViewChild('fileInputPlan', { static: false }) fileInputPlan: ElementRef<HTMLInputElement>;
    sub: any;
    model: CotizaResumenLim = {
        idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, prestaciones: 0, provisiones: 0,
        material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0,
        subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', utilidadPor: '', indirectoPor: '', csvPor: '', comisionSV: 0, comisionExt: 0, comisionExtPor: '', polizaCumplimiento: false, totalPolizaCumplimiento: 0
    };
    dirs: ItemN[] = [];
    cotdirs: Catalogo[] = [];
    lsdir: ListaDireccion = {} as ListaDireccion;
    lspue: ListaPuesto = {} as ListaPuesto;
    lsmat: ListaMaterial = {} as ListaMaterial;
    lsher: ListaMaterial = {} as ListaMaterial;
    lsser: ListaServicio = {} as ListaServicio;
    selDireccion: number = 0;
    selPuesto: number = 0;
    selMatDir: number = 0;
    selMatPue: number = 0;
    edit: number = 0;
    selTipo: string = 'material';
    txtMatKey: string = '';
    sDir: boolean = false;
    modelcot: Cotizacionupd = {
        idCotizacion: 0, indirecto: '', utilidad: '', comisionSV: '', comisionExt: ''
    };
    indirectoValue: string = this.model.utilidadPor;
    utilidadValue: string = this.model.indirectoPor;
    CSV: string = this.model.csvPor;
    ComisionExtValue: string = this.model.comisionExtPor;
    modelDir: DireccionCotizacion = {
        idCotizacion: 0, idDireccionCotizacion: 0, idDireccion: 0, nombreSucursal: ''
    };
    idpro: number = 0;
    idope: number = 0;
    idDC: number = 0;
    urlF: string = '';
    reportData: Blob;
    pdfUrl: string;
    autorizacion: number = 0;
    validaMess: string = '';
    idCotN: number = 0;
    isLoadinglay: boolean = false;
    isGen: number = 0;
    isSuc: number = 0;
    nombreSucursal: string = '';
    puesto: string = '';

    lerr: any = {};

    isLoading: boolean = false;

    allTabsOpen = true;
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;


    constructor(
        @Inject('BASE_URL') private url: string,
        private http: HttpClient,
        private route: ActivatedRoute,
        private router: Router,
        private reportService: ReportService,
        public user: StoreUser
    ) {
        this.nuevo();
        this.lsdir = {
            pagina: 1, idCotizacion: this.model.idProspecto, idProspecto: this.model.idProspecto,
            idDireccion: 0, direcciones: [], rows: 0, numPaginas: 0
        };
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;
        }, err => console.log(err));

    }

    toggleAllTabs() {
        this.allTabsOpen = !this.allTabsOpen;
        this.quitarFocoDeElementos2();
    }

    nuevo() {
        this.model = {
            idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, prestaciones: 0, provisiones: 0,
            material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0,
            subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', utilidadPor: '', indirectoPor: '', csvPor: '', comisionSV: 0, comisionExt: 0, comisionExtPor: '', polizaCumplimiento: false, totalPolizaCumplimiento: 0
        };
    }

    existe(id: number) {
        //this.obtenerUsuarioCotizacion(id);
        this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${id}`).subscribe(response => {
            this.model = response;
            this.getAllDirs();
            this.getDirs();
            this.getPlan();

        }, err => {
            console.log(err)
        });
    }

    onFileChangeDir(event: any): void {
        this.isLoadinglay = true;
        const file = event.target.files[0];
        const formData = new FormData();
        formData.append('file', file);
        this.http.post<boolean>(`${this.url}api/cargamasiva/CargarDirecciones/${this.model.idCotizacion}/${this.model.idProspecto}`, formData).subscribe((response) => {
            this.okToast('Direcciones cargadas');
            this.getAllDirs();
            this.getDirs();
            this.isLoadinglay = false;
        }, err => {
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
        this.http.post<boolean>(`${this.url}api/cargamasiva/CargarPlantilla/${this.model.idCotizacion}`, formData).subscribe((response) => {
            setTimeout(() => {
                this.getPlan();
                this.isLoadinglay = false;
                Swal.fire({
                    icon: 'success',
                    timer: 500,
                    showConfirmButton: false,
                });
            }, 300);
        }, err => {
            setTimeout(() => {
                this.isLoadinglay = false;
                Swal.fire({
                    title: 'Error',
                    text: 'Ocurri\u00F3 un error al cargar el layout, verifique la informaci\u00F3n',
                    icon: 'error',
                    timer: 2000,
                    showConfirmButton: false,
                });
            }, 300);
        });
        this.fileInputPlan.nativeElement.value = '';
    }

    descargarLayoutDirectorio() {
        this.http.post(`${this.url}api/cargamasiva/DescargarLayoutDirectorio`, null, { responseType: 'blob' }).subscribe((response: Blob) => {
            saveAs(response, 'LayoutDirectorio.xlsx');
            this.okToast('Layout descargado');
        },
            error => {
                this.errorToast('Ocurri\u00F3 un error')
            }
        );
    }
    descargarLayoutPlantilla() {
        this.http.post(`${this.url}api/cargamasiva/DescargarLayoutPlantilla/${this.model.idCotizacion}`, null, { responseType: 'blob' }).subscribe((response: Blob) => {
            saveAs(response, 'LayoutPlantilla.xlsx');
            this.okToast('Layout descargado');
        },
            error => {
                this.errorToast('Ocurri\u00F3 un error')
            }
        );
    }

    savePros(event) {
        this.model.idProspecto = event;
        this.getAllDirs();
    }

    savePlan(event) {
        this.getPlan();
    }

    getAllDirs() {
        this.http.get<ItemN[]>(`${this.url}api/direccion/getcatalogo/${this.model.idProspecto}`).subscribe(response => {
            this.dirs = response;
        }, err => console.log(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getsucursalbycot/${this.model.idCotizacion}`).subscribe(response => {
            this.cotdirs = response;
        }, err => console.log(err));
    }

    getDirs() {
        this.lsdir.direcciones = [];
        this.http.get<ListaDireccion>(`${this.url}api/cotizacion/limpiezadirectorio/${this.model.idCotizacion}/${this.lsdir.pagina}`).subscribe(response => {
            this.lsdir = response;
        }, err => console.log(err));
    }

    saveDir($event) {
        this.modelDir.idDireccion = $event;
        this.addDir(0);
        this.getAllDirs();
    }
    
    addDir(edit: number) {
        this.lerr = {};
        this.modelDir.idCotizacion = this.model.idCotizacion;
        this.modelDir.idDireccionCotizacion = 0;
        if (this.modelDir.idDireccion != 0) {
            this.http.post<DireccionCotizacion>(`${this.url}api/cotizacion/agregardireccion`, this.modelDir).subscribe(response => {
                this.okToast('Direcci\u00F3n agregada');
                this.getDirs();
            }, err => {
                if (err.error) {
                    if (edit == 1) {
                        this.errorToast(err.error.message);
                    }
                    
                }
                console.log(err)
            });
        }
        else {
            if (this.modelDir.idDireccion == 0) {
                this.lerr['SLidDireccion'] = ['Seleccione una direcci\u00F3n'];
                
            }
        }
    }

    getPlan() {
        this.http.get<ListaPuesto>(`${this.url}api/cotizacion/${this.model.idCotizacion}/0/0`).subscribe(response => {
            this.lspue = response;
        }, err => console.log(err));
        this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${this.model.idCotizacion}`).subscribe(response => {
            this.model = response;
        }, err => console.log(err));
    }

    addPlan(id: number, tb: number, nombreSucursal: string) {
        this.selDireccion = id;
        this.pueAdd.open(this.model.idCotizacion, id, tb, 0, nombreSucursal);
    }

    updPlan(id: number, tb: number, nombreSucursal: string, puesto: string) {
        //this.selPuesto = id;
        this.pueAdd.open(this.model.idCotizacion, this.selDireccion, tb, id, nombreSucursal, puesto);
    }

    removePlan(id: number) {
        this.http.delete<boolean>(`${this.url}api/puesto/${id}`).subscribe(response => {
            if (response) {
                this.okToast('Puesto eliminado');
                this.getPlan();
            }
        }, err => {
            console.log(err);
            this.errorToast('Ocurri\u00F3 un error');
        });
    }

    filtroPlan(id: number) {
        let list = this.lspue.puestosDireccionesCotizacion.filter(p => p.idDireccionCotizacion == id);
        return list;
    }

    getMat(tb: string) {
        //this.selPuesto = 0;
        this.isLoading = true;
        this.selTipo = tb;
        let fil: string = (this.txtMatKey != '' ? 'keywords=' + this.txtMatKey : '');
        if (fil.length > 0) fil += '&';
        fil += 'idDir=' + this.selMatDir + '&idPues=' + this.selMatPue;
        this.http.get<ListaMaterial>(`${this.url}api/${tb}/${this.model.idCotizacion}/${this.lsmat.pagina == undefined ? 1 : this.lsmat.pagina}?${fil}`).subscribe(response => {
            this.lsmat = response;
            this.isLoading = false;

        }, err => {
            this.isLoading = false;
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
        this.proPue.open(this.model.idCotizacion, dir, id, tp, this.edit, nombreSucursal, puesto);
    }

    newMate(event) {
        this.cloMate();
        this.sDir = false;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, event, this.model.idServicio, this.selTipo, false, this.edit, this.nombreSucursal, this.puesto);
    }

    saveMate(event) {
        this.getMat(this.selTipo);
    }

    getNewMat(tp: string) {
        this.selPuesto = 0;
        this.selDireccion = 0;
        this.sDir = true;
        this.selTipo = tp;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, 0, this.model.idServicio, tp, true, this.edit);
    }

    selNewMat(id: number, tp: string, edit: number) {
        this.edit = 1;
        this.sDir = true;
        this.selTipo = tp;
        this.proAdd.open(this.model.idCotizacion, this.selDireccion, this.selPuesto, id, this.model.idServicio, tp, true, this.edit);
    }

    removeMat(id: number) {
        this.http.delete<boolean>(`${this.url}api/${this.selTipo}/${id}`).subscribe(response => {
            if (response) {
                this.okToast('Producto eliminado');
                this.getMat(this.selTipo);
            }
        }, err => {
            console.log(err);
            this.errorToast('Ocurri\u00F3 un error')
        });
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

    elige(idCotizacion) {
        this.eliw.titulo = "";
        this.eliw.mensaje = "";
        this.idpro = idCotizacion;
        this.eliw.titulo = 'Duplicar cotizaci\u00F3n';
        this.eliw.mensaje = "Se crear\u00E1 un duplicado";
        this.eliw.open();
    }
    elimina($event) {
        if ($event == true) {
            this.model.idCotizacion = this.model.idCotizacion;
            if (this.model.idCotizacion != 0) {
                this.http.post<number>(`${this.url}api/cotizacion/DuplicarCotizacion`, this.model.idCotizacion).subscribe(response => {
                    this.idCotN = response;

                    this.router.navigate(['/exclusivo/resumen/', this.idCotN]);
                    this.eliope.titulo = 'Duplicado correctamente';
                    this.eliope.mensaje = 'Ahora se encuentra en la nueva cotizaci\u00F3n';
                    this.eliope.open();

                }, err => console.log(err));
            }
        }
        if ($event == false) {
            this.http.delete<boolean>(`${this.url}api/puesto/${this.idope}`).subscribe(response => {
                if (response) {
                    this.okToast('Puesto eliminado');
                    this.getPlan();
                }
            }, err => {
                console.log(err);
                this.errorToast('Ocurri\u00F3 un error')
            });
        }
    }
    eligeOperario(idOperario) {
        this.idope = idOperario;
        this.eliope.titulo = 'Eliminar puesto';
        this.eliope.mensaje = 'Se eliminar\u00E1n los items relacionados';
        this.eliope.open();
    }

    limpiarInputs() {
        this.modelcot.indirecto = "";
        this.modelcot.utilidad = "";
        this.modelcot.comisionSV = "";
        this.indirectoValue = "";
        this.utilidadValue = "";
        this.CSV = "";
    }
    actualizarIndirectoUtilidad() {
        this.isLoading = true;
        this.modelcot.idCotizacion = this.model.idCotizacion;
        this.modelcot.indirecto = this.indirectotxt.nativeElement.value;
        this.modelcot.utilidad = this.utilidadtxt.nativeElement.value;
        this.modelcot.comisionSV = this.CSVtxt.nativeElement.value;
        this.modelcot.comisionExt = this.comisionExttxt.nativeElement.value;
        if (this.model.idCotizacion != 0) {
            this.http.post<boolean>(`${this.url}api/cotizacion/ActualizarIndirectoUtilidadService`, this.modelcot).subscribe(response => {
                setTimeout(() => {
                    this.okToast('Porcentajes actualizados');
                    this.isLoading = false;
                    this.limpiarInputs();
                    this.existe(this.model.idCotizacion)
                    this.getDirs();
                }, 300);
            }, err => {
                setTimeout(() => {
                    this.isLoading = false;
                    console.log(err)
                    this.errorToast('Ocurri\u00F3 un error')
                }, 300);
            });
        }

    }

    validarSoloNumeros(utilidadValue: string): boolean {
        utilidadValue = this.utilidadValue;
        const regex = /^[0-9]+$/;
        return regex.test(utilidadValue);
    }
    validaDireccionCotizacion(idDireccionCotizacion) {
        this.idDC = idDireccionCotizacion;
        this.elidir.titulo = 'Eliminar directorio';
        this.elidir.mensaje = 'Se eliminar\u00E1n los items relacionados';
        this.elidir.open();
    }
    eliminaDireccionCotizacion($event) {
        if ($event == true) {
            this.http.get<boolean>(`${this.url}api/cotizacion/EliminarDireccionCotizacion/${this.idDC}`,).subscribe(response => {
                this.okToast('Direcci\u00F3n eliminada');
                this.getDirs();
            }, err => {
                console.log(err);
                this.errorToast('Ocurri\u00F3 un error')
            });
        }
    }

    servicio(id: number) {
        this.serAdd.open(id, this.model.idCotizacion);
    }

    return($event) {
        if ($event == true) {

            this.getMatPues(this.selPuesto, this.selDireccion, this.selTipo);
        }
        else {
            this.getMat(this.selTipo);
        }
    }
    validaActualizacionCotizacion() {
        this.actCot.open();
    }
    descargarCotizacionComponent(tipo: number) {
        this.isLoading = true;
        this.existe(this.model.idCotizacion)
        this.iniciarAnimacion();
        this.http.post(`${this.url}api/report/DescargarReporteCotizacion/${tipo}`, this.model.idCotizacion, { responseType: 'arraybuffer' })
            .subscribe(
                (data: ArrayBuffer) => {
                    this.okToast('Propuesta descargada');
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
                    this.isLoading = false;
                },
                error => {
                    this.errorToast('Ocurri\u00F3 un error')
                    console.error('Error al obtener el archivo PDF', error);
                    this.isLoading = false;
                }
            );
    }
    arrayBufferToDataUrl(buffer: ArrayBuffer): string {
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const dataUrl = URL.createObjectURL(blob);
        return dataUrl;
    }

    iniciarAnimacion() {
        const boton1 = document.getElementById("miBoton1") as HTMLButtonElement;
        const boton2 = document.getElementById("miBoton2") as HTMLButtonElement;
        boton1.disabled = true;
        boton2.disabled = true;
        setTimeout(function () {
            boton1.disabled = false;
            boton2.disabled = false;
        }, 3000);
    }
    getServ() {
        this.http.get<ListaServicio>(`${this.url}api/material/ObtenerListaServiciosCotizacion/${this.model.idCotizacion}/${this.selMatDir}`).subscribe(response => {
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
        }, err => console.log(err));
    }
    eliSer(id: number) {
        this.http.delete(`${this.url}api/material/EliminarServicioCotizacion/${id}`).subscribe(response => {
            this.okToast('Servicio eliminado');
            this.getServ();

        }, err => {
            console.log(err);
            this.errorToast('Ocurri\u00F3 un error')
        });
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

    modalContrato() {
        this.contrato.open(this.model.idCotizacion, this.model.idProspecto);
    }

    descargarDatosCotizacion() {
        this.isLoading = true;
        this.http.post(`${this.url}api/cargamasiva/DescargarDatosCotizacion/${this.model.idCotizacion}`, null, { responseType: 'blob' }).subscribe((response: Blob) => {
            this.okToast('Datos descargados');
            saveAs(response, 'DatosCotizacion_Id' + this.model.idCotizacion + '.xlsx');
            this.isLoading = false;
        },
            error => {
                console.error('Error al descargar el archivo:', error);
                this.isLoading = false;
                this.errorToast('Ocurri\u00F3 un error');
            }
        );
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
}   
