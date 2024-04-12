import { Component, Inject, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { Prospecto } from '../../../models/prospecto';
import { ItemN } from '../../../models/item';
import { ListaDireccion } from 'src/app/models/listadireccion';
import { DireccionWidget } from 'src/app/widgets/direccion/direccion.widget';
import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';
import { ProspectoComponent } from 'src/app/exclusivo/prospecto/prospecto.component';


@Component({
    selector: 'pros-nuevo',
    templateUrl: './nuevo.component.html',
    providers: [DatePipe],
    animations: [fadeInOut],
})
export class ProsNuevoComponent implements OnInit, OnDestroy {
    @ViewChild(DireccionWidget, { static: false }) dirAdd: DireccionWidget;
    pro: Prospecto = {} as Prospecto;
    sub: any;
    docs: ItemN[] = [];
    direcs: ListaDireccion = {
        idProspecto: 0, idCotizacion: 0, idDireccion: 0, pagina: 0, direcciones: [], rows: 0, numPaginas: 0
    };
    idDirecc: number = 0;
    lerr: any = {};
    evenSub: Subject<void> = new Subject<void>();
    isErr: boolean = false;
    errMessage: string = '';
    isLoading: boolean = false;
 
    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe,
        private router: Router, private route: ActivatedRoute, private sinU: StoreUser
    ) {
        http.get<ItemN[]>(`${url}api/prospecto/getdocumento`).subscribe(response => {
            this.docs = response;
        }, err => console.log(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.pro = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '',
            representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [],
            idPersonal: this.sinU.idPersonal, idEstatusProspecto: 0, polizaCumplimiento: false
        };
    }

    existe(id: number) {
        this.http.get<Prospecto>(`${this.url}api/prospecto/${id}`).subscribe(response => {
            this.pro = response;
            this.docs = this.pro.listaDocumentos;
            this.getDir();
        }, err => console.log(err));
    }

    getDir() {
        this.http.get<ListaDireccion>(`${this.url}api/direccion/${this.pro.idProspecto}`).subscribe(response => {
            this.direcs = response;
        }, err => console.log(err));
    }

    selDir(idD: number) {
        this.idDirecc = idD;
        this.dirAdd.open(this.pro.idProspecto, idD);
    }

    guarda() {
        this.errMessage = ''
        this.quitarFocoDeElementos();
        this.pro.listaDocumentos = this.docs;
        this.lerr = {};
        if (this.valida()) {
            if (this.pro.idProspecto == 0) {
                this.http.post<Prospecto>(`${this.url}api/prospecto`, this.pro).subscribe(response => {
                    this.pro.idProspecto = response.idProspecto;
                    console.log(response);
                    this.isErr = false;
                    this.errMessage = 'Creado';
                    this.evenSub.next();
                    setTimeout(() => {
                        this.router.navigate(['/exclusivo/prospecto']);
                    }, 2000);
                }, err => {
                    this.isLoading = false;
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        } else if (err.error) {
                            this.isErr = true;
                            for (let key in err.error) {
                                if (err.error.hasOwnProperty(key)) {
                                    this.errMessage += key + ": " + err.error[key] + "\n";
                                }
                            }
                            this.evenSub.next();
                        }
                    }
                });
            } else {
                this.http.put<Prospecto>(`${this.url}api/prospecto`, this.pro).subscribe(response => {
                    console.log(response);
                    this.isErr = false;
                    this.errMessage = 'Actualizado';
                    this.evenSub.next();
                    setTimeout(() => {
                        this.router.navigate(['/exclusivo/prospecto']);
                    }, 2000);
                }, err => {
                    this.isLoading = false;
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        } else if (err.error) {
                            this.isErr = true;
                            for (let key in err.error) {
                                if (err.error.hasOwnProperty(key)) {
                                    this.errMessage += key + ": " + err.error[key] + "\n";
                                }
                            }
                            this.evenSub.next();
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

    //recEvent($event) {
    //    this.selDir($event);
    //    this.getDir();
    //}

    ngOnInit(): void {
        this.sub = this.route.params.subscribe(params => {
            let idpro: number = +params['id'];
            if (idpro) {
                if (idpro == 0)
                    this.nuevo();
                else
                    this.existe(+params['id']);
            } else
            this.nuevo();
        });
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }
    goBack() {
        window.history.back();
    }
    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}