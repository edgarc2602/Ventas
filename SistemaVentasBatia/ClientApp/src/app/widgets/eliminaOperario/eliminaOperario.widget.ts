import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'eliminaOperario-widget',
    templateUrl: './eliminaOperario.widget.html'
})
export class EliminaOperarioWidget implements OnChanges {
    @Input() mensaje: string = '';
    @Input() titulo: string = '';
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();

    constructor() {}

    open() {
        let docModal = document.getElementById('modalEliminarOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.sendEvent.emit(false);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalEliminarOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}