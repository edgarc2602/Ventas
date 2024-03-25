import { Component, Inject, Output, EventEmitter, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PuestoCotiza } from '../../models/puestocotiza';
import { Catalogo } from '../../models/catalogo';
import { ItemN } from '../../models/item';
import { SalarioMin } from '../../models/salariomin';
import { StoreUser } from '../../stores/StoreUser';
import { Subject } from 'rxjs';
import { ToastWidget } from '../toast/toast.widget';
declare var bootstrap: any;

@Component({
    selector: 'pues-widget',
    templateUrl: './puesto.widget.html'
})
export class PuestoWidget {
    idD: number = 0;
    idC: number = 0;
    idP: number = 0;
    idT: number = 0;
    @Output('smEvent') sendEvent = new EventEmitter<number>();
    model: PuestoCotiza = {} as PuestoCotiza;
    pues: Catalogo[] = [];
    turs: Catalogo[] = [];
    dias: ItemN[] = [];
    hors: string[] = [];
    suel: SalarioMin = {} as SalarioMin;
    lerr: any = {};
    tabs: Catalogo[] = [];
    ljor: Catalogo[] = [];
    lclas: Catalogo[] = [];
    evenSub: Subject<void> = new Subject<void>();
    isErr: boolean = false;
    validaMess: string = '';
    validacion: boolean = false;
    jornada: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private sinU: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
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

        http.get<Catalogo[]>(`${url}api/catalogo/getjornada`).subscribe(response => {
            this.ljor = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getclase`).subscribe(response => {
            this.lclas = response;
        }, err => console.log(err));
        //this.http.get<ItemN[]>(`${this.url}api/catalogo/getfrecuencia`).subscribe(response => {
        //    this.fres = response;
        //}, err => console.log(err));
    }

    nuevo() {
        let dt: Date = new Date();
        this.model = {
            idPuestoDireccionCotizacion: 0, idPuesto: 0, idDireccionCotizacion: this.idD,
            jornada: 0, idTurno: 0, hrInicio: '', hrFin: '', diaInicio: 0, diaFin: 0,
            fechaAlta: dt.toISOString(), sueldo: 0, vacaciones: 0, primaVacacional: 0, imss: 0,
            isn: 0, aguinaldo: 0, total: 0, idCotizacion: this.idC, idPersonal: this.sinU.idPersonal,
            idSalario: 0, idClase: 0, idTabulador: 0, jornadadesc: '', idZona: 0, cantidad: 0, diaFestivo: false, festivo: 0, bonos: 0, vales: 0, diaDomingo: false, domingo: 0
        };
    }

    existe(id: number) {
        this.http.get<PuestoCotiza>(`${this.url}api/puesto/${id}`).subscribe(response => {
            this.model = response;
            this.model.hrInicio = this.model.hrInicio.substring(0, 5);
            this.model.hrFin = this.model.hrFin.substring(0, 5);
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
        this.lerr = {};
        if (this.model.bonos == null) {
            this.model.bonos = 0;
        }
        if (this.model.vales == null) {
            this.model.vales = 0;
        }
        if (this.valida()) {
            if (this.model.idPuestoDireccionCotizacion == 0) {
                this.model.idDireccionCotizacion = this.idD;
                this.model.idCotizacion = this.idC;
                this.http.post<PuestoCotiza>(`${this.url}api/puesto`, this.model).subscribe(response => {
                    this.sendEvent.emit(0);
                    this.close();
                    this.isErr = false;
                    this.evenSub.next();
                }, err => {
                    console.log(err);
                    this.isErr = true;
                    this.evenSub.next();
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            } else {
                this.http.put<boolean>(`${this.url}api/puesto`, this.model).subscribe(response => {
                    this.sendEvent.emit(0);
                    this.close();
                    this.isErr = false;
                    this.validaMess = 'Guardado correctamente';
                    this.evenSub.next();
                }, err => {
                    console.log(err);
                    this.isErr = true;
                    this.validaMess = '¡Ha ocurrido un error!';
                    this.evenSub.next();
                    if (err.error) {
                        if (err.error.errors) {
                            this.lerr = err.error.errors;
                        }
                    }
                });
            }
        }
    }

    chgSalario() {
        this.http.get<SalarioMin>(`${this.url}api/salario/${this.idT}/${this.model.idPuesto}/${this.model.idTurno}`).subscribe(response => {
            this.suel = response;
            this.model.idSalario = response.idSalario;
            this.model.sueldo = response.salarioI;
        }, err => console.log(err));
    }
    chgSalariodos() {
        this.http.get<number>(`${this.url}api/salario/${this.model.idPuesto}/${this.model.idClase}/${this.model.idTabulador}/${this.model.idTurno}`).subscribe(response => {
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
        }, err => console.log(err));
    }

    valida() {
        this.validacion = true;
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
        //if (this.model.diaFestivo == null) {
        //    this.lerr['diaFestivo'] = ['Seleccione una opción'];
        //    this.validacion = false;
        //}
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

    open(cot: number, dir: number, tab: number, pue: number) {
        this.lerr = {};
        this.idC = cot;
        this.idD = dir;
        this.idP = pue;
        this.idT = tab;
        if (pue == 0) {
            this.nuevo();
            this.getZonaDefault(this.idD);
        } else {
            this.existe(this.idP);
        }
        let docModal = document.getElementById('modalAgregarOperario');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
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
}