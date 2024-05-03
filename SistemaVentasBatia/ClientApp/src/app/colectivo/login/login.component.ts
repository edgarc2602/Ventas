import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Acceso } from 'src/app/models/acceso';
import { Usuario } from 'src/app/models/usuario';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { fadeInOut } from 'src/app/fade-in-out';

@Component({
    selector: 'login-comp',
    templateUrl: './login.component.html',
    animations: [fadeInOut],
})
export class LoginComponent {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    model: Acceso = { usuario: '', contrasena: '' };
    usu: Usuario = {} as Usuario;
    isLoading: boolean = false;
    lerr: any = {};

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private rtr: Router) { }

    onLogin() {
        this.isLoading = true;
        this.lerr = {};
        if (this.valida()) {
            this.http.post<Usuario>(`${this.url}api/usuario/login`, this.model).subscribe(response => {
                setTimeout(() => {
                    this.isLoading = false;
                    console.log(response);
                    this.usu = response;
                    localStorage.setItem('singaUser', JSON.stringify(this.usu));
                    this.rtr.navigate(['/exclusivo']);
                }, 500);

            }, err => {
                setTimeout(() => {
                    this.isLoading = false;
                    console.log(err);
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        } else if (err.error.message) {
                            this.toastWidget.isErr = true;
                            this.toastWidget.errMessage = err.error.message;
                            this.toastWidget.open();
                        }
                    }
                }, 500);
            });
        }
    }

    review(event: any) {
        if (event.keyCode == 13) {
            this.onLogin();
        }
    }

    ferr(nm: string) {
        let fld = this.lerr[nm];
        if (fld)
            return true;
        else
            return false;
    }

    terr(nm: string) {
        let fld = this.lerr[nm];
        let msg: string = fld.map((x: string) => "-" + x);
        return msg;
    }

    valida() {
        return true;
    }
}