import { Component, OnInit, AfterViewInit } from '@angular/core'; // Import AfterViewInit
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { Task, PaginatedResult, SetCompletion, CreateTask, UpdateTask } from '../../models/task.models';
import { AddEditTaskModalComponent } from '../add-edit-task-modal/add-edit-task-modal.component'; // Import modal component
import { Modal } from 'bootstrap'; // Import bootstrap Modal type (requires @types/bootstrap)

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
      CommonModule,
      AddEditTaskModalComponent // <<<--- Import the modal component here
      ],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
// Implement AfterViewInit
export class TaskListComponent implements OnInit, AfterViewInit {
  paginatedResult: PaginatedResult<Task> | null = null;
  currentPage = 1;
  pageSize = 10;
  isLoading = false;
  error: string | null = null;

  // Properties for modal management
  taskModalInstance: Modal | null = null; // Instance of the Bootstrap modal
  taskToEdit: Task | null = null; // Holds data for editing, null for adding

  constructor(private taskService: TaskService) { }

  ngOnInit(): void {
    this.loadTasks();
  }

  // Initialize modal after the view is ready
  ngAfterViewInit(): void {
    const modalElement = document.getElementById('addEditTaskModal');
    if (modalElement) {
      this.taskModalInstance = new Modal(modalElement);
    } else {
        console.error('Modal element #addEditTaskModal not found');
    }
  }

  loadTasks(page: number = this.currentPage): void {
    this.isLoading = true;
    this.error = null;
    this.currentPage = page;

    this.taskService.getTasks(this.currentPage, this.pageSize).subscribe({
      next: (result) => {
        this.paginatedResult = result;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.error = 'Failed to load tasks. Please ensure you are logged in and the API is running.';
        console.error('Failed to load tasks:', err);
      }
    });
  }

  deleteTask(id: number): void {
    if (confirm(`Are you sure you want to delete task ${id}?`)) {
       this.isLoading = true;
       this.taskService.deleteTask(id).subscribe({
         next: () => {
           console.log(`Task ${id} deleted`);
           this.loadTasks(); 
         },
         error: (err) => {
           this.isLoading = false;
           this.error = `Failed to delete task ${id}.`;
           console.error(`Failed to delete task ${id}:`, err);
         }
       });
    }
  }

  toggleComplete(task: Task): void {
     this.isLoading = true;
     const completionData: SetCompletion = { isCompleted: !task.isCompleted };
     this.taskService.setTaskCompletion(task.id, completionData).subscribe({
       next: () => {
         task.isCompleted = completionData.isCompleted; 
         this.isLoading = false;
       },
       error: (err) => {
         this.isLoading = false;
         this.error = `Failed to update task ${task.id} status.`;
         console.error(`Failed to update task ${task.id} status:`, err);
       }
     });
  }

  goToPage(page: number): void {
    if (page >= 1 && (!this.paginatedResult || page <= this.paginatedResult.totalPages)) {
      this.loadTasks(page);
    }
  }


  openAddTaskModal(): void {
    console.log('Opening Add Task Modal');
    this.taskToEdit = null;
    this.taskModalInstance?.show(); 
  }

  openEditTaskModal(task: Task): void {
    console.log('Opening Edit Task Modal for:', task);
    this.taskToEdit = { ...task };
    this.taskModalInstance?.show(); 
  }

  closeTaskModal(): void {
     console.log('Closing modal');
     this.taskModalInstance?.hide();
  }

  handleTaskSave(event: { taskData: CreateTask | UpdateTask, isEditing: boolean }): void {
    this.isLoading = true;
    this.error = null;
    console.log('Handling save:', event);

    if (event.isEditing && this.taskToEdit) {
      this.taskService.updateTask(this.taskToEdit.id, event.taskData as UpdateTask).subscribe({
        next: () => {
          console.log('Task updated successfully');
          this.closeTaskModal();
          this.loadTasks(); 
        },
        error: (err) => {
          this.isLoading = false;
          this.error = 'Failed to update task.'; 
          console.error('Update task error:', err);
           alert('Error updating task: ' + (err.error?.Message || err.message) ); 
        }
      });
    } else {
      this.taskService.createTask(event.taskData as CreateTask).subscribe({
        next: (createdTask) => {
          console.log('Task created successfully:', createdTask);
          this.closeTaskModal();
          
          this.loadTasks(); 
        },
        error: (err) => {
          this.isLoading = false;
          this.error = 'Failed to create task.'; 
          console.error('Create task error:', err);
           alert('Error creating task: ' + (err.error?.Message || err.message) ); 
        }
      });
    }
  }
}