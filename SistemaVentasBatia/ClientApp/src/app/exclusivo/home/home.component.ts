import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import * as Highcharts from 'highcharts';
import { fadeInOut } from 'src/app/fade-in-out';
import { HttpClient } from '@angular/common/http';
import { ItemN } from 'src/app/models/item';
import { UsuarioGrafica } from '../../models/usuariografica';
import { UsuarioGraficaMensual } from '../../models/usuariograficamensual';
import { Catalogo } from '../../models/catalogo';
import { CotizacionVendedorDetalle } from '../../models/cotizacionvendedordetalle';
import { ToastWidget } from 'src/app/widgets/toast/toast.widget';
import { saveAs } from 'file-saver';

interface DatosAgrupados {
    nombre: string;
    cotizacionMes: number[];
}
@Component({
    selector: 'home-comp',
    templateUrl: './home.component.html',
    animations: [fadeInOut],
})
export class HomeComponent implements OnInit {
    @ViewChild(ToastWidget, { static: false }) toastWidget: ToastWidget;
    model: UsuarioGrafica = {
        idPersonal: 0, nombre: '', cotizaciones: 0, prospectos: 0
    };
    modelMensual: UsuarioGraficaMensual = {
        idPersonal: 0, nombre: '', mes: 0, cotizacionesPorMes: 0
    }
    cotizacionDetalle: CotizacionVendedorDetalle = {
        idVendedor: 0, cotizacionDetalle: []
    }
    datosAgrupadosMensuales: Record<number, UsuarioGraficaMensual[]> = {};
    usuariosMensual: UsuarioGraficaMensual[] = [];
    datosAgrupados: DatosAgrupados[] = [];
    usuarios: UsuarioGrafica[] = [];
    vendedores: Catalogo[] = [];
    lests: ItemN[] = [];
    idVendedor: number = 0;
    isLoading: boolean = false;
    idEstatus: number = 0;
    Finicio: Date = null;
    Ffin: Date = null;
    lerr: any = {};
    validacion: boolean = false;


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {
        http.get<UsuarioGrafica[]>(`${url}api/usuario/obtenercotizacionesusuarios/`).subscribe(response => {
            this.usuarios = response;
            this.getDonut();
        }, err => console.log(err));
        http.get<UsuarioGraficaMensual[]>(`${url}api/usuario/obtenercotizacionesmensuales`).subscribe(response => {
            this.usuariosMensual = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/prospecto/getestatus`).subscribe(response => {
            this.lests = response;
        }, err => console.log(err));
    }

    ngOnInit(): void {
        this.agruparDatosMensuales()
        this.getTop();
        this.getDonut();
        this.getVendedores();
    }

    getVendedores() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/ObtenerCatalogoVendedores`).subscribe(response => {
            this.vendedores = response;
        }, err => console.log(err));
    }

    agruparDatosMensuales() {
        this.http.get<UsuarioGraficaMensual[]>(`${this.url}api/usuario/obtenercotizacionesmensuales`).subscribe(response => {
            const datosAgrupados: DatosAgrupados[] = [];
            response.forEach(dato => {
                const { nombre, mes, cotizacionesPorMes } = dato;
                const datosAgrupadosExistente = datosAgrupados.find(item => item.nombre === nombre);
                if (datosAgrupadosExistente) {
                    datosAgrupadosExistente.cotizacionMes[mes - 1] = cotizacionesPorMes;
                } else {
                    const nuevoDatoAgrupado: DatosAgrupados = {
                        nombre: nombre,
                        cotizacionMes: Array(12).fill(0)
                    };
                    nuevoDatoAgrupado.cotizacionMes[mes - 1] = cotizacionesPorMes;
                    datosAgrupados.push(nuevoDatoAgrupado);
                }
            });
            this.datosAgrupados = datosAgrupados;
            this.getTop();
        }, err => console.log(err));

    }
    getDonut() {
        let container: HTMLElement = document.getElementById('dvdonut');
        const seriesOptions: Highcharts.SeriesColumnOptions[] = [];
        this.usuarios.forEach(usuario => {
            seriesOptions.push({
                name: usuario.nombre,
                data: [usuario.cotizaciones, usuario.prospectos],
                type: 'column',
            });
        });
        Highcharts.chart(container, {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Total'
            },
            xAxis: {
                categories: ['Cotizaciones', 'Prospectos'],
                crosshair: true
            },
            yAxis: {
                allowDecimals: false,
                min: 0,
                title: {
                    text: ' '
                }
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.x + '</b><br/>' +
                        this.series.name + ': ' + this.y + '<br/>';
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: seriesOptions,
            credits: {
                enabled: false
            }
        });
    }

    getTop() {
        let container: HTMLElement = document.getElementById('dvtop');
        const seriesOptions: Highcharts.SeriesColumnOptions[] = [];
        this.datosAgrupados.forEach(dato => {
            seriesOptions.push({
                name: dato.nombre,
                data: dato.cotizacionMes,
                type: 'column',
            });
        });
        Highcharts.chart(container, {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Cotizaciones por mes'
            },
            xAxis: {
                categories: [
                    'Jan',
                    'Feb',
                    'Mar',
                    'Apr',
                    'May',
                    'Jun',
                    'Jul',
                    'Aug',
                    'Sep',
                    'Oct',
                    'Nov',
                    'Dec'
                ],
                crosshair: true
            },
            yAxis: {
                allowDecimals: false,

                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                    '<td style="padding:0"><b>{point.y:.0f}</b></td></tr>',

                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: seriesOptions,
            credits: {
                enabled: false
            }
        });
    }

    goBack() {
        window.history.back();
    }

    getCotizacionesVendedor(idVendedor: number) {
        this.cotizacionDetalle.cotizacionDetalle = [];
        if (idVendedor != 0) {
            this.isLoading = true;
            this.http.get<CotizacionVendedorDetalle>(`${this.url}api/cotizacion/CotizacionVendedorDetallePorIdVendedor/${idVendedor}`).subscribe(response => {
                this.isLoading = false;
                this.cotizacionDetalle = response;
            }, err => {
                this.isLoading = false;
                console.log(err)
            });
        }
    }

    descargarReporteProspectosOld() {
        this.isLoading = true;
        this.http.get(`${this.url}api/report/DescargarReporteProspectos/${this.idEstatus}`, { responseType: 'arraybuffer' })
            .subscribe(
                (data: ArrayBuffer) => {
                    this.okToast('Reporte descargado');
                    const file = new Blob([data], { type: 'application/pdf' });
                    const fileURL = URL.createObjectURL(file);
                    const width = 800;
                    const height = 550;
                    const left = window.innerWidth / 2 - width / 2;
                    const top = window.innerHeight / 2 - height / 2;
                    const newWindow = window.open(fileURL, '_blank', `width=${width}, height=${height}, top=${top}, left=${left}`);
                    if (newWindow) {
                        newWindow.focus();
                    } else {
                        alert('La ventana emergente ha sido bloqueada. Por favor, permite ventanas emergentes para este sitio.');
                    }
                    this.isLoading = false;
                },
                error => {
                    this.errorToast('Ocurri\u00F3 un error')
                    console.error('Error al obtener el archivo PDF', error);
                    this.isLoading = false;
                }
            );
    }
    descargarReporteProspectos(formato: string) {
        this.lerr = {};
        if (this.valida()) {
            if (formato == 'Word') {
                this.http.get(`${this.url}api/report/DescargarProspectosCotizacionesDocx/${this.idEstatus}/${this.Finicio}/${this.Ffin}`, { responseType: 'arraybuffer' })
                    .subscribe(
                        (data: ArrayBuffer) => {
                            const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' });
                            saveAs(blob, 'ReporteProspectosCotizaciones.docx');
                            this.isLoading = false;
                            this.toastWidget.isErr = false;
                            this.toastWidget.errMessage = 'Reporte descargado';
                            this.toastWidget.open();
                            error => {
                                console.error('Error al obtener el archivo DOCX', error);
                                this.isLoading = false;
                                this.toastWidget.isErr = true;
                                this.toastWidget.errMessage = 'Ocurri\u00F3 un error';
                                this.toastWidget.open();
                            }
                        });
            }
            if (formato == 'Excel') {
                this.isLoading = true;
                this.http.post(`${this.url}api/report/DescargarProspectosCotizacionesExcel/${this.idEstatus}/${this.Finicio}/${this.Ffin}`, null, { responseType: 'blob' }).subscribe((response: Blob) => {
                    this.okToast('Reporte descargado');
                    saveAs(response, 'ReporteProspectosCotizaciones.xlsx');
                    this.isLoading = false;
                },
                    error => {
                        console.error('Error al descargar el archivo:', error);
                        this.isLoading = false;
                        this.errorToast('Ocurri\u00F3 un error');
                    }
                );
            }
        }
    }
    valida() {
        this.validacion = true;
        if (this.Finicio == undefined || this.Finicio == null) {
            this.lerr['Finicio'] = ['Seleccione la fecha inicial'];
            this.validacion = false;
        }
        if (this.Ffin == undefined || this.Ffin == null) {
            this.lerr['Ffin'] = ['Seleccione la fecha fin'];
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
}