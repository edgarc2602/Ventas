import { Component, Inject, Output, ViewChild,EventEmitter } from '@angular/core';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { PuestoCotiza } from '../../models/puestocotiza';
import { Catalogo } from '../../models/catalogo';
import { ItemN } from '../../models/item';
import { SalarioMin } from '../../models/salariomin';
import { StoreUser } from '../../stores/StoreUser';
import { Router } from '@angular/router';
import { ToastWidget } from '../toast/toast.widget';
declare var bootstrap: any;

@Component({
    selector: 'pueslayout-widget',
    templateUrl: './puestolayout.widget.html'
})
export class PuestoLayoutWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    model: PuestoCotiza = {} as PuestoCotiza;
    lclas: Catalogo[] = [];
    tips: Catalogo[] = [];
    pues: Catalogo[] = [];
    turs: Catalogo[] = [];
    tabs: Catalogo[] = [];
    ljor: Catalogo[] = [];
    edos: Catalogo[] = [];
    hors: string[] = [];
    dias: ItemN[] = [];
    fres: Catalogo[] = [];
    suel: SalarioMin = {} as SalarioMin;
    jornada: number = 0;
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idT: number = 0;
    validacion: boolean = false;
    lerr: any = {};
    
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser, private rtr: Router) {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

        http.get<Catalogo[]>(`${url}api/catalogo/getpuesto/0`, {headers}).subscribe(response => {
            this.pues = response;
        }, err => this.validaError(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getturno`, { headers }).subscribe(response => {
            this.turs = response;
        }, err => this.validaError(err));
        http.get<ItemN[]>(`${url}api/catalogo/getdia`, { headers }).subscribe(response => {
            this.dias = response;
        }, err => this.validaError(err));
        var i = 0;
        this.http.get<string[]>(`${this.url}api/catalogo/gethorario`, { headers }).subscribe(response => {
            this.hors = response;
            for (let h of this.hors) {
                this.hors[i] = i.toString();
                i += 1;
            }
        }, err => this.validaError(err));
        http.get<Catalogo[]>(`${this.url}api/tabulador/getbyedo/${1}`, { headers }).subscribe(response => {
            this.tabs = response;
        }, err => this.validaError(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getjornada/0`, { headers }).subscribe(response => {
            this.ljor = response;
        }, err => this.validaError(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getclase`, { headers }).subscribe(response => {
            this.lclas = response;
        }, err => this.validaError(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getinmuebletipo`, { headers }).subscribe(response => {
            this.tips = response;
        }, err => this.validaError(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getfrecuencia`, { headers }).subscribe(response => {
            this.fres = response;
        }, err => this.validaError(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getestado`, { headers }).subscribe(response => {
            this.edos = response;
        }, err => this.validaError(err));
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
    open() {
        let docModal = document.getElementById('modalPuestoLayout');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalPuestoLayout');
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
}