import api from "../config/api";
import type { TaskItem, TaskFilter, PagedResult } from "../types";

export const taskService = {
  getTasks: async (filter: TaskFilter): Promise<PagedResult<TaskItem>> => {
    const params = new URLSearchParams();
    if (filter.status) params.append("status", filter.status);
    if (filter.priority) params.append("priority", filter.priority);
    if (filter.searchTerm) params.append("searchTerm", filter.searchTerm);
    if (filter.tag) params.append("tag", filter.tag);
    if (filter.categoryId)
      params.append("categoryId", String(filter.categoryId));
    if (filter.projectId) params.append("projectId", String(filter.projectId));
    if (filter.sortBy) params.append("sortBy", filter.sortBy);
    if (filter.sortOrder) params.append("sortOrder", filter.sortOrder);
    params.append("pageNumber", String(filter.pageNumber));
    params.append("pageSize", String(filter.pageSize));

    const response = await api.get(`/tasks?${params.toString()}`);
    return response.data;
  },

  getTask: async (id: number): Promise<TaskItem> => {
    const response = await api.get(`/tasks/${id}`);
    return response.data;
  },

  createTask: async (
    task: Partial<TaskItem> & { projectId: number },
  ): Promise<TaskItem> => {
    const response = await api.post("/tasks", task);
    return response.data;
  },

  updateTask: async (
    id: number,
    task: Partial<TaskItem>,
  ): Promise<TaskItem> => {
    const response = await api.put(`/tasks/${id}`, task);
    return response.data;
  },

  updateTaskStatus: async (id: number, status: string): Promise<TaskItem> => {
    const response = await api.patch(
      `/tasks/${id}/status`,
      JSON.stringify(status),
      {
        headers: { "Content-Type": "application/json" },
      },
    );
    return response.data;
  },

  deleteTask: async (id: number): Promise<void> => {
    await api.delete(`/tasks/${id}`);
  },
};
