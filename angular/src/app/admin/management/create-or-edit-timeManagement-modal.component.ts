import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { TimeManagementsServiceProxy, CreateOrEditTimeManagementDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditTimeManagementModal',
    templateUrl: './create-or-edit-timeManagement-modal.component.html'
})
export class CreateOrEditTimeManagementModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    date: Date;
    selectedDateRange: moment.Moment[] = [moment().add(-7, 'days').startOf('day'), moment().endOf('day')];
    timeManagement: CreateOrEditTimeManagementDto = new CreateOrEditTimeManagementDto();



    constructor(
        injector: Injector,
        private _timeManagementsServiceProxy: TimeManagementsServiceProxy
    ) {
        super(injector);
    }
    onOpenCalendar(container) {
        container.monthSelectHandler = (event: any): void => {
          container._store.dispatch(container._actions.select(event.date));
        };
        container.setViewMode('month');
      }
    onChange() {
        abp.event.trigger('app.dashboardFilters.dateRangePicker.onDateChange', this.selectedDateRange);
      }
    show(timeManagementId?: number): void {

        if (!timeManagementId) {
            this.timeManagement = new CreateOrEditTimeManagementDto();
            this.timeManagement.id = timeManagementId;
            this.timeManagement.month = moment().startOf('day');

            this.active = true;
            this.modal.show();
        } else {
            this._timeManagementsServiceProxy.getTimeManagementForEdit(timeManagementId).subscribe(result => {
                this.timeManagement = result.timeManagement;


                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
            this.saving = true;
            this._timeManagementsServiceProxy.createOrEdit(this.timeManagement)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
