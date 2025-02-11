import { Component, Inject, ViewChild, ElementRef, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { ListaProspecto } from '../../models/listaprospecto';
import { ItemN } from 'src/app/models/item';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { ConfirmacionWidget } from 'src/app/widgets/confirmacion/confirmacion.widget'

@Component({
    selector: 'prospecto',
    templateUrl: './prospecto.component.html',
    animations: [fadeInOut],
})
export class ProspectoComponent implements OnInit, OnDestroy {
    @ViewChild(ConfirmacionWidget, { static: false }) confirma: ConfirmacionWidget;
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild('tbprospectos', { static: false }) tablaContainer: ElementRef;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    lspro: ListaProspecto = {
        idEstatusProspecto: 1, keywords: '', numPaginas: 0, pagina: 1, prospectos: [], rows: 0
    };
    lests: ItemN[] = [];
    idpro: number = 0;
    isLoading: boolean = false;
    private searchKeyword$ = new Subject<string>();
    sub: any;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private rter: Router, public user: StoreUser) {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        http.get<ItemN[]>(`${url}api/prospecto/getestatus`, {headers}).subscribe(response => {
            this.lests = response;
        }, err => {
            this.validaError(err);
        });
        this.lista();
        this.searchKeyword$.pipe(
            debounceTime(800),
            distinctUntilChanged()
        ).subscribe(() => {
            this.lspro.pagina = 1;
            this.lista();
        });
    }

    ngOnInit(): void {
        this.init();
    }

    ngOnDestroy(): void {
    }

    init() {
        this.isLoading = true;
        let qust: string = this.lspro.keywords == '' ? '' : '?keywords=' + this.lspro.keywords;
        this.http.get<ListaProspecto>(`${this.url}api/prospecto/${this.user.idPersonal}/${this.lspro.pagina}/${this.lspro.idEstatusProspecto}${qust}`, { headers: this.getHeaders() }).subscribe(response => {
            this.isLoading = false;
            this.lspro = response;
        }, err => {
            this.isLoading = false;
            this.validaError(err);
        });
    }

    validaError(err: any) {
        if (err.status === 401) {
            this.errorToast('⚠️ No autorizado. Inicia sesión nuevamente.');
            this.rter.navigate(['']);
        } else {
            this.errorToast('Ocurrió un error');
        }
    }

    getHeaders() {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        return headers;
    }

    lista() {
        let qust: string = this.lspro.keywords == '' ? '' : '?keywords=' + this.lspro.keywords;
        this.http.get<ListaProspecto>(`${this.url}api/prospecto/${this.user.idPersonal}/${this.lspro.pagina}/${this.lspro.idEstatusProspecto}${qust}`, { headers: this.getHeaders() }).subscribe(response => {
            setTimeout(() => {
                this.lspro = response;
                if (this.tablaContainer) {
                    const container = this.tablaContainer.nativeElement;
                    container.scrollTop = 0;
                }
                this.detenerCarga();
            }, 300);
        }, err => {
            setTimeout(() => {
                this.isLoading = false;
                this.validaError(err);
                this.detenerCarga();
            }, 300);
        });
    }

    nuevo() {
        this.rter.navigate(['/exclusivo/nuevopros']);
    }

    muevePagina(event) {
        this.iniciarCarga();
        this.lspro.pagina = event;
        this.lista();
    }

    onKeywordsInput() {
        this.searchKeyword$.next(this.lspro.keywords);
    }


    //Abrir modal cofirmacion
    confirmaActivarProspecto(idProspecto: number) {
        this.idpro = idProspecto;
        const tipo = 'activarProspecto';
        const titulo = 'Activar prospecto';
        const mensaje = 'Se activara el prospecto';
        this.confirma.open(tipo, titulo, mensaje);
    }

    confirmaDesactivarProspecto(idProspecto: number) {
        this.idpro = idProspecto;
        const tipo = 'desactivarProspecto';
        const titulo = 'Desactivar prospecto';
        const mensaje = 'Se desactivarán las cotizaciones relacionadas y no serán visibles';
        this.confirma.open(tipo, titulo, mensaje);
    }


    //Respuesta de confirmacion
    confirmacionEvent($event) {
        if ($event == 'activarProspecto') {
            this.activarProspecto();
        }
        if ($event == 'desactivarProspecto') {
            this.desactivarProspecto();
        }
    }


    //acciones de confirmacion
    desactivarProspecto() {
        this.http.put<boolean>(`${this.url}api/prospecto/desactivarprospecto`, this.idpro, {headers: this.getHeaders()}).subscribe(response => {
            this.lista();
            this.okToast('Prospecto desactivado');
        }, err => {
            this.validaError(err);
        });
        this.idpro = 0;
    }

    activarProspecto() {
        this.http.put<boolean>(`${this.url}api/prospecto/activarprospecto`, this.idpro, { headers: this.getHeaders() }).subscribe(response => {
            this.lista();
            this.okToast('Prospecto activado');
        }, err => {
            this.validaError(err);
        });
    }

    //eliminarProspecto() {
    //    this.http.delete<boolean>(`${this.url}api/prospecto/${this.idpro}`).subscribe(response => {
    //        this.lista();
    //        console.log(response);
    //    }, err => console.log(err));
    //}

    //imports

    goBack() {
        window.history.back();
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