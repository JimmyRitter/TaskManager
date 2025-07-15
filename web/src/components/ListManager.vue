<template>
  <div>
    <!-- Empty State -->
    <div v-if="lists.length === 0" class="text-center py-16">
      <div class="mb-8">
        <svg class="w-24 h-24 text-orange-200 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"></path>
        </svg>
        <h3 class="text-2xl font-semibold text-gray-900 mb-2">No lists yet</h3>
        <p class="text-gray-600 mb-8">Create your first list to start organizing your tasks</p>
      </div>
      <button 
        @click="showCreateList = true"
        class="bg-gradient-to-r from-orange-500 to-orange-600 text-white px-6 py-3 rounded-lg font-medium hover:from-orange-600 hover:to-orange-700 transform hover:scale-105 transition-all duration-200 shadow-lg hover:shadow-xl"
      >
        Create Your First List
      </button>
    </div>

    <!-- Lists Grid -->
    <div v-else>
      <!-- Header with Create Button -->
      <div class="flex justify-between items-center mb-8">
        <h2 class="text-2xl font-bold text-gray-900">Your Lists</h2>
        <button 
          @click="showCreateList = true"
          class="bg-gradient-to-r from-orange-500 to-orange-600 text-white px-4 py-2 rounded-lg font-medium hover:from-orange-600 hover:to-orange-700 transform hover:scale-105 transition-all duration-200 shadow-lg hover:shadow-xl"
        >
          + Create List
        </button>
      </div>

      <!-- Lists Grid - Max 3 per row -->
      <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
        <div 
          v-for="list in lists" 
          :key="list.id" 
          class="bg-white border border-gray-200 rounded-xl p-6 shadow-sm hover:shadow-md transition-shadow duration-200 flex flex-col min-h-[280px]"
        >
          <!-- Main Content -->
          <div class="flex-grow">
            <!-- List Header -->
            <div class="flex justify-between items-start mb-4">
              <h3 class="text-lg font-semibold text-gray-900 truncate pr-2">{{ list.name }}</h3>
              <button 
                @click="onDeleteList(list)"
                class="text-red-500 hover:text-red-700 hover:bg-red-50 p-1 rounded transition-colors duration-200"
                title="Delete list"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                </svg>
              </button>
            </div>

            <!-- List Description -->
            <p v-if="list.description" class="text-gray-600 text-sm mb-4">{{ list.description }}</p>

            <!-- Task List Component -->
            <TaskList 
              :tasks="list.tasks || []" 
              :list-id="list.id"
              @task-updated="fetchLists"
              @task-reordered="updateTaskOrder"
              @task-deleted="handleTaskDeleted"
              @task-delete-failed="handleTaskDeleteFailed"
            />
          </div>

          <!-- Task Count -->
          <div class="border-t border-gray-100 pt-3">
            <div class="flex justify-center items-center text-sm">
              <span class="text-gray-500">
                {{ list.tasks?.length || 0 }} {{ (list.tasks?.length || 0) === 1 ? 'task' : 'tasks' }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create List Modal -->
    <div v-if="showCreateList" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="showCreateList = false">
      <div class="bg-white rounded-2xl p-8 max-w-md w-full mx-4 shadow-2xl">
        <h3 class="text-2xl font-bold text-gray-900 mb-6">Create New List</h3>
        <form @submit.prevent="createList" class="space-y-4">
          <div>
            <label for="listName" class="block text-sm font-medium text-gray-700 mb-2">
              List Name
            </label>
            <input 
              id="listName"
              v-model="newListName" 
              type="text"
              placeholder="Enter list name" 
              required 
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-colors duration-200"
            />
          </div>
          
          <div>
            <label for="listDescription" class="block text-sm font-medium text-gray-700 mb-2">
              Description (optional)
            </label>
            <textarea 
              id="listDescription"
              v-model="newListDescription" 
              placeholder="Brief description of this list" 
              rows="3"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-colors duration-200 resize-none"
            ></textarea>
          </div>
          
          <div class="flex gap-3 pt-4">
            <button 
              type="submit"
              class="flex-1 bg-gradient-to-r from-orange-500 to-orange-600 text-white py-3 rounded-lg font-medium hover:from-orange-600 hover:to-orange-700 transform hover:scale-105 transition-all duration-200"
            >
              Create List
            </button>
            <button 
              type="button" 
              @click="showCreateList = false"
              class="flex-1 bg-gray-100 text-gray-700 py-3 rounded-lg font-medium hover:bg-gray-200 transition-colors duration-200"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { createList as createListService, getUserLists, deleteList as deleteListService } from '@/services/list'
import { updateTaskOrder as updateTaskOrderService } from '@/services/task'
import { setAuthToken } from '@/services/api'
import TaskList from './TaskList.vue'

const auth = useAuthStore()
const lists = ref<any[]>([])
const showCreateList = ref(false)
const newListName = ref('')
const newListDescription = ref('')

if (auth.token) setAuthToken(auth.token)

async function fetchLists() {
  try {
    const data = await getUserLists()
    // Tasks are now included in the API response
    lists.value = data
  } catch (e) {
    lists.value = []
  }
}

onMounted(fetchLists)

async function createList() {
  if (!newListName.value.trim()) return
  try {
    await createListService(newListName.value, newListDescription.value)
    showCreateList.value = false
    newListName.value = ''
    newListDescription.value = ''
    
    // Refresh the lists to get the complete data structure
    await fetchLists()
  } catch (e) {
    alert('Failed to create list')
  }
}

async function onDeleteList(list: any) {
  if (!confirm(`Are you sure you want to delete the list "${list.name}"?`)) return
  try {
    await deleteListService(list.id)
    lists.value = lists.value.filter(l => l.id !== list.id)
  } catch (e) {
    alert('Failed to delete list')
  }
}

// Handle task reordering from TaskList component
async function updateTaskOrder(movedTask: { taskId: string, newOrder: number }) {
  // Find the list containing the task
  const listWithTask = lists.value.find(list => 
    list.tasks?.some((task: any) => task.id === movedTask.taskId)
  )
  
  if (!listWithTask) {
    console.error('Could not find list containing task')
    return
  }

  // Store original state for rollback if needed
  const originalTasks = [...listWithTask.tasks]
  
  // Optimistically update the UI immediately
  const taskIndex = listWithTask.tasks.findIndex((task: any) => task.id === movedTask.taskId)
  if (taskIndex === -1) {
    console.error('Could not find task in list')
    return
  }

  // Remove task from its current position
  const [movedTaskData] = listWithTask.tasks.splice(taskIndex, 1)
  
  // Insert task at new position
  listWithTask.tasks.splice(movedTask.newOrder, 0, movedTaskData)
  
  // Update all task orders to be sequential (0, 1, 2, ...)
  listWithTask.tasks.forEach((task: any, index: number) => {
    task.order = index
  })

  try {
    // Call API in background
    await updateTaskOrderService(movedTask.taskId, movedTask.newOrder)
    // Success! No need to fetch again since we already updated the UI optimistically
  } catch (e) {
    console.error('Failed to update task order:', e)
    
    // Revert optimistic update by restoring original state
    listWithTask.tasks = originalTasks
    
    alert('Failed to update task order. Changes have been reverted.')
  }
}

// Store deleted tasks temporarily for potential restoration
const deletedTasks = ref<Map<string, { task: any, listId: string }>>(new Map())

// Handle optimistic task deletion
function handleTaskDeleted(taskId: string) {
  // Find the list containing the task
  const listWithTask = lists.value.find(list => 
    list.tasks?.some((task: any) => task.id === taskId)
  )
  
  if (!listWithTask) {
    console.error('Could not find list containing task to delete')
    return
  }

  // Find and store the task before removing it
  const taskIndex = listWithTask.tasks.findIndex((task: any) => task.id === taskId)
  if (taskIndex === -1) {
    console.error('Could not find task in list')
    return
  }

  // Store the task for potential restoration
  const taskToDelete = listWithTask.tasks[taskIndex]
  deletedTasks.value.set(taskId, { task: taskToDelete, listId: listWithTask.id })
  
  // Remove task from UI immediately (optimistic)
  listWithTask.tasks.splice(taskIndex, 1)
}

// Handle failed task deletion - restore the task
function handleTaskDeleteFailed(taskId: string) {
  const deletedTaskData = deletedTasks.value.get(taskId)
  
  if (!deletedTaskData) {
    console.error('Could not find deleted task data to restore')
    // Fallback: refresh all data from server
    fetchLists()
    return
  }

  // Find the list to restore the task to
  const listToRestore = lists.value.find(list => list.id === deletedTaskData.listId)
  
  if (!listToRestore) {
    console.error('Could not find list to restore task to')
    // Fallback: refresh all data from server
    fetchLists()
    return
  }

  // Restore the task to the list
  if (!listToRestore.tasks) {
    listToRestore.tasks = []
  }
  listToRestore.tasks.push(deletedTaskData.task)
  
  // Sort tasks by order to maintain proper sequence
  listToRestore.tasks.sort((a: any, b: any) => a.order - b.order)
  
  // Clean up stored deleted task
  deletedTasks.value.delete(taskId)
}
</script>

<style scoped>
.lists-header {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 1.5rem;
}
.lists-header button {
  background: #42b983;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.6rem 1.2rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
}
.lists {
  list-style: none;
  padding: 0;
}
.list-item {
  background: #fff;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(60,60,60,0.08);
  margin-bottom: 1.5rem;
  padding: 1.2rem 1rem;
}
.list-item h3 {
  margin: 0 0 0.7rem 0;
}
.tasks {
  list-style: disc inside;
  margin: 0.5rem 0 0 1rem;
  padding: 0;
}
.empty-state, .empty-tasks {
  text-align: center;
  margin: 2rem 0;
}
.empty-state button, .empty-tasks button {
  background: #42b983;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.5rem 1.1rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  margin-top: 1rem;
}
.modal-backdrop {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.18);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal {
  background: #fff;
  border-radius: 10px;
  padding: 2rem 1.5rem;
  min-width: 300px;
  box-shadow: 0 4px 24px rgba(60,60,60,0.12);
}
.modal h3 {
  margin-top: 0;
}
.modal form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.modal input {
  padding: 0.6rem;
  border-radius: 6px;
  border: 1px solid #ccc;
  font-size: 1rem;
}
.modal textarea {
  padding: 0.6rem;
  border-radius: 6px;
  border: 1px solid #ccc;
  font-size: 1rem;
  resize: vertical;
}
.modal button[type="submit"] {
  background: #42b983;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.6rem 1.2rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
}
.modal button[type="button"] {
  background: #eee;
  color: #333;
  border: none;
  border-radius: 6px;
  padding: 0.6rem 1.2rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
}
.list-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.delete-btn {
  background: #e53935;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.4rem 1rem;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}
.delete-btn:hover {
  background: #b71c1c;
}
.list-description {
  color: #666;
  font-size: 1rem;
  margin: 0.2rem 0 0.7rem 0;
}
</style> 