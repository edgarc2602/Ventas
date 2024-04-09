import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { CounterComponent } from './colectivo/counter/counter.component';
import { FetchDataComponent } from './colectivo/fetch-data/fetch-data.component';
import { ColectivoComponent } from './colectivo/colectivo.component';
import { ColMenuComponent } from './colectivo/menu/menu.component';
import { LoginComponent } from './colectivo/login/login.component';
import { ExclusivoComponent } from './exclusivo/exclusivo.component';
import { ExMenuComponent } from './exclusivo/menu/menu.component';
import { LatMenuComponent } from './exclusivo/menu/latmenu.component';
import { HomeComponent } from './exclusivo/home/home.component';
import { ProspectoComponent } from './exclusivo/prospecto/prospecto.component';
import { ProsNuevoComponent } from './exclusivo/prospecto/nuevo/nuevo.component';
import { ProspectoWidget } from './widgets/prospecto/prospecto.widget';
import { DireccionWidget } from './widgets/direccion/direccion.widget';
import { CotizacionComponent } from './exclusivo/cotizacion/cotizacion.component';
import { CotizaComponent } from './exclusivo/cotizacion/nuevo/nuevo.component';
import { PuestoWidget } from './widgets/puesto/puesto.widget';
import { MaterialWidget } from './widgets/material/material.widget';
import { MaterialAddWidget } from './widgets/materialadd/materialadd.widget';
import { MaterialOperarioAddWidget } from './widgets/materialoperarioadd/materialoperarioadd.widget';
import { ResumenComponent } from './exclusivo/cotizacion/resumen/resumen.component';
import { CatalogoComponent } from './exclusivo/catalogo/catalogo.component';
import { PaginaWidget } from './widgets/paginador/paginador.widget';
import { ToastWidget } from './widgets/toast/toast.widget';
import { EliminaWidget } from './widgets/elimina/elimina.widget';
import { EliminaOperarioWidget } from './widgets/eliminaOperario/eliminaOperario.widget';
import { EliminaDirectorioWidget } from './widgets/eliminadirectorio/eliminadirectorio.widget';
import { ProductoWidget } from './widgets/producto/producto.widget';
import { ActualizaCotizacionWidget } from './widgets/actualizacotizacion/actualizacotizacion.widget';
import { EditarCotizacion } from './widgets/editacotizacion/editacotizacion.widget';
import { ServicioAddWidget } from './widgets/servicioadd/servicioadd.widget';
import { AgregarServicioWidget } from './widgets/agregarservicio/agregarservicio.widget';
import { StoreUser } from './stores/StoreUser';
import { DescargarComponent } from './exclusivo/cotizacion/descargar/descargar.component';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { fadeInOut } from './fade-in-out';
import { UsuarioAddWidget } from './widgets/usuarioadd/usuarioadd.widget';
import { PuestoLayoutWidget } from './widgets/puestolayout/puestolayout.widget';
import { MarcaVenta } from './widgets/marcaventa/marcaventa.widget';
@NgModule({
    declarations: [
        AppComponent,
        CounterComponent,
        FetchDataComponent,
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
        MaterialOperarioAddWidget,
        ResumenComponent,
        CatalogoComponent,
        PaginaWidget,
        ToastWidget,
        EliminaWidget,
        EliminaOperarioWidget,
        ProductoWidget,
        EliminaDirectorioWidget,
        ActualizaCotizacionWidget,
        DescargarComponent,
        EditarCotizacion,
        ServicioAddWidget,
        AgregarServicioWidget,
        UsuarioAddWidget,
        PuestoLayoutWidget,
        MarcaVenta
        
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
                    { path: '', component: LoginComponent, pathMatch: 'full' },
                    { path: 'counter', component: CounterComponent },
                    { path: 'fetch-data', component: FetchDataComponent }
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
                    { path: 'catalogo', component: CatalogoComponent },
                    { path: 'descargar/:id', component: DescargarComponent }
                ]
            }
        ])
    ],
    providers: [StoreUser],
    bootstrap: [AppComponent]
})
export class AppModule { }
