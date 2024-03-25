import { Component } from '@angular/core';

@Component({
    selector: 'col-menu',
    templateUrl: './menu.component.html'
})
export class ColMenuComponent {
    isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}