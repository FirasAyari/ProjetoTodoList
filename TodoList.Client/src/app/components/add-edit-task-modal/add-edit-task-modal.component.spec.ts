import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditTaskModalComponent } from './add-edit-task-modal.component';

describe('AddEditTaskModalComponent', () => {
  let component: AddEditTaskModalComponent;
  let fixture: ComponentFixture<AddEditTaskModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditTaskModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditTaskModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
