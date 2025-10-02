import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class GrupoService {
    grupoCambiado: EventEmitter<number> = new EventEmitter<number>();

    cambiaGrupo(idGrupo: number) {
        this.grupoCambiado.emit(idGrupo);
    }
}