import {Component} from '@angular/core';
import {Autenticacao} from './models/usuario';
import {Router} from '@angular/router';
import {LoginService} from "./login/login.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  autenticacao: Autenticacao;

  constructor(
    private router: Router,
    private loginService: LoginService
  ) {
    this.loginService.autenticacaoObservable$.subscribe(x => this.autenticacao = x);
  }

  logout() {
    this.loginService.logout();
    this.router.navigate(['/login']);
  }
}
