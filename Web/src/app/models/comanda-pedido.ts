export class ComandaPedido {
  id: number;
  comandaId: number;
  produtoId: number;
  produtoNome: string;
  quantidade: number;
  situacao: ComandaPedidoSituacao;
}

export enum ComandaPedidoSituacao {
  pedido,
  preparando,
  pronto,
  cancelado
}
