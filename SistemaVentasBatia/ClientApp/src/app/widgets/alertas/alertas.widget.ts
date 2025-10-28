import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
import { StoreUser } from 'src/app/stores/StoreUser';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

declare var bootstrap: any;

@Component({
    selector: 'alertas-widget',
    templateUrl: './alertas.widget.html'
})
export class AlertasWidget implements OnChanges {
    @Output('confirmaEvent') confirmaEvent = new EventEmitter<string>();
    iframeProspectos: SafeResourceUrl;

    constructor(public user: StoreUser, private sanitizer: DomSanitizer) {

        const urlProj = `https://www.singa.com.mx:8097/alertas?user_id=${this.user.idPersonal}`;
        this.iframeProspectos = this.sanitizer.bypassSecurityTrustResourceUrl(urlProj);

    }

    open() { 
        let docModal = document.getElementById('alertaswidget');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('alertaswidget');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();

    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}