import {Component, OnInit} from '@angular/core';
import {LoginService} from "../login/login.service";
import {Autenticacao, UsuarioSetor} from "../models/usuario";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  isGarcom: boolean;

  constructor(private loginService: LoginService) {
    this.loginService.autenticacaoObservable$.subscribe(x => {
      if (x && x.usuario)
        this.isGarcom = x.usuario.setor === UsuarioSetor.Garcon;
    });
  }

  ngOnInit() {
  }
}
