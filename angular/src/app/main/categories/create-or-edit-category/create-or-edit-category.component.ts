import { Component, OnInit, ViewChild, Input, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesServiceProxy, CreateOrEditCategoryDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CategoriesComponent } from '../categories.component';
import { StoreDateService } from '../../../services/storedate.service';

@Component({
  selector: 'app-create-or-edit-category',
  templateUrl: './create-or-edit-category.component.html',
  styleUrls: ['./create-or-edit-category.component.css']
})
export class CreateOrEditCategoryComponent extends AppComponentBase implements OnInit {
  getCaregoryId: string = "";
  active = false;
  saving = false;
  public saveBtnText: string = "Create";
  public formTitle: string = "Create a Category";
  @ViewChild(CategoriesComponent, { static: false }) child: CategoriesComponent;
  categoryobj: CreateOrEditCategoryDto = new CreateOrEditCategoryDto();
  categoryId: number = 0;
  redirectPath: any;
  categoriesList :any = []
  isCategoryExist:boolean = false;
  constructor(private _router: Router,
    private _categoriesServiceProxy: CategoriesServiceProxy,
    injector: Injector,
    private storeData: StoreDateService) {
    super(injector);
  }

  ngOnInit() {
    this.categoryId = history.state.data.id;
    this.redirectPath = history.state.data.redirectPath;
    if (this.categoryId > 0) {
      this._categoriesServiceProxy.getCategoryForEdit(this.categoryId).subscribe(result => {
        this.categoryobj = result.category;
      });
    }
  }
  backToRoute(title,id): void {

    if (this.redirectPath === "checkList") {
      this._router.navigate(['/app/main/checklist/createtask'],{ state: { data: { categoryid: id , categoryTitle : title} } });
    }
    else if (this.redirectPath === "editChecklist") {
      var closingChecklistId = history.state.data.checklistTask;
      this._router.navigate(['/app/main/checklist/edit-task'], { state: { data: { id: closingChecklistId,categoryid: id , categoryTitle : title } } })
    }
    else if (this.redirectPath === "duplicate")
    {
    var closingChecklistId = history.state.data.checklistTask;
     this._router.navigate(['/app/main/checklist/duplicate-task'], { state: { data: { id: closingChecklistId,categoryid: id , categoryTitle : title } } });
    }
    else {
      this._router.navigate(['/app/main/categories']);
    }
  }

  BackToCategoriesList(): void {
    this.backToRoute("",0);
  }

  onSubmit(): void {
    if (this.isCategoryExist == false) {
      this.saving = true;
      this._categoriesServiceProxy.createOrEdit(this.categoryobj)
      .pipe(finalize(() => { this.saving = false;}))
      .subscribe(result => {
        this._categoriesServiceProxy.categoryDropDown().subscribe(result => { 
          this.categoriesList = result
          this.storeData.setCategoriesList(this.categoriesList)
      })
        this.backToRoute(this.categoryobj.title,result);
        this.notify.info(this.l('SavedSuccessfully'));

      });
    }  
  }

  save(event){
    this.onSubmit();
  }
  CheckCategory(){
    const {categoryobj,categoryId,_categoriesServiceProxy} = this  
    if (categoryId==0)  {
      _categoriesServiceProxy.isCategoryExist(categoryobj.title).subscribe(result=>{
        this.isCategoryExist = result;
      });
    }
  
  }
}
