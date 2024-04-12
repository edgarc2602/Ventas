import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
declare var bootstrap: any;

@Component({
    selector: 'toast-widget',
    templateUrl: './toast.widget.html'
})
export class ToastWidget implements OnInit, OnDestroy {
    eventsSubscription: Subscription;
    @Input() isError: boolean = false;
    @Input() message: string = "";
    @Input() events: Observable<void>;

    constructor() {}

    refresh(isError: boolean, message: string) {
        this.isError = isError;
        this.message = message;
        let toastExample = document.getElementById('glotoast');
        let toast = new bootstrap.Toast(toastExample);
        toast.show();
    }

    ngOnInit() {
        this.eventsSubscription = this.events.subscribe(() => {
            this.refresh(this.isError, this.message);
        });
    }


    ngOnDestroy() {
        this.eventsSubscription.unsubscribe();
    }
}