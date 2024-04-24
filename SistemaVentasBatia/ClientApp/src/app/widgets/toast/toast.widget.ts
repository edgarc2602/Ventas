import { Component, Input } from '@angular/core';

@Component({
    selector: 'toast-widget',
    templateUrl: './toast.widget.html'
})
export class ToastWidget {
    @Input() isErr: boolean = false;
    @Input() errMessage: string = "";

    open() {
        let toastElement = document.getElementById('glotoast');
        let titleElement = toastElement.querySelector('.toast-header');
        if (this.isErr == false) {
            titleElement.textContent = "Ok";
        }
        else {
            titleElement.textContent = "Revisa";
        }
        let messageElement = toastElement.querySelector('.toast-body');
        messageElement.textContent = this.errMessage;
        toastElement.classList.add('show');
        toastElement.classList.toggle('bg-danger', this.isErr);
        toastElement.classList.toggle('bg-success', !this.isErr);
        setTimeout(() => {
            toastElement.classList.remove('show');
            this.isErr = false;
            this.errMessage
            messageElement.textContent = '';
            titleElement.textContent = '';
        }, 3000);
    }
}
