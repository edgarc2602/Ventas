import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { Material } from 'src/app/models/material';
import { StoreUser } from 'src/app/stores/StoreUser';
import { numberFormat } from 'highcharts';
declare var bootstrap: any;

import { Cotizacionupd } from 'src/app/models/cotizacionupd';

@Component({
    selector: 'actualizacotizacion-widget',
    templateUrl: './actualizacotizacion.widget.html'
})
export class ActualizaCotizacionWidget {

    model: Material = {} as Material;
    modelcot: Cotizacionupd = {
        idCotizacion: 0, indirecto: '', utilidad: '', comisionSV: '',comisionExt: ''
    };
    indirectoValue: string = "";
    utilidadValue: string = "";
    clave: string = "";


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) { }

    guarda() {
        this.modelcot.idCotizacion = this.model.idCotizacion;
        this.modelcot.indirecto = this.indirectoValue;
        this.modelcot.utilidad = this.utilidadValue;

        if (this.model.idCotizacion != 0) {
            this.http.post<Cotizacionupd>(`${this.url}api/cotizacion/ActualizarIndirectoUtilidadService`, this.modelcot).subscribe(response => {
                
            }, err => console.log(err));
        }
        location.reload();
    }

    open() {
        let docModal = document.getElementById('modalActualizarCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalActualizarCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }
}