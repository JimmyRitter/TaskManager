import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { usePomodoroStore } from './stores/pomodoro'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)

// Initialize Pomodoro store for global title management
const pomodoroStore = usePomodoroStore()
pomodoroStore.initializeTitle()

app.mount('#app')
