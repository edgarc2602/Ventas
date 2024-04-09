import { Component, Inject, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;
import Swal from 'sweetalert2';

@Component({
    selector: 'editcot-widget',
    templateUrl: './editacotizacion.widget.html'
})
export class EditarCotizacion {
    @Output('editCot') sendEvent = new EventEmitter<boolean>();
    sers: ItemN[] = [];
    idCotizacion: number = 0;
    idServicio: number = 0;
    validacion: boolean = false;
    lerr: any = {};

    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient,
        private rtr: Router, private sinU: StoreUser
    ) {
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    openSel(idCotizacion: number, servicio: string) {
        this.lerr = {};
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
            this.http.get<boolean>(`${this.url}api/cotizacion/ActualizarCotizacion/${this.idCotizacion}/ ${this.idServicio}`).subscribe(response => {
                this.close();
                Swal.fire({
                    icon: 'success',
                    timer: 1000,
                    showConfirmButton: false,
                });
                this.sendEvent.emit(true);
            }, err => {
                this.close();
                Swal.fire({
                    title: 'Error',
                    text: 'Ocurrió un error al editar el tipo de servicio',
                    icon: 'error',
                    timer: 3000,
                    showConfirmButton: false,
                });
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
}