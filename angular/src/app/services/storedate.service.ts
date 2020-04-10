import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class StoreDateService {

  private userList = new BehaviorSubject([]);
  allUsersInformationofTenant = this.userList.asObservable();

  private accountSubypeList = new BehaviorSubject([]);
  allAccountSubTypes = this.accountSubypeList.asObservable();

  private categoriesList = new BehaviorSubject([]);
  allCategories = this.categoriesList.asObservable();

  constructor() { 
  }
  setUserList(users:[]) {
    this.userList.next(users)
  }

  setAccountSubTypeList(accountsSubTypes:[]) {
    this.accountSubypeList.next(accountsSubTypes)
  }

  setCategoriesList(categories:[]) {
    this.categoriesList.next(categories)
  }

}