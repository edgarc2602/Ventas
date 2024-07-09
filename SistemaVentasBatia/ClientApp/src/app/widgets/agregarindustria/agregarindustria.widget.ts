import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';

@Component({
    selector: 'agregarindustria-widget',
    templateUrl: './agregarindustria.widget.html'
})
export class AgregarIndustriaWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('indEvent') sendEvent = new EventEmitter<number>();
    industria: string = '';
    validaciones: boolean = false;
    isLoading: boolean = false;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) { }

    guarda() {
        this.quitarFocoDeElementos();
        this.lerr = {};
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                this.http.get<boolean>(`${this.url}api/producto/agregarindustria/${this.industria}/${this.sinU.idPersonal}`).subscribe(response => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Industria agregada');
                    }, 300);
                    this.close();
                    this.industria = '';
                    this.sendEvent.emit(4);
                }, err => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.errorToast('Ocurri\u00F3 un error');
                    }, 300);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }, 300);
        }
    }

    open() {
        let docModal = document.getElementById('modalLimpiezaAgregarIndustria');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }
    close() {
        let docModal = document.getElementById('modalLimpiezaAgregarIndustria');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    valida() {
        this.validaciones = true;
        if (this.industria == '') {
            this.lerr['Industria'] = ['Ingrese un tipo de industria'];
            this.validaciones = false;
        }
        return this.validaciones;
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

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
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