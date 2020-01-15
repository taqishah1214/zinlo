import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesServiceProxy, CreateOrEditCategoryDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-creat-or-edit-category',
  templateUrl: './creat-or-edit-category.component.html',
  styleUrls: ['./creat-or-edit-category.component.css']
})
export class CreatOrEditCategoryComponent implements OnInit {
  categoryobj: CreateOrEditCategoryDto = new CreateOrEditCategoryDto();
  constructor(
    private _router : Router,
    private _categoriesServiceProxy: CategoriesServiceProxy


  ) { }

  ngOnInit() {
  }

  
  BackToCategoriesList(): void {
    this._router.navigate(['/app/main/categories/categories']);
  }

  onSubmit(): void {
    console.log(this.categoryobj.title)
    console.log(this.categoryobj.description)
    debugger;
   


    this._categoriesServiceProxy.createOrEdit(this.categoryobj)
     .pipe(finalize(() => {}))
     .subscribe(() => {
       console.log("Category Saved")
       this.BackToCategoriesList();
        // this.notify.info(this.l('SavedSuccessfully'));
        // this.close();
        // this.modalSave.emit(null);
     });
}


}
