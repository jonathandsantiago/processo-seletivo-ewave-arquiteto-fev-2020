import {Component, OnDestroy, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';
import {RxStompService} from '@stomp/ng2-stompjs';
import {Message} from '@stomp/stompjs';
import {Comanda} from '../../../models/comanda';
import {ComandaService} from "../comanda.service";
import {ComandaPedido, ComandaPedidoSituacao} from "../../../models/comanda-pedido";

@Component({
  selector: 'app-comanda-cozinha',
  templateUrl: './comanda-cozinha.component.html',
  styleUrls: ['./comanda-cozinha.component.scss']
})
export class ComandaCozinhaComponent implements OnInit, OnDestroy {
  public comandas: Comanda[] = [];
  private comandaSubscription: Subscription;

  constructor(protected rxStompService: RxStompService,
              protected comandaService: ComandaService) {
  }

  ngOnInit() {
    this.comandaSubscription = this.rxStompService.watch('/mensageria_queue').subscribe((message: Message) => {
      this.comandas.push(JSON.parse(message.body));
    });
    this.comandaService.getAll().subscribe((items) => this.comandas = items);
  }

  ngOnDestroy() {
    this.comandaSubscription.unsubscribe();
  }

  getSituacao(situacao: ComandaPedidoSituacao) {
    switch (situacao) {
      case ComandaPedidoSituacao.pedido:
        return "Aguardando preparo";
      case ComandaPedidoSituacao.preparando:
        return "Preparando";
      case ComandaPedidoSituacao.pronto:
        return "Pronto pra entrega";
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
    return pedido.situacao == ComandaPedidoSituacao.pronto ? 'line-through' : 'dashed';
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
    let index = this.comandas.indexOf(comanda);
    this.comandas[index] = result;
  }
}
