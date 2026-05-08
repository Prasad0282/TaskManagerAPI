import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { taskService } from "../services/taskService";
import { projectService } from "../services/projectService";
import { categoryService } from "../services/categoryService";
import type { TaskItem, TaskFilter, Project, Category } from "../types";
import Pagination from "../components/Pagination";

export default function TaskList() {
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [filters, setFilters] = useState<TaskFilter>({
    pageNumber: 1,
    pageSize: 10,
    sortBy: "createdAt",
    sortOrder: "desc",
  });
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [fetchTrigger, setFetchTrigger] = useState(0);

  const triggerFetch = () => setFetchTrigger((prev) => prev + 1);

  useEffect(() => {
    let cancelled = false;

    const fetchTasks = async () => {
      setLoading(true);
      try {
        const result = await taskService.getTasks(filters);
        if (!cancelled) {
          setTasks(result.items);
          setTotalPages(result.totalPages);
        }
      } catch (error) {
        console.error("Failed to fetch tasks:", error);
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    fetchTasks();
    return () => {
      cancelled = true;
    };
  }, [filters, fetchTrigger]);

  useEffect(() => {
    projectService.getProjects().then(setProjects).catch(console.error);
    categoryService.getCategories().then(setCategories).catch(console.error);
  }, []);

  const handleStatusChange = async (taskId: number, newStatus: string) => {
    try {
      await taskService.updateTaskStatus(taskId, newStatus);
      triggerFetch();
    } catch (error) {
      console.error("Failed to update status:", error);
    }
  };

  const handleDelete = async (taskId: number) => {
    if (!confirm("Delete this task?")) return;
    try {
      await taskService.deleteTask(taskId);
      triggerFetch();
    } catch (error) {
      console.error("Failed to delete task:", error);
    }
  };

  const updateFilter = (
    key: keyof TaskFilter,
    value: string | number | undefined,
  ) => {
    setFilters((prev) => ({ ...prev, [key]: value, pageNumber: 1 }));
  };

  const getPriorityColor = (priority: string): string => {
    switch (priority) {
      case "Critical":
        return "bg-red-100 text-red-800";
      case "High":
        return "bg-orange-100 text-orange-800";
      case "Medium":
        return "bg-yellow-100 text-yellow-800";
      default:
        return "bg-green-100 text-green-800";
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Tasks</h1>
        <Link
          to="/tasks/new"
          className="bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700"
        >
          + New Task
        </Link>
      </div>

      {/* Filters */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 bg-white p-4 rounded-lg shadow">
        <input
          type="text"
          placeholder="Search..."
          className="border rounded px-3 py-2"
          value={filters.searchTerm || ""}
          onChange={(e) => updateFilter("searchTerm", e.target.value)}
        />
        <select
          className="border rounded px-3 py-2"
          value={filters.status || ""}
          onChange={(e) => updateFilter("status", e.target.value)}
        >
          <option value="">All Statuses</option>
          <option value="Todo">Todo</option>
          <option value="InProgress">In Progress</option>
          <option value="Done">Done</option>
        </select>
        <select
          className="border rounded px-3 py-2"
          value={filters.priority || ""}
          onChange={(e) => updateFilter("priority", e.target.value)}
        >
          <option value="">All Priorities</option>
          <option value="Critical">Critical</option>
          <option value="High">High</option>
          <option value="Medium">Medium</option>
          <option value="Low">Low</option>
        </select>
        <select
          className="border rounded px-3 py-2"
          value={filters.projectId || ""}
          onChange={(e) =>
            updateFilter(
              "projectId",
              e.target.value ? Number(e.target.value) : undefined,
            )
          }
        >
          <option value="">All Projects</option>
          {projects.map((p) => (
            <option key={p.id} value={p.id}>
              {p.name}
            </option>
          ))}
        </select>
      </div>

      {/* Category filter */}
      {categories.length > 0 && (
        <div className="flex flex-wrap gap-2">
          <button
            onClick={() => updateFilter("categoryId", undefined)}
            className={`px-3 py-1 rounded-full text-sm ${
              !filters.categoryId
                ? "bg-indigo-600 text-white"
                : "bg-gray-200 text-gray-700"
            }`}
          >
            All
          </button>
          {categories.map((cat) => (
            <button
              key={cat.id}
              onClick={() => updateFilter("categoryId", cat.id)}
              className={`px-3 py-1 rounded-full text-sm ${
                filters.categoryId === cat.id
                  ? "bg-indigo-600 text-white"
                  : "bg-gray-200 text-gray-700"
              }`}
            >
              {cat.name}
            </button>
          ))}
        </div>
      )}

      {/* Task List */}
      {loading ? (
        <div className="text-center py-8">Loading tasks...</div>
      ) : (
        <div className="space-y-3">
          {tasks.map((task) => (
            <div
              key={task.id}
              className="bg-white p-4 rounded-lg shadow flex flex-col md:flex-row justify-between items-start md:items-center"
            >
              <div className="flex-1">
                <div className="flex items-center space-x-2 flex-wrap gap-1">
                  <h3 className="font-semibold text-lg">{task.title}</h3>
                  <span
                    className={`px-2 py-0.5 rounded-full text-xs ${getPriorityColor(task.priority)}`}
                  >
                    {task.priority}
                  </span>
                  {task.tags?.map((tag) => (
                    <span
                      key={tag}
                      className="bg-gray-200 text-gray-700 px-2 py-0.5 rounded-full text-xs"
                    >
                      {tag}
                    </span>
                  ))}
                </div>
                <p className="text-gray-600 mt-1">{task.description}</p>
                <div className="text-sm text-gray-500 mt-2">
                  {task.projectName} · {task.categoryName || "Uncategorized"} ·
                  Due{" "}
                  {task.dueDate
                    ? new Date(task.dueDate).toLocaleDateString()
                    : "No deadline"}
                </div>
              </div>
              <div className="flex items-center space-x-2 mt-3 md:mt-0">
                <select
                  value={task.status}
                  onChange={(e) => handleStatusChange(task.id, e.target.value)}
                  className="border rounded px-2 py-1 text-sm"
                >
                  <option value="Todo">Todo</option>
                  <option value="InProgress">In Progress</option>
                  <option value="Done">Done</option>
                </select>
                <Link
                  to={`/tasks/${task.id}/edit`}
                  className="text-indigo-600 hover:underline text-sm"
                >
                  Edit
                </Link>
                <button
                  onClick={() => handleDelete(task.id)}
                  className="text-red-600 hover:underline text-sm"
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
          {tasks.length === 0 && (
            <p className="text-center text-gray-500 py-8">
              No tasks found. Create your first task!
            </p>
          )}
        </div>
      )}

      <Pagination
        pageNumber={filters.pageNumber}
        totalPages={totalPages}
        onPageChange={(page) =>
          setFilters((prev) => ({ ...prev, pageNumber: page }))
        }
      />
    </div>
  );
}
