import { Component, OnInit, Inject } from '@angular/core';
declare var bootstrap: any;
import { StoreUser } from 'src/app/stores/StoreUser';
import { HttpClient } from '@angular/common/http';


@Component({
    selector: 'lat-menu',
    templateUrl: './latmenu.component.html'
})
export class LatMenuComponent implements OnInit {
    autorizacion: number = 0;
    isDarkTheme: boolean = false;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;   
        }, err => console.log(err));
    }
    ngOnInit(): void {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });

    }
    toggleTheme() {
        this.isDarkTheme = !this.isDarkTheme;
        if (this.isDarkTheme) {
            document.body.classList.add('dark-theme');
        } else {
            document.body.classList.remove('dark-theme');
        }
        this.quitarFocoDeElementos();
    }
    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}