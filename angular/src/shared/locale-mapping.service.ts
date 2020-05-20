import { AppConsts } from '@shared/AppConsts';
import * as _ from 'lodash';
import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from "rxjs";
@Injectable({
    providedIn: 'root'
  })
export class LocaleMappingService {
    private message= new BehaviorSubject <number>(0);
    count = this.message.asObservable();
    itemCount:number=0;
    constructor() { }

    updatedItemCount(data: any){
        this.itemCount=this.itemCount + data;
        this.message.next(this.itemCount);
    }
    map(mappingSource: string, locale: string): string {
        if (!AppConsts.localeMappings && !AppConsts.localeMappings[mappingSource]) {
            return locale;
        }

        const localeMappings = _.filter(AppConsts.localeMappings[mappingSource], { from: locale });
        if (localeMappings && localeMappings.length) {
            return localeMappings[0]['to'];
        }

        return locale;
    }
}
