import {ComandaPedido} from './comanda-pedido';

export class Comanda {
  id: number;
  garcomId: number;
  garcomNome: string;
  pedidos: ComandaPedido[];
  totalAPagar: number;
  gorjetaGarcom: number;
}
