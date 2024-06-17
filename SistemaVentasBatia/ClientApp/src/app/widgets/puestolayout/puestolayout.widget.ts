import { Component, Inject, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PuestoCotiza } from '../../models/puestocotiza';
import { Catalogo } from '../../models/catalogo';
import { ItemN } from '../../models/item';
import { SalarioMin } from '../../models/salariomin';
import { StoreUser } from '../../stores/StoreUser';
declare var bootstrap: any;

@Component({
    selector: 'pueslayout-widget',
    templateUrl: './puestolayout.widget.html'
})
export class PuestoLayoutWidget {
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    model: PuestoCotiza = {} as PuestoCotiza;
    lclas: Catalogo[] = [];
    tips: Catalogo[] = [];
    pues: Catalogo[] = [];
    turs: Catalogo[] = [];
    tabs: Catalogo[] = [];
    ljor: Catalogo[] = [];
    hors: string[] = [];
    dias: ItemN[] = [];
    fres: Catalogo[] = [];
    suel: SalarioMin = {} as SalarioMin;
    jornada: number = 0;
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idT: number = 0;
    validacion: boolean = false;
    lerr: any = {};
    
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getturno`).subscribe(response => {
            this.turs = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/catalogo/getdia`).subscribe(response => {
            this.dias = response;
        }, err => console.log(err));
        var i = 0;
        this.http.get<string[]>(`${this.url}api/catalogo/gethorario`).subscribe(response => {
            this.hors = response;
            for (let h of this.hors) {
                this.hors[i] = i.toString();
                i += 1;
            }
        }, err => console.log(err));
        http.get<Catalogo[]>(`${this.url}api/tabulador/getbyedo/${1}`).subscribe(response => {
            this.tabs = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getjornada`).subscribe(response => {
            this.ljor = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getclase`).subscribe(response => {
            this.lclas = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getinmuebletipo`).subscribe(response => {
            this.tips = response;
        }, err => console.log(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getfrecuencia`).subscribe(response => {
            this.fres = response;
        }, err => console.log(err));
    }

    open() {
        let docModal = document.getElementById('modalPuestoLayout');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalPuestoLayout');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }
}