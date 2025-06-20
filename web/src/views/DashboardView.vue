<template>
  <section class="dashboard">
    <h2>Todo Lists</h2>
    <div class="lists-header">
      <button @click="showCreateList = true">+ Create List</button>
    </div>
    <div v-if="lists.length === 0" class="empty-state">
      <p>No lists yet.</p>
      <button @click="showCreateList = true">Create your first list</button>
    </div>
    <ul v-else class="lists">
      <li v-for="list in lists" :key="list.id" class="list-item">
        <h3>{{ list.name }}</h3>
        <div v-if="list.tasks && list.tasks.length > 0">
          <ul class="tasks">
            <li v-for="task in list.tasks" :key="task.id">{{ task.description }}</li>
          </ul>
        </div>
        <div v-else class="empty-tasks">
          <button @click="addTask(list)">Click here to add your first task</button>
        </div>
      </li>
    </ul>

    <!-- Create List Modal -->
    <div v-if="showCreateList" class="modal-backdrop" @click.self="showCreateList = false">
      <div class="modal">
        <h3>Create New List</h3>
        <form @submit.prevent="createList">
          <input v-model="newListName" placeholder="List name" required />
          <button type="submit">Create</button>
          <button type="button" @click="showCreateList = false">Cancel</button>
        </form>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { createList as createListService, getUserLists } from '@/services/list'
import { setAuthToken } from '@/services/api'

const auth = useAuthStore()
const lists = ref<any[]>([])
const showCreateList = ref(false)
const newListName = ref('')

if (auth.token) setAuthToken(auth.token)

async function fetchLists() {
  try {
    const data = await getUserLists()
    lists.value = data.map((l: any) => ({ ...l, tasks: l.tasks || [] }))
  } catch (e) {
    lists.value = []
  }
}

onMounted(fetchLists)

async function createList() {
  if (!newListName.value.trim()) return
  try {
    const data = await createListService(newListName.value, '')
    lists.value.push({
      id: data.id,
      name: data.name,
      description: data.description || '',
      tasks: []
    })
    showCreateList.value = false
    newListName.value = ''
  } catch (e) {
    alert('Failed to create list')
  }
}

function addTask(list: any) {
  // TODO: open add task modal for this list
  alert('Add task for list: ' + list.name)
}
</script>

<style scoped>
.dashboard {
  max-width: 800px;
  margin: 3rem auto;
  padding: 2rem;
}
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
</style> 