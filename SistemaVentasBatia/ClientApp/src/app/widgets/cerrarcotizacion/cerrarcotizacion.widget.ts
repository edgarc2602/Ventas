import { Component, Inject, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ToastWidget } from '../toast/toast.widget';
declare var bootstrap: any;

@Component({
    selector: 'cerrarcotizacion-widget',
    templateUrl: './cerrarcotizacion.widget.html'
})
export class CerrarCotizacion {
    @Output('cerrarCotizacionEvent') sendEvent = new EventEmitter<boolean>();
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    sers: ItemN[] = [];
    idCotizacion: number = 0;
    idServicio: number = 0;
    motivoLength: number = 500;
    validacion: boolean = false;
    isLoading: boolean = false;
    motivoCierre: string = '';
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private rtr: Router, private sinU: StoreUser) {
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    open(idCotizacion: number) {
        this.idCotizacion = idCotizacion;
        let docModal = document.getElementById('cerrarCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    guarda() {
        this.lerr = {};
        if (this.valida()) {
            this.http.get<boolean>(`${this.url}api/cotizacion/CerrarCotizacion/${this.idCotizacion}/${this.motivoCierre}`).subscribe(response => {
                this.okToast('Se cerr\u00F3 correctamente la cotizaci\u00F3n ' + this.idCotizacion)
                this.close();
                this.sendEvent.emit(true);
            }, err => {
                this.errorToast('Ocurri\u00F3 un error');
                err => console.log(err);
            })
        }
    }

    valida() {
        this.validacion = true;
        if (this.motivoCierre == '' || this.motivoCierre == null) {
            this.lerr['motivoCierre'] = ['Indique el motivo de cierre de cotizaci\u00F3n'];
            this.validacion = false;
        }
        if (this.motivoCierre.length > 500) {
            this.lerr['motivoCierre'] = ['Max. 500 caracteres'];
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

    close() {
        let docModal = document.getElementById('cerrarCotizacion');
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