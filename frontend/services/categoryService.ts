import api from "../config/api";
import type { Category } from "../types";

export const categoryService = {
  getCategories: async (): Promise<Category[]> => {
    const response = await api.get("/categories");
    return response.data;
  },
  createCategory: async (
    name: string,
    description: string,
  ): Promise<Category> => {
    const response = await api.post("/categories", { name, description });
    return response.data;
  },
  deleteCategory: async (id: number): Promise<void> => {
    await api.delete(`/categories/${id}`);
  },
};
