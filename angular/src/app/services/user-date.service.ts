import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class UserDateService {

  private userList = new BehaviorSubject([]);
  allUsersInformationofTenant = this.userList.asObservable();
  constructor() { 
  }
  setUserList(users:[]) {
    this.userList.next(users)
  }
}