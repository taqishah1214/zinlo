import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesServiceProxy, NameValueDto, TaskDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-or-edit-task',
  templateUrl: './create-or-edit-task.component.html',
  styleUrls: ['./create-or-edit-task.component.css']
})

export class CreateOrEditTaskComponent implements OnInit {
  categories : any;
  taskDto : TaskDto;
  constructor(private _router: Router,
    private _categoryService: CategoriesServiceProxy
    ) { }

  ngOnInit() {
   
    this._categoryService.categoryDropDown().subscribe(result => {
      debugger
      this.categories = result;
  });
    console.log(this.categories);
  }

  BackToTaskList() :void {
    this._router.navigate(['/app/main/TasksCheckList/tasks']);   
}

}

