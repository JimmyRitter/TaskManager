import api from './api'

export interface CreateTaskRequest {
  description: string
  priority: TaskPriority
  listId: string
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2
}

export async function createTask(request: CreateTaskRequest) {
  const { data } = await api.post('/tasks/create', request)
  return data
}

export async function deleteTask(taskId: string) {
  const { data } = await api.delete('/tasks/delete', { data: { taskId } })
  return data
}

export async function toggleTaskStatus(taskId: string) {
  const { data } = await api.put('/tasks/toggle', { taskId })
  return data
}

export async function getListTasks(listId: string) {
  const { data } = await api.get('/tasks/get-list-tasks', { data: { listId } })
  return data.data
}

export async function updateTaskOrder(taskId: string, newOrder: number) {
  const { data } = await api.put('/tasks/update-order', { taskId, newOrder })
  return data
} 