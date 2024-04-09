import { Component, Inject, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from '../../stores/StoreUser';

declare var bootstrap: any;

@Component({
    selector: 'marcaventa-widget',
    templateUrl: './marcaventa.widget.html'
})
export class MarcaVenta {

    @Output('smEvent') sendEvent = new EventEmitter<number>();
    //model: PuestoCotiza = {} as PuestoCotiza;
    validacion: boolean = false;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));

    }

    nuevo() {

    }

    guarda() {
        this.lerr = {};
        //if (this.valida()) {
        //    this.http.post<PuestoCotiza>(`${this.url}api/puesto`, this.model).subscribe(response => {
        //        this.sendEvent.emit(0);
        //        this.close();
        //        //this.isErr = false;
        //        //this.evenSub.next();
        //    }, err => {
        //        console.log(err);
        //        //this.isErr = true;
        //        //this.evenSub.next();
        //        if (err.error) {
        //            if (err.error.errors) {
        //                this.lerr = err.error.errors;
        //            }
        //        }
        //    });
        //}
    }

    valida() {
        //this.validacion = true;
        //if (this.model.idTurno == 0) {
        //    this.lerr['IdTurno'] = ['Turno es obligatorio'];
        //    this.validacion = false;
        //}
        //if (this.model.idPuesto == 0) {
        //    this.lerr['IdPuesto'] = ['Puesto es obligatorio'];
        //    this.validacion = false;
        //}
        //return this.validacion;
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

    open(idCotizacion: number) {
            this.nuevo();

        let docModal = document.getElementById('modalMarcaVenta');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalMarcaVenta');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }
}