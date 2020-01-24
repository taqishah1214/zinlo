import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import {ClosingChecklistServiceProxy} from '@shared/service-proxies/service-proxies';
import { UserListComponentComponent } from '../user-list-component/user-list-component.component';


@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {
  @ViewChild(UserListComponentComponent,{ static: false }) selectedUserId: UserListComponentComponent;

  
  ClosingCheckList : any = []
  id:number;
  AssigniInputBox : boolean;
  AssigniBoxView : boolean;
  StatusColorBox : any = ["bg-purple","bg-golden","","bg-sea-green"]
  FilterBoxOpen : boolean;
  public rowId : number = 0;


  constructor(private _router: Router,     private _closingChecklistService: ClosingChecklistServiceProxy) {
    this.FilterBoxOpen = false;
  }

  ngOnInit() {
    this.AssigniInputBox = false;
    this.AssigniBoxView = true;
    this._closingChecklistService.getAll().subscribe(result => {
      this.ClosingCheckList = result.items;

      this.ClosingCheckList.forEach( i=> {
        var a =  i.closingCheckListForViewDto.assigniName[0].toUpperCase();
        var b = i.closingCheckListForViewDto.assigniName.substr(i.closingCheckListForViewDto.assigniName.indexOf(' ')+1)[0].toUpperCase();
        i.closingCheckListForViewDto["NameAvatar"] = a+b;
        if (i.closingCheckListForViewDto.status === "Inprogress")
        {
          i.closingCheckListForViewDto["StatusColor"] = this.StatusColorBox[0]
        }
        else if (i.closingCheckListForViewDto.status === "Open")
        {
          i.closingCheckListForViewDto["StatusColor"] = this.StatusColorBox[1]
        }
        else if (i.closingCheckListForViewDto.status === "Complete")
        {
          i.closingCheckListForViewDto["StatusColor"] = this.StatusColorBox[2]
        }

      });
          
         
      });
  }
  ChangeAssigniBox (id) : void {
    this.rowId = id; 
  }
  openFilterClick() : void {
    this.FilterBoxOpen = !this.FilterBoxOpen;
  }
  ChangeStatus(value) : void {
  debugger;
  }

  RedirectToCreateTask() :void {
  this._router.navigate(['/app/main/TasksCheckList/create-or-edit-task']);   
}

RedirectToDetail(recordId) :void{
  this._router.navigate(['/app/main/TasksCheckList/task-details'],{state: {data: {id:recordId}}});   

}

}
