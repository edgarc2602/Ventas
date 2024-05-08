import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ListaProspecto } from '../../models/listaprospecto';
import { ItemN } from 'src/app/models/item';
import { EliminaWidget } from 'src/app/widgets/elimina/elimina.widget';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
    selector: 'prospecto',
    templateUrl: './prospecto.component.html',
    animations: [fadeInOut],
})
export class ProspectoComponent {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(EliminaWidget, { static: false }) eliw: EliminaWidget;
    @ViewChild('tbprospectos', { static: false }) tablaContainer: ElementRef;

    lspro: ListaProspecto = {
        idEstatusProspecto: 1, keywords: '', numPaginas: 0, pagina: 1, prospectos: [], rows: 0       
    };
    lests: ItemN[] = [];
    idpro: number = 0;
    isLoading: boolean = false;
    private searchKeyword$ = new Subject<string>();
    
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private rter: Router, public user: StoreUser) {
        http.get<ItemN[]>(`${url}api/prospecto/getestatus`).subscribe(response => {
            this.lests = response;
        }, err => console.log(err));
        this.lista();
        this.searchKeyword$.pipe(
            debounceTime(800),
            distinctUntilChanged()
        ).subscribe(() => {
            this.lspro.pagina = 1;
            this.lista();
        });
    }

    onKeywordsInput() {
        this.searchKeyword$.next(this.lspro.keywords);
    }
    lista() {
        this.isLoading = true;
        let qust: string = this.lspro.keywords == '' ? '' : '?keywords=' + this.lspro.keywords;
        this.http.get<ListaProspecto>(`${this.url}api/prospecto/${this.user.idPersonal}/${this.lspro.pagina}/${this.lspro.idEstatusProspecto}${qust}`).subscribe(response => {
            setTimeout(() => {
                this.lspro = response;
                this.isLoading = false;
                const container = this.tablaContainer.nativeElement;
                container.scrollTop = 0;
            }, 300);
        }, err => {
            setTimeout(() => {
                this.isLoading = false;
                console.log(err)
            }, 300);
        });
    }

    muevePagina(event) {
        this.lspro.pagina = event;
        this.lista();
    }

    nuevo() {
        this.rter.navigate(['/exclusivo/nuevopros']);
    }

    elige(idProspecto: number) {
        this.idpro = idProspecto;
        this.eliw.titulo = 'Desactivar prospecto';
        this.eliw.mensaje = 'Se desactivarán las cotizaciones relacionadas y no serán visibles';
        this.eliw.open();
    }

    elimina($event) {
        if ($event) {
            this.http.delete<boolean>(`${this.url}api/prospecto/${this.idpro}`).subscribe(response => {
                console.log(response);
            }, err => console.log(err));
        }
        this.lista();
    }

    goBack() {
        window.history.back();
    }

    desactivar() {
            this.http.put<boolean>(`${this.url}api/prospecto/desactivarprospecto`, this.idpro).subscribe(response => {
                this.lista();
                this.okToast('Prospecto desactivado');
            }, err => {
                console.log(err)
                this.errorToast('Ocurrió un error');
            });
        this.idpro = 0;
    }

    activar(idProspecto: number) {
        this.http.put<boolean>(`${this.url}api/prospecto/activarprospecto`, idProspecto).subscribe(response => {
            this.lista();
            this.okToast('Prospecto activado');
        }, err => {
            console.log(err)
            this.errorToast('Ocurrió un error');
        });
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
}