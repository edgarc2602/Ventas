import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
declare var bootstrap: any;
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { Catalogo } from '../../models/catalogo';
import { ItemN } from '../../models/item';
import { MaterialPuesto } from '../../models/materialpuesto';
import { each } from 'highcharts';

@Component({
    selector: 'productogeneral-widget',
    templateUrl: './productogeneral.widget.html'
})
export class ProductoGeneralWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('prodgEvent') sendEvent = new EventEmitter<number>();
    tipo: number = 0;
    idServicio: number = 0;

    validaciones: boolean = false;
    isLoading: boolean = false;
    lerr: any = {};

    producto: Catalogo[] = [];
    cantidad: number = 0;
    frecuencia: ItemN[] = [];

    material: MaterialPuesto[] = [];
    uniforme: MaterialPuesto[] = [];
    equipo: MaterialPuesto[] = [];
    herramienta: MaterialPuesto[] = [];
    productoSeleccionado: Catalogo | null = null;
    frecuenciaSeleccionada: ItemN | null = null;

    tipoProd: string = '';
    existe: boolean = false;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        http.get<ItemN[]>(`${url}api/catalogo/getfrecuencia`).subscribe(response => {
            this.frecuencia = response;
        }, err => console.log(err));

    }

    nuevo() {
        this.cantidad = 0;
        this.material = [];
        this.uniforme = [];
        this.equipo = [];
        this.herramienta = [];
        this.productoSeleccionado = null;
        this.frecuenciaSeleccionada = null;
    }

    guarda() {
        this.quitarFocoDeElementos();
        this.lerr = {};
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                //this.http.get<boolean>(`${this.url}api/producto/agregarindustria/${this.industria}/${this.sinU.idPersonal}`).subscribe(response => {
                //    this.detenerCarga();
                //    setTimeout(() => {
                //        this.okToast('Industria agregada');
                //    }, 300);
                //    this.close();
                //    this.industria = '';
                //    this.sendEvent.emit(4);
                //}, err => {
                //    this.detenerCarga();
                //    setTimeout(() => {
                //        this.errorToast('Ocurri\u00F3 un error');
                //    }, 300);
                //    if (err.error) {
                //        if (err.error.errors) {
                //            this.lerr = err.error.errors;
                //        }
                //    }
                //});
            }, 300);
        }
    }

    agregar(tipo: string) {
        this.existe = false;
        const producto: MaterialPuesto = {
            idMaterialPuesto: 0,
            claveProducto: this.productoSeleccionado.clave,
            descripcion: this.productoSeleccionado.descripcion,
            idPuesto: 0,
            precio: 0,
            cantidad: this.cantidad,
            idFrecuencia: this.frecuenciaSeleccionada.id,
            frecuencia: this.frecuenciaSeleccionada.nom,
            idPersonal: 0
        };
        switch (tipo) {
            case "material":
                this.material.forEach(pro => {
                    if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                        this.existe = true;
                    }
                });
                if (this.existe) {
                    this.errorToast("Este material ya esta fue agregado");
                    break;
                }
                else {
                    this.material.push(producto);
                }
                break;
            case "uniforme":
                this.uniforme.forEach(pro => {
                    if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                        this.existe = true;
                    }
                });
                if (this.existe) {
                    this.errorToast("Este material ya esta fue agregado");
                    break;
                }
                else {
                    this.uniforme.push(producto);
                }
                break;
            case "equipo":
                this.equipo.forEach(pro => {
                    if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                        this.existe = true;
                    }
                });
                if (this.existe) {
                    this.errorToast("Este material ya esta fue agregado");
                    break;
                }
                else {
                    this.equipo.push(producto);
                }
                break;
            case "herramienta":
                this.herramienta.forEach(pro => {
                    if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                        this.existe = true;
                    }
                });
                if (this.existe) {
                    this.errorToast("Este material ya esta fue agregado");
                    break;
                }
                else {
                    this.herramienta.push(producto);
                }
                break;

        }
    }
    elimina(clave: string, descripcion: string, cantidad: number, frecuencia: string, tipo: string) {
        switch (tipo) {
            case "material":
                this.material = this.material.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion ||
                    producto.cantidad !== cantidad || producto.frecuencia !== frecuencia
                );
                break;
            case "uniforme":
                this.uniforme = this.uniforme.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion ||
                    producto.cantidad !== cantidad || producto.frecuencia !== frecuencia
                );
                break;
            case "equipo":
                this.equipo = this.equipo.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion ||
                    producto.cantidad !== cantidad || producto.frecuencia !== frecuencia
                );
                break;
            case "herramienta":
                this.herramienta = this.herramienta.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion ||
                    producto.cantidad !== cantidad || producto.frecuencia !== frecuencia
                );
                break;
        }
    }

    open(tipo: number, idServicio: number) {
        this.nuevo();
        this.tipo = tipo;
        this.idServicio = idServicio;
        let docModal = document.getElementById('modalLimpiezaProductoGeneral');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
        this.get("material");
    }

    get(tipo: string) {
        this.tipoProd = tipo;
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idServicio}/${tipo}`).subscribe(response => {
            this.producto = response;
        }, err => console.log(err));
    }

    close() {
        let docModal = document.getElementById('modalLimpiezaProductoGeneral');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    valida() {
        this.validaciones = true;
        //if (this.industria == '') {
        //    this.lerr['Industria'] = ['Ingrese un tipo de industria'];
        //    this.validaciones = false;
        //}
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