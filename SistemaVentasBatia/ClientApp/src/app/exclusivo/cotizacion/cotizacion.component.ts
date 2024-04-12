import { Component, Inject, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { ListaCotizacion } from 'src/app/models/listacotizacion';
import { Prospecto } from 'src/app/models/prospecto';
import { ItemN } from 'src/app/models/item';
import { Subject } from 'rxjs';
import { CotizaResumenLim } from 'src/app/models/cotizaresumenlim';
//import { DireccionCotizacion } from '../../models/direccioncotizacion';
import { ListaDireccion } from '../../models/listadireccion';
import { Cotizacion } from '../../models/cotizacion';

import { EliminaWidget } from 'src/app/widgets/elimina/elimina.widget';
import { EditarCotizacion } from 'src/app/widgets/editacotizacion/editacotizacion.widget';

import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';
import Swal from 'sweetalert2';

@Component({
    selector: 'cotizacion',
    templateUrl: './cotizacion.component.html',
    animations: [fadeInOut],

})
export class CotizacionComponent implements OnInit, OnDestroy {
    sub: any;
    lcots: ListaCotizacion = {
        idProspecto: 0, idServicio: 0, pagina: 1, numPaginas: 0,
        rows: 0, cotizaciones: [], idEstatusCotizacion: 1, idAlta: '', total: 0
    };
    lsers: ItemN[] = [];
    lests: ItemN[] = [];
    lpros: Prospecto[] = [];
    model: CotizaResumenLim = {
        idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, provisiones: 0, prestaciones: 0,
        material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0,
        subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', indirectoPor: '', utilidadPor: '', csvPor: '', comisionSV: 0, comisionExtPor: '', comisionExt: 0, totalPolizaCumplimiento: 0, polizaCumplimiento: false
    };
    lsdir: ListaDireccion = {} as ListaDireccion;
    idpro: number = 0;
    @ViewChild(EliminaWidget, { static: false }) eliw: EliminaWidget;
    @ViewChild(EditarCotizacion, { static: false }) ediw: EditarCotizacion;
    estatus: number = 1;
    idCotizacion: number = 0;
    lerr: any = {};
    evenSub: Subject<void> = new Subject<void>();
    isErr: boolean = false;
    errMessage: string = '';
    isLoading: boolean = false;


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private route: ActivatedRoute, public user: StoreUser) {

        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.lsers = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/cotizacion/getestatus`).subscribe(response => {
            this.lests = response;
        }, err => console.log(err));
    }

    nuevo() {
        this.lcots = {
            idProspecto: 0, idServicio: 0, pagina: 1, numPaginas: 0,
            rows: 0, cotizaciones: [], idEstatusCotizacion: 1, idAlta: '', total: 0
        };
    }
    init() {
        this.lcots.cotizaciones = [];
        this.isLoading = true;
        let fil: string = (this.lcots.idEstatusCotizacion > 0 ? `estatus=1` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idServicio > 0 ? `servicio=${this.lcots.idServicio}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idProspecto > 0 ? `idProspecto=${this.lcots.idProspecto}` : '');
        if (fil.length > 0) fil = '?' + fil;
        this.http.get<ListaCotizacion>(`${this.url}api/cotizacion/${this.user.idPersonal}/${this.lcots.pagina}${fil}`).subscribe(response => {
            setTimeout(() => {
                this.lcots = response;
                this.isLoading = false;
            }, 300);

        }, err => {
            setTimeout(() => {
                this.isLoading = false;
                console.log(err)
            }, 300);

        });
        this.http.post<Prospecto[]>(`${this.url}api/prospecto/getcatalogo`, this.user.idPersonal).subscribe(response => {
            this.lpros = response;
        }, err => console.log(err));
    }

    lista() {
        this.lcots.cotizaciones = [];
        this.isLoading = true;
        let fil: string = (this.lcots.idEstatusCotizacion > 0 ? `estatus=${this.lcots.idEstatusCotizacion}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idServicio > 0 ? `servicio=${this.lcots.idServicio}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idProspecto > 0 ? `idProspecto=${this.lcots.idProspecto}` : '');
        if (fil.length > 0) fil = '?' + fil;
        this.http.get<ListaCotizacion>(`${this.url}api/cotizacion/${this.user.idPersonal}/${this.lcots.pagina}${fil}`).subscribe(response => {
            setTimeout(() => {
                this.lcots = response;
                this.isLoading = false;
            }, 300);

        }, err => {
            setTimeout(() => {
                this.isLoading = false;
                console.log(err)
            }, 300);

        });
    }

    busca() {
        this.lcots.pagina = 1;
        this.lista();
    }

    getDet(id: number, ser: string) {
        console.log(`${id} : ${ser}`);
    }

    muevePagina(event) {
        this.lcots.pagina = event;
        this.lista();
    }

    ngOnInit(): void {


        this.sub = this.route.params.subscribe(params => {
            let idp: number = +params['idp'];
            if (idp > 0) {
                this.lcots.idProspecto = idp;
            } else {
                this.nuevo();
            }
            this.lista();
            this.init();
        });
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }

    getDirs() {
        this.http.get<ListaDireccion>(`${this.url}api/cotizacion/limpiezadirectorio/${this.model.idCotizacion}`).subscribe(response => {
            this.lsdir = response;
        }, err => console.log(err));
    }


    elige(idCotizacion) {
        this.idCotizacion = idCotizacion;
        this.eliw.titulo = 'Eliminar'; //error
        this.eliw.mensaje = 'Se eliminar\u00E1 la cotizaci\u00F3n';
        this.eliw.open();
    }

    elimina($event) {
        if ($event) {
            this.http.post<boolean>(`${this.url}api/cotizacion/EliminarCotizacion`, this.idCotizacion).subscribe(response => {
                this.isErr = false;
                this.errMessage = 'Cotizaci\u00F3n ' + this.idCotizacion + ' eliminada';
                this.evenSub.next();
            }, err => console.log(err));
        }
        this.init();
    }
    editar(idCotizacion: number, servicio: string) {
        this.ediw.openSel(idCotizacion,servicio);
    }
    goBack() {
        window.history.back();
    }
    chgEstatus(idCotizacion: number, idEstatusCotizacion: number) {
        this.isErr = false;
        this.errMessage = '';
        if (idEstatusCotizacion === 1) {
            this.http.put<boolean>(`${this.url}api/cotizacion/desactivarcotizacion`, idCotizacion).subscribe(response => {
                this.lista();
                this.isErr = false;
                this.errMessage = 'Cotizaci\u00F3n ' + idCotizacion + ' desactivada';
                this.evenSub.next();
            }, err => {
                console.log(err)
            });
        } else {
            this.http.put<boolean>(`${this.url}api/cotizacion/activarcotizacion`, idCotizacion).subscribe(response => {
                this.lista();
                this.isErr = false;
                this.errMessage = 'Cotizaci\u00F3n ' + idCotizacion + ' activada';
                this.evenSub.next();
            }, err => {
                console.log(err)
            });
        }
    }
    editReturn($event) {    
        this.init();
    }
}
