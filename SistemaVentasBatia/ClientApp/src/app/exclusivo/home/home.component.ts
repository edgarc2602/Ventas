import { Component, OnInit, Inject } from '@angular/core';
import * as Highcharts from 'highcharts';
import { fadeInOut } from 'src/app/fade-in-out';
import { HttpClient } from '@angular/common/http';
import { UsuarioGrafica } from '../../models/usuariografica';
import { UsuarioGraficaMensual } from '../../models/usuariograficamensual';
import { CotizaResumenLim } from '../../models/cotizaresumenlim';
import { Catalogo } from '../../models/catalogo';
import { CotizacionVendedorDetalle } from '../../models/cotizacionvendedordetalle';
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
    model: UsuarioGrafica = {
        idPersonal: 0,
        nombre: '',
        cotizaciones: 0,
        prospectos: 0
    };
    modelMensual: UsuarioGraficaMensual = {
        idPersonal: 0,
        nombre: '',
        mes: 0,
        cotizacionesPorMes: 0
    }
    usuarios: UsuarioGrafica[] = [];
    usuariosMensual: UsuarioGraficaMensual[] = [];
    datosAgrupadosMensuales: Record<number, UsuarioGraficaMensual[]> = {};
    datosAgrupados: DatosAgrupados[] = [];
    cotizacionDetalle: CotizacionVendedorDetalle = {
        idVendedor: 0, cotizacionDetalle: []
    }
    vendedores: Catalogo[] = [];
    idVendedor: number = 0;


    constructor(
        @Inject('BASE_URL') private url: string,
        private http: HttpClient
    ) {
        http.get<UsuarioGrafica[]>(`${url}api/usuario/obtenercotizacionesusuarios/`).subscribe(response => {
            this.usuarios = response;
            this.getDonut();
        }, err => console.log(err));

        http.get<UsuarioGraficaMensual[]>(`${url}api/usuario/obtenercotizacionesmensuales`).subscribe(response => {
            this.usuariosMensual = response;
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
                        this.series.name + ': ' + this.y + '<br/>' ;
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
        this.http.get<CotizacionVendedorDetalle>(`${this.url}api/cotizacion/CotizacionVendedorDetallePorIdVendedor/${idVendedor}`).subscribe(response => {
            this.cotizacionDetalle = response;
        }, err => {
            console.log(err)
        });
    }
}