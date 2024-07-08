import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Prospecto } from 'src/app/models/prospecto';
import { Cotizacion } from 'src/app/models/cotizacion';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { AgregarUsuario } from 'src/app/models/agregarusuario';
import { DatePipe } from '@angular/common';
import { fadeInOut } from 'src/app/fade-in-out';
import { Catalogo } from '../../../models/catalogo';
declare var bootstrap: any;


@Component({
    selector: 'cot-nuevo',
    templateUrl: './nuevo.component.html',
    animations: [fadeInOut],
    providers: [DatePipe]
})
export class CotizaComponent {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    model: Cotizacion = {} as Cotizacion;
    modelp: Prospecto = {} as Prospecto;
    lerr: any = {};
    lpros: Prospecto[] = [];
    listUsu: AgregarUsuario[];
    validaciones: boolean = false;
    var1: boolean = false;
    var2: boolean = false;
    var3: boolean = false;
    var4: boolean = false;
    var5: boolean = false;
    sers: ItemN[] = [];
    salt: ItemN[] = [];
    docs: ItemN[] = [];
    agregarTipo: number = 0;
    idVendedor: number = 0;
    salTipo: number = 0;
    isEvento: boolean = false;
    isInsumo: boolean = false;
    indust: Catalogo[] = [];

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe, private rtr: Router, public user: StoreUser) {
        this.nuevo();
        http.post<Prospecto[]>(`${url}api/prospecto/getcatalogo`, this.user.idPersonal).subscribe(response => {
            this.lpros = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getsalariotipo`).subscribe(response => {
            this.salt = response;
        }, err => console.log(err));
        http.get<AgregarUsuario[]>(`${url}api/usuario/obtenervendedores`).subscribe(response => {
            this.listUsu = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getdocumento`).subscribe(response => {
            this.docs = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/ObtenerCatalogoTiposdeIndustria`).subscribe(response => {
            this.indust = response;
        }, err => console.log(err));
    }

    nuevo() {
        this.isEvento = false;
        let fec: Date = new Date();
        this.model = {
            idCotizacion: 0, idProspecto: 0, idServicio: 0, total: 0,
            fechaAlta: fec.toISOString(), idCotizacionOriginal: 0,
            idPersonal: this.user.idPersonal, listaServicios: [], salTipo: 0, listaTipoSalarios: [], polizaCumplimiento: false, diasVigencia: 0, diasEvento: 0
        };
        this.sers.forEach(s => s.act = false);
        //this.salt.forEach(s => s.act = false);
        this.modelp = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '',
            representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [], idPersonal: this.user.idPersonal,
            idEstatusProspecto: 0, idTipoIndustria: 0
        };
        this.docs.forEach(d => d.act = false);
        if (this.user.idAutoriza == 0) {
            this.idVendedor = this.user.idPersonal;
        }
        else {
            this.idVendedor = 0;
        }
        
    }
    guarda() {
        this.lerr = {};
        if (this.valida()) {
            if (this.agregarTipo == 1) {
                this.guardaC();
            }
            else if (this.agregarTipo == 2) {
                this.guardaP();
            }
        }
    }

    guardaC() {
        this.quitarFocoDeElementos();
        this.model.listaServicios = this.sers;
        this.model.listaTipoSalarios = this.salt;
        this.model.idPersonal = this.idVendedor;
        //if (this.isInsumo == true) {
        //    this.model.salTipo = 1
        //}
        if (this.model.idCotizacion == 0) {
            this.http.post<boolean>(`${this.url}api/cotizacion`, this.model).subscribe(response => {
                console.log(response);
                this.closeNew(0);
                this.rtr.navigate(['/exclusivo/cotiza/' + this.model.idProspecto]);
                this.okToast('Cotizaci\u00F3n creada');

            }, err => {
                console.log(err);
                this.errorToast('Ocurri\u00F3 un error');
                if (err.error) {
                    if (err.error.errors) {
                        this.lerr = err.error.errors;
                    }
                }
            });
        }
    }

    guardaP() {
        this.quitarFocoDeElementos();
        this.modelp.listaDocumentos = this.docs;
        this.modelp.idPersonal = this.idVendedor;
        if (this.modelp.idProspecto == 0) {
            this.http.post<Prospecto>(`${this.url}api/prospecto`, this.modelp).subscribe(response => {
                console.log(response);
                this.model.idProspecto = response.idProspecto;
                this.guardaC();
                this.okToast('Prospecto guardado');
            }, err => {
                console.log(err);
                this.errorToast('Ocurri\u00F3 un error');
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
            });
        }
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

    valida() {
        this.lerr = {};
        this.validaciones = true;
        let val: ItemN = this.sers.filter(x => x.act)[0];
        if (this.agregarTipo == 2) {
            if (this.modelp.nombreComercial == '' || this.modelp.nombreComercial == null) {
                this.lerr['NombreComercial'] = ['Nombre Comercial es requerido'];
                this.validaciones = false;
            }
            if (this.modelp.rfc.length > 13) {
                this.lerr['RfcLenght'] = ['El RFC no puede contener m\u00E1s de 13 caracteres'];
                this.validaciones = false;
            }
            if (this.modelp.nombreContacto == '' || this.modelp.nombreContacto == null) {
                this.lerr['NombreContacto'] = ['Contacto es requerido'];
                this.validaciones = false;
            }
            if (this.modelp.emailContacto == '' || this.modelp.emailContacto == null) {
                this.lerr['EmailContacto'] = ['Email es requerido'];
                this.validaciones = false;
            }
            if (this.modelp.numeroContacto == '' || this.modelp.numeroContacto == null) {
                this.lerr['NumeroContacto'] = ['Tel. Contacto es requerido'];
                this.validaciones = false;
            }
            if (this.modelp.idTipoIndustria == 0 || this.modelp.idTipoIndustria == null) {
                this.lerr['IdTipoIndustria'] = ['Tipo de industria es requerido'];
                this.validaciones = false;
            }
            if (this.isEvento == true) {
                if (this.model.diasEvento == 0 || this.model.diasEvento == null) {
                    this.lerr['DiasEvento'] = ['Duraci\u00F3n del evento es requerido'];
                    this.validaciones = false;
                }
            }
        }
        else {
            if (this.model.idProspecto == 0 || this.model.idProspecto == null) {
                this.lerr['Prospecto'] = ['Prospecto es requerido'];
                this.validaciones = false;
            }
            if (this.isEvento == true) {
                if (this.model.diasEvento == 0 || this.model.diasEvento == null) {
                    this.lerr['DiasEvento'] = ['Duraci\u00F3n del evento es requerido'];
                    this.validaciones = false;
                }
            }
        }
        if (!val) {
            this.lerr['ListaServicio'] = ['Servicio es requerido'];
            this.validaciones = false;
        }
        let val2: ItemN = this.salt.filter(x => x.act)[0];
        if (!val2) {
            this.lerr['ListaTipoSalario'] = ['Tipo de salario es requerido'];
            this.validaciones = false;
        }
        if (this.idVendedor == 0 || this.idVendedor == null) {
            this.lerr['IdVendedor'] = ['Vendedor es requerido'];
            this.validaciones = false;
        }
        if (this.model.diasVigencia == 0 || this.model.diasVigencia == null) {
            this.lerr['diasVigencia'] = ['vigencia es requerida'];
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

    openNew(tipo: number) {
        this.agregarTipo = tipo;
        this.nuevo();
        this.lerr = {};
        let docModal = document.getElementById('modalAgregarCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    closeNew(tipo: number) {
        this.agregarTipo = 0;
        let docModal = document.getElementById('modalAgregarCotizacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
        this.agregarTipo == 0;
    }

    savePros(event) {
        console.log(event);
        this.model.idProspecto = event;
        this.guarda();
        this.closeNew(0);
    }

    goBack() {
        window.history.back();
    }

    updateSelectedServicio(selectedServicio: any): void {
        if (selectedServicio.id == 5) {
            this.isEvento = true;
        }
        else {
            this.isEvento = false;
        }
        if (selectedServicio.id == 4) {
            this.isInsumo = true;
        }
        else {
            this.isInsumo = false;
        }
        this.sers.forEach(s => s.act = false);
        selectedServicio.act = true;
    }

    updateSelectedSalario(selectedSalario: any): void {
        this.salt.forEach(s => s.act = false);
        selectedSalario.act = true;
    }

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}