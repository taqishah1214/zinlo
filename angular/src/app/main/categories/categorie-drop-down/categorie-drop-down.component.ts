import { Component, OnInit } from '@angular/core';
import { CategoriesServiceProxy, ClosingChecklistServiceProxy } from '@shared/service-proxies/service-proxies';


@Component({
  selector: 'app-categorie-drop-down',
  templateUrl: './categorie-drop-down.component.html',
  styleUrls: ['./categorie-drop-down.component.css']
})
export class CategorieDropDownComponent implements OnInit {
  categories: any;
  constructor
  (
   private _categoryService: CategoriesServiceProxy,
   private _closingChecklistService: ClosingChecklistServiceProxy) {
}
  ngOnInit() {
      this._categoryService.categoryDropDown().subscribe(result => {
        this.categories = result;
    });
  }

}
