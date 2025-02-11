import { Injectable, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subject, fromEvent, merge, timer, Subscription } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { StoreUser } from '../stores/StoreUser';

@Injectable({
    providedIn: 'root'
})
export class IdleService {
    private idleTime = 50 * 60 * 1000; // 50 minutos
    private alertTime = 5 * 60 * 1000; // 5 minutos antes del cierre
    private resetIdle = new Subject<void>();
    private idle$: Observable<any>;
    private idleSubscription: Subscription | null = null;
    private warningSubscription: Subscription | null = null;
    private warningShown = false; // Para evitar múltiples avisos
    private toastWidget: ToastWidget | null = null;
    constructor(private router: Router, private authService: AuthService, public user: StoreUser,) {
        this.idle$ = merge(
            fromEvent(document, 'mousemove'),
            fromEvent(document, 'keydown'),
            fromEvent(document, 'click')
        ).pipe(
            tap(() => {
                this.resetIdle.next();
                this.warningShown = false; // Reseteamos la alerta si hay actividad
            })
        );

        this.startIdleTimer();
    }

    // 🔹 Método para asignar el ToastWidget desde un componente
    setToastWidget(toast: ToastWidget) {
        this.toastWidget = toast;
    }

    private startIdleTimer() {
        // Cancelamos suscripciones previas si existen
        if (this.idleSubscription) {
            this.idleSubscription.unsubscribe();
        }
        if (this.warningSubscription) {
            this.warningSubscription.unsubscribe();
        }


        // **Temporizador para la advertencia**
        this.warningSubscription = this.idle$.pipe(
            switchMap(() => timer(this.idleTime - this.alertTime)), // Se activa cuando faltan 5 minutos
            tap(() => this.showWarning())
        ).subscribe();

        // **Temporizador para el cierre de sesión**
        this.idleSubscription = this.idle$.pipe(
            switchMap(() => timer(this.idleTime)), // Espera el tiempo de inactividad completo
            tap(() => this.logoutUser())
        ).subscribe();
    }

    private showWarning() {
        if (!this.warningShown) {
            this.warningShown = true;
            if (this.router.url != '/') {
                this.errorToast('⚠️ Inactividad: Tu sesión expirará en 5 minutos.');
            }
        }
    }

    private logoutUser() {
        console.log('Usuario inactivo. Cerrando sesión...');
        localStorage.clear();
        this.authService.setToken('');
        localStorage.removeItem('singaUser');
        localStorage.removeItem('token');
        this.user = null;
        window.history.back();
        this.router.navigate(['']);
    }

    okToast(message: string) {
        this.toastWidget.errMessage = message;
        this.toastWidget.isErr = false;
        this.toastWidget.open(0);
    }
    errorToast(message: string) {
        this.toastWidget.isErr = true;
        this.toastWidget.errMessage = message;
        this.toastWidget.open(1);
    }
}
