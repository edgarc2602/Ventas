export interface Usuario {
    identificador: string;
    nombre: string;
    idPersonal: number;
    idInterno: number;
    idEmpleado: number;
    estatus: number;
    idAutoriza: number;
    idSupervisa: number;
    grupo: UsuarioGrupo[];
    idGrupoActivo: number;
    descripcionGrupoActivo: string;
}

export class UsuarioGrupo {
    idGrupo: number;
    descripcion: string;
    principal: boolean;
}