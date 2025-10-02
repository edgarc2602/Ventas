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
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { listacatalogoproductos } from '../../models/listacatalogoproductos';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
    selector: 'mateaddsumco-widget',
    templateUrl: './materialaddsumco.widget.html'
})
export class MaterialAddSumcoWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('returnModal') returnModal = new EventEmitter<boolean>();
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    dirs: Catalogo[] = [];
    pues: Catalogo[] = [];
    mats: Catalogo[] = [];
    filteredMats: Catalogo[] = [];
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
    productosfil: listacatalogoproductos = {
        productos: [], pagina:1, keywords: '', numPaginas: 0, rows: 0
    };
    private searchProductoKeyword$ = new Subject<string>();


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser, private rtr: Router) {
        this.searchProductoKeyword$.pipe(
            debounceTime(500),
            distinctUntilChanged()
        ).subscribe(() => {
            this.productosfil.pagina = 1;
            this.GetAllProductosSinga();
        });
        
    }
    lista() {
        //this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idS}/${this.tipo}`, {headers:this.getHeaders()}).subscribe(response => {
        //    this.mats = response;
        //    this.filteredMats = response;
        //}, err => this.validaError(err));
        //this.http.get<Catalogo[]>(`${this.url}api/catalogo/getpuestobycot/${this.idC}`, { headers: this.getHeaders() }).subscribe(response => {
        //    this.pues = response;
        //}, err => this.validaError(err));
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getsucursalbycot/${this.idC}`, { headers: this.getHeaders() }).subscribe(response => {
            this.dirs = response;
        }, err => this.validaError(err));
        //this.http.get<ItemN[]>(`${this.url}api/catalogo/getfrecuencia`, { headers: this.getHeaders() }).subscribe(response => {
        //    this.fres = response;
        //}, err => this.validaError(err));
    }
    //filterMats(value: string) {
    //    const filterValue = (value && value.toLowerCase()) || '';
    //    this.filteredMats = this.mats.filter(m =>
    //        m.descripcion.toLowerCase().includes(filterValue) ||
    //        m.clave.toLowerCase().includes(filterValue)
    //    );
    //}


    //onProductoSelected(clave: string) {
    //    this.model.claveProducto = clave;
    //}


    onKeywordsProductoInput() {
        this.searchProductoKeyword$.next(this.productosfil.keywords);
    }

    GetAllProductosSinga() {
        let fil: string = (this.productosfil.keywords != '' ? 'keywords=' + this.productosfil.keywords : '');
        this.http.get<listacatalogoproductos>(`${this.url}api/catalogo/getproductobygrupofiltrado/${this.idS}/${this.tipo}/${this.productosfil.pagina}?${fil}`, { headers: this.getHeaders() }).subscribe(response => {
            this.productosfil = response;
        }, err => this.validaError(err));
    }

    prodPagina(event) {
        this.productosfil.pagina = event;
        this.GetAllProductosSinga();
    }
    nuevo(id: number) {
        this.edit = 0;
        let fec: Date = new Date();
        this.model = {
            idMaterialCotizacion: 0, claveProducto: '', idCotizacion: this.idC,
            idPuestoDireccionCotizacion: id, precioUnitario: 0, cantidad: 0, idFrecuencia: 1,
            total: 0, fechaAlta: fec.toISOString(), idDireccionCotizacion: this.idD, idPersonal: this.sinU.idPersonal, edit: this.edit, diasEvento: 0,descripcion: '', frecuencia: ''
        };
        this.lerr = {};
    }

    existe(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<Material>(`${this.url}api/${this.tipo}/getbyid/${id}`, { headers: this.getHeaders() }).subscribe(response => {
            this.model = response;
            this.model.edit = this.edit;
        }, err => this.validaError(err));
    }
    guarda() {
        //if (this.idServicioCotizacion == 4 || this.idServicioCotizacion == 5) {
        //    this.model.idFrecuencia = 1
        //}
        //this.quitarFocoDeElementos();
        //this.lerr = {};
        //this.model.idPersonal = this.sinU.idPersonal;
        //this.model.diasEvento = this.diasEvento;
        //if (this.valida()) {
        //    this.iniciarCarga();
        //    setTimeout(() => {
        //        this.http.post<Material>(`${this.url}api/${this.tipo}`, this.model, { headers: this.getHeaders() }).subscribe(response => {
        //            this.detenerCarga();
        //            setTimeout(() => {
        //                this.okToast(this.model.claveProducto + ' guardado');
        //            }, 300);
        //            this.close();
        //            if (this.model.idPuestoDireccionCotizacion != 0) {
        //                this.returnModal.emit(true);
        //            }
        //            else {
        //                this.returnModal.emit(false);
        //            }
        //        }, err => {
        //            this.detenerCarga();
        //            this.validaError(err);
        //            console.log(err);
        //            if (err.error) {
        //                if (err.error.errors) {
        //                    this.lerr = err.error.errors;
        //                }
        //            }
        //        });
        //    }, 300);
        //}
    }
    validaError(err: any) {
        if (err.status === 401) {
            this.errorToast('⚠️ No autorizado. Inicia sesión nuevamente.');
            localStorage.clear();
            localStorage.setItem('token', '');
            localStorage.setItem('usuario', '');
            this.rtr.navigate(['']);

        } else {
            this.errorToast('Ocurri\u00F3 un error');
        }
    }
    getHeaders() {
        const token = localStorage.getItem('token');
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        return headers;
    }
    open(idProducto: number, idCotizacion: number, idServicio: number, tipo: string) {
        //this.diasEvento = diasEvento;
        //this.idServicioCotizacion = idServicioCotizacion;
        //this.isExtra = false;
        //if (puesto != undefined && puesto != "") {
        //    this.puesto = puesto.charAt(0).toUpperCase() + puesto.slice(1).toLowerCase();
        //}
        //else {
        //    this.isExtra = true;
        //}
        //this.nombreSucursal = nombreSucursal;
        //this.lerr = {};
        //this.edit = edit;
        //this.idC = cot;
        //this.idD = dir;
        //this.idP = pue;
        //this.idProducto = id;
        //if (this.idP != 0) {
        //    this.hidedir = 1;
        //}
        //else {
        //    this.hidedir = 0;
        //}
        this.idS = 2;
        this.tipo = 'material';
        //this.showSuc = showS;
        //this.lista();
        //if (id == 0) {
        //    this.nuevo(this.idP);
        //} else {
        //    this.existe(id);
        //}
        if (idProducto != 0) {

        }
        else {
            this.GetAllProductosSinga();
        }
        let docModal = document.getElementById('materialaddsumco');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('materialaddsumco');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
        //if (this.model.idPuestoDireccionCotizacion != 0) {
        //    this.returnModal.emit(true);
        //}
        //else {
        //    this.returnModal.emit(false);
        //}
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