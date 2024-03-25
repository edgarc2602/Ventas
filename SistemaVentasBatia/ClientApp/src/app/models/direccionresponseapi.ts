import { DireccionAPI } from "./direccionapi";

export interface DireccionResponseAPI {
    error: boolean;
    message: string;
    codigoPostal: DireccionAPI;
}