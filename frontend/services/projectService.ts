import api from "../config/api";
import type { Project } from "../types";

export const projectService = {
  getProjects: async (): Promise<Project[]> => {
    const response = await api.get("/projects");
    return response.data;
  },
  createProject: async (
    name: string,
    description: string,
  ): Promise<Project> => {
    const response = await api.post("/projects", { name, description });
    return response.data;
  },
  deleteProject: async (id: number): Promise<void> => {
    await api.delete(`/projects/${id}`);
  },
};
