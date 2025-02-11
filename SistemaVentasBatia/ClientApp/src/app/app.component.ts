import { Component, ViewChild } from '@angular/core';
import { IdleService } from './services/idle.service';
import { ToastWidget } from './widgets/toast/toast.widget';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
    title = 'app';
    @ViewChild(ToastWidget, { static: true }) toastWidget!: ToastWidget;

    constructor(private idleService: IdleService) { }
    ngOnInit() {
        this.idleService.setToastWidget(this.toastWidget); // 🔹 Asigna el ToastWidget al servicio
    }
}
