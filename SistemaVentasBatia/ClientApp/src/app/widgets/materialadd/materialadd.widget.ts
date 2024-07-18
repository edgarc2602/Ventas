import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { Material } from 'src/app/models/material';
import { StoreUser } from 'src/app/stores/StoreUser';
import { numberFormat } from 'highcharts';
declare var bootstrap: any;
import { Subject } from 'rxjs';
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';

@Component({
    selector: 'mateadd-widget',
    templateUrl: './materialadd.widget.html'
})
export class MaterialAddWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('returnModal') returnModal = new EventEmitter<boolean>();
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    dirs: Catalogo[] = [];
    pues: Catalogo[] = [];
    mats: Catalogo[] = [];
    fres: ItemN[] = [];
    model: Material = {} as Material;
    lerr: any = {};
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idS: number = 0;
    showSuc: boolean = false;
    isExtra: boolean = false;
    validaciones: boolean = false;
    isLoading: boolean = false;
    edit: number = 0;
    hidedir: number = 0;
    nombreSucursal: string = '';
    puesto: string = '';
    tipo: string = 'material';
    idServicioCotizacion: number = 0;
    diasEvento: number = 0;
    idProducto: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) { }

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
        this.edit = 0;
        let fec: Date = new Date();
        this.model = {
            idMaterialCotizacion: 0, claveProducto: '', idCotizacion: this.idC,
            idPuestoDireccionCotizacion: id, precioUnitario: 0, cantidad: 0, idFrecuencia: 0,
            total: 0, fechaAlta: fec.toISOString(), idDireccionCotizacion: this.idD, idPersonal: this.sinU.idPersonal, edit: this.edit, diasEvento: 0,descripcion: '', frecuencia: ''
        };
        this.lerr = {};
    }

    existe(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<Material>(`${this.url}api/${this.tipo}/getbyid/${id}`).subscribe(response => {
            this.model = response;
            this.model.edit = this.edit;
        }, err => console.log(err));
    }
    guarda() {
        if (this.idServicioCotizacion == 4 || this.idServicioCotizacion == 5) {
            this.model.idFrecuencia = 1
        }
        this.quitarFocoDeElementos();
        this.lerr = {};
        this.model.idPersonal = this.sinU.idPersonal;
        this.model.diasEvento = this.diasEvento;
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                this.http.post<Material>(`${this.url}api/${this.tipo}`, this.model).subscribe(response => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast(this.model.claveProducto + ' guardado');
                    }, 300);
                    this.close();
                    if (this.model.idPuestoDireccionCotizacion != 0) {
                        this.returnModal.emit(true);
                    }
                    else {
                        this.returnModal.emit(false);
                    }
                }, err => {
                    this.detenerCarga();
                    this.errorToast('Ocurri\u00F3 un error');
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }, 300);
        }
    }

    open(cot: number, dir: number, pue: number, id: number, ser: number, tp: string, showS: boolean = false, edit: number, nombreSucursal?: string, puesto?: string, idServicioCotizacion?: number, diasEvento?: number) {
        this.diasEvento = diasEvento;
        this.idServicioCotizacion = idServicioCotizacion;
        this.isExtra = false;
        if (puesto != undefined && puesto != "") {
            this.puesto = puesto.charAt(0).toUpperCase() + puesto.slice(1).toLowerCase();
        }
        else {
            this.isExtra = true;
        }
        this.nombreSucursal = nombreSucursal;
        this.lerr = {};
        this.edit = edit;
        this.idC = cot;
        this.idD = dir;
        this.idP = pue;
        this.idProducto = id;
        if (this.idP != 0) {
            this.hidedir = 1;
        }
        else {
            this.hidedir = 0;
        }
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
        myModal.hide();
        if (this.model.idPuestoDireccionCotizacion != 0) {
            this.returnModal.emit(true);
        }
        else {
            this.returnModal.emit(false);
        }
    }

    valida() {
        this.validaciones = true;
        if (this.model.idDireccionCotizacion == 0) {
            this.lerr['IdDireccionCotizacion'] = ['Sucursal es necesaria'];
            this.validaciones = false;
        }
        if (this.model.idFrecuencia == 0) {
            this.lerr['IdFrecuencia'] = ['Frecuencia es necesaria'];
            this.validaciones = false;
        }
        if (this.model.claveProducto == '' || this.model.claveProducto == '0') {
            this.lerr['ClaveProducto'] = ['Producto es necesario'];
            this.validaciones = false;
        }
        if (this.model.cantidad <= 0) {
            this.lerr['Cantidad'] = ['Cantidad debe ser mayor que 0'];
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