import { Component, Inject, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { ReportService } from 'src/app/report.service';



@Component({
    selector: 'descargar',
    templateUrl: './descargar.component.html'
})
export class DescargarComponent implements OnInit, OnDestroy {

    @ViewChild('descargar', { static: false }) resumen: ElementRef;

    sub: any;
    reportData: any;

    constructor(
        @Inject('BASE_URL') private url: string, private http: HttpClient,
        private route: ActivatedRoute,
        private reportService: ReportService,

    ) {
    }

    ngOnInit(): void {
        this.sub = this.route.params.subscribe(params => {
            let idcot: number = +params['id'];

            //this.obtieneReporte();
        });
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }
    
}
