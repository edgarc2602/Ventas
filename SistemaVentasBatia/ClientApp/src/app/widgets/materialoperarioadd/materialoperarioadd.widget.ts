import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { Material } from 'src/app/models/material';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;

@Component({
    selector: 'mateoperarioadd-widget',
    templateUrl: './materialoperarioadd.widget.html'
})
export class MaterialOperarioAddWidget {
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idS: number = 0;
    edit: number = 0;
    showSuc: boolean = false;
    tipo: string = 'material';
    @Output('smEvent2') sendEvent = new EventEmitter<number>();
    model: Material = {} as Material;
    dirs: Catalogo[] = [];
    pues: Catalogo[] = [];
    mats: Catalogo[] = [];
    fres: ItemN[] = [];
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {}
        
    lista() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idS}/${this.tipo}`).subscribe(response => {
            this.mats = response;
        }, err => console.log(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getpuestobycot/${this.idC}`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getsucursalbycot/${this.idC}`).subscribe(response => {
            this.dirs = response;
        }, err => console.log(err));
        this.http.get<ItemN[]>(`${this.url}api/catalogo/getfrecuencia`).subscribe(response => {
            this.fres = response;
        }, err => console.log(err));
    }

    nuevo(id: number) {
        let fec: Date = new Date();
        this.model = {
            idMaterialCotizacion: 0, claveProducto: '', idCotizacion: this.idC,
            idPuestoDireccionCotizacion: id, precioUnitario: 0, cantidad: 0, idFrecuencia: 0,
            total: 0, fechaAlta: fec.toISOString(), idDireccionCotizacion: this.idD, idPersonal: this.sinU.idPersonal, edit: this.edit,
        };
    }

    existe(id: number) {
        this.http.get<Material>(`${this.url}api/${this.tipo}/getbyid/${id}`).subscribe(response => {
            this.model = response;
        }, err => console.log(err));
    }
    guarda() {
        this.http.post<Material>(`${this.url}api/${this.tipo}`, this.model).subscribe(response => {
            this.close();
            this.sendEvent.emit(2);
        }, err => console.log(err));
        let docModal = document.getElementById('modalLimpiezaMaterialOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    open(cot: number, dir: number, pue: number, id: number, ser: number, tp: string, showS: boolean = false) {
        this.idC = cot;
        this.idD = dir;
        this.idP = pue;
        this.idS = ser;
        this.tipo = tp;
        this.showSuc = showS;
        this.lista();
        if (id == 0) {
            this.nuevo(this.idP);
        } else {
            this.existe(id);
        }
        let docModal = document.getElementById('modalLimpiezaAgregarMaterialCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalLimpiezaAgregarMaterialCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.open();
    }
}