import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { ProductoItem } from 'src/app/models/productoitem';
import { ProductoWidget } from 'src/app/widgets/producto/producto.widget';
import { AgregarServicioWidget } from 'src/app/widgets/agregarservicio/agregarservicio.widget'

import { PuestoTabulador } from 'src/app/models/puestotabulador';
import { Subject } from 'rxjs';
import { fadeInOut } from 'src/app/fade-in-out';
import { CotizaPorcentajes } from 'src/app/models/cotizaporcentajes';

import { StoreUser } from 'src/app/stores/StoreUser';
import { UsuarioRegistro } from 'src/app/models/usuarioregistro';
import { Usuario } from '../../models/usuario';
import { UsuarioAddWidget } from 'src/app/widgets/usuarioadd/usuarioadd.widget';
import { AgregarUsuario } from 'src/app/models/agregarusuario';
import { EliminaWidget } from 'src/app/widgets/elimina/elimina.widget';


import Swal from 'sweetalert2';
import { ListaProducto } from '../../models/listaproducto';
import { ImmsJornada } from '../../models/immsjornada';

@Component({
    selector: 'catalogo-comp',
    templateUrl: './catalogo.component.html',
    animations: [fadeInOut],
})
export class CatalogoComponent {
    @ViewChild(ProductoWidget, { static: false }) prow: ProductoWidget;
    @ViewChild(AgregarServicioWidget, { static: false }) addSer: AgregarServicioWidget;
    @ViewChild(UsuarioAddWidget, { static: false }) addUsu: UsuarioAddWidget;
    @ViewChild(EliminaWidget, { static: false }) eliw: EliminaWidget;

    @ViewChild('zona1txt', { static: false }) zona1txt: ElementRef;
    @ViewChild('zona2txt', { static: false }) zona2txt: ElementRef;
    @ViewChild('zona3txt', { static: false }) zona3txt: ElementRef;
    @ViewChild('zona4txt', { static: false }) zona4txt: ElementRef;
    @ViewChild('zona5txt', { static: false }) zona5txt: ElementRef;

    @ViewChild('costoIndirectotxt', { static: false }) costoIndirectotxt: ElementRef;
    @ViewChild('utilidadtxt', { static: false }) utilidadtxt: ElementRef;
    @ViewChild('comisionSobreVentatxt', { static: false }) comisionSobreVentatxt: ElementRef;
    @ViewChild('comisionExternatxt', { static: false }) comisionExternatxt: ElementRef;
    @ViewChild('fechaAplicatxt', { static: false }) fechaAplicatxt: ElementRef;

    @ViewChild('imsstxt', { static: false }) imsstxt: ElementRef;

    pues: Catalogo[] = [];
    selPuesto: number = 0;
    tipoServicio: number = 2;
    mates: ProductoItem[] = [];
    sers: Catalogo[] = [];
    tser: Catalogo[] = [];
    grupo: string = 'material';
    zona1: number = 0;
    zona2: number = 0;
    zona3: number = 0;
    zona4: number = 0;
    zona5: number = 0;
    costoIndirecto: number = 0;
    utilidad: number = 0;
    comisionSobreVenta: number = 0;
    comisionExterna: number = 0;
    fechaAplica: Date;
    cotpor: CotizaPorcentajes = {
        idPersonal: 0, costoIndirecto: 0, utilidad: 0, comisionSobreVenta: 0, comisionExterna: 0, fechaAlta: null, personal: '', fechaAplica: null
    };
    sal: PuestoTabulador = {
        idSueldoZonaClase: 0, idPuesto: 0, idClase: 0, zona1: 0, zona2: 0, zona3: 0, zona4: 0, zona5: 0
    };
    validaMess: string = '';
    evenSub: Subject<void> = new Subject<void>();
    selectedImage: string | ArrayBuffer | null = null;
    idPersonal: number = 0;
    autorizacion: number = 0;
    usuario: UsuarioRegistro = {
        idAutorizacionVentas: 0, idPersonal: 0, autoriza: 0, nombres: '', apellidos: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '',
        firma: '', revisa: 0
    }
    lclas: Catalogo[] = [];
    idClase: number = 1;
    tipoProd: string = '';
    imss: number = 0;

