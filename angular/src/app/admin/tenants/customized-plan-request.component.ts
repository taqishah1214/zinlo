import { Component, Injector, OnInit, ViewChild, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ContactusServiceServiceProxy, ContactusDto } from '@shared/service-proxies/service-proxies';
@Component({
    selector: 'customizedPlanRequest',
    templateUrl: './customized-plan-request.component.html',
    styleUrls: ['./create-tenant-modal.component.css']
})
export class CustomizedPlanRequestComponent extends AppComponentBase implements OnInit, AfterViewInit {
    
    saving = false;
    contactus:ContactusDto;
    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _contectusService:ContactusServiceServiceProxy
    ) {
        super(injector);
        this.contactus = new ContactusDto();
    }
    ngAfterViewInit(): void {
        
    }
    ngOnInit() {
        var tenantId=this._activatedRoute.snapshot.queryParams['tenantId'];
        if(tenantId!==undefined&&tenantId!==null)
        {
        this._contectusService.getContactusByTenantId(tenantId).subscribe ((result) => {
            this.contactus = result;
            
        });
        }
    }
    onOpenCalendar(container) {
        container.monthSelectHandler = (event: any): void => {
          container._store.dispatch(container._actions.select(event.date));
        };
        container.setViewMode('month');
      }
    approveRequest()
    {
       this.saving=true
       this._contectusService.approveRequest(this.contactus).subscribe ((result) => {
        this._router.navigate(['app/admin/custom-tenants']);
        this.saving=false
        
    });
    }
}
