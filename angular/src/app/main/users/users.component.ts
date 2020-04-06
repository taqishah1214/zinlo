import { Component, Injector, ViewChild, ChangeDetectorRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/api';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent extends AppComponentBase implements OnInit {
//   cars: any = [];

//   lazyCars: any =[];
  
//   brands: string[];

//   colors: string[];

//   totalLazyCarsLength: number;

//   timeout: any;

//   sortKey: string;

//   sortOptions: any = [];

  constructor(injector :  Injector) {
    super(injector);
  }

  ngOnInit() {
      //this.brands = [
        //  'Audi', 'BMW', 'Fiat', 'Ford', 'Honda', 'Jaguar', 'Mercedes', 'Renault', 'Volvo', 'VW'
//];

//       this.colors = [
//           'Black', 'White', 'Red', 'Blue', 'Silver', 'Green', 'Yellow'
//       ];

//       for (let i = 0; i < 10000; i++) {
//           this.cars.push(this.generateCar());
//       }

//       //in a real application, make a remote request to retrieve the number of records only, not the actual records
//       this.totalLazyCarsLength = 10000;

//       this.sortOptions = [
//           {label: 'Newest First', value: '!year'},
//           {label: 'Oldest First', value: 'year'}
//       ];
   }

//   generateCar() {
//       return {
//           vin: this.generateVin(),
//           brand: this.generateBrand(),
//           color: this.generateColor(),
//           year: this.generateYear()
//       }
//   }

//   generateVin() {
//       let text = "";
//       let possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
      
//       for (var i = 0; i < 5; i++) {
//           text += possible.charAt(Math.floor(Math.random() * possible.length));
//       }
      
//       return text;
//   }

//   generateBrand() {
//       return this.brands[Math.floor(Math.random() * Math.floor(10))];
//   }

//   generateColor() {
//       return this.colors[Math.floor(Math.random() * Math.floor(7))];
//   }

//   generateYear() {
//       return 2000 + Math.floor(Math.random() * Math.floor(19));
//   }

//   loadCarsLazy(event: LazyLoadEvent) {
//       //in a real application, make a remote request to load data using state metadata from event
//       //event.first = First row offset
//       //event.rows = Number of rows per page

//       //imitate db connection over a network
//       if (this.timeout) {
//           clearTimeout(this.timeout);
//       }
      
//       this.timeout = setTimeout(() => {
//           this.lazyCars = [];
//           if (this.cars) {
//               this.lazyCars = this.cars.slice(event.first, (event.first + event.rows));
//           }
//       }, 1000);
//   }

//   onSortChange() {
//       if (this.sortKey.indexOf('!') === 0)
//           this.sort(-1);
//       else
//           this.sort(1);
//   }

//   sort(order: number): void {
//       let cars = [...this.cars];
//       cars.sort((data1, data2) => {
//           let value1 = data1.year;
//           let value2 = data2.year;
//           let result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

//           return (order * result);
//       });

//       this.cars = cars;
//   }
}





