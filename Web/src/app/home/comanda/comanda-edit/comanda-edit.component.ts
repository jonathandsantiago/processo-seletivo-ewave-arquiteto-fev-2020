import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup} from '@angular/forms';
import {Comanda} from '../../../models/comanda';
import {ActivatedRoute, Router} from '@angular/router';
import {ComandaService} from '../comanda.service';
import {Observable, Subscription} from 'rxjs';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ComandaQuery} from '../comanda.query';
import {AkitaNgFormsManager} from '@datorama/akita-ng-forms-manager';
import {Produto} from '../../../models/produto';
import {ProdutoService} from '../produto.service';
import {LoginService} from '../../../login/login.service';
import {ComandaPedido, ComandaPedidoSituacao} from '../../../models/comanda-pedido';
import {MatTableDataSource} from '@angular/material/table';
import {Usuario} from '../../../models/usuario';

@Component({
  selector: 'app-comanda-edit',
  templateUrl: './comanda-edit.component.html',
  styleUrls: ['./comanda-edit.component.scss']
})
export class ComandaEditComponent implements OnInit, OnDestroy {

  public item$: Observable<Comanda>;
  public item: Comanda;
  public comandaPedidoForm: FormGroup;
  public produtos: Produto[];
  public pedidos = [] as ComandaPedido[];
  public comandaId: any;
  public usuario: Usuario;
  public displayedColumns: string[] = ['produtoNome', 'quantidade', 'actions'];
  public dataSource = new MatTableDataSource<ComandaPedido>();

  protected subscription: Subscription = new Subscription();

  constructor(
    protected activatedRoute: ActivatedRoute,
    protected router: Router,
    protected comandaQuery: ComandaQuery,
    protected comandaService: ComandaService,
    protected produtoService: ProdutoService,
    protected loginService: LoginService,
    protected snackbar: MatSnackBar,
    protected formBuilder: FormBuilder,
    protected formManager: AkitaNgFormsManager<{ comandaPedidoForm: ComandaPedido }>,
  ) {
  }

  ngOnInit() {
    this.subscription.add(
      this.activatedRoute.paramMap
        .subscribe(paramMap => {
          this.comandaId = paramMap.get('comandaId');
          this.usuario = this.loginService.currentUser;

          this.comandaPedidoForm = this.formBuilder.group({
            comandaId: new FormControl({value: this.comandaId, disabled: true}),
            garcom: new FormControl({value: this.usuario.nome, disabled: true}),
            produto: new FormControl(''),
            quantidade: new FormControl('')
          });

          this.produtoService.getAll().subscribe((produtos) => {
            this.produtos = produtos;
          }, (error) => this.alert('Não foi possivel carregar os produtos!'));

          setTimeout(() => {
            if (this.comandaId) {
              this.comandaService.getById(this.comandaId).subscribe((item: Comanda) => {
                  if (!item) {
                    this.router.navigateByUrl('/not-found');
                  }

                  if (!item.pedidos) {
                    this.pedidos = item.pedidos;
                  }
                },
                (error) => this.alert('Registro inválido!')
              );
              this.item$ = this.comandaQuery.selectEntity(this.comandaId);
              this.subscription.add(this.item$.subscribe(item => {
                this.item = item;

                if (!item.pedidos) {
                  this.pedidos = item.pedidos;
                }

                this.initializeForm();
              }));
            }
            this.initializeForm();
          }, 0);
        })
    );
  }

  alert(message: string) {
    this.snackbar.open(message, null, {duration: 2000});
  }

  initializeForm() {
    if (this.item) {
      this.comandaPedidoForm.reset(this.item);
    } else {
      this.formManager.upsert('comandaPedidoForm', this.comandaPedidoForm);
    }
  }

  submit() {
    if (this.comandaPedidoForm.valid) {
      if (this.pedidos.length === 0) {
        this.alert('É obrigatório ao menos um produto para salvar a comanda!');
        return;
      }

      const comandaPedido = this.comandaPedidoForm.getRawValue();
      const comanda = {id: comandaPedido.comandaId, pedidos: this.pedidos} as Comanda;

      if (!comanda.id) {
        this.comandaService.add(comanda).subscribe(c => {
            this.alert('Registro cadastrado com sucesso!');
            this.router.navigate(['home']);
          }, (error) => this.alert(error)
        );
      } else {
        this.comandaService.update(comanda.id, comanda).subscribe(c => {
          this.alert('Registro atualizado com sucesso!');
        }, (error) => this.alert(error));
      }
    }
  }

  adicionarPedido() {
    if (!this.comandaPedidoForm.valid || this.comandaPedidoForm.value === null) {
      this.alert('Pedido invalido!');
      return;
    }

    if (!this.comandaPedidoForm.value.produto) {
      this.alert('Produto é obrigatório!');
      return;
    }

    const comandaPedido = this.comandaPedidoForm.value;

    if (comandaPedido.quantidade <= 0) {
      this.alert('Quantidade invalida!');
      return;
    }

    this.pedidos.push({
      produtoId: comandaPedido.produto.id,
      produtoNome: comandaPedido.produto.nome,
      quantidade: comandaPedido.quantidade
    } as ComandaPedido);
    this.dataSource = new MatTableDataSource<ComandaPedido>(this.pedidos);
    this.comandaPedidoForm.reset();
    this.comandaPedidoForm.setValue({
      comandaId: this.comandaId,
      garcom: this.usuario.nome,
      produto: {},
      quantidade: 0
    });
  }

  removerPedido(value: any) {
    if (value) {
      if (value.id > 0) {
        if (value.situacao === ComandaPedidoSituacao.preparando) {
          alert('Pedido já está em preparo!');
          return;
        }

        if (value.situacao === ComandaPedidoSituacao.pronto) {
          alert('Pedido já está pronto!');
          return;
        }
      }

      const index = this.pedidos.indexOf(value);
      this.pedidos.splice(index, 1);
      this.dataSource = new MatTableDataSource<ComandaPedido>(this.pedidos);
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
