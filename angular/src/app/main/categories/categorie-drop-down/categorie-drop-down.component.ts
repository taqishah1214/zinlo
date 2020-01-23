import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { CategoriesServiceProxy, ClosingChecklistServiceProxy, NameValueDto, NameValueDtoOfInt64, } from '@shared/service-proxies/service-proxies';


@Component({
  selector: 'app-categorie-drop-down',
  templateUrl: './categorie-drop-down.component.html',
  styleUrls: ['./categorie-drop-down.component.css']
})
export class CategorieDropDownComponent implements OnInit {
  public categoryValue : any;
  categories: any;
  category : NameValueDtoOfInt64[] = [];
  categoryId : any;
  
  @Input() SelectedCategory: any;
  @Output() messageEvent = new EventEmitter<string>();

  constructor
  (
   private _categoryService: CategoriesServiceProxy) {
}
  ngOnInit() {
      this.categoryId = 0;
      this._categoryService.categoryDropDown().subscribe(result => {
        this.category = result;
    });
   

  }
  categoryValuee() : void {
      console.log("child",this.categoryId);
      this.messageEvent.emit(this.categoryId)

  }
}
