/* eslint-disable react-hooks/set-state-in-effect */

import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { projectService } from "../services/projectService";
import { taskService } from "../services/taskService";
import type { Project, TaskItem } from "../types";

export default function Dashboard() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [recentTasks, setRecentTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(true);

  // Project creation form states
  const [newProjectName, setNewProjectName] = useState("");
  const [newProjectDesc, setNewProjectDesc] = useState("");
  const [creating, setCreating] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const [projRes, taskRes] = await Promise.all([
        projectService.getProjects(),
        taskService.getTasks({
          pageNumber: 1,
          pageSize: 5,
          sortBy: "createdAt",
          sortOrder: "desc",
        }),
      ]);
      setProjects(projRes);
      setRecentTasks(taskRes.items);
    } catch (err) {
      console.error("Failed to load dashboard data:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleCreateProject = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newProjectName.trim()) return;
    setCreating(true);
    try {
      await projectService.createProject(newProjectName, newProjectDesc);
      setNewProjectName("");
      setNewProjectDesc("");
      await fetchData(); // refresh projects and tasks
    } catch (err) {
      console.error("Failed to create project:", err);
    } finally {
      setCreating(false);
    }
  };

  if (loading) return <div className="text-center">Loading...</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Dashboard</h1>

      {/* ───── Project Creation ───── */}
      <div className="bg-white p-4 rounded-lg shadow">
        <h2 className="text-xl font-semibold mb-3">Create New Project</h2>
        <form
          onSubmit={handleCreateProject}
          className="flex flex-col sm:flex-row gap-3"
        >
          <input
            type="text"
            placeholder="Project name"
            value={newProjectName}
            onChange={(e) => setNewProjectName(e.target.value)}
            className="border rounded px-3 py-2 flex-1"
            required
          />
          <input
            type="text"
            placeholder="Description (optional)"
            value={newProjectDesc}
            onChange={(e) => setNewProjectDesc(e.target.value)}
            className="border rounded px-3 py-2 flex-1"
          />
          <button
            type="submit"
            disabled={creating}
            className="bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700 disabled:opacity-50"
          >
            {creating ? "Creating..." : "Create Project"}
          </button>
        </form>
      </div>

      {/* ───── Projects & Tasks Grid ───── */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white p-4 rounded-lg shadow">
          <h2 className="text-xl font-semibold mb-3">
            Your Projects ({projects.length})
          </h2>
          {projects.length === 0 ? (
            <p className="text-gray-500">No projects yet. Create one above!</p>
          ) : (
            <ul className="space-y-2">
              {projects.map((proj) => (
                <li key={proj.id} className="flex justify-between items-center">
                  <span>{proj.name}</span>
                  <span className="text-sm text-gray-500">
                    {proj.taskCount ?? 0} tasks
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="bg-white p-4 rounded-lg shadow">
          <h2 className="text-xl font-semibold mb-3">Recent Tasks</h2>
          {recentTasks.length === 0 ? (
            <p className="text-gray-500">
              No tasks yet.{" "}
              <Link to="/tasks/new" className="text-indigo-600 hover:underline">
                Create one
              </Link>
            </p>
          ) : (
            <ul className="space-y-2">
              {recentTasks.map((task) => (
                <li key={task.id} className="flex justify-between items-center">
                  <span className="truncate">{task.title}</span>
                  <span
                    className={`px-2 py-1 text-xs rounded-full ${
                      task.priority === "Critical"
                        ? "bg-red-200 text-red-800"
                        : task.priority === "High"
                          ? "bg-orange-200 text-orange-800"
                          : task.priority === "Medium"
                            ? "bg-yellow-200 text-yellow-800"
                            : "bg-green-200 text-green-800"
                    }`}
                  >
                    {task.priority}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}
