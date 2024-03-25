import { Component, Inject, OnChanges, Input, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Prospecto } from 'src/app/models/prospecto';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from '../../stores/StoreUser';
import { Subject } from 'rxjs';

@Component({
    selector: 'pros-widget',
    templateUrl: './prospecto.widget.html',
    providers: [DatePipe]
})
export class ProspectoWidget implements OnChanges {
    @Input() idP: number = 0;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    model: Prospecto = {} as Prospecto;
    docs: ItemN[] = [];
    lerr: any = {};
    evenSub: Subject<void> = new Subject<void>();
    isErr: boolean = false;
    validaMess: string = '';

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe, private sinU: StoreUser) {
        http.get<ItemN[]>(`${url}api/prospecto/getdocumento`).subscribe(response => {
            this.docs = response;
        }, err => console.log(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.model = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '',
            representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [], idPersonal: this.sinU.idPersonal, idEstatusProspecto: 0, polizaCumplimiento: false
        };
        this.docs.forEach(d => d.act = false);
    }

    existe(id: number) {
        this.http.get<Prospecto>(`${this.url}api/prospecto/${id}`).subscribe(response => {
            this.model = response;
            this.docs = this.model.listaDocumentos;
        }, err => console.log(err));
    }

    guarda() {
        this.quitarFocoDeElementos();
        this.model.listaDocumentos = this.docs;
        this.lerr = {};
        if (this.valida()) {
            if (this.model.idProspecto == 0) {
                this.http.post<Prospecto>(`${this.url}api/prospecto`, this.model).subscribe(response => {
                    console.log(response);
                    this.sendEvent.emit(response.idProspecto);
                    this.isErr = false;
                    this.validaMess = 'Prospecto guardado';
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
            } else {
                this.http.put<Prospecto>(`${this.url}api/prospecto`, this.model).subscribe(response => {
                    console.log(response);
                    this.sendEvent.emit(response.idProspecto);
                    this.isErr = false;
                    this.validaMess = 'Prospecto actualizado';
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

    valida() {
        return true;
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
        if (this.idP == 0) {
            this.nuevo();
        } else {
            this.existe(this.idP);
        }
    }
    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}