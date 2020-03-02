import {Injectable, Inject} from '@angular/core';
import {cacheable, ID, PaginationResponse} from '@datorama/akita';
import {Observable, pipe, throwError} from 'rxjs';
import {COMANDA_CONFIG, ComandaConfig} from './comanda.config';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, finalize, map, tap} from 'rxjs/operators';
import {Paginacao} from '../../helpers/paginacao';
import {ComandaStore} from './comanda.store';
import {Comanda} from '../../models/comanda';
import {LoginService} from "../../login/login.service";
import {ComandaPedido, ComandaPedidoSituacao} from "../../models/comanda-pedido";

@Injectable({providedIn: 'root'})
export class ComandaService {

  constructor(
    protected store: ComandaStore,
    protected http: HttpClient,
    protected loginService: LoginService,
    @Inject(COMANDA_CONFIG) protected config: ComandaConfig,
  ) {
  }

  public get(paginacao: Paginacao): Observable<PaginationResponse<Comanda>> {
    const headers = new HttpHeaders({'Content-Type': 'application/json;charset=utf-8'});

    return this.http.get<any>(`${this.config.api}/comandasabertas/${paginacao.pageNumber}/${paginacao.pageSize}`, {headers})
      .pipe(
        map((result) => {
          const paginatedResponse = {
            data: result.items,
            total: result.total,
            perPage: paginacao.pageSize,
            currentPage: paginacao.pageNumber,
          } as PaginationResponse<Comanda>;
          return paginatedResponse;
        }),
        catchError((error: HttpErrorResponse) => {
          return throwError(error);
        }),
      );
  }

  public getAll(): Observable<Comanda[]> {
    const headers = new HttpHeaders({'Content-Type': 'application/json;charset=utf-8'});

    return this.http.get<any>(`${this.config.api}/TotasComandasAbertas`, {headers})
      .pipe(
        map((result) => {
          return result;
        }),
        catchError((error: HttpErrorResponse) => {
          return throwError(error);
        }),
      );
  }

  public setActive(ids: ID[]) {
    return this.store.setActive(ids);
  }

  public getById(id: ID) {
    this.store.setLoading(true);

    const request$ = this.http.get<Comanda>(`${this.config.api}/obter/${id}`)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.store.setError(error);
          return throwError(error);
        }),
        tap((item) => this.store.add(item)),
      );

    const cacheableRequest = cacheable(this.store, request$)
      .pipe(finalize(() => {
        this.store.setLoading(false);
      }));
    return cacheableRequest;
  }

  public add(comanda: Comanda) {
    comanda.id = 0;
    comanda.garcomId = this.loginService.currentUser.id;

    return this.http.post<Comanda>(`${this.config.api}/cadatrar`, comanda)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.store.setError(error);
          return throwError(error);
        }),
        tap((modelAdded) => {
          this.store.add(modelAdded);
        })
      );
  }

  public alterarSituacaoPedido(comandaId: number, situacao: ComandaPedidoSituacao) {
    return this.http.put<Comanda>(`${this.config.api}/AlterarSituacaoPedido/${comandaId}`, situacao)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.store.setError(error);
          return throwError(error);
        }));
  }

  public update(id: ID, model: Partial<Comanda>) {
    return this.http.put(`${this.config.api}/editar`, model)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.store.setError(error);
          return throwError(error);
        }),
        tap(() => {
          this.store.update(id, model);
        })
      ) as Observable<Comanda>;
  }
}
