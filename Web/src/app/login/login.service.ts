import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {Autenticacao, Usuario} from '../models/usuario';
import {environment} from '../../environments/environment';

@Injectable({providedIn: 'root'})
export class LoginService {
  private autenticacaoSubject$: BehaviorSubject<Autenticacao>;
  public autenticacaoObservable$: Observable<Autenticacao>;

  constructor(private http: HttpClient) {
    this.autenticacaoSubject$ = new BehaviorSubject<Autenticacao>(JSON.parse(localStorage.getItem('currentUser')));
    this.autenticacaoObservable$ = this.autenticacaoSubject$.asObservable();
  }

  public get currentUser(): Usuario {
    const autenticacao = this.currentAutenticacao;
    if (!autenticacao) {
      return null;
    }
    return autenticacao.usuario;
  }

  public get currentAutenticacao(): Autenticacao {
    return this.autenticacaoSubject$.value;
  }

  login(login: string, password: string): Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/usuario/authenticate`, {login, password})
      .pipe(map(user => {
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.autenticacaoSubject$.next(user);
        return user;
      }));
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.autenticacaoSubject$.next(null);
  }
}
