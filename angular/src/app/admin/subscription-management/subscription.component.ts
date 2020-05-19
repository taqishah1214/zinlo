import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';


@Component({
    templateUrl: './subscription.component.html',
    styleUrls: ['./subscription.component.css'],
})

export class SubscriptionComponent  extends AppComponentBase implements OnInit {
   
   
    constructor(
        injector: Injector,
        
    ) {
        super(injector);
        
    }
    ngOnInit(): void {
      
    }
}