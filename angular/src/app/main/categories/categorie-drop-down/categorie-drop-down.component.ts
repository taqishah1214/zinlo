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
  
  @Input() SelectedCategory;
  @Output() messageEvent = new EventEmitter<string>();

  constructor
  (
   private _categoryService: CategoriesServiceProxy,
   private _router:Router,private cdf: ChangeDetectorRef
   ) {
}
  ngOnInit() {
      this.categoryId = 0;
      this._categoryService.categoryDropDown().subscribe(result => {
        this.category = result;
        console.log("selected catogery",result)
    });
   

  }
  categoryValuee() : void {
      console.log("child",this.categoryId);
      this.messageEvent.emit(this.categoryId);
      console.log("catogery",this.categoryId);

  }

  addNew(){
    console.log("=-=-=-=-=")
    // this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: 0 } } });

  }
  createCategory(): void {

    this._router.navigate(['/app/main/categories/create-or-edit-category'], { state: { data: { id: 0 } } });
}
ngOnChanges(changes: SimpleChanges){
  if(changes.currentValue != undefined )
    this.SelectedCategory= changes.currentValue;
    this.cdf.detectChanges()
    console.log("agchjdbshdb",this.SelectedCategory);
  console.log("===> ",changes)
}
}
