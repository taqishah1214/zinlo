import { Component, OnInit, ViewChild, Input, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesServiceProxy, CreateOrEditCategoryDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CategoriesComponent } from '../categories.component';
@Component({
  selector: 'app-create-or-edit-category',
  templateUrl: './create-or-edit-category.component.html',
  styleUrls: ['./create-or-edit-category.component.css']
})
export class CreateOrEditCategoryComponent extends AppComponentBase implements OnInit {
  getCaregoryId: string = "";
  public saveBtnText: string = "Create";
  public formTitle: string = "Create a Category";
  @ViewChild(CategoriesComponent, { static: false }) child: CategoriesComponent;
  categoryobj: CreateOrEditCategoryDto = new CreateOrEditCategoryDto();
  categoryId: number = 0;
  constructor(private _router: Router,
    private _categoriesServiceProxy: CategoriesServiceProxy,
    injector: Injector) {
    super(injector);
    
  }

  ngOnInit() {
    this.categoryId = history.state.data.id;
    if (this.categoryId > 0) {
      this._categoriesServiceProxy.getCategoryForEdit(this.categoryId).subscribe(result => {
        this.categoryobj = result.category;
      });
    }
  }


  BackToCategoriesList(): void {
    this._router.navigate(['/app/main/categories']);
  }

  onSubmit(): void {
    this._categoriesServiceProxy.createOrEdit(this.categoryobj)
      .pipe(finalize(() => { }))
      .subscribe(() => {

        this.BackToCategoriesList();
        this.notify.info(this.l('SavedSuccessfully'));
      });
  }


}
