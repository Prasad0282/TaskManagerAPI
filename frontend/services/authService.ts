import api from "../config/api";

export const authService = {
  register: async (username: string, email: string, password: string) => {
    const response = await api.post("/auth/register", {
      username,
      email,
      password,
    });
    localStorage.setItem("token", response.data.token);
    return response.data.token;
  },
  login: async (username: string, password: string) => {
    const response = await api.post("/auth/login", { username, password });
    localStorage.setItem("token", response.data.token);
    return response.data.token;
  },
  logout: () => localStorage.removeItem("token"),
  isAuthenticated: () => !!localStorage.getItem("token"),
};
