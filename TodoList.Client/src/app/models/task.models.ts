export interface Task {
    id: number; 
    title: string;
    description?: string; 
    isCompleted: boolean;
  }
  
  export interface CreateTask {
    title: string;
    description?: string;
  }
  
  export interface UpdateTask {
    title: string;
    description?: string;
    isCompleted: boolean;
  }
  
  export interface SetCompletion {
      isCompleted: boolean;
  }
  
  export interface PaginatedResult<T> {
    items: T[];
    pageNumber: number;
    totalPages: number;
    totalCount: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
  }