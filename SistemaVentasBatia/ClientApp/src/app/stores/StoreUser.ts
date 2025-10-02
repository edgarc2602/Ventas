import { Injectable } from '@angular/core';

@Injectable()
export class StoreUser {
    identificador: string;
    nombre: string;
    idPersonal: number;
    idInterno: number;
    idEmpleado: number;
    estatus: number;
    idAutoriza: number;
    idSupervisa: number;
    direccionIP: string;
    grupo: StoreUserGroup[];
    idGrupoActivo: number;
    descripcionGrupoActivo: string;
}
export class StoreUserGroup {
    idGrupo: number;
    descripcion: string;
    principal: boolean;
}