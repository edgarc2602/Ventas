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
import Swal from 'sweetalert2';
import { error } from 'protractor';
import { Material } from '../../models/material';

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

    material: Material[] = [];
    uniforme: Material[] = [];
    equipo: Material[] = [];
    herramienta: Material[] = [];
    productoSeleccionado: Catalogo | null = null;
    frecuenciaSeleccionada: ItemN | null = null;

    tipoProd: string = '';
    existe: boolean = false;
    idCotizacion: number = 0;

    cantidadElimina: number = 0;
    materialElimina: Material[] = [];
    uniformeElimina: Material[] = [];
    equipoElimina: Material[] = [];
    herramientaElimina: Material[] = [];
    productoSeleccionadoElimina: Catalogo | null = null;
    frecuenciaSeleccionadaElimina: ItemN | null = null;



    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        http.get<ItemN[]>(`${url}api/catalogo/getfrecuencia`).subscribe(response => {
            this.frecuencia = response;
        }, err => console.log(err));

    }

    open(tipo: number, idServicio: number, idCotizacion: number) {
        this.nuevo();
        this.nuevoElimina();
        this.tipo = tipo;
        this.idServicio = idServicio;
        this.idCotizacion = idCotizacion;

        if (tipo == 0) {
            this.setTipo("material");
        }
        else {
            this.setTipoElimina("material");
        }
        let docModal = document.getElementById('modalLimpiezaProductoGeneral');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();

    }

    nuevo() {
        this.lerr = {};
        this.material = [];
        this.uniforme = [];
        this.equipo = [];
        this.herramienta = [];
        this.cantidad = 0;
        this.productoSeleccionado = null;
        this.frecuenciaSeleccionada = null;
        this.idCotizacion = 0;
    }

    nuevoElimina() {
        this.lerr = {};
        this.materialElimina = [];
        this.uniformeElimina = [];
        this.equipoElimina = [];
        this.herramientaElimina = [];
        this.cantidadElimina = 0;
        this.productoSeleccionadoElimina = null;
        this.frecuenciaSeleccionadaElimina = null;
        this.idCotizacion = 0;
    }

    setTipo(tipo: string) {
        this.tipoProd = tipo;
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupo/${this.idServicio}/${tipo}`).subscribe(response => {
            this.producto = response;
        }, err => console.log(err));
    }

    setTipoElimina(tipo: string) {
        this.tipoProd = tipo;
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getproductobygrupoElimina/${tipo}/${this.idCotizacion}`).subscribe(response => {
            this.producto = response;
        }, err => console.log(err));
    }

    agregar(tipo: string) {
        this.lerr = {};
        if (this.valida()) {
            this.existe = false;
            let fec: Date = new Date();

            const producto: Material = {
                idMaterialCotizacion: 0,
                claveProducto: this.productoSeleccionado.clave,
                idCotizacion: this.idCotizacion,
                idDireccionCotizacion: 1,
                idPuestoDireccionCotizacion: 0,
                precioUnitario: 0,
                cantidad: this.cantidad,
                idFrecuencia: this.frecuenciaSeleccionada.id,
                total: 0,
                fechaAlta: fec.toISOString(),
                idPersonal: this.sinU.idPersonal,
                edit: 0,
                diasEvento: 0,
                descripcion: this.productoSeleccionado.descripcion,
                frecuencia: this.frecuenciaSeleccionada.nom

            };
            this.productoSeleccionado = null;
            this.okToast("Agregado a la selecci\u00F3n");
            switch (tipo) {
                case "material":
                    this.material.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.material.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
                case "uniforme":
                    this.uniforme.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.uniforme.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
                case "equipo":
                    this.equipo.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.equipo.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
                case "herramienta":
                    this.herramienta.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.herramienta.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
            }
        }

    }

    agregarElimina(tipo: string) {
        this.lerr = {};
        if (this.validaElimina()) {
            this.existe = false;
            let fec: Date = new Date();

            const producto: Material = {
                idMaterialCotizacion: 0,
                claveProducto: this.productoSeleccionadoElimina.clave,
                idCotizacion: this.idCotizacion,
                idDireccionCotizacion: 1,
                idPuestoDireccionCotizacion: 0,
                precioUnitario: 0,
                cantidad: 1,
                idFrecuencia: 1,
                total: 0,
                fechaAlta: fec.toISOString(),
                idPersonal: this.sinU.idPersonal,
                edit: 0,
                diasEvento: 0,
                descripcion: this.productoSeleccionadoElimina.descripcion,
                frecuencia: ''

            };
            this.productoSeleccionadoElimina = null;
            switch (tipo) {
                case "material":
                    this.materialElimina.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.materialElimina.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
                case "uniforme":
                    this.uniformeElimina.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.uniformeElimina.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
                case "equipo":
                    this.equipoElimina.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.equipoElimina.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
                case "herramienta":
                    this.herramientaElimina.forEach(pro => {
                        if (pro.claveProducto == producto.claveProducto && pro.cantidad == producto.cantidad && pro.idFrecuencia == producto.idFrecuencia) {
                            this.existe = true;
                        }
                    });
                    if (this.existe) {
                        this.errorToast("Este producto ya est\u00E1 seleccionado");
                        break;
                    }
                    else {
                        this.herramientaElimina.push(producto);
                        this.okToast("Agregado a la selecci\u00F3n");
                    }
                    break;
            }
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

    eliminaElimina(clave: string, descripcion: string, cantidad: number, frecuencia: string, tipo: string) {
        switch (tipo) {
            case "material":
                this.materialElimina = this.materialElimina.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion
                );
                break;
            case "uniforme":
                this.uniformeElimina = this.uniformeElimina.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion
                );
                break;
            case "equipo":
                this.equipoElimina = this.equipoElimina.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion
                );
                break;
            case "herramienta":
                this.herramientaElimina = this.herramientaElimina.filter(producto =>
                    producto.claveProducto !== clave || producto.descripcion !== descripcion
                );
                break;
        }
    }

    guarda() {
        if (this.material.length == 0 && this.uniforme.length == 0 && this.equipo.length == 0 && this.herramienta.length == 0) {
            this.errorToast("No hay productos seleccionados");
        }
        else {
            Swal.fire({
                title: "Cargar productos",
                text: "Estos productos se cargar\u00E1n en todas las plantillas de su cotizaci\u00F3n, \u00BFDesea continuar?",
                icon: 'info',
                showCancelButton: true,
                confirmButtonText: 'S\u00ED',
                cancelButtonText: 'No',
                customClass: {
                    popup: 'custom-swal-width'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    this.quitarFocoDeElementos();
                    this.lerr = {};
                    this.iniciarCarga();
                    const data = {
                        material: this.material,
                        uniforme: this.uniforme,
                        equipo: this.equipo,
                        herramienta: this.herramienta,
                        idCotizacion: this.idCotizacion
                    };
                    setTimeout(() => {
                        this.http.post<boolean>(`${this.url}api/producto/AgregarProductosGeneral`, data).subscribe(response => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.okToast('Productos agregados');
                            }, 300);
                            this.nuevo();
                            this.close();
                            this.tipoProd = '';
                            this.sendEvent.emit(4);
                        }, err => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.errorToast('Ocurri\u00F3 un error');
                            }, 300);
                            if (err.error) {
                                if (err.error.errors) {
                                    this.lerr = err.error.errors;
                                }
                            }
                        });
                    }, 300);

                } else if (result.dismiss === Swal.DismissReason.cancel) {

                }
            });
        }
    }

    guardaElimina() {
        if (this.materialElimina.length == 0 && this.uniformeElimina.length == 0 && this.equipoElimina.length == 0 && this.herramientaElimina.length == 0) {
            this.errorToast("No hay productos seleccionados");
        }
        else {
            Swal.fire({
                title: "Eliminar productos",
                text: "Estos productos se eliminar\u00E1n de todas las plantillas de su cotizaci\u00F3n \u00BFDesea continuar?",
                icon: 'info',
                showCancelButton: true,
                confirmButtonText: 'S\u00ED',
                cancelButtonText: 'No',
                customClass: {
                    popup: 'custom-swal-width'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    this.quitarFocoDeElementos();
                    this.lerr = {};
                    this.iniciarCarga();
                    const data = {
                        material: this.materialElimina,
                        uniforme: this.uniformeElimina,
                        equipo: this.equipoElimina,
                        herramienta: this.herramientaElimina,
                        idCotizacion: this.idCotizacion
                    };
                    setTimeout(() => {
                        this.http.post<boolean>(`${this.url}api/producto/EliminarProductosGeneral`, data).subscribe(response => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.okToast('Productos eliminados');
                            }, 300);
                            this.nuevo();
                            this.close();
                            this.tipoProd = '';
                            this.sendEvent.emit(4);
                        }, err => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.errorToast('Ocurri\u00F3 un error');
                            }, 300);
                            if (err.error) {
                                if (err.error.errors) {
                                    this.lerr = err.error.errors;
                                }
                            }
                        });
                    }, 300);

                } else if (result.dismiss === Swal.DismissReason.cancel) {

                }
            });
        }
    }

    valida() {
        this.validaciones = true;
        if (this.productoSeleccionado == null) {
            this.lerr['Producto'] = ['Seleccione un producto'];
            this.validaciones = false;
        }
        if (this.cantidad == 0) {
            this.lerr['Cantidad'] = ['Ingrese una cantidad'];
            this.validaciones = false;
        }
        if (this.frecuenciaSeleccionada == null) {
            this.lerr['Frecuencia'] = ['Seleccione una frecuencia'];
            this.validaciones = false;
        }
        return this.validaciones;
    }

    validaElimina() {
        this.validaciones = true;
        if (this.productoSeleccionadoElimina == null) {
            this.lerr['ProductoElimina'] = ['Seleccione un producto'];
            this.validaciones = false;
        }
        return this.validaciones;
    }

    close() {
        if (this.material.length == 0 && this.uniforme.length == 0 && this.equipo.length == 0 && this.herramienta.length == 0) {
            let docModal = document.getElementById('modalLimpiezaProductoGeneral');
            let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
            myModal.hide();
            const firstTabEl = document.getElementById('tab-material');
            if (firstTabEl) {
                firstTabEl.click();
            }
        }
        else {
            Swal.fire({
                title: "Ha seleccionado uno o mas productos",
                text: "\u00BFDesea salir de todas maneras?",
                icon: 'info',
                showCancelButton: true,
                confirmButtonText: 'S\u00ED',
                cancelButtonText: 'No',
                customClass: {
                    popup: 'custom-swal-width'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    let docModal = document.getElementById('modalLimpiezaProductoGeneral');
                    let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
                    myModal.hide();
                    const firstTabEl = document.getElementById('tab-material');
                    if (firstTabEl) {
                        firstTabEl.click();
                    }
                } else if (result.dismiss === Swal.DismissReason.cancel) {

                }
            });

        }

    }

    closeElimina() {
        if (this.materialElimina.length == 0 && this.uniformeElimina.length == 0 && this.equipoElimina.length == 0 && this.herramientaElimina.length == 0) {
            let docModal = document.getElementById('modalLimpiezaProductoGeneral');
            let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
            myModal.hide();
            const firstTabEl = document.getElementById('tab-material');
            if (firstTabEl) {
                firstTabEl.click();
            }
        }
        else {
            Swal.fire({
                title: "Ha seleccionado uno o mas productos",
                text: "\u00BFDesea salir de todas maneras?",
                icon: 'info',
                showCancelButton: true,
                confirmButtonText: 'S\u00ED',
                cancelButtonText: 'No',
                customClass: {
                    popup: 'custom-swal-width'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    let docModal = document.getElementById('modalLimpiezaProductoGeneral');
                    let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
                    myModal.hide();
                    const firstTabEl = document.getElementById('tab-material');
                    if (firstTabEl) {
                        firstTabEl.click();
                    }
                } else if (result.dismiss === Swal.DismissReason.cancel) {

                }
            });

        }

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