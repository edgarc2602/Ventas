import { Component, Inject, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Prospecto } from '../../../models/prospecto';
import { ItemN } from '../../../models/item';
import { ListaDireccion } from 'src/app/models/listadireccion';
import { DireccionWidget } from 'src/app/widgets/direccion/direccion.widget';
import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { Catalogo } from '../../../models/catalogo';

@Component({
    selector: 'pros-nuevo',
    templateUrl: './nuevo.component.html',
    providers: [DatePipe],
    animations: [fadeInOut],
})
export class ProsNuevoComponent implements OnInit, OnDestroy {
    @ViewChild(DireccionWidget, { static: false }) dirAdd: DireccionWidget;
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    direcs: ListaDireccion = {
        idProspecto: 0, idCotizacion: 0, idDireccion: 0, pagina: 0, direcciones: [], rows: 0, numPaginas: 0
    };
    pro: Prospecto = {} as Prospecto;
    idDirecc: number = 0;
    isLoading: boolean = false;
    docs: ItemN[] = [];
    lerr: any = {};
    sub: any;
    indust: Catalogo[] = [];
    validacion: boolean = false;


    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe,
        private router: Router, private route: ActivatedRoute, private sinU: StoreUser
    ) {
        http.get<ItemN[]>(`${url}api/prospecto/getdocumento`).subscribe(response => {
            this.docs = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/ObtenerCatalogoTiposdeIndustria`).subscribe(response => {
            this.indust = response;
        }, err => console.log(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.pro = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '', representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [], idPersonal: this.sinU.idPersonal, idEstatusProspecto: 0, idTipoIndustria : 0
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
        this.quitarFocoDeElementos();
        this.pro.listaDocumentos = this.docs;
        this.lerr = {};
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                if (this.pro.idProspecto == 0) {
                    this.http.post<Prospecto>(`${this.url}api/prospecto`, this.pro).subscribe(response => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast('Prospecto creado');
                        }, 300);
                        this.pro.idProspecto = response.idProspecto;
                        setTimeout(() => {
                            this.router.navigate(['/exclusivo/prospecto']);
                        }, 1000);
                    }, err => {
                        this.detenerCarga();
                        console.log(err);
                        if (err.error) {
                            if (err.error.errors) {
                                this.lerr = err.error.errors;
                            } else if (err.error) {
                                for (let key in err.error) {
                                    if (err.error.hasOwnProperty(key)) {
                                        this.toastWidget.errMessage += key + ": " + err.error[key] + "\n";
                                    }
                                }
                                setTimeout(() => {
                                    this.errorToast();
                                }, 300);
                            }
                        }
                    });
                } else {
                    this.http.put<Prospecto>(`${this.url}api/prospecto`, this.pro).subscribe(response => {
                        this.detenerCarga();
                        setTimeout(() => {
                            this.okToast('Prospecto actualizado');
                        }, 300);
                        setTimeout(() => {
                            this.router.navigate(['/exclusivo/prospecto']);
                        }, 1000);
                    }, err => {
                        this.detenerCarga();
                        console.log(err);
                        if (err.error) {
                            if (err.error.errors) {
                                this.lerr = err.error.errors;
                            } else if (err.error) {
                                for (let key in err.error) {
                                    if (err.error.hasOwnProperty(key)) {
                                        this.toastWidget.errMessage += key + ": " + err.error[key] + "\n";
                                    }
                                }
                                setTimeout(() => {
                                    this.errorToast();
                                }, 300);
                            }
                        }
                    });
                }
            }, 300);
        }
    }

    okToast(message: string) {
        this.toastWidget.errMessage = message;
        this.toastWidget.isErr = false;
        this.toastWidget.open();
    }

    errorToast() {
        this.toastWidget.isErr = true;
        this.toastWidget.open();
    }

    valida() {
        this.validacion = true;
        if (this.pro.nombreComercial == '') {
            this.lerr['NombreComercial'] = ['Nombre comercial es obligatorio.'];
            this.validacion = false;
        }
        if (this.pro.domicilioFiscal == '') {
            this.lerr['DomicilioFiscal'] = ['Domicilio Fiscal es obligatorio.'];
            this.validacion = false;
        }
        if (this.pro.nombreContacto == '') {
            this.lerr['NombreContacto'] = ['Contacto es obligatorio.'];
            this.validacion = false;
        }
        if (this.pro.emailContacto == '') {
            this.lerr['EmailContacto'] = ['Email es obligatorio.'];
            this.validacion = false;
        }
        if (this.pro.numeroContacto == '') {
            this.lerr['NumeroContacto'] = ['Numero de contacto es obligatorio.'];
            this.validacion = false;
        }
        if (this.pro.idTipoIndustria == 0) {
            this.lerr['IdTipoIndustria'] = ['Tipo de industria es obligatorio.'];
            this.validacion = false;
        }

        return this.validacion;
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

    iniciarCarga() {
        this.isLoading = true;
        this.cargaWidget.open(true);
    }

    detenerCarga() {
        this.isLoading = false;
        this.cargaWidget.open(false);
    }
}