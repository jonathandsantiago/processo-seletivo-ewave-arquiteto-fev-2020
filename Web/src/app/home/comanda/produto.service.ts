import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {Produto} from "../../models/produto";
import {environment} from "../../../environments/environment";

@Injectable({providedIn: 'root'})
export class ProdutoService {

  constructor(private http: HttpClient) {
  }

  getAll(): Observable<Produto[]> {
    return this.http.get<any>(`${environment.apiUrl}/produto/ObterTodos`)
      .pipe(map(produtos => {
        return produtos;
      }));
  }
}
