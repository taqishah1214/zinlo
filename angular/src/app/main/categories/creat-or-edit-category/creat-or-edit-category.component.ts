import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CategoriesServiceProxy, CreateOrEditCategoryDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { CategoriesComponent } from '../categories/categories.component';
import { state } from '@angular/animations';

@Component({
  selector: 'app-creat-or-edit-category',
  templateUrl: './creat-or-edit-category.component.html',
  styleUrls: ['./creat-or-edit-category.component.css']
})
export class CreatOrEditCategoryComponent implements OnInit {
  getCaregoryId: string = "";
  public saveBtnText : string = "Create";
  public formTitle : string = "Create a Category";
  @ViewChild(CategoriesComponent, { static: false }) child: CategoriesComponent;
  categoryobj: CreateOrEditCategoryDto = new CreateOrEditCategoryDto();
  editId: any;
  id: number = 0;
  constructor(
    private _router: Router,
    private _activateRouter: ActivatedRoute,
    private _categoriesServiceProxy: CategoriesServiceProxy) {
      this.id = parseInt(this._activateRouter.snapshot.params.id);
      if (this.id > 0) {
        this.saveBtnText = "Update";
        this.formTitle = "Update Category";
      }
      else{
        this.saveBtnText = "Create";
        this.formTitle = "Create a Category";
      }
     }

  ngOnInit() {
    debugger;
    this.id = parseInt(this._activateRouter.snapshot.params.id);
    if(this.id == NaN)
    {
      this.id = 0;
    }
    if (this.id != 0) {
     
      this._categoriesServiceProxy.getCategoryForEdit(this.id).subscribe(result => {
        this.categoryobj = result.category;

      });
    }
  }

  getcategoryId(id: number) {
    console.log("Category", id);
  }
  BackToCategoriesList(): void {
    this._router.navigate(['/app/main/categories/categories']);
  }

  onSubmit(): void {
    this._categoriesServiceProxy.createOrEdit(this.categoryobj)
      .pipe(finalize(() => { }))
      .subscribe(() => {
        console.log("Category Saved")
        this.BackToCategoriesList();
        // this.notify.info(this.l('SavedSuccessfully'));
        // this.close();
        // this.modalSave.emit(null);
      });
  }


}
