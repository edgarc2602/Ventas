import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { Observable,  } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable()
export class AuthInterceptorService implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Obtener el token actual
        let authToken = localStorage.getItem('token');

        // Clonar la petición para agregar el token en la cabecera
        const authReq = req.clone({
            setHeaders: {
                Authorization: authToken ? `Bearer ${authToken}` : ''
            }
        });

        return next.handle(authReq).pipe(
            tap(event => {
                if (event instanceof HttpResponse) {
                    // Extraer el nuevo token del header si existe
                    const newToken = event.headers.get('Authorization');
                    if (newToken) {
                        localStorage.setItem('token', newToken.replace('Bearer ', ''));
                        console.log('Nuevo token: ' + newToken);
                    }
                }
            })
        );
    }
}
