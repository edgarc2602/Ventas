import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import * as FileSaver from 'file-saver';

@Injectable({
    providedIn: 'root'
})
export class ReportService {




    private ssrsUrl = 'http://192.168.2.4:80/ReportServer/report';
    private reportPath = '/reportecotizacion';
    private username = 'Administrador';
    private password = 'GrupoBatia@';
    constructor(private http: HttpClient) { }

    getReport(idCotizacion: number): void {

        const headers = new HttpHeaders({
            'Content-Type': 'application/pdf',
            'Accept': 'application/pdf',
            'Authorization': 'Basic ' + btoa(`${this.username}:${this.password}`)
        });

        const params = new HttpParams()
            .set('rs:Command', 'Render')
            .set('rs:Format', 'pdf')
            .set('rc:Toolbar', 'false')
            .set('idCotizacion', idCotizacion.toString());

        const reportUrl = `${this.ssrsUrl}${this.reportPath}`;
        this.http.get(reportUrl, { responseType: 'blob', headers, params }).subscribe((response) => {
            const blob = new Blob([response], { type: 'application/pdf' });

            FileSaver.saveAs(blob, 'reporte.pdf');
        });

        //const url = this.http.get<string>(reportUrl, { headers, params });
        //return url;
    }
}








//this.ssrsUrl = 'http://192.168.2.4/ReportServer/report/reportecotizacion?idCotizacion=' + idCotizacion ;