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
          class="task-item text-sm text-gray-700 flex items-center cursor-pointer group bg-gray-50 p-2 rounded-lg hover:bg-gray-100 transition-colors"
        >
          <!-- Drag Handle -->
          <div class="drag-handle mr-2 text-gray-400 hover:text-gray-600 cursor-grab active:cursor-grabbing">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8h16M4 16h16"/>
            </svg>
          </div>
          
          <!-- Task Toggle -->
          <div 
            @click="toggleTask(task)"
            :class="['w-4 h-4 mr-2 flex-shrink-0 rounded-full border-2 flex items-center justify-center transition-colors', task.isCompleted ? 'bg-orange-400 border-orange-500' : 'bg-white border-gray-300 group-hover:border-orange-400']"
            :title="task.isCompleted ? 'Mark as incomplete' : 'Mark as complete'"
          >
            <svg v-if="task.isCompleted" class="w-3 h-3 text-white" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
            </svg>
          </div>
          
          <!-- Task Content -->
          <span :class="['truncate transition-colors flex-1', task.isCompleted ? 'line-through text-gray-400' : '']">
            {{ task.description }}
          </span>
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
      <div class="flex gap-2">
        <input
          ref="taskInput"
          v-model="newTaskDescription"
          type="text"
          placeholder="Enter task description..."
          class="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm"
          @keyup.enter="createTask"
          @keyup.escape="cancelAddTask"
        />
        <button
          @click="createTask"
          class="bg-orange-500 text-white px-3 py-2 rounded-lg hover:bg-orange-600 transition-colors duration-200 text-sm font-medium"
          :disabled="!newTaskDescription.trim()"
        >
          Add
        </button>
        <button
          @click="cancelAddTask"
          class="bg-gray-100 text-gray-700 px-3 py-2 rounded-lg hover:bg-gray-200 transition-colors duration-200 text-sm font-medium"
        >
          Cancel
        </button>
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
import { createTask as createTaskService, toggleTaskStatus, TaskPriority, type CreateTaskRequest } from '@/services/task'

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
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const showTaskInput = ref(false)
const newTaskDescription = ref('')
const taskInput = ref<HTMLInputElement>()

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
    taskInput.value?.focus()
  })
}

async function createTask() {
  if (!newTaskDescription.value.trim()) return
  
  try {
    const taskRequest: CreateTaskRequest = {
      description: newTaskDescription.value.trim(),
      priority: TaskPriority.Medium,
      listId: props.listId
    }
    
    await createTaskService(taskRequest)
    
    // Reset state
    showTaskInput.value = false
    newTaskDescription.value = ''
    
    // Notify parent to refresh data
    emit('task-updated')
  } catch (e) {
    alert('Failed to create task')
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