    lusu: AgregarUsuario[];
    agregarusuario: AgregarUsuario = {
        idPersonal: 0, autorizaVentas: 0, estatusVentas: 0, cotizadorVentas: 0, revisaVentas: 0, nombres: '', apellidoPaterno: '', apellidoMaterno: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '', firma: '', usuario: '', password: ''
    }
    elimina: number = 0;
    model: ListaProducto = {
        productos: [], familias: [], pagina: 1, numPaginas: 0, rows: 0, idProveedor: 0, proveedor: ''
    }
    idEstado: number = 0;
    isLoading: boolean = false;
    estados: Catalogo[];
    idFamilia: number = 0;
    immsJornada: ImmsJornada = {
        idImmsJornadaCotizador: 0, normal2: 0, normal4: 0, normal8: 0, normal12: 0, frontera2: 0, frontera4: 0, frontera8: 0, frontera12: 0, fechaAlta: null, idPersonal: 0, usuario: ''
    }


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
            this.pues = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/gettiposervicio`).subscribe(response => {
            this.tser = response;
        }, err => console.log(err));
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getclase`).subscribe(response => {
            this.lclas = response;
        }, err => console.log(err));
        http.get<Catalogo[]>(`${url}api/catalogo/getestado`).subscribe(response => {
            this.estados = response;
        }, err => console.log(err));

        this.http.get<CotizaPorcentajes>(`${this.url}api/cotizacion/obtenerporcentajescotizacion`).subscribe(response => { //falta
            this.cotpor = response;
            this.costoIndirecto = this.cotpor.costoIndirecto;
            this.utilidad = this.cotpor.utilidad;
            this.comisionSobreVenta = this.cotpor.comisionSobreVenta;
            this.comisionExterna = this.cotpor.comisionExterna;
            this.fechaAplica = this.cotpor.fechaAplica;
        }, err => console.log(err));
        this.http.get<number>(`${this.url}api/cotizacion/obtenerimssbase`).subscribe(response => {
            this.imss = response;
        }, err => console.log(err));
        this.http.get<AgregarUsuario[]>(`${this.url}api/usuario/obtenerusuarios`).subscribe(response => {
            this.lusu = response;
        })
        this.http.get<ImmsJornada>(`${this.url}api/cotizacion/ObtenerImssJornada`).subscribe(response => {
            this.immsJornada = response;
        })
    }
    chgServicio() {

    }
    chgPuesto() {
        this.getMaterial();
        this.getTabulador();
    }

    chgTab(nm: string) {
        this.grupo = nm;
        this.getMaterial();
    }

    openMat(id: number) {
        this.grupo = 'material';
        this.prow.inicio(id, 0, this.selPuesto);
    }

    openEqui(id: number) {
        this.grupo = 'equipo';
        this.prow.inicio(id, 0, this.selPuesto);
    }

    openHer(id: number) {
        this.grupo = 'herramienta';
        this.prow.inicio(id, 0, this.selPuesto);
    }

    openUni(id: number) {
        this.grupo = 'uniforme';
        this.prow.inicio(id, 0, this.selPuesto);
    }

    openSer() {
        this.addSer.open();
    }
    closeMat($event) {
        this.getMaterial();
    }
    reloadServicios() {
        this.getServicios();
    }

    getServicios() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/getservicio`).subscribe(response => {
            this.sers = response;
        }, err => console.log(err));
    }

    getMaterial() {
        this.mates = [];
        this.http.get<ProductoItem[]>(`${this.url}api/producto/get${this.grupo}/${this.selPuesto}`).subscribe(response => {
            this.mates = response;
        }, err => console.log(err));
    }

    deleteMat(id: number) {
        this.http.delete<boolean>(`${this.url}api/producto/del${this.grupo}/${id}`).subscribe(response => {
            this.getMaterial();
        }, err => console.log(err));
    }

    updateProd(id: number, tipo: number) {

        this.prow.inicio(id, tipo, this.selPuesto);
    }

    deleteServ(id) {
        this.http.delete(`${this.url}api/producto/EliminarServicio/${id}`).subscribe(response => {
            this.getServicios();
        }, err => console.log(err));
    }

    limpiarPorcentajesNG() {
        this.costoIndirecto = 0;
        this.utilidad = 0;
        this.comisionSobreVenta = 0;
        this.comisionExterna = 0;
        this.fechaAplica = null;
    }

    limpiarPorcentajes() {
        this.cotpor.costoIndirecto = 0;
        this.cotpor.utilidad = 0;
        this.cotpor.comisionSobreVenta = 0;
        this.cotpor.comisionExterna = 0;
        this.cotpor.fechaAplica = null;
    }

    obtenerPorcentajesCotizacion() {
        this.cotpor.costoIndirecto = parseFloat(this.costoIndirectotxt.nativeElement.value);
        this.cotpor.utilidad = parseFloat(this.utilidadtxt.nativeElement.value);
        this.cotpor.comisionSobreVenta = parseFloat(this.comisionSobreVentatxt.nativeElement.value);
        this.cotpor.comisionExterna = parseFloat(this.comisionExternatxt.nativeElement.value);
        this.cotpor.fechaAplica = this.fechaAplicatxt.nativeElement.value;
        this.cotpor.idPersonal = this.user.idPersonal;
    }

    getPorcentajes() {
        this.limpiarPorcentajes();
        this.limpiarPorcentajesNG();
        this.http.get<CotizaPorcentajes>(`${this.url}api/cotizacion/obtenerporcentajescotizacion`).subscribe(response => { //falta
            this.cotpor = response;
            this.costoIndirecto = this.cotpor.costoIndirecto;
            this.utilidad = this.cotpor.utilidad;
            this.comisionSobreVenta = this.cotpor.comisionSobreVenta;
            this.comisionExterna = this.cotpor.comisionExterna;
            this.fechaAplica = this.cotpor.fechaAplica;
            //this.limpiarPorcentajesNG();
        }, err => console.log(err));
        this.getImss();
    }

    actualizarPorcentajesPredeterminadosCotizacion() {
        this.limpiarPorcentajes();
        this.obtenerPorcentajesCotizacion();
        this.http.post<CotizaPorcentajes>(`${this.url}api/cotizacion/actualizarporcentajespredeterminadoscotizacion`, this.cotpor).subscribe(response => {
            this.limpiarPorcentajesNG();
            this.limpiarPorcentajes();
            this.getPorcentajes();
            Swal.fire({
                icon: 'success',
                timer: 1000,
                showConfirmButton: false,
            });
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'No se guardaron los cambios, porfavor revise la informaci\u00F3n',
                icon: 'error',
                timer: 2000,
                showConfirmButton: false,
            });
            this.getPorcentajes()
            console.log(err)
        });
    }

    onFileSelected(event: any): void {
        const selectedFile = event.target.files[0];
        if (selectedFile) {
            const reader = new FileReader();
            reader.onload = (e: any) => {
                this.selectedImage = e.target.result as string | ArrayBuffer | null;
            };
            reader.readAsDataURL(selectedFile);
        }
    }

    guardarImagen(): void {
        if (this.selectedImage) {
            if (this.selectedImage instanceof ArrayBuffer) {
                const base64Firma = this.arrayBufferToBase64(this.selectedImage);
                this.usuario.firma = base64Firma;
            } else if (typeof this.selectedImage === 'string') {
                this.usuario.firma = this.selectedImage;
            } else {
                console.error('Tipo no compatible para selectedImage');
            }
            if (this.usuario.autoriza == 1) {
                this.usuario.autoriza = 1;
            }
            else {
                this.usuario.autoriza = 0;
            }
            if (this.usuario.revisa == 1) {
                this.usuario.revisa = 1;
            }
            else {
                this.usuario.revisa = 0;
            }
            this.usuario.idPersonal = this.idPersonal;
            this.http.post<boolean>(`${this.url}api/usuario/agregarusuario`, this.usuario).subscribe(response => {
                this.nuevoUsuario();
            });
        }
    }
    goBack() {
        window.history.back();
    }
    arrayBufferToBase64(arrayBuffer: ArrayBuffer): string {
        const uint8Array = new Uint8Array(arrayBuffer);
        return btoa(String.fromCharCode.apply(null, uint8Array));
    }
    nuevoUsuario() {
        this.usuario = {
            idAutorizacionVentas: 0, idPersonal: 0, autoriza: 0, nombres: '', apellidos: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '',
            firma: '', revisa: 0
        }
    }
    openUsu(idPuesto: number) {
        this.addUsu.open(idPuesto);
    }

    getTabulador() {
        this.http.get<PuestoTabulador>(`${this.url}api/tabulador/ObtenerTabuladorPuesto/${this.selPuesto}/${this.idClase}`).subscribe(response => {
            this.sal = response;
        }, err => console.log(err));
    }
    actualizarSalarios(id: number) {
        this.obtenerValores();
        this.http.post<PuestoTabulador>(`${this.url}api/cotizacion/actualizarsalarios`, this.sal).subscribe(response => {
            this.getTabulador();
            Swal.fire({
                icon: 'success',
                timer: 1000,
                showConfirmButton: false,
            });
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'No se guardaron los cambios, porfavor revise la informaci\u00F3n',
                icon: 'error',
                timer: 2000,
                showConfirmButton: false,
            });
            console.log(err)
        });
    }
    limpiarObjeto() {
        this.sal.zona1 = 0;
        this.sal.zona2 = 0;
        this.sal.zona3 = 0;
        this.sal.zona4 = 0;
        this.sal.zona5 = 0;
    }
    obtenerValores() {
        this.sal.zona1 = parseFloat(this.zona1txt.nativeElement.value);
        this.sal.zona2 = parseFloat(this.zona2txt.nativeElement.value);
        this.sal.zona3 = parseFloat(this.zona3txt.nativeElement.value);
        this.sal.zona4 = parseFloat(this.zona4txt.nativeElement.value);
        this.sal.zona5 = parseFloat(this.zona5txt.nativeElement.value);
        this.sal.idClase = this.idClase;
        this.sal.idPuesto = this.selPuesto;
    }

    getImss() {
        this.http.get<number>(`${this.url}api/cotizacion/obtenerimssbase`).subscribe(response => {
            this.imss = response;
        }, err => console.log(err));
    }

    updImss() {
        this.imss = parseFloat(this.imsstxt.nativeElement.value);
        this.http.put<boolean>(`${this.url}api/cotizacion/actualizarimssbase`, this.imss).subscribe(response => {
            Swal.fire({
                icon: 'success',
                timer: 1000,
                showConfirmButton: false,
            });
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'No se guardaron los cambios, porfavor revise la informaci\u00F3n',
                icon: 'error',
                timer: 2000,
                showConfirmButton: false,
            });
            this.getPorcentajes();
            console.log(err)
        });
    }

    obtenerUsuarios() {
        this.http.get<AgregarUsuario[]>(`${this.url}api/usuario/obtenerusuarios`).subscribe(response => {
            this.lusu = response;
        })
    }

    chgEstatus(estatusVentas: number, idPersonal: number) {
        const dato = { idPersonal: idPersonal };
        if (estatusVentas === 1) {
            this.http.put<boolean>(`${this.url}api/usuario/desactivarusuario`, idPersonal).subscribe(response => {
                this.obtenerUsuarios();
            }, err => {
                console.log(err)
            });
            //estatusVentas = 0;
        } else {
            this.http.put<boolean>(`${this.url}api/usuario/activarusuario`, idPersonal).subscribe(response => {
                this.obtenerUsuarios();
            }, err => {
                console.log(err)
            });
            //estatusVentas = 1;
        }
    }

    eliminarUsuario($event) {
        this.idPersonal = this.elimina;
        this.http.put<boolean>(`${this.url}api/usuario/eliminarusuario`, this.idPersonal).subscribe(response => {
            this.obtenerUsuarios();
        }, err => {
            console.log(err)
        });
        this.idPersonal = 0;
        this.elimina = 0;
    }

    confirmarEliminarUsuario(idPersonal: number) {
        this.elimina = idPersonal;
        this.eliw.titulo = 'Eliminar Usuario';
        this.eliw.mensaje = 'Las cotizaciones y prospectos relacionados no ser\u00E1n afectados';
        this.eliw.open();
    }

    getPrecioProductoByEstado(filtro: number) {
        if (filtro == 2) {
            this.model.pagina = 1;
            this.http.get<ListaProducto>(`${this.url}api/producto/GetProductoProveedorByIdEstado/${this.idEstado}/${this.model.pagina}/${this.idFamilia}`).subscribe(response => {
                this.model = response;
            }, err => {
            });
        }
        if (filtro == 0) {
            this.http.get<ListaProducto>(`${this.url}api/producto/GetProductoProveedorByIdEstado/${this.idEstado}/${this.model.pagina}/${this.idFamilia}`).subscribe(response => {
                this.model = response;
            }, err => {
            });
        }
        if (filtro == 1) {
            this.model.productos = [];
            this.model.proveedor = '';
            this.idFamilia = 0;
            this.model.pagina = 1;
            this.isLoading = true;

            this.http.get<ListaProducto>(`${this.url}api/producto/GetProductoProveedorByIdEstado/${this.idEstado}/${this.model.pagina}/${this.idFamilia}`).subscribe(response => {
                setTimeout(() => {
                    this.model = response;
                    this.isLoading = false;
                }, 300);
            }, err => {
                setTimeout(() => {
                    this.isLoading = false;
                }, 300);
            });
            if (this.model.productos.length == 0) {
                this.model.familias = [];
            }
        }


    }
    muevePagina(event) {
        this.model.pagina = event;
        this.getPrecioProductoByEstado(0);
    }

    descargarListaProductosPorEstado() {
        const boton1 = document.getElementById("btnProdDes") as HTMLButtonElement;
        boton1.disabled = true;
        this.http.get(`${this.url}api/report/DescargarListaProductosPorEstado/${this.idEstado}/${this.idFamilia}`, { responseType: 'arraybuffer' })
            .subscribe(
                (data: ArrayBuffer) => {
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
                    boton1.disabled = false;                },
                error => {
                    console.error('Error al obtener el archivo PDF', error);
                    boton1.disabled = false;
                }
        );
        boton1.disabled = false;
    }
    arrayBufferToDataUrl(buffer: ArrayBuffer): string {
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const dataUrl = URL.createObjectURL(blob);
        return dataUrl;
    }

    obtenerImmsJornada() {
        this.http.get<ImmsJornada>(`${this.url}api/cotizacion/ObtenerImssJornada`).subscribe(response => {
            this.immsJornada = response;
        })
    }
    actualizarImmsJornada() {
        this.immsJornada.idPersonal = this.user.idPersonal;
        this.http.post<boolean>(`${this.url}api/cotizacion/ActualizarImssJornada`, this.immsJornada).subscribe(response => {
            this.obtenerImmsJornada();
            Swal.fire({
                icon: 'success',
                timer: 1000,
                showConfirmButton: false,
            });
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'No se guardaron los cambios, porfavor revise la informaci\u00F3n',
                icon: 'error',
                timer: 2000,
                showConfirmButton: false,
            });
        });
    }
}