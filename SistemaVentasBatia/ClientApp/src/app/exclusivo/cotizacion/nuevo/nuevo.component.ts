import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Prospecto } from 'src/app/models/prospecto';
import { Cotizacion } from 'src/app/models/cotizacion';
import { ItemN } from 'src/app/models/item';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { Subject } from 'rxjs';
import { AgregarUsuario } from 'src/app/models/agregarusuario';
import { DatePipe } from '@angular/common';



declare var bootstrap: any;
import { fadeInOut } from 'src/app/fade-in-out';



@Component({
    selector: 'cot-nuevo',
    templateUrl: './nuevo.component.html',
    animations: [fadeInOut],
    providers: [DatePipe]
})
export class CotizaComponent {
    lpros: Prospecto[] = [];
    model: Cotizacion = {} as Cotizacion;
    modelp: Prospecto = {} as Prospecto;
    sers: ItemN[] = [];
    salt: ItemN[] = [];
    var1: boolean = false;
    var2: boolean = false;
    var3: boolean = false;
    var4: boolean = false;
    var5: boolean = false;
    validaciones: boolean = false;
    salTipo: number = 0;
    idVendedor: number = 0;
    listUsu: AgregarUsuario[];
    docs: ItemN[] = [];

    lerr: any = {};
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;


    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient, private dtpipe: DatePipe,
        private rtr: Router, public user: StoreUser
    ) {
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
        }),
        http.get<ItemN[]>(`${url}api/prospecto/getdocumento`).subscribe(response => {
            this.docs = response;
        }, err => console.log(err));
    }

    nuevo() {
        let fec: Date = new Date();
        this.model = {
            idCotizacion: 0, idProspecto: 0, idServicio: 0, total: 0,
            fechaAlta: fec.toISOString(), idCotizacionOriginal: 0,
            idPersonal: this.user.idPersonal, listaServicios: [], salTipo: 0, listaTipoSalarios: []
        };
        this.sers.forEach(s => s.act = false);
        this.salt.forEach(s => s.act = false);

        this.modelp = {
            idProspecto: 0, nombreComercial: '', razonSocial: '', rfc: '', domicilioFiscal: '',
            representanteLegal: '', telefono: '', fechaAlta: this.dtpipe.transform(fec, 'yyyy-MM-ddTHH:mm:ss'), nombreContacto: '',
            emailContacto: '', numeroContacto: '', extContacto: '', idCotizacion: 0, listaDocumentos: [], idPersonal: this.user.idPersonal,
            idEstatusProspecto: 0, polizaCumplimiento: false,poderRepresentanteLegal: '', actaConstitutiva: '', registroPatronal: '', empresaVenta: 0
        };
        this.docs.forEach(d => d.act = false);
        this.idVendedor = 0;
    }

    guarda() {
        if (this.user.idAutoriza == 0) {
            this.idVendedor = this.user.idPersonal;
        }
        this.quitarFocoDeElementos();
        this.lerr = {};
        this.model.listaServicios = this.sers;
        this.model.listaTipoSalarios = this.salt;
        if (this.valida1()) {
            this.model.idPersonal = this.idVendedor;
            if (this.model.idCotizacion == 0) {
                this.http.post<boolean>(`${this.url}api/cotizacion`, this.model).subscribe(response => {
                    console.log(response);
                    this.okToast('Cotizaci\u00F3n creada');
                    this.closeNew();
                    this.closeSel();
                    this.rtr.navigate(['/exclusivo/cotiza/' + this.model.idProspecto]);
                    
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

    }

    guardaP() {
        this.quitarFocoDeElementos();
        this.modelp.listaDocumentos = this.docs;
        this.modelp.idPersonal = this.idVendedor;
        this.lerr = {};
        if (this.user.idAutoriza == 0) {
            this.modelp.idPersonal = this.user.idPersonal;
            this.idVendedor = this.user.idPersonal;
        }
        if (this.valida2()) {
            if (this.modelp.idProspecto == 0) {
                this.http.post<Prospecto>(`${this.url}api/prospecto`, this.modelp).subscribe(response => {
                    console.log(response);
                    //this.sendEvent.emit(response.idProspecto);
                    this.model.idProspecto = response.idProspecto;
                    this.guarda();
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
            } else {
                this.http.put<Prospecto>(`${this.url}api/prospecto`, this.modelp).subscribe(response => {
                    console.log(response);
                    //this.sendEvent.emit(response.idProspecto);
                    this.model.idProspecto = response.idProspecto;
                    this.guarda();
                    this.okToast('Prospecto actualizado');
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

    valida1() {
        this.validaciones = true;
        let val: ItemN = this.sers.filter(x => x.act)[0];

        if (!val) {
            this.lerr['ListaServicio'] = ['Servicio es requerido'];
            this.validaciones = false;
        }
        let val2: ItemN = this.salt.filter(x => x.act)[0];
        if (!val2) {
            this.lerr['ListaTipoSalario'] = ['Tipo de salario es requerido'];
            this.validaciones = false;
        }
        if (this.model.idProspecto == 0 || this.model.idProspecto == null) {
            this.lerr['Prospecto'] = ['Prospecto es requerido'];
            this.validaciones = false;
        }
        if (this.idVendedor == 0 || this.idVendedor == null) {
            this.lerr['IdVendedor'] = ['Vendedor es requerido'];
            this.validaciones = false;
        }
        return this.validaciones;
    }

    valida2() {
        this.validaciones = true;
        let val: ItemN = this.sers.filter(x => x.act)[0];
        
        if (this.modelp.nombreComercial == '' || this.idVendedor == null) {
            this.lerr['NombreComercial'] = ['Nombre Comercial es requerido'];
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
        //if (this.modelp.extContacto == '' || this.modelp.extContacto == null) {
        //    this.lerr['ExtContacto'] = ['Extension es requerido'];
        //    this.validaciones = false;
        //}
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
        if (this.modelp.rfc.length > 13) {
            this.lerr['RfcLenght'] = ['El RFC no puede contener m\u00E1s de 13 caracteres'];
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

    openSel() {
        this.nuevo();
        this.lerr = {};
        let docModal = document.getElementById('modalSeleccionarProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    closeSel() {
        let docModal = document.getElementById('modalSeleccionarProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    openNew() {
        this.nuevo();
        this.lerr = {};
        let docModal = document.getElementById('modalCrearProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    closeNew() {
        let docModal = document.getElementById('modalCrearProspecto');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    savePros(event) {
        console.log(event);
        this.model.idProspecto = event;
        this.guarda();
        this.closeNew();
    }
    goBack() {
        window.history.back();
    }

    updateSelectedServicio(selectedServicio: any): void {
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