import { Component, Inject, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { ListaCotizacion } from 'src/app/models/listacotizacion';
import { Prospecto } from 'src/app/models/prospecto';
import { ItemN } from 'src/app/models/item';
import { CotizaResumenLim } from 'src/app/models/cotizaresumenlim';
import { ListaDireccion } from '../../models/listadireccion';
import { EditarCotizacion } from 'src/app/widgets/editacotizacion/editacotizacion.widget';
import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { ConfirmacionWidget } from 'src/app/widgets/confirmacion/confirmacion.widget'
import { CerrarCotizacion } from '../../widgets/cerrarcotizacion/cerrarcotizacion.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';

@Component({
    selector: 'cotizacion',
    templateUrl: './cotizacion.component.html',
    animations: [fadeInOut],
})
export class CotizacionComponent implements OnInit, OnDestroy {
    @ViewChild(EditarCotizacion, { static: false }) ediw: EditarCotizacion;
    @ViewChild(ConfirmacionWidget, { static: false }) conw: ConfirmacionWidget;
    @ViewChild(CerrarCotizacion, { static: false }) cerrarCot: CerrarCotizacion;
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @ViewChild('tbcotizaciones', { static: false }) tablaContainer: ElementRef;
    model: CotizaResumenLim = {
        idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, provisiones: 0, prestaciones: 0, material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0, subTotal: 0, indirecto: 0, utilidad: 0,
        total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', indirectoPor: '', utilidadPor: '', csvPor: '', comisionSV: 0, comisionExtPor: '', comisionExt: 0, totalPolizaCumplimiento: 0, polizaCumplimiento: false, idEstatus: 0, diasEvento: 0
    };
    lcots: ListaCotizacion = {
        idProspecto: 0, idServicio: 0, pagina: 1, numPaginas: 0,
        rows: 0, cotizaciones: [], idEstatusCotizacion: 1, idAlta: '', total: 0
    };
    lsers: ItemN[] = [];
    lests: ItemN[] = [];
    lpros: Prospecto[] = [];
    lsdir: ListaDireccion = {} as ListaDireccion;
    idpro: number = 0;
    idEstatus: number = 0;
    estatus: number = 1;
    idCotizacion: number = 0;
    isLoading: boolean = false;
    lerr: any = {};
    validaDato1: any;
    validaDato2: any;
    sub: any;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private route: ActivatedRoute, public user: StoreUser) {
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.lsers = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/cotizacion/getestatus`).subscribe(response => {
            this.lests = response;
        }, err => console.log(err));
    }

    ngOnInit(): void {
        this.sub = this.route.params.subscribe(params => {
            let idp: number = +params['idp'];
            if (idp > 0) {
                this.lcots.idProspecto = idp;
            } else {
                this.nuevo();
            }
            this.init();
        });
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }

    init() {
        this.lcots.cotizaciones = [];
        let fil: string = (this.lcots.idEstatusCotizacion > 0 ? `estatus=1` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idServicio > 0 ? `servicio=${this.lcots.idServicio}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idProspecto > 0 ? `idProspecto=${this.lcots.idProspecto}` : '');
        if (fil.length > 0) fil = '?' + fil;
        this.http.get<ListaCotizacion>(`${this.url}api/cotizacion/${this.user.idPersonal}/${this.lcots.pagina}${fil}`).subscribe(response => {
            this.lcots = response;
            }, err => {
            console.log(err);
        });
        this.http.post<Prospecto[]>(`${this.url}api/prospecto/getcatalogo`, this.user.idPersonal).subscribe(response => {
            this.lpros = response;
        }, err => console.log(err));
    }

    lista() {
        let fil: string = (this.lcots.idEstatusCotizacion > 0 ? `estatus=${this.lcots.idEstatusCotizacion}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idServicio > 0 ? `servicio=${this.lcots.idServicio}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idProspecto > 0 ? `idProspecto=${this.lcots.idProspecto}` : '');
        if (fil.length > 0) fil = '?' + fil;
        this.http.get<ListaCotizacion>(`${this.url}api/cotizacion/${this.user.idPersonal}/${this.lcots.pagina}${fil}`).subscribe(response => {
            setTimeout(() => {
                this.lcots = response;
                if (this.tablaContainer) {
                    const container = this.tablaContainer.nativeElement;
                    container.scrollTop = 0;
                }
                this.detenerCarga();
            }, 300);
        }, err => {
            setTimeout(() => {
                this.detenerCarga();
                console.log(err);
            }, 300);
        });
    }

    nuevo() {
        this.lcots = {
            idProspecto: 0, idServicio: 0, pagina: 1, numPaginas: 0,
            rows: 0, cotizaciones: [], idEstatusCotizacion: 1, idAlta: '', total: 0
        };
    }

    busca() {
        this.lcots.pagina = 1;
        this.lista();
    }

    muevePagina(event) {
        this.iniciarCarga();
        this.lcots.pagina = event;
        this.lista();
    }

    getDirs() {
        this.http.get<ListaDireccion>(`${this.url}api/cotizacion/limpiezadirectorio/${this.model.idCotizacion}`).subscribe(response => {
            this.lsdir = response;
        }, err => console.log(err));
    }

    //Abrir modal editar cotizacion
    editar(idCotizacion: number, servicio: string, polizaCumplimiento: boolean) {
        this.ediw.openSel(idCotizacion, servicio, polizaCumplimiento);
    }

    openCerrarCotizacion(idCotizacion: number) {
        this.cerrarCot.open(idCotizacion);
        //Vigente, vencida
    }
    cerrarCotizacionEvent($event) {
        this.lista();
    }

    //Abrir modal cofirmacion
    openValida(tipo: string, mensaje: string, dato1?: any, dato2?: any) {
        this.idCotizacion = dato1;
        this.estatus = dato2;
        const titulo = "Confirmaci\u00F3n";
        this.conw.mensaje = mensaje;
        this.conw.open(tipo, titulo, mensaje)
    }

    //Respuesta de confirmacion
    confirmacionEvent($event) {
        if ($event == 'cambiarEstatusCotizacion') {
            this.cambiarEstatusCotizacion();
        }
        if ($event == 'eliminarCotizacion') {
            this.eliminarCotizacion();
        }
    }

    cambiarEstatusCotizacion() {
        this.iniciarCarga();
        if (this.idEstatus === 1) {
            this.http.put<boolean>(`${this.url}api/cotizacion/desactivarcotizacion`, this.idCotizacion).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast('Cotizaci\u00F3n ' + this.idCotizacion + ' desactivada');
                }, 300);
                this.lista();
            }, err => {
                this.detenerCarga();
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);

            });
        } else {
            this.http.put<boolean>(`${this.url}api/cotizacion/activarcotizacion`, this.idCotizacion).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast('Cotizaci\u00F3n ' + this.idCotizacion + ' activada');
                }, 300);
                this.lista();

            }, err => {
                this.detenerCarga();
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);

            });
        }
    }

    eliminarCotizacion() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.post<boolean>(`${this.url}api/cotizacion/EliminarCotizacion`, this.idCotizacion).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast('Cotizaci\u00F3n ' + this.idCotizacion + ' eliminada');
                }, 300);
                this.init();
            }, err => {
                this.detenerCarga();
                setTimeout(() => {
                    this.errorToast('Ocurri\u00F3 un error');
                }, 300);
                console.log(err);

            });
        }, 300);
    }

    editReturn($event) {
        this.nuevo();
        this.lista();
        this.lista();

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

    goBack() {
        window.history.back();
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
