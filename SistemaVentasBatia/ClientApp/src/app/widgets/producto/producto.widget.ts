import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { MaterialPuesto } from 'src/app/models/materialpuesto';
import { StoreUser } from '../../stores/StoreUser';
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
declare var bootstrap: any;

@Component({
    selector: 'producto-widget',
    templateUrl: './producto.widget.html'
})
export class ProductoWidget implements OnChanges {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('saveEvent') sendEvent = new EventEmitter<boolean>();
    @Input() grupo: string = '';
    @Input() idP: number = 0;
    model: MaterialPuesto = {} as MaterialPuesto;
    lsmat: Catalogo[] = [];
    sers: ItemN[] = [];
    fres: ItemN[] = [];
    idSer: number = 2;
    validaciones: boolean = false;
    isLoading: boolean = false;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        http.get<ItemN[]>(`${url}api/catalogo/getfrecuencia`).subscribe(response => {
            this.fres = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    inicio(id: number, tipo: number, idPuesto: number) {
        this.lerr = {};
        if (id != 0) {
            this.existe(id, tipo, idPuesto);
        }
        else {
            this.model = {
                idMaterialPuesto: 0, claveProducto: '', idPuesto: this.idP,
                precio: 0, cantidad: 0, idFrecuencia: 0, idPersonal: this.sinU.idPersonal
            };
            this.idSer = 2;
            this.getProductos();
            this.open();
        }
    }

    existe(idProducto: number, tipo: number, idPuesto: number) {
        this.http.get<MaterialPuesto>(`${this.url}api/producto/obtenerproductodefault/${idProducto}/${tipo}/${idPuesto}`).subscribe(response => {
            this.model = response;
        }, err => console.log(err));
        this.open();
        this.getProductos();
    }

    guarda() {
        this.quitarFocoDeElementos();
        this.lerr = {};
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                if (this.model.idMaterialPuesto == 0) {
                    this.http.post<MaterialPuesto>(`${this.url}api/producto/post${this.grupo}`, this.model).subscribe(response => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast(this.grupo + ' agregado');
                        }, 300);
                        this.sendEvent.emit(true);
                        this.close();
                    }, err => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.errorToast('Ocurri\u00F3 un error');
                        }, 300);
                        console.log(err);
                        if (err.error) {
                            if (err.error.errors) {
                                this.lerr = err.error.errors;
                            }
                        }
                    });
                }
                if (this.model.idMaterialPuesto != 0) {
                    this.http.post<MaterialPuesto>(`${this.url}api/producto/post${this.grupo}`, this.model).subscribe(response => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast(this.grupo + ' actualizado');
                        }, 300);
                        this.sendEvent.emit(true);
                        this.close();
                    }, err => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.errorToast('Ocurri\u00F3 un error');
                        }, 300);
                        console.log(err);
                        if (err.error) {
                            if (err.error.errors) {
                                this.lerr = err.error.errors;
                            }
                        }
                    });
                }
            }, 300);
        }
    }

    getProductos() {
        this.lsmat = [];
        if (this.idSer > 0) {
            this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idSer}/${this.grupo}`).subscribe(response => {
                this.lsmat = response;
            }, err => console.log(err));
        }
    }

    chgServicio() {
        this.model.claveProducto = '';
        this.getProductos();
    }

    open() {
        this.lerr = {};
        let docModal = document.getElementById('modalAgregarProductoPuesto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalAgregarProductoPuesto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    valida() {
        this.validaciones = true;
        if (this.idSer == 0) {
            this.lerr['Servicio'] = ['Servicio es necesario'];
            this.validaciones = false;
        }
        if (this.model.claveProducto == '') {
            this.lerr['ClaveProducto'] = ['Clave de producto es necesario'];
            this.validaciones = false;
        }
        if (this.model.cantidad == null) {
            this.lerr['Cantidad'] = ['Cantidad es necesario'];
            this.validaciones = false;
        }
        if (this.model.cantidad <= 0) {
            this.lerr['Cantidad'] = ['Cantidad debe ser mayor que 0'];
            this.validaciones = false;
        }
        if (this.model.idFrecuencia == 0) {
            this.lerr['IdFrecuencia'] = ['Frecuencia es necesario'];
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

    ngOnChanges(changes: SimpleChanges): void {
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