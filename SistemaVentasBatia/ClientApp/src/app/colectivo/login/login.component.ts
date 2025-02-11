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
    validacion: boolean = false;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private rtr: Router) { }

    onLogin() {
        this.isLoading = true;
        this.lerr = {};

        if (this.valida()) {
            this.http.post<Usuario>(`${this.url}api/usuario/login`, this.model, { observe: 'response' })
                .subscribe(response => {
                    setTimeout(() => {
                        this.isLoading = false;

                        // Obtener el token desde los headers
                        const authHeader = response.headers.get('Authorization');
                        const token = authHeader ? authHeader.replace('Bearer ', '') : null;

                        if (token) {
                            localStorage.setItem('token', token);
                        }

                        // Guardar usuario en localStorage
                        localStorage.setItem('singaUser', JSON.stringify(response.body));
                        this.rtr.navigate(['/exclusivo']);
                    }, 500);
                }, err => {
                    setTimeout(() => {
                        this.isLoading = false;
                        if (err.error.message) {
                            this.toastWidget.isErr = true;
                            this.toastWidget.errMessage = err.error.message;
                            this.toastWidget.open();
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
        this.validacion = true;
        if (this.model.usuario == '') {
            this.lerr['Usuario'] = ['Ingrese su usuario'];
            this.validacion = false;
        }
        if (this.model.contrasena == '') {
            this.lerr['Contrasena'] = ['Ingrese su contraseña'];
            this.validacion = false;
        }
        return this.validacion;;
    }
}