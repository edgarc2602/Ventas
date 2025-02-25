import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ListaMaterial } from 'src/app/models/listamaterial';
declare var bootstrap: any;
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { Router } from '@angular/router';

@Component({
    selector: 'mate-widget',
    templateUrl: './material.widget.html'
})
export class MaterialWidget {
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    model: ListaMaterial = {} as ListaMaterial;
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    edit: number = 0;
    total: number = 0;
    nombreSucursal: string = '';
    puesto: string = '';
    tipo: string = '';
    isLoading: boolean = false;
    idEstatus: number = 0;
    idTipoServicio: number = 0;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private rtr: Router) { }

    existe(id: number) {
        this.total = 0;
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<ListaMaterial>(`${this.url}api/${this.tipo}/getbypuesto/${id}`, {headers: this.getHeaders()}).subscribe(response => {
            this.model.edit = this.edit;
            this.model = response;
            this.totalModal();
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
    agregarMaterial() {
        this.model.edit = 0;
        this.sendEvent.emit(0);
        this.total = 0;
    }

    select(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.sendEvent.emit(id);
        this.total = 0;
    }

    remove(id: number) {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.delete<boolean>(`${this.url}api/${this.tipo}/${id}`, {headers: this.getHeaders()}).subscribe(response => {
                this.detenerCarga();
                setTimeout(() => {
                    this.okToast(this.tipo + ' eliminado')
                }, 300);
            if (response) {
                this.existe(this.idP);
            }
            }, err => {
                this.detenerCarga();
                setTimeout(() => {
                    this.validaError(err);
                }, 300);
                console.log(err);
            });
            this.total = 0;
        }, 300);
    }

    open(cot: number, dir: number, pue: number, tp: string, edit: number, nombreSucursal?: string, puesto?: string, idEstatus?: number, idTipoServicio?: number) {
        this.idEstatus = idEstatus;
        this.idTipoServicio = idTipoServicio;
        if (nombreSucursal == undefined || nombreSucursal == '') {
        }
        else {
            this.nombreSucursal = nombreSucursal;
        }
        if (puesto != undefined) {
            this.puesto = puesto.charAt(0).toUpperCase() + puesto.slice(1).toLowerCase();
        }
        else {
            puesto = '';
        }
        this.idC = cot;
        this.idD = dir;
        this.idP = pue;
        this.tipo = tp.charAt(0).toUpperCase() + tp.slice(1).toLowerCase();
        this.total = 0;
        this.existe(this.idP);
        let docModal = document.getElementById('modalLimpiezaMaterialOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalLimpiezaMaterialOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
        this.total = 0;
    }

    totalModal() {
        this.total = 0;
        this.model.materialesCotizacion.forEach((total) => {
            this.total = this.total + total.total;
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

    iniciarCarga() {
        this.isLoading = true;
        this.cargaWidget.open(true);
    }

    detenerCarga() {
        this.isLoading = false;
        this.cargaWidget.open(false);
    }
}