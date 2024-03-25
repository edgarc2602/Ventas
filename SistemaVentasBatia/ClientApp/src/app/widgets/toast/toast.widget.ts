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
    @Input() message: string = '';
    @Input() events: Observable<void>;

    constructor() {}

    refresh() {
        let toastExample = document.getElementById('glotoast');
        let toast = new bootstrap.Toast(toastExample);
        toast.show();
        console.log(this.isError);
    }

    ngOnInit() {
        this.eventsSubscription = this.events.subscribe(() => this.refresh());
    }

    ngOnDestroy() {
        this.eventsSubscription.unsubscribe();
    }
}