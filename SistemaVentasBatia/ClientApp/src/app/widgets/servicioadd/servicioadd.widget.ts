import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ItemN } from 'src/app/models/item';
import { Servicio } from 'src/app/models/servicio';
import { StoreUser } from 'src/app/stores/StoreUser';
import { numberFormat } from 'highcharts';
declare var bootstrap: any;
import { Subject } from 'rxjs';

@Component({
    selector: 'servadd-widget',
    templateUrl: './servicioadd.widget.html'
})
export class ServicioAddWidget {
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idS: number = 0;
    showSuc: boolean = false;
    tipo: string = 'servicio';
    edit: number = 0;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    @Output('returnModal') returnModal = new EventEmitter<boolean>();
    model: Servicio = {} as Servicio;
    dirs: Catalogo[] = [];
    pues: Catalogo[] = [];
    mats: Catalogo[] = [];
    sers: Catalogo[] = [];
    fres: ItemN[] = [];
    lerr: any = {};
    evenSub: Subject<void> = new Subject<void>();
    isErr: boolean = false;
    validaMess: string = '';
    validaciones: boolean = false;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {}
        
    lista() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getservicio`).subscribe(response => {
            this.sers = response;
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
            idServicioExtraCotizacion: 0,
            idServicioExtra: 0,
            ServicioExtra: '',
            idCotizacion: this.idC,
            idDireccionCotizacion: this.idD,
            direccion: '',
            precioUnitario: 0,
            cantidad: 0,
            total: 0,
            importeMensual: 0,
            idFrecuencia: 0,
            fechaAlta: fec.toISOString(),
            idPersonal: this.sinU.idPersonal,
            edit: this.edit
        };
    }
        
    existe(id: number) {
        this.edit = 1;
        this.model.edit = this.edit;
        this.http.get<Servicio>(`${this.url}api/material/serviciogetbyid/${id}`).subscribe(response => {
            this.model = response;
            this.model.edit = this.edit;
        }, err => console.log(err));
    }
    guarda() {
        this.quitarFocoDeElementos();
        this.lerr = {};
        if (this.valida()) {
            //if (this.model.idDireccionCotizacion == 0) {
            //    this.model.idDireccionCotizacion = 0
            //}
            if (this.edit == 0) {
                this.http.post<Servicio>(`${this.url}api/material/insertarserviciocotizacion`, this.model).subscribe(response => {
                    this.close();
                    this.sendEvent.emit(2);
                    this.isErr = false;
                    this.validaMess = 'Guardado correctamente';
                    this.evenSub.next();
                }, err => {
                    console.log(err);
                    this.isErr = true;
                    this.validaMess = 'Ocurrio un error';
                    this.evenSub.next();
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }
            if (this.edit == 1) {
                this.http.post<Servicio>(`${this.url}api/material/actualizarserviciocotizacion`, this.model).subscribe(response => {

                    this.close();
                    this.sendEvent.emit(2);
                    console.log(response);
                    this.isErr = false;
                    this.validaMess = 'Servicio actualizado';
                    this.evenSub.next();
                }, err => {
                    console.log(err);
                    this.isErr = true;
                    this.validaMess = 'Ocurrio un error';
                    this.evenSub.next();
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }
        }
    }

    open(id: number, idCotizacion) {
        this.lerr = {};
        this.idC = idCotizacion;
        this.lista();
        if (id == 0) {
            this.nuevo(this.idP);
        } else {
            this.existe(id);
        }
        let docModal = document.getElementById('modalLimpiezaAgregarServicioCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    close() {
        let docModal = document.getElementById('modalLimpiezaAgregarServicioCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }
    ok() {
        this.isErr = false;
        this.validaMess = 'Actualizado correctamente';
        this.evenSub.next();
    }

    valida() {
        this.validaciones = true;
        //if (this.model.idDireccionCotizacion == 0) {
        //    this.lerr['IdDireccionCotizacion'] = ['Sucursal es necesaria'];
        //    this.validaciones = false;
        //}
        if (this.model.idServicioExtra == 0) {
            this.lerr['IdServicioExtra'] = ['Servicio es necesario'];
            this.validaciones = false;
        }
        if (this.model.idFrecuencia == 0) {
            this.lerr['IdFrecuencia'] = ['Frecuencia es necesaria'];
            this.validaciones = false;
        }
        if (this.model.precioUnitario == null) {
            this.lerr['PrecioUnitario'] = ['Costo es necesario'];
            this.validaciones = false;
        }
        if (this.model.precioUnitario <= 0) {
            this.lerr['PrecioUnitario'] = ['Costo debe ser mayor que 0'];
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
}