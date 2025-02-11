import { Component } from '@angular/core';
import { StoreUser } from 'src/app/stores/StoreUser';
import * as Highcharts from 'highcharts';
import HC_exporting from 'highcharts/modules/exporting';
import HC_accessibility from 'highcharts/modules/accessibility';
import { Router } from '@angular/router';

// Inicializa los módulos
HC_exporting(Highcharts);
HC_accessibility(Highcharts);

@Component({
    selector: 'exclusivo',
    templateUrl: './exclusivo.component.html'
})
export class ExclusivoComponent {

    constructor(public user: StoreUser, private rtr: Router) {
        let uST = eval('(' + localStorage.getItem('singaUser') + ')');
        if (uST != undefined) {
            // this.user = uST as StoreUser;
            this.user.idPersonal = uST.idPersonal;
            this.user.identificador = uST.identificador;
            this.user.nombre = uST.nombre;
            this.user.idInterno = uST.idInterno;
            this.user.idEmpleado = uST.idEmpleado;
            this.user.estatus = uST.estatus;
            this.user.idAutoriza = uST.idAutoriza;
            this.user.idSupervisa = uST.idSupervisa;
            this.user.direccionIP = uST.direccionIP;
        }
        else {
            this.rtr.navigate(['']);
        }
    }
    goBack() {
        window.history.back();
        this.quitarFocoDeElementos();
    }
    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');

        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}