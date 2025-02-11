import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap, switchMap, filter, take } from 'rxjs/operators';


@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'https://tu-api.com/api/auth'; // URL del backend
    private refreshingToken = false; // Bandera para evitar múltiples refresh simultáneos
    private refreshTokenSubject = new BehaviorSubject<string | null>(null);

    constructor(private http: HttpClient) { }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    setToken(token: string) {
        localStorage.setItem('token', token);
        this.refreshTokenSubject.next(token);
    }

    isTokenExpired(): boolean {
        const token = this.getToken();
        if (!token) return true;

        const payload = JSON.parse(atob(token.split('.')[1])); // Decodifica el JWT
        const exp = payload.exp * 1000; // Convierte a milisegundos
        return Date.now() > exp - 60000; // Consideramos expirar si faltan menos de 60s
    }

    refrescarToken(): Observable<string> {
        if (this.refreshingToken) {
            return this.refreshTokenSubject.pipe(
                filter(token => token !== null),
                take(1)
            ) as Observable<string>;
        }

        this.refreshingToken = true;
        return this.http.get<{ token: string }>(`${this.apiUrl}/refrescar-token`).pipe(
            tap(response => {
                this.setToken(response.token);
                this.refreshingToken = false;
            }),
            switchMap(response => {
                this.refreshTokenSubject.next(response.token);
                return this.refreshTokenSubject.asObservable();
            })
        );
    }
}
