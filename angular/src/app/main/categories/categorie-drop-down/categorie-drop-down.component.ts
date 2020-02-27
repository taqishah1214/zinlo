import { Component, OnInit, EventEmitter, Output, Input, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CategoriesServiceProxy, ClosingChecklistServiceProxy, NameValueDto, NameValueDtoOfInt64, } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';


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
  categoryName : any;
  categoriesList:any;
  
  @Input() SelectedCategory;
  @Output() messageEvent = new EventEmitter<number>();

  constructor
  (
   private _categoryService: CategoriesServiceProxy,
   private _router:Router,private cdf: ChangeDetectorRef
   ) {
}
  ngOnInit() {
    this.categoryName = "Select Category"
    this._categoryService.categoryDropDown().subscribe(result => {
      this.categoriesList = result;
    });
  }
  categoryClick(id,name) : void {
    console.log("wertyuiiiiiiiiiiiiiii")
    this.categoryId = id;
    this.categoryName = name;
    this.messageEvent.emit(this.categoryId);
  }
  routeToAddNewCategory() :void {
    this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: 0 ,redirectPath : "checkList" } } });   

  }


}
