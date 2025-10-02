//ESTE MODAL ES PARA AGREGAR MATERIAL A LA COTIZACION YA SEA DE SINGA O SUMCO
import { DatePipe } from '@angular/common';
import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { Material } from 'src/app/models/material';
import { StoreUser } from 'src/app/stores/StoreUser';
import { numberFormat } from 'highcharts';
declare var bootstrap: any;
import { Subject } from 'rxjs';
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { Router } from '@angular/router';
import { listaproductosumco } from '../../models/listaproductosumco';
import { productossumco } from '../../models/productosumco';

@Component({
    selector: 'registermaterialsumco-widget',
    templateUrl: './registermaterialsumco.widget.html'
})
export class RegisterMaterialSumcoWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    //@Output('returnModal') returnModal = new EventEmitter<boolean>();
    @Output('sumcoEvent') sendEvent = new EventEmitter<boolean>();
    dirs: Catalogo[] = [];
    pues: Catalogo[] = [];
    mats: Catalogo[] = [];
    unidades: Catalogo[] = [];

    fres: ItemN[] = [];
    model: productossumco = {} as productossumco;
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

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser, private rtr: Router, private dtpipe: DatePipe) { }
    
    lista() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/ObtenerCatalogoUnidadMedida`, { headers: this.getHeaders() }).subscribe(response => {
            this.unidades = response;
        }, err => this.validaError(err));
        //this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idS}/${this.tipo}`, {headers:this.getHeaders()}).subscribe(response => {
        //    this.mats = response;
        //}, err => this.validaError(err));
        //this.http.get<Catalogo[]>(`${this.url}api/catalogo/getpuestobycot/${this.idC}`, { headers: this.getHeaders() }).subscribe(response => {
            //this.pues = response;
        //}, err => this.validaError(err));
        //this.http.get<Catalogo[]>(`${this.url}api/catalogo/getsucursalbycot/${this.idC}`, { headers: this.getHeaders() }).subscribe(response => {
        //    this.dirs = response;
        //}, err => this.validaError(err));
        //this.http.get<ItemN[]>(`${this.url}api/catalogo/getfrecuencia`, { headers: this.getHeaders() }).subscribe(response => {
            //this.fres = response;
        //}, err => this.validaError(err));
    }

    
    guarda() {
        //if (this.idServicioCotizacion == 4 || this.idServicioCotizacion == 5) {
        //    this.model.idFrecuencia = 1
        //}
        this.quitarFocoDeElementos();
        this.lerr = {};
        //this.model.idPersonal = this.sinU.idPersonal;
        //this.model.diasEvento = this.diasEvento;
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                if (this.model.idProducto == 0) {
                    //CREATE
                    this.http.post<boolean>(`${this.url}api/producto/InsertProductoSumco`, this.model, { headers: this.getHeaders() }).subscribe(response => {
                        this.detenerCarga();
                        this.sendEvent.emit(true);
                        setTimeout(() => {
                            this.okToast("Producto guardado");
                        }, 300);
                        this.close();
                    }, err => {
                        this.detenerCarga();
                        this.validaError(err);
                        console.log(err);
                    });
                }
                else {
                    //UPDATE
                    this.model.personalMod = this.sinU.idPersonal;
                    this.http.put<boolean>(`${this.url}api/producto/UpdateProductoSumco`, this.model, { headers: this.getHeaders() }).subscribe(response => {
                        this.detenerCarga();
                        this.sendEvent.emit(true);
                        setTimeout(() => {
                            this.okToast("Producto actualizado");
                        }, 300);
                        this.close();
                    }, err => {
                        this.detenerCarga();
                        this.validaError(err);
                    });
                }
            }, 300);
        }
    }

    open(idProducto: number) {
        this.model.idProducto = idProducto;
        this.lista();
        if (idProducto == 0) {
            this.nuevo();
        } else {
            this.existe(idProducto);
        }
        let docModal = document.getElementById('registermaterialsumco');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    nuevo() {
        //INICIALIZAR VALORES DEL FORMULARIO
        this.edit = 0;
        let fec: Date = new Date();
        this.model = {
            idProducto: 0, clave: '', nombre: '', idUnidad: 0, precio: 0, precioCompra: 0, fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), personalAlta: this.sinU.idPersonal, fechaMod: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), personalMod: 0, idEstatus: 0, unidad: ''
            
        };
        this.lerr = {};
    }

    existe(idProducto: number) {
        //CONSULTAR VALORES DEL MATERIAL SELECCIONADO
        this.edit = 1;
        this.http.get<productossumco>(`${this.url}api/producto/GetProductoSumco/${idProducto}`, { headers: this.getHeaders() }).subscribe(response => {
            this.model = response;
        }, err => this.validaError(err));
    }


    close() {
        let docModal = document.getElementById('registermaterialsumco');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    valida() {
        this.validaciones = true;
        if (this.model.nombre == null || this.model.nombre == '') {
            this.lerr['Nombre'] = ['Nombre es necesario'];
            this.validaciones = false;
        }
        if (this.model.idUnidad == null || this.model.idUnidad == 0) {
            this.lerr['Unidad'] = ['Unidad es necesario'];
            this.validaciones = false;
        }
        if (this.model.precio == null || this.model.precio == 0) {
            this.lerr['Precio'] = ['Precio es necesario'];
            this.validaciones = false;
        }
        if (this.model.precioCompra == null || this.model.precioCompra == 0) {
            this.lerr['PrecioCompra'] = ['Precio de Compra es necesario'];
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

    validaError(err: any) {
        if (err.status === 401) {
            this.errorToast('⚠️ No autorizado. Inicia sesión nuevamente.');
            localStorage.clear();
            localStorage.setItem('token', '');
            localStorage.setItem('usuario', '');
            this.rtr.navigate(['']);

        } else {
            if (err.error) {
                for (let key in err.error) {
                    if (err.error.hasOwnProperty(key)) {
                        this.lerr[key] = [err.error[key]];
                    }
                }
                if (err.error.errors) {
                    this.lerr = err.error.errors;
                }
            }
            if (err.message != null) {
                if (err.error.message) {
                    this.errorToast(err.error.message);
                }
                else {
                this.errorToast(err.message);
                }
            }
        }
    }
    getHeaders() {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        return headers;
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