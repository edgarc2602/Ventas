import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { ColectivoComponent } from './colectivo/colectivo.component';
import { ColMenuComponent } from './colectivo/menu/menu.component';
import { LoginComponent } from './colectivo/login/login.component';
import { ExclusivoComponent } from './exclusivo/exclusivo.component';
import { ExMenuComponent } from './exclusivo/menu/menu.component';
import { LatMenuComponent } from './exclusivo/menu/latmenu.component';
import { HomeComponent } from './exclusivo/home/home.component';
import { ProspectoComponent } from './exclusivo/prospecto/prospecto.component';
import { ProsNuevoComponent } from './exclusivo/prospecto/nuevo/nuevo.component';
import { CotizacionComponent } from './exclusivo/cotizacion/cotizacion.component';
import { CotizaComponent } from './exclusivo/cotizacion/nuevo/nuevo.component';
import { ResumenComponent } from './exclusivo/cotizacion/resumen/resumen.component';
import { CatalogoComponent } from './exclusivo/catalogo/catalogo.component';
import { ProspectoWidget } from './widgets/prospecto/prospecto.widget';
import { DireccionWidget } from './widgets/direccion/direccion.widget';
import { PuestoWidget } from './widgets/puesto/puesto.widget';
import { MaterialWidget } from './widgets/material/material.widget';
import { MaterialAddWidget } from './widgets/materialadd/materialadd.widget';
import { PaginaWidget } from './widgets/paginador/paginador.widget';
import { ToastWidget } from './widgets/toast/toast.widget';
import { ProductoWidget } from './widgets/producto/producto.widget';
import { EditarCotizacion } from './widgets/editacotizacion/editacotizacion.widget';
import { ServicioAddWidget } from './widgets/servicioadd/servicioadd.widget';
import { AgregarServicioWidget } from './widgets/agregarservicio/agregarservicio.widget';
import { StoreUser } from './stores/StoreUser';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { fadeInOut } from './fade-in-out';
import { UsuarioAddWidget } from './widgets/usuarioadd/usuarioadd.widget';
import { PuestoLayoutWidget } from './widgets/puestolayout/puestolayout.widget';
import { MarcaVenta } from './widgets/marcaventa/marcaventa.widget';
import { ContratoWidget } from './widgets/contrato/contrato.widget';
import { ConfirmacionWidget } from './widgets/confirmacion/confirmacion.widget';
import { CerrarCotizacion } from './widgets/cerrarcotizacion/cerrarcotizacion.widget';
import { CargaWidget } from './widgets/carga/carga.widget';
import { SubirContratoClienteWidget } from './widgets/subircontratocliente/subircontratocliente.widget';
import { ClienteWidget } from './widgets/cliente/cliente.widget';
import { AgregarIndustriaWidget } from './widgets/agregarindustria/agregarindustria.widget';

@NgModule({
    declarations: [
        AppComponent,
        ColectivoComponent,
        ColMenuComponent,
        LoginComponent,
        ExclusivoComponent,
        ExMenuComponent,
        LatMenuComponent,
        HomeComponent,
        ProspectoComponent,
        ProsNuevoComponent,
        ProspectoWidget,
        DireccionWidget,
        CotizacionComponent,
        CotizaComponent,
        PuestoWidget,
        MaterialWidget,
        MaterialAddWidget,
        ResumenComponent,
        CatalogoComponent,
        PaginaWidget,
        ToastWidget,
        ProductoWidget,
        EditarCotizacion,
        ServicioAddWidget,
        AgregarServicioWidget,
        UsuarioAddWidget,
        PuestoLayoutWidget,
        MarcaVenta,
        ContratoWidget,
        ConfirmacionWidget,
        CerrarCotizacion,
        CargaWidget,
        SubirContratoClienteWidget,
        ClienteWidget,
        AgregarIndustriaWidget
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        BrowserAnimationsModule,
        FormsModule,
        CommonModule,
        RouterModule.forRoot([
            {
                path: '', component: ColectivoComponent,
                children: [
                    { path: '', component: LoginComponent, pathMatch: 'full' }
                ]
            },
            {
                path: 'exclusivo', component: ExclusivoComponent,
                children: [
                    { path: '', component: HomeComponent, pathMatch: 'full' },
                    { path: 'prospecto', component: ProspectoComponent },
                    { path: 'nuevopros', component: ProsNuevoComponent },
                    { path: 'prospecto/:id', component: ProsNuevoComponent },
                    { path: 'cotiza/:idp', component: CotizacionComponent },
                    { path: 'nuevocot', component: CotizaComponent },
                    { path: 'resumen/:id', component: ResumenComponent },
                    { path: 'catalogo', component: CatalogoComponent }
                ]
            }
        ])
    ],
    providers: [StoreUser],
    bootstrap: [AppComponent]
})
export class AppModule { }
