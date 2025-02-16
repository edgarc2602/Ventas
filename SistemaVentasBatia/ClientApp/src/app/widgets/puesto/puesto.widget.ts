﻿import { Component, Inject, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PuestoCotiza } from '../../models/puestocotiza';
import { Catalogo } from '../../models/catalogo';
import { ItemN } from '../../models/item';
import { SalarioMin } from '../../models/salariomin';
import { StoreUser } from '../../stores/StoreUser';
import { Subject } from 'rxjs';
import { ToastWidget } from '../toast/toast.widget';
import { CargaWidget } from 'src/app/widgets/carga/carga.widget';
import { CatalogoSueldoJornalero } from '../../models/catalogosueldojornalero';
declare var bootstrap: any;

@Component({
    selector: 'pues-widget',
    templateUrl: './puesto.widget.html'
})
export class PuestoWidget {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    @ViewChild(CargaWidget, { static: false }) cargaWidget: CargaWidget;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    model: PuestoCotiza = {} as PuestoCotiza;
    lclas: Catalogo[] = [];
    pues: Catalogo[] = [];
    turs: Catalogo[] = [];
    tabs: Catalogo[] = [];
    ljor: Catalogo[] = [];
    lcli: Catalogo[] = [];
    lsuc: Catalogo[] = [];
    lsuel: CatalogoSueldoJornalero[] = [];
    hors: string[] = [];
    dias: ItemN[] = [];
    suel: SalarioMin = {} as SalarioMin;
    lerr: any = {};
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idT: number = 0;
    jornada: number = 0;
    validacion: boolean = false;
    isLoading: boolean = false;
    nombreSucursal: string = '';
    diasEvento: number = 0;
    sueldoDiario: number = 0;
    existeProducto: boolean = false;
    idEstado: number = 0;
    idCliente: number = 0;
    idSucursal: number = 0;
    selectedSueldo: any;
    idServicio: number = 0;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        
        http.get<Catalogo[]>(`${url}api/catalogo/getturno`).subscribe(response => {
            this.turs = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/catalogo/getdia`).subscribe(response => {
            this.dias = response;
        }, err => console.log(err));
        http.get<string[]>(`${url}api/catalogo/gethorario`).subscribe(response => {
            this.hors = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${this.url}api/tabulador/getbyedo/${1}`).subscribe(response => {
            this.tabs = response;
        }, err => console.log(err));
        
        http.get<Catalogo[]>(`${url}api/catalogo/getclase`).subscribe(response => {
            this.lclas = response;
        }, err => console.log(err));
        
    }

    

    nuevo() {
        this.lerr = {};
        let dt: Date = new Date();
        this.model = {
            idPuestoDireccionCotizacion: 0, idPuesto: 0, idDireccionCotizacion: this.idD, jornada: 0, idTurno: 0, hrInicio: '', hrFin: '', diaInicio: 0, diaFin: 0, fechaAlta: dt.toISOString(), sueldo: 0, vacaciones: 0, primaVacacional: 0,
            imss: 0, isn: 0, aguinaldo: 0, total: 0, idCotizacion: this.idC, idPersonal: this.sinU.idPersonal, idSalario: 0, idClase: 1, idTabulador: 0, jornadadesc: '', idZona: 0, cantidad: 0, diaFestivo: false, festivo: 0, bonos: 0, vales: 0,
            diaDomingo: false, domingo: 0, diaCubreDescanso: false, cubreDescanso: 0, hrInicioFin: '', hrFinFin: '', diaInicioFin: 0, diaFinFin: 0, diaDescanso: 0, diasEvento: 0, incluyeMaterial: false
        };
        this.model.sueldo = 0
        this.idCliente = 0;
        this.idEstado = 0;
        this.lcli = null;
        this.lsuc = null;
    }

