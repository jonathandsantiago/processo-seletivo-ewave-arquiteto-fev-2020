import {ChangeDetectionStrategy, Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import {ControlContainer, FormGroup} from '@angular/forms';
import {Comanda} from "../../../models/comanda";
import {Produto} from "../../../models/produto";
import {ComandaPedido} from "../../../models/comanda-pedido";

@Component({
  selector: 'app-comanda-form',
  templateUrl: './comanda-form.component.html',
  styleUrls: ['./comanda-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ComandaFormComponent implements OnInit, OnDestroy {
  @Input() public parentFormGroup: FormGroup;
  @Input() public produtos: Produto[];
  @Output() public submit = new EventEmitter<Comanda>();
  @Output() public adicionar = new EventEmitter<ComandaPedido>();

  constructor(public controlContainer: ControlContainer) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

}
