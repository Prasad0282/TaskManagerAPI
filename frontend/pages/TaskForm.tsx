import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { taskService } from "../services/taskService";
import { projectService } from "../services/projectService";
import { categoryService } from "../services/categoryService";
import type { TaskItem, Project, Category } from "../types";

// Use TaskItem's actual types so it's compatible with Partial<TaskItem>
interface TaskFormData {
  title: string;
  description?: string;
  dueDate?: string;
  priority?: TaskItem["priority"];
  tags?: string;
  projectId: number;
  categoryId?: number;
  status?: TaskItem["status"];
}

export default function TaskForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [priority, setPriority] = useState<TaskItem["priority"]>("Medium");
  const [tags, setTags] = useState("");
  const [projectId, setProjectId] = useState<number>(0);
  const [categoryId, setCategoryId] = useState<number | undefined>();
  const [status, setStatus] = useState<TaskItem["status"]>("Todo");

  const [projects, setProjects] = useState<Project[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    projectService.getProjects().then(setProjects).catch(console.error);
    categoryService.getCategories().then(setCategories).catch(console.error);

    if (isEdit && id) {
      taskService
        .getTask(Number(id))
        .then((task: TaskItem) => {
          setTitle(task.title);
          setDescription(task.description);
          setDueDate(task.dueDate?.split("T")[0] || "");
          setPriority(task.priority);
          setTags(task.tags?.join(", ") || "");
          setProjectId(task.projectId);
          setCategoryId(task.categoryId);
          setStatus(task.status);
        })
        .catch(console.error);
    }
  }, [id, isEdit]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    const taskData: TaskFormData = {
      title,
      description: description || undefined,
      dueDate: dueDate || undefined,
      priority: priority || undefined,
      tags: tags || undefined,
      projectId,
      categoryId: categoryId ?? undefined,
    };

    if (isEdit) {
      taskData.status = status;
    }

    try {
      if (isEdit) {
        await taskService.updateTask(
          Number(id),
          taskData as unknown as Partial<TaskItem>,
        );
      } else {
        await taskService.createTask(
          taskData as unknown as Partial<TaskItem> & { projectId: number },
        );
      }
      navigate("/tasks");
    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Failed to save task";
      setError(message);
      console.error("Failed to save task:", err);
    } finally {
      setLoading(false);
    }
  };

  const statusOptions: TaskItem["status"][] = ["Todo", "InProgress", "Done"];
  const priorityOptions: TaskItem["priority"][] = [
    "Low",
    "Medium",
    "High",
    "Critical",
  ];

  return (
    <div className="max-w-2xl mx-auto bg-white p-6 rounded-lg shadow">
      <h2 className="text-2xl font-bold mb-6">
        {isEdit ? "Edit Task" : "New Task"}
      </h2>

      {error && (
        <div className="bg-red-100 text-red-700 p-3 rounded mb-4">{error}</div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-gray-700 mb-1">Title *</label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="w-full border rounded px-3 py-2"
            required
          />
        </div>
        <div>
          <label className="block text-gray-700 mb-1">Description</label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full border rounded px-3 py-2"
            rows={3}
          />
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-gray-700 mb-1">Due Date</label>
            <input
              type="date"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
              className="w-full border rounded px-3 py-2"
            />
          </div>
          <div>
            <label className="block text-gray-700 mb-1">Priority</label>
            <select
              value={priority}
              onChange={(e) =>
                setPriority(e.target.value as TaskItem["priority"])
              }
              className="w-full border rounded px-3 py-2"
            >
              {priorityOptions.map((p) => (
                <option key={p} value={p}>
                  {p}
                </option>
              ))}
            </select>
          </div>
        </div>
        <div>
          <label className="block text-gray-700 mb-1">
            Tags (comma separated)
          </label>
          <input
            type="text"
            value={tags}
            onChange={(e) => setTags(e.target.value)}
            placeholder="e.g., bug, urgent"
            className="w-full border rounded px-3 py-2"
          />
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-gray-700 mb-1">Project *</label>
            <select
              value={projectId}
              onChange={(e) => setProjectId(Number(e.target.value))}
              className="w-full border rounded px-3 py-2"
              required
            >
              <option value={0}>Select Project</option>
              {projects.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-gray-700 mb-1">Category</label>
            <select
              value={categoryId || ""}
              onChange={(e) =>
                setCategoryId(
                  e.target.value ? Number(e.target.value) : undefined,
                )
              }
              className="w-full border rounded px-3 py-2"
            >
              <option value="">No Category</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </div>
        </div>
        {isEdit && (
          <div>
            <label className="block text-gray-700 mb-1">Status</label>
            <select
              value={status}
              onChange={(e) => setStatus(e.target.value as TaskItem["status"])}
              className="w-full border rounded px-3 py-2"
            >
              {statusOptions.map((s) => (
                <option key={s} value={s}>
                  {s === "InProgress" ? "In Progress" : s}
                </option>
              ))}
            </select>
          </div>
        )}
        <button
          type="submit"
          disabled={loading}
          className="w-full bg-indigo-600 text-white py-2 rounded hover:bg-indigo-700 disabled:opacity-50"
        >
          {loading ? "Saving..." : isEdit ? "Update Task" : "Create Task"}
        </button>
      </form>
    </div>
  );
}
