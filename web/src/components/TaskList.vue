<template>
  <div class="task-list">
    <!-- Tasks with Drag & Drop -->
    <div v-if="tasks && tasks.length > 0" class="mb-4">
      <VueDraggable 
        v-model="sortedTasks" 
        :animation="150"
        @start="onDragStart"
        @end="onDragEnd"
        handle=".drag-handle"
        class="space-y-2"
      >
        <div 
          v-for="task in sortedTasks" 
          :key="task.id"
          class="task-item text-sm text-gray-700 flex items-start cursor-pointer group bg-gray-50 p-2 rounded-lg hover:bg-gray-100 transition-colors"
        >
          <!-- Drag Handle -->
          <div class="drag-handle mr-2 text-gray-400 hover:text-gray-600 cursor-grab active:cursor-grabbing" style="padding-top: 2px;">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8h16M4 16h16"/>
            </svg>
          </div>
          
          <!-- Task Toggle -->
          <div 
            @click="toggleTask(task)"
            :class="['w-4 h-4 mr-2 flex-shrink-0 rounded-full border-2 flex items-center justify-center transition-colors', task.isCompleted ? 'bg-orange-400 border-orange-500' : 'bg-white border-gray-300 group-hover:border-orange-400']"
            :title="task.isCompleted ? 'Mark as incomplete' : 'Mark as complete'"
            style="margin-top: 2px;"
          >
            <svg v-if="task.isCompleted" class="w-3 h-3 text-white" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
            </svg>
          </div>
          
          <!-- Task Content - Edit Mode -->
          <div v-if="editingTaskId === task.id" class="flex-1">
            <div class="space-y-2">
              <textarea
                ref="editInput"
                v-model="editingDescription"
                class="w-full px-2 py-1 border border-gray-300 rounded focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm resize-none"
                style="min-height: 2rem; max-height: none; word-wrap: break-word; white-space: pre-wrap; overflow: hidden; box-sizing: border-box; line-height: 1.25;"
                @keydown="handleEditKeydown"
                @input="autoResizeTextarea"
                @click.stop
              ></textarea>
              <div class="flex gap-2 justify-end">
                <button
                  @click.stop="saveEdit(task)"
                  class="bg-orange-500 text-white px-3 py-1 rounded hover:bg-orange-600 transition-colors text-xs font-medium"
                  :disabled="!editingDescription.trim()"
                >
                  Save
                </button>
                <button
                  @click.stop="cancelEdit"
                  class="bg-gray-100 text-gray-700 px-3 py-1 rounded hover:bg-gray-200 transition-colors text-xs font-medium"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
          
          <!-- Task Content - View Mode -->
          <div v-else class="flex-1 flex items-start justify-between">
            <span 
              :class="['transition-colors flex-1 break-words pr-2', task.isCompleted ? 'line-through text-gray-400' : '']"
              @dblclick="startEdit(task)"
              style="line-height: 1.25;"
            >
              {{ task.description }}
            </span>
            <div class="flex items-center gap-1">
              <button
                @click.stop="startEdit(task)"
                class="opacity-0 group-hover:opacity-100 flex-shrink-0 text-gray-400 hover:text-gray-600 transition-opacity"
                title="Edit task"
                style="padding-top: 2px;"
              >
                <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                </svg>
              </button>
              <button
                @click.stop="deleteTaskHandler(task)"
                class="opacity-0 group-hover:opacity-100 flex-shrink-0 text-gray-400 hover:text-red-600 transition-all duration-200"
                title="Delete task"
                style="padding-top: 2px;"
              >
                <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1-1H7a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </VueDraggable>
    </div>

    <!-- Empty Tasks State -->
    <div v-else class="mb-4">
      <div class="text-center py-4">
        <svg class="w-8 h-8 text-gray-300 mx-auto mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
        </svg>
        <p class="text-sm text-gray-500 mb-3">No tasks yet</p>
        <button 
          v-if="!showTaskInput"
          @click="startAddTask"
          class="text-orange-600 hover:text-orange-700 text-sm font-medium hover:bg-orange-50 px-3 py-1 rounded transition-colors duration-200"
        >
          Add your first task
        </button>
      </div>
    </div>
    
    <!-- Inline Task Input -->
    <div v-if="showTaskInput" class="mb-4">
      <div class="space-y-2">
        <textarea
          ref="taskInput"
          v-model="newTaskDescription"
          placeholder="Enter task description... (Press Enter to add, Esc to finish)"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm resize-none overflow-hidden"
          rows="1"
          style="min-height: 2.5rem; word-wrap: break-word; white-space: pre-wrap; line-height: 1.25;"
          @keydown="handleAddTaskKeydown"
          @input="autoResizeAddTaskTextarea"
        ></textarea>
        <div class="flex gap-2 justify-end">
          <button
            @click="createTask"
            class="bg-orange-500 text-white px-3 py-2 rounded-lg hover:bg-orange-600 transition-colors duration-200 text-sm font-medium"
            :disabled="!newTaskDescription.trim()"
          >
            Add Task
          </button>
          <button
            @click="cancelAddTask"
            class="bg-gray-100 text-gray-700 px-3 py-2 rounded-lg hover:bg-gray-200 transition-colors duration-200 text-sm font-medium"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
    
    <!-- Add Task Button for lists with tasks -->
    <div v-if="tasks && tasks.length > 0 && !showTaskInput" class="mb-4">
      <button 
        @click="startAddTask"
        class="w-full text-orange-600 hover:text-orange-700 text-sm font-medium hover:bg-orange-50 py-2 rounded transition-colors duration-200 border border-dashed border-orange-300 hover:border-orange-400"
      >
        + Add another task
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick } from 'vue'
import { VueDraggable } from 'vue-draggable-plus'
import { createTask as createTaskService, updateTask, deleteTask, toggleTaskStatus, TaskPriority, type CreateTaskRequest, type UpdateTaskRequest } from '@/services/task'

