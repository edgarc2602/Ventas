import { Component, Inject, OnChanges, Input, SimpleChanges, ViewChild, Output, EventEmitter } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Prospecto } from 'src/app/models/prospecto';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from '../../stores/StoreUser';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { ToastWidget } from '../toast/toast.widget';

@Component({
    selector: 'pros-widget',
    templateUrl: './prospecto.widget.html',
    providers: [DatePipe]
})
export class ProspectoWidget implements OnChanges {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    @Input() idP: number = 0;
    evenSub: Subject<void> = new Subject<void>();
    model: Prospecto = {} as Prospecto;
    docs: ItemN[] = [];
    lerr: any = {};
    isErr: boolean = false;
    errMessage: string = '';

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe, private sinU: StoreUser, private rtr: Router) {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

        http.get<ItemN[]>(`${url}api/prospecto/getdocumento`, {headers}).subscribe(response => {
            this.docs = response;
        }, err => this.validaError(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.model = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '',
            representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [], idPersonal: this.sinU.idPersonal,
            idEstatusProspecto: 0, idTipoIndustria : 0
        };
        this.docs.forEach(d => d.act = false);
    }

    existe(id: number) {
        this.http.get<Prospecto>(`${this.url}api/prospecto/${id}`, { headers: this.getHeaders() }).subscribe(response => {
            this.model = response;
            this.docs = this.model.listaDocumentos;
        }, err => this.validaError(err));
    }

    guarda() {
        this.quitarFocoDeElementos();
        this.model.listaDocumentos = this.docs;
        this.lerr = {};
        if (this.valida()) {
            if (this.model.idProspecto == 0) {
                this.http.post<Prospecto>(`${this.url}api/prospecto`, this.model, { headers: this.getHeaders() }).subscribe(response => {
                    console.log(response);
                    this.sendEvent.emit(response.idProspecto);
                    this.isErr = false;
                    this.errMessage = 'Prospecto guardado';
                    this.evenSub.next();
                }, err => {
                    console.log(err);
                    this.isErr = true;
                    this.validaError(err);
                    this.evenSub.next();
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            } else {
                this.http.put<Prospecto>(`${this.url}api/prospecto`, this.model, { headers: this.getHeaders() }).subscribe(response => {
                    console.log(response);
                    this.sendEvent.emit(response.idProspecto);
                    this.isErr = false;
                    this.errMessage = 'Prospecto actualizado';
                    this.evenSub.next();
                }, err => {
                    console.log(err);
                    this.isErr = true;
                    this.validaError(err);
                    this.evenSub.next();
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }
        }
    }
    validaError(err: any) {
        if (err.status === 401) {
            this.errorToast('⚠️ No autorizado. Inicia sesión nuevamente.');
            localStorage.clear();
            localStorage.setItem('token', '');
            localStorage.setItem('usuario', '');
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
    valida() {
        return true;
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

    ngOnChanges(changes: SimpleChanges): void {
        if (this.idP == 0) {
            this.nuevo();
        } else {
            this.existe(this.idP);
        }
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

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }

}