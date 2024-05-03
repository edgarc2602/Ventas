import { Component, Inject, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;
import { ToastWidget } from '../toast/toast.widget';
import { CotizaResumenLim } from '../../models/cotizaresumenlim';

@Component({
    selector: 'editcot-widget',
    templateUrl: './editacotizacion.widget.html'
})
export class EditarCotizacion {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @Output('editCot') sendEvent = new EventEmitter<boolean>();
    model: CotizaResumenLim = {
        idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, prestaciones: 0, provisiones: 0,
        material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0,
        subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', utilidadPor: '', indirectoPor: '', csvPor: '', comisionSV: 0, comisionExt: 0, comisionExtPor: '', polizaCumplimiento: false, totalPolizaCumplimiento: 0
    };
    sers: ItemN[] = [];
    idServicio: number = 0;
    idCotizacion: number = 0;
    polizaCumplimiento: boolean = false;
    validacion: boolean = false;
    isLoading: boolean = false;
    lerr: any = {};

    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient,
        private rtr: Router, private sinU: StoreUser
    ) {
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    openSel(idCotizacion: number, servicio: string, polizaCumplimiento: boolean) {
        this.lerr = {};
        this.polizaCumplimiento = polizaCumplimiento;
        switch (servicio) {
            case 'Mantenimiento':
                this.idServicio = 1;
                break;
            case 'Limpieza':
                this.idServicio = 2;
                break;
            case 'Sanitización':
                this.idServicio = 3;
                break;
            default:
                break;
        }
        this.idCotizacion = idCotizacion;
        let docModal = document.getElementById('editcot');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    guarda() {
        if (this.valida()) {
            this.http.get<boolean>(`${this.url}api/cotizacion/ActualizarCotizacion/${this.idCotizacion}/${this.idServicio}/${this.polizaCumplimiento}`).subscribe(response => {
                //actualizar monto
                this.http.get<CotizaResumenLim>(`${this.url}api/cotizacion/limpiezaresumen/${this.idCotizacion}`).subscribe(response => {
                    this.model = response;
                }, err => {
                    console.log(err)
                });
                this.close();
                this.okToast('Cotizaci\u00F3n ' + this.idCotizacion + ' actualizada');
                this.sendEvent.emit(true);
            }, err => {
                this.close();
                this.errorToast('Ocurrió un error');
            });
        }
    }

    valida() {
        this.validacion = true;
        if (this.idServicio == 0) {
            this.lerr['IdServicio'] = ['Seleccione un servicio'];
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
        let docModal = document.getElementById('editcot');
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