    existe(id: number) {
        this.model.sueldo = 0
        this.http.get<PuestoCotiza>(`${this.url}api/puesto/${id}`).subscribe(response => {
            this.existeProducto = response.incluyeMaterial;
            this.idD = response.idDireccionCotizacion;
                this.http.get<number>(`${this.url}api/salario/getestadodireccion/${this.idD}`).subscribe(response => {
                    this.idEstado = response;
                }, err => console.log(err));
            this.model = response;
            this.model.hrInicio = this.model.hrInicio.substring(0, 5);
            this.model.hrFin = this.model.hrFin.substring(0, 5);
            this.model.hrInicioFin = this.model.hrInicioFin.substring(0, 5);
            this.model.hrFinFin = this.model.hrFinFin.substring(0, 5);
            this.model.idCotizacion = this.idC;
            if (this.model.idPuesto == 77) {
                this.model.sueldo = this.model.sueldo / this.diasEvento;
            }
            this.chgSalariodos();
        }, err => {
            console.log(err);
            if (err.error) {
                if (err.error.errors) {
                    this.lerr = err.error.errors;
                }
            }
        });
    }

    guarda() {
        if (this.idServicio != 6) {
            this.lerr = {};
            if (this.model.bonos == null) {
                this.model.bonos = 0;
            }
            if (this.model.vales == null) {
                this.model.vales = 0;
            }
            if (this.model.diaFinFin == 0) {
                this.model.diaFinFin = 0;
            }
            if (this.model.diaInicioFin == 0) {
                this.model.diaInicioFin = 0;
            }
            this.model.diasEvento = this.diasEvento;
            if (this.valida()) {
                this.iniciarCarga();
                setTimeout(() => {
                    if (this.model.idPuestoDireccionCotizacion == 0) {
                        this.http.post<PuestoCotiza>(`${this.url}api/puesto/${this.idServicio}`, this.model).subscribe(response => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.okToast('Puesto agregado');
                            }, 300);
                            this.sendEvent.emit(0);
                            this.close();
                        }, err => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.errorToast('Ocurri\u00F3 un error');
                            }, 300);
                            console.log(err);
                            if (err.error) {
                                if (err.error.errors) {
                                    this.lerr = err.error.errors;
                                }
                            }
                        });
                    } else {
                        this.http.put<boolean>(`${this.url}api/puesto/${this.existeProducto}/${this.idServicio}`, this.model).subscribe(response => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.okToast('Puesto actualizado');
                            }, 300);
                            this.sendEvent.emit(0);
                            this.close();
                        }, err => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.errorToast('Ocurri\u00F3 un error');
                            }, 300);
                            console.log(err);
                            if (err.error) {
                                if (err.error.errors) {
                                    this.lerr = err.error.errors;
                                }
                            }
                        });
                    }
                }, 300);
            }
        }
        else {
            this.model.diaDescanso = 7;
            this.model.diaFin = 1;
            this.model.diaInicio = 1;
            this.model.idTurno = 1;
            this.lerr = {};
            if (this.model.bonos == null) {
                this.model.bonos = 0;
            }
            if (this.model.vales == null) {
                this.model.vales = 0;
            }
            if (this.model.diaFinFin == 0) {
                this.model.diaFinFin = 0;
            }
            if (this.model.diaInicioFin == 0) {
                this.model.diaInicioFin = 0;
            }
            
            this.model.diasEvento = this.diasEvento;
            if (this.valida()) {
                this.iniciarCarga();
                setTimeout(() => {
                    if (this.model.idPuestoDireccionCotizacion == 0) {
                        this.http.post<PuestoCotiza>(`${this.url}api/puesto/${this.idServicio}`, this.model).subscribe(response => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.okToast('Puesto agregado');
                            }, 300);
                            this.sendEvent.emit(0);
                            this.close();
                        }, err => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.errorToast('Ocurri\u00F3 un error');
                            }, 300);
                            console.log(err);
                            if (err.error) {
                                if (err.error.errors) {
                                    this.lerr = err.error.errors;
                                }
                            }
                        });
                    } else {
                        this.http.put<boolean>(`${this.url}api/puesto/${this.existeProducto}/${this.idServicio}`, this.model).subscribe(response => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.okToast('Puesto actualizado');
                            }, 300);
                            this.sendEvent.emit(0);
                            this.close();
                        }, err => {
                            this.detenerCarga();
                            setTimeout(() => {
                                this.errorToast('Ocurri\u00F3 un error');
                            }, 300);
                            console.log(err);
                            if (err.error) {
                                if (err.error.errors) {
                                    this.lerr = err.error.errors;
                                }
                            }
                        });
                    }
                }, 300);
            }
        }
        
    }

    chgSalario() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.get<SalarioMin>(`${this.url}api/salario/${this.idT}/${this.model.idPuesto}/${this.model.idTurno}`).subscribe(response => {
                this.detenerCarga();
                this.suel = response;
                this.model.idSalario = response.idSalario;
                this.model.sueldo = response.salarioI;
                this.sueldoDiario = response.salarioI / 30.4167;

            }, err => {
                this.detenerCarga();
                console.log(err);
            });
        }, 300);
    }

    chgSalarioSeg() {
        this.iniciarCarga();
        setTimeout(() => {
            this.http.get<SalarioMin>(`${this.url}api/salario/${this.idT}/${this.model.idPuesto}/${this.model.idTurno}`).subscribe(response => {
                this.detenerCarga();
                this.suel = response;
                this.model.idSalario = response.idSalario;
                this.model.sueldo = response.salarioI;
                this.sueldoDiario = response.salarioI / 30.4167;

            }, err => {
                this.detenerCarga();
                console.log(err);
            });
        }, 300);
    }

    obtenerIdEstado() {
        this.http.get<number>(`${this.url}api/salario/getestadodireccion/${this.idD}`).subscribe(response => {
            this.idEstado = response;
        }, err => console.log(err));
    }

    loadSucursalesCliente() {
        this.chgSalariodos();
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/GetCatalogoSucursalesCliente/${this.idEstado}/${this.idCliente}`).subscribe(response => {
            this.lsuc = response;
        }, err => console.log(err));
    }

    salarioSelected(importe: number, jornada: string, idJornada: number) {
        this.model.sueldo = importe;
        this.model.jornadadesc = jornada;
        this.model.jornada = idJornada;
        this.okToast("Sueldo seleccionado correctamente");
        this.quitarFocoDeElementos2(); 
    }

    chgSalariodos() {
        this.iniciarCarga();
        if (this.model.idPuesto == 77) {
            this.http.get<Catalogo[]>(`${this.url}api/catalogo/getcatalogoclientes/${this.idEstado}`).subscribe(response => {
                this.lcli = response;
            }, err => console.log(err));
            this.http.get<CatalogoSueldoJornalero[]>(`${this.url}api/salario/ObtenerSueldoJornal/${this.idD}/${this.idCliente}/${this.idSucursal}`).subscribe(response => {
                this.lsuel = response;
                this.model.sueldo = 0;
                this.model.jornada = 0;
            })
            this.detenerCarga();

        }
        else {
            setTimeout(() => {
                this.http.get<number>(`${this.url}api/salario/${this.model.idPuesto}/${this.model.idClase}/${this.model.idTabulador}/${this.model.idTurno}/${this.model.jornada}`).subscribe(response => {
                    this.detenerCarga();
                    this.model.sueldo = response;

                    this.jornada = 0;
                    switch (this.model.jornada) {
                        case 1:
                            this.jornada = 2
                            this.model.sueldo = this.model.sueldo * 0.35;
                            break;
                        case 2:
                            this.jornada = 4
                            this.model.sueldo = this.model.sueldo * 0.60;
                            break;
                        case 3:
                            this.jornada = 8
                            this.model.sueldo = this.model.sueldo;
                            break;
                        case 4:
                            this.jornada = 12
                            this.model.sueldo = this.model.sueldo * 1.5;
                            break;
                        default:
                            break;
                    }
                    this.sueldoDiario = parseFloat((this.model.sueldo / 30.4167).toFixed(2));

                }, err => {
                    this.detenerCarga();
                    console.log(err);
                });
            }, 300);
        }
    }

    valida() {
        this.validacion = true;
        if (this.model.sueldo == 0 || this.model.sueldo == null) {
            this.lerr['Sueldo'] = ['Sueldo es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idTurno == 0) {
            this.lerr['IdTurno'] = ['Turno es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idPuesto == 0) {
            this.lerr['IdPuesto'] = ['Puesto es obligatorio'];
            this.validacion = false;
        }
        if (this.model.diaInicio == 0) {
            this.lerr['DiaInicio'] = ['Día inicio es obligatorio'];
            this.validacion = false;
        }
        if (this.model.diaFin == 0) {
            this.lerr['DiaFin'] = ['Día fin es obligatorio'];
            this.validacion = false;
        }
        if (this.model.hrInicio == '' || this.model.hrInicio == "''") {
            this.lerr['HrInicio'] = ['Hora inicio es obligatorio'];
            this.validacion = false;
        }
        if (this.model.hrFin == '' || this.model.hrFin == "''") {
            this.lerr['HrFin'] = ['Hora fin es obligatorio'];
            this.validacion = false;
        }
        if (this.model.jornada == 0) {
            this.lerr['Jornada'] = ['Jornada es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idTabulador == 0) {
            this.lerr['idZona'] = ['Zona es obligatorio'];
            this.validacion = false;
        }
        if (this.model.idClase == 0) {
            this.lerr['idClase'] = ['Clase es obligatorio'];
            this.validacion = false;
        }
        if (this.model.cantidad == 0) {
            this.lerr['Cantidad'] = ['Cantidad es obligatorio'];
            this.validacion = false;
        }
        if (this.model.diaDescanso == 0 || this.model.diaDescanso == undefined) {
            this.lerr['DiaDescanso'] = ['Dia Descanso es obligatorio'];
            this.validacion = false;
        }
        return this.validacion;
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

    openAdd(idCotizacion: number, idDireccionCotizacion: number, nombreSucursal?: string, diasEvento?: number, idServicio?: number) {
        this.idServicio = idServicio;
        this.diasEvento = diasEvento;
        this.nombreSucursal = nombreSucursal;
        this.idC = idCotizacion;
        this.idD = idDireccionCotizacion;
        this.idP = 0;
        this.nuevo();
        this.cargarHorarios();
        this.cargarPuestos();
        this.getZonaDefault(this.idD);
        let docModal = document.getElementById('modalAgregarOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
        this.obtenerIdEstado();
    }

    cargarHorarios() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getjornada/${this.idServicio}`).subscribe(response => {
            this.ljor = response;
        }, err => console.log(err));
    }
    cargarPuestos() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getpuesto/${this.idServicio}`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
    }

    openEdit(idCotizacion: number, idPuesto: number, nombreSucursal: string, diasEvento: number, idServicio: number) {
        this.idServicio = idServicio;
        this.diasEvento = diasEvento;
        this.nombreSucursal = nombreSucursal;
        this.idC = idCotizacion;
        this.idP = idPuesto;
        this.cargarHorarios();
        this.cargarPuestos();
        this.existe(this.idP);
        let docModal = document.getElementById('modalAgregarOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
        this.idD = this.model.idDireccionCotizacion;
    }

    close() {
        let docModal = document.getElementById('modalAgregarOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    getZonaDefault(idDireccionCotizacion: number) {
        this.http.get<number>(`${this.url}api/salario/getzonadefault/${idDireccionCotizacion}`).subscribe(response => {
            this.model.idTabulador = response;
        }, err => console.log(err));
    }

    okToast(message: string) {
        this.toastWidget.errMessage = message;
        this.toastWidget.isErr = false;
        this.toastWidget.open();
    }

    errorToast(message: string) {
        this.toastWidget.isErr = true;
        this.toastWidget.errMessage = message;
        this.toastWidget.open();
    }

    iniciarCarga() {
        this.isLoading = true;
        this.cargaWidget.open(true);
    }

    detenerCarga() {
        this.isLoading = false;
        this.cargaWidget.open(false);
    }
    quitarFocoDeElementos2(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');

        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}