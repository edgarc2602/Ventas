import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;
import { ToastWidget } from '../toast/toast.widget';


@Component({
    selector: 'agregarservicio-widget',
    templateUrl: './agregarservicio.widget.html'
})
export class AgregarServicioWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @Output('serEvent') sendEvent = new EventEmitter<number>();
    servicio: string = '';
    validaciones: boolean = false;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) { }

    guarda() {
        this.quitarFocoDeElementos();
        this.lerr = {};
        if (this.valida()) {
            this.http.get<boolean>(`${this.url}api/producto/agregarservicio/${this.servicio}/${this.sinU.idPersonal}`).subscribe(response => {
                this.okToast('Servicio agregado'); 
                this.close();
                this.servicio = '';
                this.sendEvent.emit(4);
            }, err => {
                this.errorToast('Ocurri\u00F3 un error');
                if (err.error) {
                    if (err.error.errors) {
                        this.lerr = err.error.errors;
                    }
                }
            });
        }
    }

    open() {
        let docModal = document.getElementById('modalLimpiezaAgregarServicio');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }
    close() {
        let docModal = document.getElementById('modalLimpiezaAgregarServicio');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    valida() {
        this.validaciones = true;
        if (this.servicio == '') {
            this.lerr['Servicio'] = ['Ingrese un servicio'];
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
}