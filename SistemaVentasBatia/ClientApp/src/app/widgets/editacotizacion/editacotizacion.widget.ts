import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Prospecto } from 'src/app/models/prospecto';
import { Cotizacion } from 'src/app/models/cotizacion';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from 'src/app/stores/StoreUser';
import { position } from 'html2canvas/dist/types/css/property-descriptors/position';
declare var bootstrap: any;

@Component({
    selector: 'editcot-widget',
    templateUrl: './editacotizacion.widget.html'
})
export class EditarCotizacion {
    lpros: Prospecto[] = [];
    model: Cotizacion = {} as Cotizacion;
    sers: ItemN[] = [];
    lerr: any = {};
    idCotizacion: number = 0;
    prospecto: number = 0;
    servicio: number = 0;
    prospectostr: string = '';
    serviciostr: string = '';

    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient,
        private rtr: Router, private sinU: StoreUser
    ) {
        this.nuevo();
        this.http.post<Prospecto[]>(`${this.url}api/prospecto/getcatalogo`, this.sinU.idPersonal).subscribe(response => {
            this.lpros = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.model = {
            idCotizacion: 0, idProspecto: 0, idServicio: 0, total: 0,
            fechaAlta: fec.toISOString(), idCotizacionOriginal: 0,
            idPersonal: this.sinU.idPersonal, listaServicios: [], salTipo: 0, listaTipoSalarios: []
        };
    }

    obtenerids(prospecto: string, servicio: string) {
        for (const pros of this.lpros) {
            if (pros.nombreComercial === prospecto) {
                this.prospecto = pros.idProspecto;
                break;
            }
        }
        for (const ser of this.sers) {
            if (ser.nom === servicio) {
                this.servicio = ser.id;
                break;
            }
        }
    }

    guarda() {

        this.model.idProspecto = this.prospecto;
        this.model.idServicio = this.servicio;
        this.http.post<boolean>(`${this.url}api/cotizacion/actualizarcotizacion`, this.model).subscribe(response => {
            console.log(response);
            if (response) {
                console.log('La respuesta es verdadera');
            } else {
                console.log('La respuesta es falsa');
            }
        }, err => console.error(err));
        this.close();
        location.reload();
    }

    openSel(idCotizacion: number, prospecto: string, servicio: string) {
        this.model.idCotizacion = idCotizacion;
        this.obtenerids(prospecto, servicio)
        this.lerr = {};
        let docModal = document.getElementById('editcot');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('editcot');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }
}