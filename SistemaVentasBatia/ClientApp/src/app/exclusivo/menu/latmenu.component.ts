import { Component, OnInit, Inject, ElementRef } from '@angular/core';
declare var bootstrap: any;
import { StoreUser } from 'src/app/stores/StoreUser';
import { HttpClient } from '@angular/common/http';


@Component({
    selector: 'lat-menu',
    templateUrl: './latmenu.component.html'
})
export class LatMenuComponent implements OnInit {
    isDarkTheme: boolean = false;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser, private elRef: ElementRef) { }

    ngOnInit(): void {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });
        document.addEventListener('click', (event) => {
            if (!this.elRef.nativeElement.contains(event.target)) {
                this.cerrarMenu();
            }
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
    cerrarMenu(): void {
        const sidebarMenu = document.getElementById('sidebarMenu');
        sidebarMenu.classList.remove('show');
    }
}