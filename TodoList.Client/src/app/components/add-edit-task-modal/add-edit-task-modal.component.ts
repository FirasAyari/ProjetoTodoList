import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Task, CreateTask, UpdateTask } from '../../models/task.models'; 

@Component({
  selector: 'app-add-edit-task-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // Import necessary modules
  templateUrl: './add-edit-task-modal.component.html',
  styleUrls: ['./add-edit-task-modal.component.css'] 
})
export class AddEditTaskModalComponent implements OnInit, OnChanges {
  @Input() taskToEdit: Task | null = null;
  @Output() save = new EventEmitter<{ taskData: CreateTask | UpdateTask, isEditing: boolean }>();
  @Output() close = new EventEmitter<void>();


  taskForm!: FormGroup;
  isEditing = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['taskToEdit'] && this.taskForm) {
        this.isEditing = !!this.taskToEdit; // Set editing mode flag
        this.taskForm.reset(); 
        if (this.isEditing && this.taskToEdit) {
          this.taskForm.patchValue({
            title: this.taskToEdit.title,
            description: this.taskToEdit.description
          });
        }
    }
  }

  private initForm(): void {
     this.taskForm = this.fb.group({
        title: ['', [Validators.required, Validators.maxLength(200)]],
        description: ['', [Validators.maxLength(1000)]]
     });
  }


  onFormSubmit(): void {
    this.taskForm.markAllAsTouched();
    if (this.taskForm.invalid) {
      return;
    }

    const formData = this.taskForm.value;
    let taskData: CreateTask | UpdateTask;

    if (this.isEditing && this.taskToEdit) {
       taskData = {
         title: formData.title,
         description: formData.description,
         isCompleted: this.taskToEdit.isCompleted 
      };
    } else {
      taskData = {
         title: formData.title,
         description: formData.description
      };
    }

    this.save.emit({ taskData, isEditing: this.isEditing });
  }

  closeModal(): void {
      this.close.emit();
  }
}