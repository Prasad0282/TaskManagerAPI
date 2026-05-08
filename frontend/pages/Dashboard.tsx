import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { projectService } from "../services/projectService";
import { taskService } from "../services/taskService";
import { Project, TaskItem } from "../types";

export default function Dashboard() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [recentTasks, setRecentTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
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
      setLoading(false);
    };
    fetchData();
  }, []);

  if (loading) return <div className="text-center">Loading...</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white p-4 rounded-lg shadow">
          <h2 className="text-xl font-semibold mb-3">
            Your Projects ({projects.length})
          </h2>
          {projects.length === 0 ? (
            <p className="text-gray-500">No projects yet.</p>
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
              <Link to="/tasks" className="text-indigo-600">
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
