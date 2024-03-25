import { AgregarUsuario } from './agregarusuario'
export interface ListaAgregarUsuario {
    idPersonal: number;
    autorizaVentas: number;
    estatusVentas: number;
    cotizadorVentas: number;
    nombre: string;
    puesto: string;
    telefono: string;
    telefonoExtension: string;
    telefonoMovil: string;
    email: string;
    firma: string;
    usuario: string;
    contraseña: string;
    usuarios: AgregarUsuario[];
}