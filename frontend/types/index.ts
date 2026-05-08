export interface Project {
  id: number;
  name: string;
  description: string;
  createdAt: string;
  taskCount?: number;
}

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  createdAt: string;
  updatedAt?: string;
  dueDate?: string;
  status: "Todo" | "InProgress" | "Done";
  priority: "Low" | "Medium" | "High" | "Critical";
  tags: string[];
  projectId: number;
  projectName: string;
  categoryId?: number;
  categoryName?: string;
}

export interface Category {
  id: number;
  name: string;
  description: string;
}

export interface TaskFilter {
  status?: string;
  priority?: string;
  searchTerm?: string;
  tag?: string;
  categoryId?: number;
  projectId?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  pageNumber: number;
  pageSize: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