interface Task {
  id: string
  description: string
  isCompleted: boolean
  order: number
  createdAt: string
}

interface Props {
  tasks: Task[]
  listId: string
}

interface Emits {
  (e: 'task-updated'): void
  (e: 'task-reordered', movedTask: { taskId: string, newOrder: number }): void
  (e: 'task-deleted', taskId: string): void
  (e: 'task-delete-failed', taskId: string): void
  (e: 'task-created', task: Task): void
  (e: 'task-create-failed', tempTaskId: string): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const showTaskInput = ref(false)
const newTaskDescription = ref('')
const taskInput = ref<HTMLTextAreaElement>()

// Store the original order before drag starts
const originalTaskOrder = ref<Task[]>([])

// Simple computed property for sorted tasks (read-only)
const sortedTasks = computed(() => {
  return [...props.tasks].sort((a, b) => a.order - b.order)
})

function onDragStart(evt: any) {
  // Store the original order before the drag operation modifies it
  originalTaskOrder.value = [...sortedTasks.value]
}

function onDragEnd(evt: any) {
  if (evt.oldIndex !== evt.newIndex) {
    // Get the task that was moved using the old index from the original order
    const movedTask = originalTaskOrder.value[evt.oldIndex]
    emit('task-reordered', { taskId: movedTask.id, newOrder: evt.newIndex })
  }
}

function startAddTask() {
  showTaskInput.value = true
  newTaskDescription.value = ''
  nextTick(() => {
    if (taskInput.value) {
      taskInput.value.focus()
      // Auto-resize to fit content
      resizeTextarea(taskInput.value)
    }
  })
}

async function createTask() {
  if (!newTaskDescription.value.trim()) return
  
  // Generate temporary task for optimistic update
  const tempTaskId = `temp_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
  const newOrder = props.tasks.length // Next available order
  
  const optimisticTask: Task & { listId: string } = {
    id: tempTaskId,
    description: newTaskDescription.value.trim(),
    isCompleted: false,
    order: newOrder,
    createdAt: new Date().toISOString(),
    listId: props.listId
  }
  
  // Optimistic update: immediately add task to UI
  emit('task-created', optimisticTask)
  
  // Keep input active for continuous task addition
  const savedDescription = newTaskDescription.value.trim()
  newTaskDescription.value = ''
  
  // Refocus on the input field for immediate next task entry
  nextTick(() => {
    if (taskInput.value) {
      taskInput.value.focus()
      resizeTextarea(taskInput.value)
    }
  })
  
  try {
    // Make API call in background
    const taskRequest: CreateTaskRequest = {
      description: savedDescription,
      priority: TaskPriority.Medium,
      listId: props.listId
    }
    
    await createTaskService(taskRequest)
    
    // Success! Keep the optimistic task in place
    // The real task data will be fetched during the next natural refresh cycle
  } catch (e) {
    // API call failed - revert optimistic update
    emit('task-create-failed', tempTaskId)
    alert('Failed to create task. Please try again.')
  }
}

function cancelAddTask() {
  showTaskInput.value = false
  newTaskDescription.value = ''
}

async function toggleTask(task: Task) {
  try {
    await toggleTaskStatus(task.id)
    emit('task-updated')
  } catch (e) {
    alert('Failed to toggle task status')
  }
}

async function deleteTaskHandler(task: Task) {
  if (confirm(`Are you sure you want to delete "${task.description}"?`)) {
    // Optimistic UI: immediately remove from UI
    emit('task-deleted', task.id)
    
    try {
      // Make the actual API call
      await deleteTask(task.id)
      // Success! The UI is already updated, so no further action needed
    } catch (e) {
      // Revert the optimistic update on failure
      emit('task-delete-failed', task.id)
      alert('Failed to delete task. It has been restored.')
    }
  }
}

const editingTaskId = ref<string | null>(null)
const editingDescription = ref('')
const editInput = ref<HTMLTextAreaElement>()

function startEdit(task: Task) {
  editingTaskId.value = task.id
  editingDescription.value = task.description
  
  // Use multiple nextTick calls to ensure DOM is fully updated
  nextTick(() => {
    nextTick(() => {
      if (editInput.value) {
        // Force the value to be set
        editInput.value.value = task.description
        
        // Remove any height constraints first
        editInput.value.style.height = 'auto'
        editInput.value.style.maxHeight = 'none'
        
        // Focus and resize
        editInput.value.focus()
        
        // Aggressive resize attempts
        for (let i = 0; i < 3; i++) {
          setTimeout(() => {
            if (editInput.value) {
              resizeTextarea(editInput.value)
            }
          }, i * 25)
        }
        
        // Final resize after longer delay
        setTimeout(() => {
          if (editInput.value) {
            resizeTextarea(editInput.value)
          }
        }, 200)
      }
    })
  })
}

async function saveEdit(task: Task) {
  if (!editingDescription.value.trim()) return
  
  try {
    const updateRequest: UpdateTaskRequest = {
      taskId: task.id,
      description: editingDescription.value.trim()
    }
    
    await updateTask(updateRequest)
    
    // Reset state
    editingTaskId.value = null
    editingDescription.value = ''
    
    // Notify parent to refresh data
    emit('task-updated')
  } catch (e) {
    alert('Failed to update task')
  }
}

function cancelEdit() {
  editingTaskId.value = null
  editingDescription.value = ''
}

function handleEditKeydown(event: KeyboardEvent) {
  if (event.key === 'Enter' && !event.shiftKey) {
    event.preventDefault()
    const currentTask = sortedTasks.value.find(t => t.id === editingTaskId.value)
    if (currentTask) {
      saveEdit(currentTask)
    }
  } else if (event.key === 'Escape') {
    event.preventDefault()
    cancelEdit()
  }
}

function resizeTextarea(textarea: HTMLTextAreaElement) {
  // Store current value and scroll position
  const value = textarea.value
  const scrollTop = textarea.scrollTop
  
  // Reset all height constraints
  textarea.style.height = 'auto'
  textarea.style.maxHeight = 'none'
  textarea.style.minHeight = '2rem'
  
  // Force reflow by setting to 0 first
  textarea.style.height = '0px'
  
  // Get the actual content height including all lines
  const scrollHeight = textarea.scrollHeight
  
  // Set height to accommodate all content with some padding
  const newHeight = Math.max(scrollHeight + 4, 32) // Add 4px buffer
  textarea.style.height = newHeight + 'px'
  
  // Restore scroll position if it was scrolled
  if (scrollTop > 0) {
    textarea.scrollTop = scrollTop
  }
}

function autoResizeTextarea(event: Event) {
  const textarea = event.target as HTMLTextAreaElement
  if (textarea) {
    resizeTextarea(textarea)
  }
}

function handleAddTaskKeydown(event: KeyboardEvent) {
  if (event.key === 'Enter' && !event.shiftKey) {
    event.preventDefault()
    createTask()
  } else if (event.key === 'Escape') {
    event.preventDefault()
    cancelAddTask()
  }
}

function autoResizeAddTaskTextarea(event: Event) {
  const textarea = event.target as HTMLTextAreaElement
  if (textarea) {
    resizeTextarea(textarea)
  }
}
</script>

<style scoped>
.task-item:hover .drag-handle {
  opacity: 1;
}

.drag-handle {
  opacity: 0.6;
  transition: opacity 0.2s;
}

.drag-handle:hover {
  opacity: 1;
}
</style> 