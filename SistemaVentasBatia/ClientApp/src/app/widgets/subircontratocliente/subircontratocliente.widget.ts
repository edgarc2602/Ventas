import { Component, Inject, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { Router } from '@angular/router';
declare var bootstrap: any;

@Component({
    selector: 'subircontratocliente-widget',
    templateUrl: './subircontratocliente.widget.html'
})
export class SubirContratoClienteWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @ViewChild('fileInput', { static: false }) fileInput!: ElementRef;
    @Output('subirContratoEvent') sendEvent = new EventEmitter<number>();
    isLoading: boolean = false;
    validaciones: boolean = false;
    lerr: any = {};
    selectedFile: File | null = null;
    idAsuntoLegal: number = 0;
    idCliente: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser, private rtr: Router) { }

    open(idCliente: number, idAsuntoLegal) {
        this.idCliente = idCliente;
        this.idAsuntoLegal = idAsuntoLegal;
        this.nuevo();
        let docModal = document.getElementById('modalSubirContratoCliente');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    nuevo() {
        this.lerr = {};
        this.selectedFile = null;
        this.limpiarArchivoSeleccionado();
    }

    guarda() {
        if (this.valida()) {
            this.iniciarCarga();
            setTimeout(() => {
                this.http.get<boolean>(`${this.url}api/prospecto/SubirContratoCliente/${this.idCliente}/${this.idAsuntoLegal}`, { headers: this.getHeaders() }).subscribe(response => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.okToast('Contrato enviado para su revisi\u00F3n');
                    }, 300);
                    this.close();
                }, err => {
                    this.detenerCarga();
                    setTimeout(() => {
                        this.errorToast('Ocurri\u00F3 un error');
                    }, 300);
                    this.validaError(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }, 300);
        }
    }
    onFileSelected(event: any) {
        this.selectedFile = event.target.files[0];
        this.quitarFocoDeElementos();
        this.lerr = {};
    }
    limpiarArchivoSeleccionado() {
        this.selectedFile = null;
        if (this.fileInput) {
            this.fileInput.nativeElement.value = '';
        }

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
    valida() {
        this.validaciones = true;
        //if (this.servicio == '') {
        //    this.lerr['Servicio'] = ['Ingrese un servicio'];
        //    this.validaciones = false;
        //}
        if (this.selectedFile == null) {
            this.lerr['archivoSeleccionado'] = ['Seleccione el contrato'];
            this.validaciones = false;
        }
        return this.validaciones;
    }

    close() {
        let docModal = document.getElementById('modalSubirContratoCliente');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
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