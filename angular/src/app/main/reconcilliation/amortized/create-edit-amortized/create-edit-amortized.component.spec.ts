import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditAmortizedComponent } from './create-edit-amortized.component';

describe('CreateEditAmortizedComponent', () => {
  let component: CreateEditAmortizedComponent;
  let fixture: ComponentFixture<CreateEditAmortizedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditAmortizedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditAmortizedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
