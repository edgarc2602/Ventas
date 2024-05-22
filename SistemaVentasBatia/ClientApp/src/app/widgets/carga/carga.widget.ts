import { Component, Input } from '@angular/core';

@Component({
    selector: 'carga-widget',
    templateUrl: './carga.widget.html'
})
export class CargaWidget {
    isCarga: boolean = false;

    open(isCarga: boolean) {
        let toastElement = document.getElementById('gloload');
        if (isCarga) {
            toastElement.classList.remove('show');
            toastElement.classList.remove('hide');
            this.isCarga = true;
            toastElement.classList.add('show');
        } else {
            this.isCarga = false;
            toastElement.classList.add('hide');
        }
    }

}