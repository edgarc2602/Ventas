import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'eliminadirectorio-widget',
    templateUrl: './eliminadirectorio.widget.html'
})
export class EliminaDirectorioWidget implements OnChanges {
    @Input() mensaje: string = '';
    @Input() titulo: string = '';
    @Output('EventDC') sendEvent = new EventEmitter<boolean>();

    constructor() {}

    open() {
        let docModal = document.getElementById('modalEliminarDirectorio');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalEliminarDirectorio');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}