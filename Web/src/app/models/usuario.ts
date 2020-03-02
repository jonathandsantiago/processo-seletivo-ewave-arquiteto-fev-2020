export class Usuario {
  id: number;
  nome: string;
  login: string;
  password: string;
  setor: UsuarioSetor;
}

export class Autenticacao {
  authenticated: boolean;
  created: Date;
  dateExpiration: Date;
  token: string;
  usuario: Usuario;
  Message: string;
}

export enum UsuarioSetor {
  Garcon,
  Cozinha
}
