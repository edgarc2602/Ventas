import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { StoreUser } from 'src/app/stores/StoreUser';
import { GrupoService } from '../../grupo.service';

@Component({
    selector: 'ex-menu',
    templateUrl: './menu.component.html'
})
export class ExMenuComponent {
    @Output('cambiagrupo') sendEvent = new EventEmitter<boolean>();

    isExpanded = false;
    visibilidadLatMenu

    constructor(public user: StoreUser, private rtr: Router, private grupoService: GrupoService) {
        console.log('Datos en localStorage:', localStorage.getItem('singaUser'));
        console.log('Usuario en StoreUser:', user);
        if (user.idPersonal == undefined) {
            rtr.navigate(['/']);
        }
    }
    cambiaGrupo(idGrupo: number) {
        if (idGrupo != this.user.idGrupoActivo) {
            this.user.idGrupoActivo = idGrupo;
            this.grupoService.cambiaGrupo(idGrupo);
        }
    } 

    logout() {
        localStorage.removeItem('singaUser');
        Object.keys(this.user).forEach(k => this.user[k] = null);
        this.rtr.navigate(['']);
    }

  collapse() {
    this.isExpanded = false;
  }

    toggle() {
        this.isExpanded = !this.isExpanded;
    }
    button() {
        this.visibilidadLatMenu = 0;
        this.quitarFocoDeElementos();
    }
    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}