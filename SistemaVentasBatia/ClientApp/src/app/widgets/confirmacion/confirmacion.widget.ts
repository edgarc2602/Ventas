import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'confirmacion-widget',
    templateUrl: './confirmacion.widget.html'
})
export class ConfirmacionWidget implements OnChanges {
    @Input() mensaje: string = " ";
    @Input() titulo: string = " ";
    @Output('confirmaEvent') confirmaEvent = new EventEmitter<string>();
    accion: string = '';

    constructor() {}

    open(accion: string) { 
        this.accion = accion;
        let docModal = document.getElementById('modalConfirmacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.confirmaEvent.emit(this.accion);
        this.close();
    }

    cancela() {
        this.accion = '';
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalConfirmacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
        this.mensaje = '';
        this.titulo = '';
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}