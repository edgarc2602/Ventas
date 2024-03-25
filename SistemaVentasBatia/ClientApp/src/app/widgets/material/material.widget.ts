import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListaMaterial } from 'src/app/models/listamaterial';
declare var bootstrap: any;

@Component({
    selector: 'mate-widget',
    templateUrl: './material.widget.html'
})
export class MaterialWidget {
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    tipo: string = '';
    edit: number = 0;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    model: ListaMaterial = {} as ListaMaterial;
    total: number = 0;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) { }

    existe(id: number) {
        this.total = 0;
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<ListaMaterial>(`${this.url}api/${this.tipo}/getbypuesto/${id}`).subscribe(response => {
            this.model.edit = this.edit;
            this.model = response;
            this.totalModal();
        }, err => console.log(err));

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
        this.http.delete<boolean>(`${this.url}api/${this.tipo}/${id}`).subscribe(response => {
            if (response) {
                this.existe(this.idP);
            }
        }, err => console.log(err));
        this.total = 0;
    }

    open(cot: number, dir: number, pue: number, tp: string, edit: number) {
        this.idC = cot;
        this.idD = dir;
        this.idP = pue;
        this.tipo = tp;
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
}