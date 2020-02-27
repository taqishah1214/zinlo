import { ProfileServiceProxy } from "@shared/service-proxies/service-proxies";
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class UserInformation  {

    
    profilePicture = new  Subject<string>();
    constructor(
        private _profileServiceProxy: ProfileServiceProxy,       
    ) 
    {  }

    getProfilePicture() :any {
       
        this._profileServiceProxy.getProfilePicture().subscribe(result =>
            {
            if (result && result.profilePicture) {
                this.profilePicture.next('data:image/jpeg;base64,'+ result.profilePicture);
                return this.profilePicture;
                
            }
            this.profilePicture.next("")
            return this.profilePicture;
        });
    }
}