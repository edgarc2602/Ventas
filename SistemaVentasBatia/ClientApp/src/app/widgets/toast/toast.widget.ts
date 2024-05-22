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
        toastElement.classList.remove('show');
        toastElement.classList.remove('hide');
        let titleElement = toastElement.querySelector('.toast-header');
        if (this.isErr == false) {
            titleElement.textContent = "Ok";
        }
        else {
            titleElement.textContent = "Error";
        }
        let messageElement = toastElement.querySelector('.toast-body');
        messageElement.textContent = this.errMessage;
        toastElement.classList.toggle('bg-danger', this.isErr);
        toastElement.classList.toggle('bg-success', !this.isErr);
        toastElement.classList.add('show');
        setTimeout(() => {
            toastElement.classList.add('hide');
            setTimeout(() => {
                this.isErr = false;
                this.errMessage = '';
                messageElement.textContent = '';
                titleElement.textContent = '';
            }, 200);
        }, 2000);
    }
}
