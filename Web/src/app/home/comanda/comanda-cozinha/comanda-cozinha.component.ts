import {Component, OnDestroy, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';
import {RxStompService} from '@stomp/ng2-stompjs';
import {Message} from '@stomp/stompjs';
import {Comanda} from '../../../models/comanda';
import {ComandaService} from '../comanda.service';
import {ComandaPedido, ComandaPedidoSituacao} from '../../../models/comanda-pedido';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-comanda-cozinha',
  templateUrl: './comanda-cozinha.component.html',
  styleUrls: ['./comanda-cozinha.component.scss']
})
export class ComandaCozinhaComponent implements OnInit, OnDestroy {
  public comandas: Comanda[] = [];
  private comandaSubscription: Subscription;

  constructor(protected rxStompService: RxStompService,
              protected comandaService: ComandaService,
              protected snackbar: MatSnackBar) {
  }

  ngOnInit() {
    this.comandaSubscription = this.rxStompService.watch('/exchange/mensageria_queue').subscribe((message: Message) => {
      const result = JSON.parse(message.body);
      if (result && result.message && result.message.comanda) {
        const comanda = JSON.parse(result.message.comanda);
        const pedidos = [] as ComandaPedido[];
        comanda.Pedidos.forEach(c => {
          pedidos.push({
            comandaId: c.ComandaId,
            produtoId: c.ProdutoId,
            produtoNome: c.ProdutoNome,
            quantidade: c.Quantidade,
            situacao: c.Situacao as ComandaPedidoSituacao
          } as ComandaPedido);
        });
        this.comandas.push({
          id: comanda.Id,
          garcomId: comanda.GarcomId,
          garcomNome: comanda.GarcomNome,
          pedidos
        } as Comanda);

        this.alert(`Novo pedido adicionado`);
      }
    });
    this.comandaService.getAll().subscribe((items) => this.comandas = items);
  }

  ngOnDestroy() {
    this.comandaSubscription.unsubscribe();
  }

  getSituacao(situacao: ComandaPedidoSituacao) {
    switch (situacao) {
      case ComandaPedidoSituacao.pedido:
        return 'Aguardando preparo';
      case ComandaPedidoSituacao.preparando:
        return 'Preparando';
      case ComandaPedidoSituacao.pronto:
        return 'Pronto pra entrega';
    }
  }

  getNovoPedido() {
    return this.comandas
      .filter(c => c.pedidos.findIndex(d => d.situacao === ComandaPedidoSituacao.pedido) >= 0);
  }

  getPedidosPreparando() {
    return this.comandas
      .filter(c => c.pedidos.findIndex(d => d.situacao === ComandaPedidoSituacao.preparando) >= 0);
  }

  getPedidosPronto() {
    return this.comandas
      .filter(c => c.pedidos.findIndex(d => d.situacao === ComandaPedidoSituacao.pronto) >= 0);
  }

  getDecoration(pedido: ComandaPedido) {
    return pedido.situacao === ComandaPedidoSituacao.pronto ? 'line-through' : 'dashed';
  }

  iniciarPreparoPedido(comanda: Comanda) {
    this.comandaService.alterarSituacaoPedido(comanda.id, ComandaPedidoSituacao.preparando)
      .subscribe((result) => {
        this.atualizarComanda(comanda, result);
      });
  }

  finalizarPedido(comanda: Comanda) {
    this.comandaService.alterarSituacaoPedido(comanda.id, ComandaPedidoSituacao.pronto)
      .subscribe((result) => {

        this.atualizarComanda(comanda, result);
      });
  }

  cancelarPedido(comanda: Comanda) {
    this.comandaService.alterarSituacaoPedido(comanda.id, ComandaPedidoSituacao.cancelado)
      .subscribe((result) => {
        this.atualizarComanda(comanda, result);
      });
  }

  atualizarComanda(comanda, result) {
    this.comandas[this.comandas.indexOf(comanda)] = result;
  }

  alert(message: string) {
    this.snackbar.open(message, null, {duration: 2000});
  }
}
