import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'elimina-widget',
    templateUrl: './elimina.widget.html'
})
export class EliminaWidget implements OnChanges {
    @Output('ansEvent') sendEventDelete = new EventEmitter<boolean>();
    @Input() mensaje: string = " ";
    @Input() titulo: string = " ";

    constructor() {}

    open() { 
        let docModal = document.getElementById('modalEliminarRegistro');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.sendEventDelete.emit(true);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalEliminarRegistro');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}