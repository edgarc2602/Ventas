import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'confirmacion-widget',
    templateUrl: './confirmacion.widget.html'
})
export class ConfirmacionWidget implements OnChanges {
    @Output('confirmaEvent') confirmaEvent = new EventEmitter<string>();
    mensaje: string = " ";
    titulo: string = " ";
    accion: string = '';

    constructor() {}

    open(accion: string, titulo: string, mensaje: string ) { 
        this.accion = accion;
        this.titulo = titulo;
        this.mensaje = mensaje;
        let docModal = document.getElementById('modalConfirmacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.confirmaEvent.emit(this.accion);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalConfirmacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
        this.accion = '';
        this.titulo = '';
        this.mensaje = '';
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}