import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges , } from '@angular/core';
import { ProspectoExistente } from '../../models/ProspectoExistente';
import { StoreUser } from 'src/app/stores/StoreUser';
import { Router } from '@angular/router';
declare var bootstrap: any;

@Component({
    selector: 'prospectoexistente-widget',
    templateUrl: './prospectoexistente.widget.html'
})
export class ProspectoExistenteWidget implements OnChanges {
    @Output('confirmaprospectoexistentewidgetEvent') confirmaEvent = new EventEmitter<string>();
    mensaje: string = " ";
    titulo: string = " ";
    accion: string = '';
    prospectos: ProspectoExistente[] = [];
    esAdmin: number = 0;
    esProspectoUnico: boolean = false;
    cantidadProspectos: number = 0;
    nombreProspecto: string = "";

    constructor(private user: StoreUser, private router: Router) {
    this.esAdmin = user.idAutoriza}

    open(prospectos: ProspectoExistente[], accion: string, titulo: string, mensaje: string) { 
        this.nombreProspecto = mensaje;
        this.esProspectoUnico = prospectos.length == 1
        this.cantidadProspectos = prospectos.length;
        this.prospectos = prospectos;
        this.accion = accion;
        this.titulo = titulo;
        this.mensaje = mensaje;
        let docModal = document.getElementById('prospectoexistentewidget');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.confirmaEvent.emit(this.accion);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('prospectoexistentewidget');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
        this.accion = '';
        this.titulo = '';
        this.mensaje = '';
    }

    ngOnChanges(changes: SimpleChanges): void {
    }

    verProspecto(idProspecto: number) {
        this.router.navigate(['/exclusivo/cotiza/', idProspecto]);
        this.close();
        //redirigir al componente 
    }
}