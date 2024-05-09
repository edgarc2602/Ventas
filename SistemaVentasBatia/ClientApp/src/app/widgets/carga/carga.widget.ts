import { Component, Input } from '@angular/core';

@Component({
    selector: 'carga-widget',
    templateUrl: './carga.widget.html'
})
export class CargaWidget {
    isCarga: boolean = false;

    open(isCarga: boolean) {
        let toastElement = document.getElementById('gloload');
        if (isCarga == true) {
            if (isCarga == this.isCarga) {
            }
            else {
                //let titleElement = toastElement.querySelector('.toast-header');
                //titleElement.textContent = 'Cargando...';
                this.isCarga = true;
                toastElement.classList.add('show');
                
            }
        }
        else {
            this.isCarga = false;
            toastElement.classList.remove('show');
        }
    }
}