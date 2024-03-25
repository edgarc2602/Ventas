import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;

@Component({
    selector: 'agregarservicio-widget',
    templateUrl: './agregarservicio.widget.html'
})
export class AgregarServicioWidget {
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
                this.close();
                this.servicio = '';
                this.sendEvent.emit(4);
            }, err => {
                //console.log(err);
                //this.isErr = true;
                //this.validaMess = 'Ocurrio un error';
                //this.evenSub.next();
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
}